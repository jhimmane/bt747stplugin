using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.IO;
using EnvDTE;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.BT747
{
    class BT747Device
    {
        public BT747Device(DeviceConfigurationInfo configInfo)
        {
            this.configInfo = configInfo;
        }
        
        public void Open(int portNumber)
        {
            port = OpenPort(portNumber);
        }

        public void Close()
        {
            if (port != null)
            {
                port.Close();
                port = null;
            }
        }
        public void setPortNumber(int portNumber)
        {
            this.portNumber = portNumber;
        }

        public void emptyLog()
        {
            string NMEAString = "$PMTK182,6,1*";

            string received = "a";
            int i = 0;

            string checksum = GetChecksum(NMEAString);
            NMEAString = NMEAString + checksum + "\r\n";

            //System.Diagnostics.Debug.WriteLine("BT747,emptyLog: " + port.PortName + "," + NMEAString);
            
            if (port.IsOpen == false)
            {
                port.Open();
            }

            port.ReadTimeout = port.WriteTimeout = 60000;

            port.Write(NMEAString);

            while (received.StartsWith("$PMTK001,182,6,3*21") == false)
            {
                received = port.ReadLine();
                //System.Diagnostics.Debug.WriteLine("BT747,emptyLog - Responce: " + received);
                //Correct responce: $PMTK001,182,6,3*21

                if (i == 50)
                {
                    //System.Diagnostics.Debug.WriteLine("BT747,sendNMEA: "+ port.PortName + "," + NMEAString);
                    port.Write(NMEAString);
                    i = 0;
                }
                i++;
            }
            Plugin.Instance.BT747LastPosition = "00000000";
        }
 
  
        // Returns True if a sentence's checksum matches the 
        // calculated checksum

        public bool IsValid(string sentence)
        {
            // Compare the characters after the asterisk to the calculation
            return sentence.Substring(sentence.IndexOf("*") + 1) ==
              GetChecksum(sentence);
        }

        /**
         * Calculates the checksum for the NMEA message
         */
          
        public static string GetChecksum(string sentence)
        {
            // Loop through all chars to get a checksum
            int Checksum = 0;
            foreach (char Character in sentence)
            {
                if (Character == '$')
                {
                    // Ignore the dollar sign
                }
                else if (Character == '*')
                {
                    // Stop processing before the asterisk
                    break;
                }
                else
                {
                    // Is this the first value for the checksum?
                    if (Checksum == 0)
                    {
                        // Yes. Set the checksum to the value
                        Checksum = Convert.ToByte(Character);
                    }
                    else
                    {
                        // No. XOR the checksum with this character's value
                        Checksum = Checksum ^ Convert.ToByte(Character);
                    }
                }
            }
            // Return the checksum formatted as a two-character hexadecimal
            return Checksum.ToString("X2");
        }

        public string ParseLogSize(String NMEAresponce)
        {
            //$PMTK182,3,8,00171AF8*16
            if (NMEAresponce.Length >= 21 ){
                if (NMEAresponce[9] == '3')
                {
                    //System.Diagnostics.Debug.WriteLine("BT747,ParseLogSize: " + NMEAresponce.Substring(13, 8));
                    WriteDebuglog("ParseLogSize - NMEAresponce: " + NMEAresponce, configInfo.debug);
                    return NMEAresponce.Substring(13, 8);
                }
                else return "";
            }
            else return "";
        }

        /**
         * Reads all tracks from the NMEA log inquiry strings
         * 
         */

        public IList<BT747Packet.TrackFileSection> ReadNMEATracks(IJobMonitor monitor, int trackChange, bool onlyNew)
        {
            
            monitor.StatusText = String.Format(CommonResources.Text.Devices.ImportJob_Status_Reading, 0 + "%");          

            IList<BT747Packet.TrackFileSection> sectionList = new List<BT747Packet.TrackFileSection>();
            BT747Packet.TrackFileSection trackSection = new BT747Packet.TrackFileSection();
            float percentComplete = 0.0F;
            
            //Disable logging to prevent unwanted recordings during the import
            string NMEAString = "$PMTK182,5*";            
            string ResponceString = SendNMEA(NMEAString);
            
            //Read log size
            NMEAString = "$PMTK"
                + BT747Constants.PMTK_CMD_LOG_STR + ","
                + BT747Constants.PMTK_LOG_QUERY_STR + ","
                + BT747Constants.PMTK_LOG_MEM_USED_STR
                + "*";            
            ResponceString = SendNMEA(NMEAString);
         
            string logstring = "";            
            string previous_logstring = "";            
            string logSize = ParseLogSize(ResponceString);
            WriteDebuglog("ReadNMEATracks - logSize: " + logSize, configInfo.debug);

            if (logSize != "")
            {
                //Acknowledge
               /* NMEAString = "$PMTK"
                + BT747Constants.PMTK_ACK_STR + ","
                + BT747Constants.PMTK_LOG_QUERY_STR + ","
                + BT747Constants.PMTK_LOG_MEM_USED_STR
                + ",3*";
                ResponceString = SendNMEA(NMEAString);*/
            }
            else
            {
                WriteDebuglog("ReadNMEATracks - Reading log size failed", configInfo.debug);
                return null;
            }

            /* 
             * PMTK_LOG_REQ_DATA
             * PMTK182,7,START,SIZE	
             * START:First address to return (hex)
             * SIZE:Number of bytes to return (hex)
             */
            
            string lastPosition = Plugin.Instance.BT747LastPosition; //Address of last read data  
            if (lastPosition.Length != 8)
            {
                lastPosition = "00000000";
            }

            WriteDebuglog("ReadNMEATracks - lastPosition: " + lastPosition, configInfo.debug);

            string readPosition = "00000000";   // First address to return           
            string readSize = logSize;          // Amount of data to be read
            int bytesToRead = 0;         

            // Get the start address of the first 0x10000 byte chunk to be read 
            int position = int.Parse(lastPosition, System.Globalization.NumberStyles.HexNumber) / 0x10000;
            position *= 0x10000;  

            if (onlyNew)
            {
                bytesToRead = int.Parse(logSize, System.Globalization.NumberStyles.HexNumber) - position;
                System.Diagnostics.Debug.WriteLine("BT747,ReadNMEATracks - bytesToRead: " + bytesToRead);
                try
                {
                    if (bytesToRead < 0)
                    {
                        //Log size < the last position --> the log has been cleared

                        readPosition = "00000000";
                        readSize = logSize;
                        System.Diagnostics.Debug.WriteLine("BT747,ReadNMEATracks - readAll: " + readSize);
                    }
                    else if (bytesToRead < 2 * 0x200)
                    {
                        //Not enough data, propaly no new tracks on the unit
                        return sectionList;
                    }
                    else
                    {
                        //TODO: Add check for log data versus data read last time
                                               
                        readPosition = position.ToString("X8");
                        readSize = bytesToRead.ToString("X8");

                        System.Diagnostics.Debug.WriteLine("BT747,ReadNMEATracks - readPosition: " + readPosition);
                        System.Diagnostics.Debug.WriteLine("BT747,ReadNMEATracks - readSize: " + readSize);
                    }
                }
                catch
                {
                    System.FormatException e;
                    lastPosition = "00000000";
                }
            }
            else
            {
                readPosition = "00000000";
                readSize = logSize;
                bytesToRead = int.Parse(logSize, System.Globalization.NumberStyles.HexNumber);
                if (bytesToRead < 2 * 0x200)
                {
                    //Not enough data, propaly no new tracks on the unit
                    return sectionList;
                }

            }
            // Read log contents, first chunk
            NMEAString = "$PMTK"
                + BT747Constants.PMTK_CMD_LOG_STR + ","
                + BT747Constants.PMTK_LOG_REQ_DATA_STR + ","
                + readPosition + ","
                + readSize + "*";
            
           // ResponceString = SendNMEA(NMEAString);
            ResponceString = readLog(NMEAString);

            int i = 0;
            if (NMEAString.Length >= 21)
            {
                if (ResponceString.StartsWith("$PMTK182,8,"))
                {

                    logstring = ResponceString.Substring(20, ResponceString.LastIndexOf('*') - 20); //Used to collect 20 responces into one logchunk which can be parsed
                    i = 1;
                }
            }
            
            //Send ack
           /*NMEAString = "$PMTK"
                + BT747Constants.PMTK_ACK_STR + ","
                + BT747Constants.PMTK_LOG_RESP_DATA_STR                
                + ",3*";
            */
            int currentPosition = 0;//int.Parse(ResponceString.Substring(11, 8), System.Globalization.NumberStyles.HexNumber);

            while (ResponceString.StartsWith("$PMTK001,182,7,3*20") == false) {

                // ResponceString = SendNMEA(NMEAString);
                ResponceString = readLog(" "/*NMEAString*/);

                if (ResponceString.StartsWith("$PMTK182,8")) 
                {
                    if (ResponceString.Length >= 20)
                    {
                        logstring = logstring + ResponceString.Substring(20, ResponceString.LastIndexOf('*') - 20); //Used to collect 20 responces into one logchunk which can be parsed
                        i = i + 1;
                        currentPosition += 0x800;
                        percentComplete = 100 * currentPosition / bytesToRead;    // int.Parse(logSize, System.Globalization.NumberStyles.HexNumber);
                        monitor.StatusText = String.Format(CommonResources.Text.Devices.ImportJob_Status_Reading, percentComplete.ToString("0") + "%");
                        monitor.PercentComplete = percentComplete / 100;
                    }
                }               
                //System.Diagnostics.Debug.WriteLine("BT747,ReadNMEATracks - Responce2: " + ResponceString);

                if (i >= 0x20 || ResponceString.StartsWith("$PMTK001,182,7,3*20")) //chunk full || end of log
                {

                    if (logstring.Length > 2 * 0x200)
                    {
                        //Read the block header
                        //System.Diagnostics.Debug.WriteLine("BT747,ReadNMEATracks - logstring: " + logstring);
                        LogBlockHeader logHeader = new LogBlockHeader(logstring.Substring(0, 40));

                        //System.Diagnostics.Debug.WriteLine("BT747,ReadNMEATracks - currentPosition: " + currentPosition + ", logSize: " + int.Parse(logSize, System.Globalization.NumberStyles.HexNumber));

                        ParseNMEALog(ref sectionList, ref trackSection, logstring, ref previous_logstring, logHeader, trackChange);

                        //System.Diagnostics.Debug.WriteLine("BT747,ReadNMEATracks - logstring length: " + logstring.Length + ", currentindex: " + i);

                        logstring = "";
                        i = 0;
                    }
                }               
            }
            sectionList.Add(trackSection);

            //Save the latest position
            Plugin.Instance.BT747LastPosition = logSize;            

            //System.Diagnostics.Debug.WriteLine("ReadNMEATracks - end: " + sectionList.Count, "BT747");
            return sectionList;
        }

        /**
         * This procedure goes through a responce string containing one log block and generates set of tracks from the 
         * block. Tracks are stored into list of trackfilesections.
         * 
         */
        private void ParseNMEALog(ref IList<BT747Packet.TrackFileSection> sectionList, ref BT747Packet.TrackFileSection trackSection, string logstring, ref string previous_logstring, LogBlockHeader logHeader, int trackChange)
        {
            //System.Diagnostics.Debug.WriteLine("BT747,ParseNMEALog - logstring: " + logstring);
            //System.Diagnostics.Debug.WriteLine("BT747,ParseNMEALog - logstring length: " + logstring.Length);
            
            WriteDebuglog("ParseNMEALog - logstring length: " + logstring.Length, configInfo.debug);

            //Remove the header string
            logstring = previous_logstring + logstring.Substring(2 * 0x200);

            int currentIndex = 0; //2 * 0x200; //2 chars per byte            
            bool end_of_chunk = false; //True if the current trackpoint is the first of the track

            while (logHeader.format.getMaxPacketSize() + currentIndex < logstring.Length && end_of_chunk == false)
            {
                end_of_chunk = BT747Packet.UnpackNMEATrackSection(logHeader, ref logstring, ref trackSection, trackChange, configInfo.debug);
                if (!end_of_chunk){
                    sectionList.Add(trackSection);
                    trackSection = null;
                    trackSection = new BT747Packet.TrackFileSection();
                }
            }
            //System.Diagnostics.Debug.WriteLine("BT747,ParseNMEALog - end_of_chunk: " + end_of_chunk + ", logstring: " + logstring);                
            
            previous_logstring = logstring;      
            logstring ="";
        }
        
        private static SerialPort OpenPort(int portNumber)
        {
            //System.Diagnostics.Debug.WriteLine("\nBT747: OpenPort start");
            try
            {
                port = new SerialPort("COM" + portNumber, 115200);
                port.ReadTimeout = port.WriteTimeout = 5000;
                //port.DtrEnable = true;
                //port.Handshake = Handshake.None;
                port.Open();

                if (port.IsOpen)
                {
                    return port;
                }
            }
            catch (Exception ex)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
                System.Diagnostics.Debug.WriteLine(ex.Message);

            }

            throw new Exception(CommonResources.Text.Devices.ImportJob_Status_CouldNotOpenDeviceError);
        }





        /**
         * 
         * This function reads the log contents from BT747 device
         * 
         */

        private string readLog(string NMEAString)
        {
            //string received = "";
            //int i = 0;
            string checksum = GetChecksum(NMEAString);
            NMEAString = NMEAString + checksum + "\r\n";

            
            if (port.IsOpen == false)
            {
                port.Open();
            }

            if (NMEAString.StartsWith("$PMTK"))
            {
                System.Diagnostics.Debug.WriteLine("BT747,readLog- Out: " + port.PortName + "," + NMEAString);
                WriteDebuglog("readLog - Out: " + NMEAString, configInfo.debug);
                port.Write(NMEAString);
            }
                        
            StringBuilder result = new StringBuilder("");
            bool eol = false;
            bool startfound = false;
            while (eol == false)
            {
                try
                {
                    int input = ' ';
                    
                    input = port.ReadChar();

                    // Remove extra characters

                    if (startfound)
                    {
                        if (input != '\r' && input != '\n')
                        {
                            result.Append((char)input);
                        }
                        else 
                            eol = true;
                            
                    }
                    else if (input == '$')
                    {
                        startfound = true;
                        result.Append((char)input);
                    }

                    //System.Diagnostics.Debug.WriteLine("BT747,sendNMEA: input: " + input);
                    
                }
                catch
                {
                }
            }
            System.Diagnostics.Debug.WriteLine("BT747,readLog: result: " + result);
            // if (received.StartsWith("$PMTK") == false)
            //System.Diagnostics.Debug.WriteLine("BT747,sendNMEA - Responce: "+received);
            WriteDebuglog("readLog - result: " + result.ToString(), configInfo.debug);

            return result.ToString();
        }

        /**
         * 
         * This function sends a NMEA string to the selected COM port and returns the responce string to caller
         * 
         */

        private string SendNMEA(string NMEAString)
        {
            string received = "a";
            int i=0;
            string checksum = GetChecksum(NMEAString);
            NMEAString = NMEAString + checksum + "\r\n";
             
            System.Diagnostics.Debug.WriteLine("BT747,sendNMEA: "+ port.PortName + "," + NMEAString);

            if (port.IsOpen == false)
            {
                port.Open();
            }
            
            WriteDebuglog("sendNMEA - Out: " + NMEAString, configInfo.debug);
            port.Write(NMEAString);
           
            while (received.StartsWith("$PMTK") == false)
            {
                received = port.ReadTo("\n");
                WriteDebuglog("sendNMEA - Responce: " + received, configInfo.debug);
                
                //Filter the incoming messages to minimize starting problems in the communication
                if (i == 50) {  
                    //System.Diagnostics.Debug.WriteLine("BT747,sendNMEA: "+ port.PortName + "," + NMEAString);
                    port.Write(NMEAString);
                    i=0;
                }
                i++;
            }
            // if (received.StartsWith("$PMTK") == false)
            //System.Diagnostics.Debug.WriteLine("BT747,sendNMEA - Responce: "+received);
            //WriteDebuglog("sendNMEA - Responce: " + received, configInfo.debug);
            
            return received;
        }

        private static BT747Packet.Response SendPacket(SerialPort port, byte[] packet)
        {
            BT747Packet.Response received = new BT747Packet.Response();

            port.Write(packet, 0, packet.Length);

            received.CommandId = (byte)port.ReadByte();
            int hiPacketLen = port.ReadByte();
            int loPacketLen = port.ReadByte();
            received.PacketLength = (Int16)((hiPacketLen << 8) + loPacketLen);
            received.PacketData = new byte[received.PacketLength];
            for (Int16 b = 0; b < received.PacketLength; b++)
            {
                received.PacketData[b] = (byte)port.ReadByte();
            }
            received.Checksum = (byte)port.ReadByte();
            return received;
        }
        public static void WriteDebuglog(string text,bool debug)
        {
            if (debug) 
            {
                System.Diagnostics.Debug.WriteLine("WriteDebuglog: " + text, "BT747");
                //System.Diagnostics.Debug.WriteLine("WriteDebuglog: " + System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\BT747Debug.log", "BT747");

                System.IO.StreamWriter file = new System.IO.StreamWriter(@System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\BT747Debug.log", true);
                file.WriteLine(System.DateTime.Now.ToString("dd.MM.yy hh:mm:ss.fff")+": "+text);
                file.Close();
            }  
        }

     /*   private void SavePosition(string logSize)
        {
              // = logSize; 
            
            
            System.Diagnostics.Debug.WriteLine("SavePosition: " + logSize, "BT747");
            int position = int.Parse(logSize, System.Globalization.NumberStyles.HexNumber) / 0x10000;
            position *= 0x10000;
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"position.txt");
            file.Write(position.ToString("X8"));
            file.Close();
        }

        private static string ReadPosition()
        {
            System.Diagnostics.Debug.WriteLine("ReadPosition: ", "\nBT747");
            // Read the file as one string.
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(@"position.txt");
                string position = file.ReadToEnd();
                file.Close();
                position = position.Replace(Environment.NewLine, "");
                System.Diagnostics.Debug.WriteLine("ReadPosition: #" + position+"#", "BT747");
                return position;
            }
            catch
            {
                System.IO.FileNotFoundException e;
                System.Diagnostics.Debug.WriteLine("ReadPosition: FileNotFoundException", "BT747");
                return "00000000";
            }
        }
        */
        public static SerialPort port;
        private DeviceConfigurationInfo settings;
       /* {
            set { settings = value; }
            get {return settings;}
        }*/
        private int portNumber;
        private DeviceConfigurationInfo configInfo;
        
    }
}
