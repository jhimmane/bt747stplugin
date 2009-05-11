using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.BT747
{
    class BT747Packet
    {
        public class Header
        {
            public DateTime StartTime;
            public long StartTimeUtc;
            public TimeSpan TotalTime;
            public double TotalDistanceMeters;
            public Int16 TotalCalories;
            public Int16 MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            public Int16 TrackPointCount;
        }

        public class TrackFileHeader : Header
        {
            public Int16 TrackPointIndex;
        }

        public class TrackFileSection : Header
        {
            public Int16 StartPointIndex;
            public Int16 EndPointIndex;
            public IList<TrackPoint> TrackPoints = new List<TrackPoint>();
        }

        public class TrackPoint
        {
            public double Latitude; 
            public double Longitude; 
            public float Altitude; // Meters
            public float Speed; // Kilometers per hour
       //     public Byte HeartRate;
            public DateTime PointTime; 

            public string ToString(){
                return "Trackpoint: lat="+Latitude+" lon="+Longitude+" >\n"+
                       "ele:"+Altitude+"\n"+
                        "time:"+PointTime+"\n"+
                        //"course"43.0499"/course"\n"+
                        "speed:"+Speed+"\n";
                        //"name"trkpt-2008-12-09T12:24:09.000Z"/name"\n"+
                        //"cmt""![CDATA[,T,99.99,2]]""/cmt"\n"+
                       // "type"T"/type"\n"+
                        //"sat"2"/sat"\n"+
                        //"hdop"99.99"/hdop"\n"+
                        //"vdop"1.00"/vdop"\n"+
                        //"pdop"99.99"/pdop"\n"+
                        //"ageofdgpsdata"0"/ageofdgpsdata"\n"+
                        //"dgpsid"0"/dgpsid"\n"+
                        //</trkpt"
            }
        }

        public class Response
        {
            public byte CommandId;
            public Int16 PacketLength;
            public byte[] PacketData;
            public byte Checksum;
        }

       
        public static IList<TrackFileHeader> UnpackNMEATrackHeaders(string responce)
        {
            int numHeaders = responce.Length / 24;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i * 24;
                TrackFileHeader header = new TrackFileHeader();
               // ReadHeader(header, responce, trackStart);
                //header.TrackPointCount = ReadInt16(responce, trackStart + 20);
                //header.TrackPointIndex = ReadInt16(responce, trackStart + 22);
                headers.Add(header);
            }
            return headers;
        }

        public static bool UnpackNMEATrackSection(LogBlockHeader logHeader, ref string logstring, ref TrackFileSection trackSection, int trackChange)
        {
            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - logstring:" + logstring);
            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - logstring length:" + logstring.Length);
                        
            int currentIndex=0, previousindex = 0, nsat = 0;
            bool firstPoint = true, valid = true;

            while (logHeader.format.getMaxPacketSize() + currentIndex < logstring.Length && 
                  (logstring.Substring(currentIndex, 8).StartsWith("FFFFFFFF")) == false)
            {
               // System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - progress: " + currentIndex + "/" + logstring.Length +
                //                                    " Maxpacket: "+logHeader.format.getMaxPacketSize()   );
                previousindex = currentIndex;
                //sep_record - not handled for now
                if (logstring.Substring(currentIndex, 14) == "AAAAAAAAAAAAAA")
                {
                    //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - sep_record");
                    switch (int.Parse(logstring.Substring(currentIndex + 14, 2)))
                    {
                        case 2: break;  //0x02 - Log bitmask change [long bitmask]
                        case 3: break;  //0x03 - Log period change [word period/10 sec]
                        case 4: break;  //0x04 - Log distance change [word distance/10 m]
                        case 5: break;  //0x05 - Log speed change [word speed/10 km/h]
                        case 6: break;  //0x06 - Log overwrite/log stop change
                                        //- argument = same as log status (PMTK182,2,7 response)
                        case 7: break;  //0x07 - Log on/off change
                                        // - argument = same as log status (P¨MTK182,2,7 response)*/
                    }
                    currentIndex += 2 * 16;  // Size of sep_record
                }
                //We are only interested on points with all the necessary data
                else
                {
                    //read the log block
                    //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - 3: ");
                    BT747Packet.TrackPoint trackpoint = new BT747Packet.TrackPoint();
                    if (logHeader.getFormat().hasUTC())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - currentIndex: " + currentIndex);
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - substring: " + logstring.Substring(currentIndex, 8));
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - reverse: " + LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 8)));
                        
                        if (firstPoint)
                        {
                            trackSection.StartTime = System.DateTime.Parse("1/1/1970");
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - StartTime1: " + trackSection.StartTime);
                            
                            trackSection.StartTimeUtc = long.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 8)), System.Globalization.NumberStyles.HexNumber);

                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - StartTimeUtc: " + trackSection.StartTimeUtc);

                            trackSection.StartTime = trackSection.StartTime.AddSeconds(trackSection.StartTimeUtc);
                            
                            //TODO: REMOVE!!!!!!!!!!!!!!!!
                            //trackSection.StartTime = trackSection.StartTime.AddSeconds(60 * 60 * 24);
                            
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - StartTime2: " + trackSection.StartTime);                            

                            trackpoint.PointTime = trackSection.StartTime;
                        }
                        else
                        {
                            long pointTime = long.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 8)), System.Globalization.NumberStyles.HexNumber);
                            
                            trackpoint.PointTime = System.DateTime.Parse("1/1/1970");

                            trackpoint.PointTime = trackpoint.PointTime.AddSeconds(pointTime);

                            //TODO: REMOVE!!!!!!!!!!!!!!!!
                            //trackpoint.PointTime = trackpoint.PointTime.AddSeconds(60*60*24);

                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - PointTime: " + trackpoint.PointTime);
                            
                        }
                            
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - StartTime3: " + trackSection.StartTime);
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - PointTime: " + trackpoint.PointTime);
                        
                        currentIndex += 2 * 4;
                    }
                    if (logHeader.getFormat().hasFix())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - fix: " + logstring.Substring(currentIndex, 4));
                        if (firstPoint && short.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 4)), System.Globalization.NumberStyles.HexNumber) == 1)                            
                            valid = false;                        
                        currentIndex += 2 * 2;
                    }
                    if (logHeader.getFormat().hasLatitude())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - latitude: " + logstring.Substring(currentIndex, 16));
                        long num = long.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 16)), System.Globalization.NumberStyles.HexNumber);
                        byte[] floatVals = BitConverter.GetBytes(num);
                        trackpoint.Latitude = BitConverter.ToDouble(floatVals, 0);

                        //trackpoint.Latitude = long.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 16)), System.Globalization.NumberStyles.HexNumber);
                        
                        currentIndex += 2 * 8;
                    }
                    if (logHeader.getFormat().hasLongitude())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - longitude: " + logstring.Substring(currentIndex, 16));
                        long num = long.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 16)), System.Globalization.NumberStyles.HexNumber);
                        byte[] floatVals = BitConverter.GetBytes(num);
                        trackpoint.Longitude = BitConverter.ToDouble(floatVals, 0);

                        //trackpoint.Longitude = long.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 16)), System.Globalization.NumberStyles.HexNumber);
                        currentIndex += 2 * 8;
                    }
                    if (logHeader.getFormat().hasHeight())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - height: " + logstring.Substring(currentIndex, 8));

                        uint num = uint.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 8)), System.Globalization.NumberStyles.HexNumber);
                        byte[] floatVals = BitConverter.GetBytes(num);
                        trackpoint.Altitude = BitConverter.ToSingle(floatVals, 0);
                        
                       //trackpoint.Altitude = int.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 8)), System.Globalization.NumberStyles.HexNumber);
                        
                        currentIndex += 2 * 4;
                    }
                    if (logHeader.getFormat().hasSpeed())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - speed: " + logstring.Substring(currentIndex, 8));
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - speed: " + speed);
                        
                        //TODO: This should be in km/h according to documentation. What is correct format for sporttracks?                           
                        uint num = uint.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 8)), System.Globalization.NumberStyles.HexNumber);
                        byte[] floatVals = BitConverter.GetBytes(num);
                        trackpoint.Speed = BitConverter.ToSingle(floatVals, 0);
                        
                        if (firstPoint && trackpoint.Speed == 0)
                            valid = false;
                        
                       // else if (trackpoint.Speed > 0)
                       //    System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - trackpoint.speed: " + trackpoint.Speed);
                        
                        currentIndex += 2 * 4;
                    }
                    if (logHeader.getFormat().hasHeading())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - heading: " + logstring.Substring(currentIndex, 8));
                        currentIndex += 2 * 4;
                    }
                    if (logHeader.getFormat().hasDSta())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - dsta: " + logstring.Substring(currentIndex, 4));
                        currentIndex += 2 * 2;
                    }
                    if (logHeader.getFormat().hasDAge())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - dage: " + logstring.Substring(currentIndex, 8));
                        currentIndex += 2 * 4;
                    }
                    if (logHeader.getFormat().hasPDOP())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - pdop: " + logstring.Substring(currentIndex, 4));
                        currentIndex += 2 * 2;
                    }
                    if (logHeader.getFormat().hasHDOP())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - hdop: " + logstring.Substring(currentIndex, 4));
                        currentIndex += 2 * 2;
                    }
                    if (logHeader.getFormat().hasVDOP())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - vdop: " + logstring.Substring(currentIndex, 4));
                        currentIndex += 2 * 2;
                    }
                    if (logHeader.getFormat().hasNSat())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - nsat: " + logstring.Substring(currentIndex, 4));
                        nsat = int.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 2)), System.Globalization.NumberStyles.HexNumber);
                        currentIndex += 2 * 2;
                    }
                    if (logHeader.getFormat().hasSID())
                    {
                        if (nsat == 0) nsat = 1;
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - sid: " + logstring.Substring(currentIndex, nsat*8));
                        currentIndex += nsat * 2 * (1 + 1 + 2);
                    }
                    if (logHeader.getFormat().hasElevation())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - elevation: " + logstring.Substring(currentIndex, 4 * nsat));
                        currentIndex += 2 * 2* nsat;
                    }
                    if (logHeader.getFormat().hasAzimuth())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - azimuth: " + logstring.Substring(currentIndex, 4 * nsat));
                        currentIndex += 2 * 2 * nsat;
                    }
                    if (logHeader.getFormat().hasSNR())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - snr: " + logstring.Substring(currentIndex, 4 * nsat));
                        currentIndex += 2 * 2 * nsat;
                    }
                    if (logHeader.getFormat().hasRCR())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - rcr: " + logstring.Substring(currentIndex, 4));
                        currentIndex += 2 * 2;
                    }
                    if (logHeader.getFormat().hasMSec())
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - msec: " + logstring.Substring(currentIndex, 4));
                        currentIndex += 2 * 2;
                    }
                    if (logHeader.getFormat().hasDist())
                    {
                        //if (trackpoint.Speed > 0)
                        //{
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - distance: " + logstring.Substring(currentIndex, 16));

                            long num = long.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 16)), System.Globalization.NumberStyles.HexNumber);
                            byte[] floatVals = BitConverter.GetBytes(num);
                            //system.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - floatVals: " + floatVals[0);
                            double meters = BitConverter.ToDouble(floatVals, 0);
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - distance2: " + meters);
                            
                            //if (meters > 10) meters = 0;
                            
                            trackSection.TotalDistanceMeters += meters;
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - distance3: " + trackSection.TotalDistanceMeters);

                        //}
                        //trackSection.TotalDistanceMeters += long.Parse(LogBlockHeader.ReverseBytes(logstring.Substring(currentIndex, 16)), System.Globalization.NumberStyles.HexNumber);
                        currentIndex += 2 * 8;
                    }
                    /*if (logHeader.getFormat().isLowPrecision())
                    {
                    }*/
                    if (valid)
                    {
                        if (firstPoint)
                        {
                            trackSection.TrackPoints.Add(trackpoint);
                            trackSection.TrackPointCount += 1;
                            firstPoint = false;
                        }
                        else if (trackpoint.PointTime.Ticks - trackSection.TrackPoints[trackSection.TrackPointCount - 1].PointTime.Ticks < trackChange * TimeSpan.TicksPerMinute)
                        {
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - add: " + trackpoint.PointTime + " - " + trackSection.TrackPoints[trackSection.TrackPointCount-1].PointTime);
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - add: " + trackSection.TrackPointCount);
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - add: " + trackpoint.PointTime.Ticks + " - " + trackSection.TrackPoints[trackSection.TrackPointCount-1].PointTime.Ticks);
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - add: " + (trackpoint.PointTime.Ticks - trackSection.TrackPoints[trackSection.TrackPointCount-1].PointTime.Ticks));
                            trackSection.TrackPoints.Add(trackpoint);
                            trackSection.TrackPointCount += 1;

                        }
                        else if ((trackpoint.PointTime.Ticks - trackSection.TrackPoints[trackSection.TrackPointCount-1].PointTime.Ticks) > trackChange * TimeSpan.TicksPerMinute )
                        {
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - change track: " + trackChange);
                            //Change track                        
                            logstring = logstring.Substring(previousindex);
                            //end_of_chunk = false;
                            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - change track: " + (trackpoint.PointTime.Ticks - trackSection.TrackPoints[trackSection.TrackPointCount-1].PointTime.Ticks));
                            return false;
                        }
                    }
                    valid = true;
                    //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - checksum: " + logstring.Substring(currentIndex, 4));
                    currentIndex += 2 * 2; // move over the "*checsum" (2A XX)
                }
            }
            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - currentIndex: " + currentIndex + ",previousindex: " + previousindex + ",length: " + logstring.Length);
            //The last point belongs to next track or is incomplete

            if (currentIndex >= logstring.Length) logstring = "";
            else logstring = logstring.Substring(currentIndex);
            
            if (logstring.StartsWith("FFFFFFFF")) logstring = "";
            
            //end_of_chunk = true;
            //System.Diagnostics.Debug.WriteLine("BT747,UnpackNMEATrackSection - end" );
            return true;
        }


        public static byte SendPacketCommandId(byte[] packet)
        {
            return packet[3];
        }

        public static byte[] GetSystemConfiguration()
        {
            byte[] payload = new byte[1];
            payload[0] = 0x85;
            return ConstructPayload(payload);
        }

        public static byte[] GetTrackFileHeaders()
        {
            byte[] payload = new byte[1];
            payload[0] = 0x78;
            return ConstructPayload(payload);
        }

        public static byte[] GetTrackFileSections(IList<Int16> trackPointIndexes)
        {
            byte[] payload = new byte[3 + trackPointIndexes.Count * 2];
            payload[0] = 0x80;
            Write(payload, 1, (Int16)trackPointIndexes.Count);
            int offset = 3;
            foreach (Int16 index in trackPointIndexes)
            {
                Write(payload, offset, index);
                offset += 2;
            }
            return ConstructPayload(payload);
        }

        public static byte[] GetNextSection()
        {
            byte[] payload = new byte[1];
            payload[0] = 0x81;
            return ConstructPayload(payload);
        }

        private BT747Packet()
        {
        }

        private static void ReadHeader(Header header, byte[] payload, int offset)
        {
            header.StartTime = ReadDateTime(payload, offset);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(payload, offset + 6)) / 10);
            header.TotalDistanceMeters = ReadInt32(payload, offset + 10);
            header.TotalCalories = ReadInt16(payload, offset + 14);
            header.MaximumSpeed = ReadInt16(payload, offset + 16);
            header.MaximumHeartRate = payload[offset + 18];
            header.AverageHeartRate = payload[offset + 19];
        }

        private static byte[] ConstructPayload(byte[] payload)
        {
            Int16 payloadLen = (Int16)payload.Length;
            byte[] payloadLenBytes = BitConverter.GetBytes(payloadLen);
            byte hiPayloadLen = payloadLenBytes[1];
            byte loPayloadLen = payloadLenBytes[0];
            byte[] data = new byte[4 + payloadLen];
            data[0] = 0x02;
            data[1] = hiPayloadLen;
            data[2] = loPayloadLen;

            byte checksum = 0;
            checksum ^= hiPayloadLen;
            checksum ^= loPayloadLen;
            for (int i = 0; i < payloadLen; i++)
            {
                data[3+i] = payload[i];
                checksum ^= payload[i];
            }
            data[payloadLen + 3] = checksum;
            return data;
        }

        /// <summary>
        /// Read a six byte representation of a date and time starting at the offset in the following format:
        /// Year = 2000 + byte[0]
        /// Month = byte[1]
        /// Day = byte[2]
        /// Hour = byte[3] 
        /// Minute = byte[4]
        /// Second = byte[5]
        /// </summary>
        private static DateTime ReadDateTime(byte[] data, int offset)
        {
            return new DateTime(data[offset + 0] + 2000, data[offset + 1], data[offset + 2], data[offset + 3], data[offset + 4], data[offset + 5]);
        }

        /// <summary>
        /// Read a two byte integer in big-endian format starting at the offset.
        /// </summary>
        private static Int16 ReadInt16(byte[] data, int offset)
        {
            return (Int16)((data[offset] << 8) + data[offset + 1]);
        }

        /// <summary>
        /// Read a four byte integer in big-endian format starting at the offset.
        /// </summary>
        private static Int32 ReadInt32(byte[] data, int offset)
        {
            return (data[offset] << 24) + (data[offset + 1] << 16) + (data[offset + 2] << 8) + data[offset + 3];
        }

        /// <summary>
        /// Write a two byte integer in big-endian format starting at the offset.
        /// </summary>
        private static void Write(byte[] data, int offset, Int16 i)
        {
            byte[] b = BitConverter.GetBytes(i);
            data[offset + 0] = b[1];
            data[offset + 1] = b[0];
        }

        /// <summary>
        /// Write a four byte integer in big-endian format starting at the offset.
        /// </summary>
        private static void Write(byte[] data, int offset, Int32 i)
        {
            byte[] b = BitConverter.GetBytes(i);
            data[offset + 0] = b[3];
            data[offset + 1] = b[2];
            data[offset + 2] = b[1];
            data[offset + 3] = b[0];
        }
    }
}
