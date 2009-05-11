using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.BT747
{
    class DeviceConfigurationInfo
    {
        public static DeviceConfigurationInfo Parse(string configurationInfo)
        {
            //System.Diagnostics.Debug.WriteLine("\nBT747: configurationInfo: " + configurationInfo);
            DeviceConfigurationInfo configInfo = new DeviceConfigurationInfo();
            if (configurationInfo != null)
            {
                string[] configurationParams = configurationInfo.Split(';');
                foreach (string configurationParam in configurationParams)
                {
                    string[] parts = configurationParam.Split('=');                    
                    
                    if (parts.Length == 2)
                    {
                        //System.Diagnostics.Debug.WriteLine("BT747: configInfo 0: " + parts[0]);
                        //System.Diagnostics.Debug.WriteLine("BT747: configInfo 1: " + parts[1]);
                        
                        switch (parts[0])
                        {
                            case "newonly":
                                configInfo.ImportOnlyNew = parts[1] == "1";                               
                                break;
                          /*  case "lastposition":
                                if (configInfo.ImportOnlyNew)
                                    configInfo.lastPosition = parts[1]; 
                                else
                                    configInfo.lastPosition = "00000000";
                                break;*/
                            case "port":
                                
                                /*if (parts[1].StartsWith("COM"))
                                    configInfo.portNumber = int.Parse(parts[1].Substring(3));
                                else*/
                                configInfo.portNumber = int.Parse(parts[1]);                                
                                break;
                            case "trackchange":
                                configInfo.trackChange = int.Parse(parts[1]);                                
                                break;
                        }
                    }
                }
            }
            return configInfo;
        }

        private DeviceConfigurationInfo()
        {
        }

        public override string ToString()
        {
            return "newonly=" + (ImportOnlyNew ? "1" : "0") +
                  // ";lastposition=" + lastPosition.ToString() +
                   ";port=" + portNumber.ToString() +
                   ";trackchange=" + trackChange.ToString();
        }

        public bool ImportOnlyNew = true;
//        public String lastPosition = "00000000";
        public int portNumber = 6;
        public int trackChange = 1;
    }
}
