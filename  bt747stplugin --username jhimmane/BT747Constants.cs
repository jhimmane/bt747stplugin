//********************************************************************
//***                           BT 747                             ***
//***                      April 14, 2007                          ***
//***                  (c)2007 Mario De Weerd                      ***
//***                     m.deweerd@ieee.org                       ***
//***  **********************************************************  ***
//***  Software is provided "AS IS," without a warranty of any     ***
//***  kind. ALL EXPRESS OR IMPLIED REPRESENTATIONS AND WARRANTIES,***
//***  INCLUDING ANY IMPLIED WARRANTY OF MERCHANTABILITY, FITNESS  ***
//***  FOR A PARTICULAR PURPOSE OR NON-INFRINGEMENT, ARE HEREBY    ***
//***  EXCLUDED. THE ENTIRE RISK ARISING OUT OF USING THE SOFTWARE ***
//***  IS ASSUMED BY THE USER. See the GNU General Public License  ***
//***  for more details.                                           ***
//***  *********************************************************** ***
//***  The application was written using the SuperWaba toolset.    ***
//***  This is a proprietary development environment based in      ***
//***  part on the Waba development environment developed by       ***                                   
//***  WabaSoft, Inc.                                              ***
//********************************************************************                              
using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.BT747
{


    /**
     * Constants for the iBlue 747 (BT747) device
     * 
     * @author Mario De Weerd
     */
    public class BT747Constants
    { // dev as in device

        /**
         * String description of the log format items of the iBlue 747 device.<br>
         * Entries are in order. The entry position corresponds to the bit position
         * in the log format 'byte'.
         */
      //  public static String[] logFmtItems = Txt.logFmtItems;
        /** Index of bit for log format setting */
        public static int FMT_UTC_IDX = 0;
        /** Index of bit for log format setting */
        public static int FMT_VALID_IDX = 1;
        /** Index of bit for log format setting */
        public static int FMT_LATITUDE_IDX = 2;
        /** Index of bit for log format setting */
        public static int FMT_LONGITUDE_IDX = 3;
        /** Index of bit for log format setting */
        public static int FMT_HEIGHT_IDX = 4;
        /** Index of bit for log format setting */
        public static int FMT_SPEED_IDX = 5;
        /** Index of bit for log format setting */
        public static int FMT_HEADING_IDX = 6;
        /** Index of bit for log format setting */
        public static int FMT_DSTA_IDX = 7;
        /** Index of bit for log format setting */
        public static int FMT_DAGE_IDX = 8;
        /** Index of bit for log format setting */
        public static int FMT_PDOP_IDX = 9;
        /** Index of bit for log format setting */
        public static int FMT_HDOP_IDX = 10;
        /** Index of bit for log format setting */
        public static int FMT_VDOP_IDX = 11;
        /** Index of bit for log format setting */
        public static int FMT_NSAT_IDX = 12;
        public static int FMT_MAX_SATS = 32; // Guess (maximum)
        /** Index of bit for log format setting */
        public static int FMT_SID_IDX = 13;
        /** Index of bit for log format setting */
        public static int FMT_ELEVATION_IDX = 14;
        /** Index of bit for log format setting */
        public static int FMT_AZIMUTH_IDX = 15;
        /** Index of bit for log format setting */
        public static int FMT_SNR_IDX = 16;
        /** Index of bit for log format setting */
        public static int FMT_RCR_IDX = 17;
        /** Index of bit for log format setting */
        public static int FMT_MILLISECOND_IDX = 18;
        /** Index of bit for log format setting */
        public static int FMT_DISTANCE_IDX = 19;
        /** Index of bit for log format setting */
        public static int FMT_LOG_PTS_WITH_VALID_FIX_ONLY_IDX = 31;

        /**
         * Size for each item of the log format of the iBlue 747.<br>
         * <b>The log record length is variable is satelite information is logged:</b><br>
         * SID, ELEVATION, AZIMUTH and SNR are repeated NSAT times.<br>
         * <br>
         * Entries are in order. The entry position corresponds to the bit position
         * in the log format 'byte'.
         */
        public static int[] logFmtByteSizes = { 4, // "UTC", // = 0x00001
            // // // 0
            2, // "VALID", // = 0x00002 // 1
            8, // "LATITUDE", // = 0x00004 // 2
            8, // "LONGITUDE",// = 0x00008 // 3
            4, // "HEIGHT", // = 0x00010 // 4
            4, // "SPEED", // = 0x00020 // 5
            4, // "HEADING", // = 0x00040 // 6
            2, // "DSTA", // = 0x00080 // 7
            4, // "DAGE", // = 0x00100 // 8
            2, // "PDOP", // = 0x00200 // 9
            2, // "HDOP", // = 0x00400 // A
            2, // "VDOP", // = 0x00800 // B
            2, // "NSAT", // = 0x01000 // C
            4, // "SID", // = 0x02000 // D
            2, // "ELEVATION",// = 0x04000 // E
            2, // "AZIMUTH", // = 0x08000 // F
            2, // "SNR", // = 0x10000 // 10
            2, // "RCR", // = 0x20000 // 11
            2, // "MILISECOND",// = 0x40000 // 12
            8, // "DISTANCE" // = 0x80000 // 13
            0, // 14
            0, // 15
            0, // 16
            0, // 17
            0, // 18
            0, // 19
            0, // 1A
            0, // 1B
            0, // 1C
            0, // 1D
            0, // 1E
            0, // 1F // Log points with valid fix only
    };

        public static int[] logFmtByteSizesHolux = { 4, // "UTC", // = //
            // 0x00001 // 0
            2, // "VALID", // = 0x00002 // 1
            4, // "LATITUDE", // = 0x00004 // 2
            4, // "LONGITUDE",// = 0x00008 // 3
            3, // "HEIGHT", // = 0x00010 // 4
            4, // "SPEED", // = 0x00020 // 5
            4, // "HEADING", // = 0x00040 // 6
            2, // "DSTA", // = 0x00080 // 7
            4, // "DAGE", // = 0x00100 // 8
            2, // "PDOP", // = 0x00200 // 9
            2, // "HDOP", // = 0x00400 // A
            2, // "VDOP", // = 0x00800 // B
            2, // "NSAT", // = 0x01000 // C
            4, // "SID", // = 0x02000 // D
            2, // "ELEVATION",// = 0x04000 // E
            2, // "AZIMUTH", // = 0x08000 // F
            2, // "SNR", // = 0x10000 // 10
            2, // "RCR", // = 0x20000 // 11
            2, // "MILISECOND",// = 0x40000 // 12
            8, // "DISTANCE" // = 0x80000 // 13
            0, // 14
            0, // 15
            0, // 16
            0, // 17
            0, // 18
            0, // 19
            0, // 1A
            0, // 1B
            0, // 1C
            0, // 1D
            0, // 1E
            0, // 1F // Log points with valid fix only
    };

        public static int RCR_TIME_MASK = 0x01;
        public static int RCR_SPEED_MASK = 0x02;
        public static int RCR_DISTANCE_MASK = 0x04;
        public static int RCR_BUTTON_MASK = 0x08;
        public static int RCR_APP1_MASK = 0x0010;
        public static int RCR_APP2_MASK = 0x0020;
        public static int RCR_APP3_MASK = 0x0040;
        public static int RCR_APP4_MASK = 0x0080;
        public static int RCR_APP5_MASK = 0x0100;
        public static int RCR_APP6_MASK = 0x0200;
        public static int RCR_APP7_MASK = 0x0400;
        public static int RCR_APP8_MASK = 0x0800;
        public static int RCR_APP9_MASK = 0x1000;
        public static int RCR_APPX_MASK = 0x2000;
        public static int RCR_APPY_MASK = 0x4000;
        public static int RCR_APPZ_MASK = 0x8000;
        public static int RCR_ALL_APP_MASK = 0xFFF0;

      //  public static String[] C_STR_RCR = Txt.C_STR_RCR;
        public static int C_RCR_COUNT = 16;

        // PMTK182 commands/replies.
        /** Set a parameter concerning the log */
        public static int PMTK_LOG_SET = 1;
        /** Get a query concerning the log */
        public static int PMTK_LOG_Q = 2;
        /** Received data concerning the log */
        public static int PMTK_LOG_DT = 3; // REPLY
        public static int PMTK_LOG_ON = 4;
        public static int PMTK_LOG_OFF = 5;
        public static int PMTK_LOG_ERASE = 6;
        public static int PMTK_LOG_Q_LOG = 7;
        public static int PMTK_LOG_DT_LOG = 8; // REPLY.
        public static int PMTK_LOG_INIT = 9;
        public static int PMTK_LOG_ENABLE = 10;
        public static int PMTK_LOG_DISABLE = 11;
        public static int PMTK_LOG_WRITE = 12;

        // PMTK182,1,DATA (DATA = what parameter to set/read/replied)
        // PMTK182,1,DATA,CMD (DATA = what parameter to set/read/replied, CMD =
        // extra param)

        public static int PMTK_LOG_USER = 1; // User initiated log point,
        // takes RCR as param.
        public static int PMTK_LOG_FLASH_STAT = 1; // User initiated log
        // point, takes RCR as
        // param.
        public static int PMTK_LOG_FORMAT = 2;
        public static int PMTK_LOG_TIME_INTERVAL = 3;
        public static int PMTK_LOG_DISTANCE_INTERVAL = 4;
        public static int PMTK_LOG_SPEED_INTERVAL = 5;
        public static int PMTK_LOG_REC_METHOD = 6;
        public static int PMTK_LOG_LOG_STATUS = 7;
        public static int PMTK_LOG_MEM_USED = 8; // bit 2 = logging on/off
        public static int PMTK_LOG_FLASH = 9; // Requires extra param
        public static int PMTK_LOG_NBR_LOG_PTS = 10;
        public static int PMTK_LOG_FLASH_SECTORS = 11;
        public static int PMTK_LOG_VERSION = 12;

        // Other MTK commands
        public static int PMTK_TEST = 000;
        public static int PMTK_ACK = 001;
        public static int PMTK_SYS_MSG = 010;
        public static int PMTK_CMD_HOT_START = 101;
        public static int PMTK_CMD_WARM_START = 102;
        public static int PMTK_CMD_COLD_START = 103;
        public static int PMTK_CMD_FULL_COLD_START = 104;
        public static int PMTK_CMD_LOG = 182;
        public static int PMTK_SET_NMEA_BAUD_RATE = 251;
        /* REPLIES: SET - Set values same as futher below, but 3XX */
        public static int PMTK_API_SET_FIX_CTL = 300;
        public static int PMTK_API_SET_DGPS_MODE = 301;
        public static int PMTK_API_SET_SBAS = 313;
        /**
         * Define NMEA output string periods.
         */
        public static int PMTK_API_SET_NMEA_OUTPUT = 314;
        public static int PMTK_API_SET_SBAS_TEST = 319;
        public static int PMTK_API_SET_PWR_SAV_MODE = 320;
        public static int PMTK_API_SET_DATUM = 330;
        public static int PMTK_API_SET_DATUM_ADVANCE = 331;
        public static int PMTK_API_SET_USER_OPTION = 390;
        public static int PMTK_API_SET_BT_MAC_ADDR = 392;
        /* REPLIES: Q - Same as above, but 4XX in stead of 3XX */
        public static int PMTK_API_Q_FIX_CTL = 400;
        public static int PMTK_API_Q_DGPS_MODE = 401;
        public static int PMTK_API_Q_SBAS = 413;
        public static int PMTK_API_Q_NMEA_OUTPUT = 414;
        public static int PMTK_API_Q_SBAS_TEST = 419;
        public static int PMTK_API_Q_PWR_SAV_MOD = 420;
        public static int PMTK_API_Q_DATUM = 430;
        public static int PMTK_API_Q_DATUM_ADVANCE = 431;
        public static int PMTK_API_Q_GET_USER_OPTION = 490;
        public static int PMTK_API_Q_BT_MAC_ADDR = 492;
        /* REPLIES: DT - Same as above, but 5XX in stead of 4XX */
        public static int PMTK_DT_FIX_CTL = 500;
        public static int PMTK_DT_DGPS_MODE = 501;
        public static int PMTK_DT_SBAS = 513;
        public static int PMTK_DT_NMEA_OUTPUT = 514;
        public static int PMTK_DT_SBAS_TEST = 519;
        public static int PMTK_DT_PWR_SAV_MODE = 520;
        public static int PMTK_DT_DATUM = 530;
        public static int PMTK_DT_FLASH_USER_OPTION = 590;
        public static int PMTK_DT_BT_MAC_ADDR = 592;

        /* Special requests * SW ? */
        public static int PMTK_Q_DGPS_INFO = 602;
        public static int PMTK_Q_VERSION = 604;
        public static int PMTK_Q_RELEASE = 605;

        public static int PMTK_DT_DGPS_INFO = 702;
        public static int PMTK_DT_VERSION = 704;
        public static int PMTK_DT_RELEASE = 705;

        /**
         * Parameter 1 of PMTK_ACK reply. Packet invalid.
         */
        public static int PMTK_ACK_INVALID = 0;
        /**
         * Parameter 1 of PMTK_ACK reply. Packet not supported by device.
         */
        public static int PMTK_ACK_UNSUPPORTED = 1;
        /**
         * Parameter 1 of PMTK_ACK reply. Packet supported, but action failed.
         */
        public static int PMTK_ACK_FAILED = 2;
        /**
         * Parameter 1 of PMTK_ACK reply. Packet success.
         */
        public static int PMTK_ACK_SUCCEEDED = 3;

        // The next constants are the same as above but convered to strings.
        public static String PMTK_LOG_SET_STR = Convert.ToString(PMTK_LOG_SET);   

        public static String PMTK_LOG_QUERY_STR = Convert
                .ToString(PMTK_LOG_Q);
        public static String PMTK_LOG_RESP_STR = Convert
                .ToString(PMTK_LOG_DT); // REPLY
        public static String PMTK_LOG_ON_STR = Convert.ToString(PMTK_LOG_ON);
        public static String PMTK_LOG_OFF_STR = Convert
                .ToString(PMTK_LOG_OFF);
        public static String PMTK_LOG_ERASE_STR = Convert
                .ToString(PMTK_LOG_ERASE);
        public static String PMTK_LOG_REQ_DATA_STR = Convert
                .ToString(PMTK_LOG_Q_LOG);
        public static String PMTK_LOG_RESP_DATA_STR = Convert
                .ToString(PMTK_LOG_DT_LOG); // REPLY.
        public static String PMTK_LOG_INIT_STR = Convert
                .ToString(PMTK_LOG_INIT);

        public static String PMTK_LOG_ERASE_YES_STR = "1";

        // PMTK182,1,DATA (DATA_STR = Convert.ToString((DATA)what parameter to
        // set/read/replied)

        public static String PMTK_LOG_USER_STR = Convert
                .ToString(PMTK_LOG_USER);
        public static String PMTK_LOG_FLASH_STAT_STR = Convert
                .ToString(PMTK_LOG_FLASH_STAT);
        public static String PMTK_LOG_FORMAT_STR = Convert
                .ToString(PMTK_LOG_FORMAT);
        public static String PMTK_LOG_TIME_INTERVAL_STR = Convert
                .ToString(PMTK_LOG_TIME_INTERVAL);
        public static String PMTK_LOG_DISTANCE_INTERVAL_STR = Convert
                .ToString(PMTK_LOG_DISTANCE_INTERVAL);
        public static String PMTK_LOG_SPEED_INTERVAL_STR = Convert
                .ToString(PMTK_LOG_SPEED_INTERVAL);
        public static String PMTK_LOG_REC_METHOD_STR = Convert
                .ToString(PMTK_LOG_REC_METHOD);
        public static String PMTK_LOG_LOG_STATUS_STR = Convert
                .ToString(PMTK_LOG_LOG_STATUS);
        public static String PMTK_LOG_MEM_USED_STR = Convert
                .ToString(PMTK_LOG_MEM_USED); // bit 2 = logging on/off
        public static String PMTK_LOG_FLASH_STR = Convert
                .ToString(PMTK_LOG_FLASH);
        public static String PMTK_LOG_NBR_LOG_PTS_STR = Convert
                .ToString(PMTK_LOG_NBR_LOG_PTS);
        public static String PMTK_LOG_FLASH_SECTORS_STR = Convert
                .ToString(PMTK_LOG_FLASH_SECTORS);
        public static String PMTK_LOG_VERSION_STR = Convert
                .ToString(PMTK_LOG_VERSION);

        /** Mask for bit in log status indicating that log is active */
        public static int PMTK_LOG_STATUS_LOGISFULL_MASK = 0x800; // bit 11
        public static int PMTK_LOG_STATUS_LOGMUSTINIT_MASK = 0x400; // bit
        // 10
        public static int PMTK_LOG_STATUS_LOGDISABLED_MASK = 0x200; // bit 9
        public static int PMTK_LOG_STATUS_LOGENABLED_MASK = 0x100; // bit 8
        public static int PMTK_LOG_STATUS_LOGSTOP_OVER_MASK = 0x04; // bit 2
        public static int PMTK_LOG_STATUS_LOGONOF_MASK = 0x02; // bit 1

        // Other MTK commands
        public static String PMTK_TEST_STR = "000"; // Convert.ToString(PMTK_TEST);
        public static String PMTK_ACK_STR = "001"; // Convert.ToString(PMTK_ACK);
        public static String PMTK_SYS_MSG_STR = "010"; // Convert.ToString(PMTK_SYS_MSG);
        public static String PMTK_CMD_HOT_START_STR = Convert
                .ToString(PMTK_CMD_HOT_START);
        public static String PMTK_CMD_WARM_START_STR = Convert
                .ToString(PMTK_CMD_WARM_START);
        public static String PMTK_CMD_COLD_START_STR = Convert
                .ToString(PMTK_CMD_COLD_START);
        public static String PMTK_CMD_FULL_COLD_START_STR = Convert
                .ToString(PMTK_CMD_FULL_COLD_START);
        public static String PMTK_CMD_LOG_STR = Convert
                .ToString(PMTK_CMD_LOG);
        public static String PMTK_SET_NMEA_BAUD_RATE_STR = Convert
                .ToString(PMTK_SET_NMEA_BAUD_RATE);
        public static String PMTK_API_SET_FIX_CTL_STR = Convert
                .ToString(PMTK_API_SET_FIX_CTL);
        public static String PMTK_API_SET_DGPS_MODE_STR = Convert
                .ToString(PMTK_API_SET_DGPS_MODE);
        public static String PMTK_API_SET_SBAS_STR = Convert
                .ToString(PMTK_API_SET_SBAS);
        public static String PMTK_API_SET_NMEA_OUTPUT_STR = Convert
                .ToString(PMTK_API_SET_NMEA_OUTPUT);
        public static String PMTK_API_SET_SBAS_TEST_STR = Convert
                .ToString(PMTK_API_SET_SBAS_TEST);
        public static String PMTK_API_SET_PWR_SAV_MODE_STR = Convert
                .ToString(PMTK_API_SET_PWR_SAV_MODE);
        public static String PMTK_API_SET_DATUM_STR = Convert
                .ToString(PMTK_API_SET_DATUM);
        public static String PMTK_API_SET_DATUM_ADVANCE_STR = Convert
                .ToString(PMTK_API_SET_DATUM_ADVANCE);
        public static String PMTK_API_SET_USER_OPTION_STR = Convert
                .ToString(PMTK_API_SET_USER_OPTION);
        public static String PMTK_API_Q_FIX_CTL_STR = Convert
                .ToString(PMTK_API_Q_FIX_CTL);
        public static String PMTK_API_Q_DGPS_MODE_STR = Convert
                .ToString(PMTK_API_Q_DGPS_MODE);
        public static String PMTK_API_Q_SBAS_STR = Convert
                .ToString(PMTK_API_Q_SBAS);
        public static String PMTK_API_Q_NMEA_OUTPUT_STR = Convert
                .ToString(PMTK_API_Q_NMEA_OUTPUT);
        public static String PMTK_API_Q_SBAS_TEST_STR = Convert
                .ToString(PMTK_API_Q_SBAS_TEST);
        public static String PMTK_API_Q_PWR_SAV_MOD_STR = Convert
                .ToString(PMTK_API_Q_PWR_SAV_MOD);
        public static String PMTK_API_Q_DATUM_STR = Convert
                .ToString(PMTK_API_Q_DATUM);
        public static String PMTK_API_Q_DATUM_ADVANCE_STR = Convert
                .ToString(PMTK_API_Q_DATUM_ADVANCE);
        public static String PMTK_API_GET_USER_OPTION_STR = Convert
                .ToString(PMTK_API_Q_GET_USER_OPTION);
        public static String PMTK_DT_FIX_CTL_STR = Convert
                .ToString(PMTK_DT_FIX_CTL);
        public static String PMTK_DT_DGPS_MODE_STR = Convert
                .ToString(PMTK_DT_DGPS_MODE);
        public static String PMTK_DT_SBAS_STR = Convert
                .ToString(PMTK_DT_SBAS);
        public static String PMTK_DT_NMEA_OUTPUT_STR = Convert
                .ToString(PMTK_DT_NMEA_OUTPUT);
        public static String PMTK_DT_SBAS_TEST_STR = Convert
                .ToString(PMTK_DT_SBAS_TEST);
        public static String PMTK_DT_PWR_SAV_MODE_STR = Convert
                .ToString(PMTK_DT_PWR_SAV_MODE);
        public static String PMTK_DT_DATUM_STR = Convert
                .ToString(PMTK_DT_DATUM);
        public static String PMTK_DT_FLASH_USER_OPTION_STR = Convert
                .ToString(PMTK_DT_FLASH_USER_OPTION);
        public static String PMTK_Q_VERSION_STR = Convert
                .ToString(PMTK_Q_VERSION);
        public static String PMTK_Q_RELEASE_STR = Convert
                .ToString(PMTK_Q_RELEASE);
        public static String PMTK_Q_DGPS_INFO_STR = Convert
                .ToString(PMTK_Q_DGPS_INFO);

        /**
         * Parameter 1 of PMTK_ACK reply. Packet invalid.
         */
        public static String PMTK_ACK_INVALID_STR = Convert
                .ToString(PMTK_ACK_INVALID);
        /**
         * Parameter 1 of PMTK_ACK reply. Packet not supported by device.
         */
        public static String PMTK_ACK_UNSUPPORTED_STR = Convert
                .ToString(PMTK_ACK_UNSUPPORTED);
        /**
         * Parameter 1 of PMTK_ACK reply. Packet supported, but action failed.
         */
        public static String PMTK_ACK_FAILED_STR = Convert
                .ToString(PMTK_ACK_FAILED);
        /**
         * Parameter 1 of PMTK_ACK reply. Packet success.
         */
        public static String PMTK_ACK_SUCCEEDED_STR = Convert
                .ToString(PMTK_ACK_SUCCEEDED);

        /***************************************************************************
         * Holux specific
         */

        public static String HOLUX_MAIN_CMD = "HOLUX241,";
        public static int HOLUX_API_SET_CONN = 1;
        public static int HOLUX_API_SET_DISCONN = 2;
        public static int HOLUX_API_Q_FIRMWARE_VERSION = 3;

        public static int HOLUX_API_SET_NAME = 4;
        public static int HOLUX_API_Q_NAME = 5;
        public static int HOLUX_API_KEEP_ALIVE = 6;
        public static int HOLUX_API_Q_HW_VERSION = 7;

        public static int HOLUX_API_SET_TZ_OFFSET = 9;
        public static int HOLUX_API_Q_TZ_OFFSET = 10;

        public static int HOLUX_API_DT_FIRMWARE_VERSION = 3;
        public static int HOLUX_API_DT_NAME = 5;
        public static int HOLUX_API_DT_HW_VERSION = 7;
        public static int HOLUX_API_DT_TZ_OFFSET = 10;

        /**
         * Get the size of the log header in the device.
         * 
         * @param p_logFormat
         *            The log format of the device
         * @param holux
         *            TODO
         * 
         * @return Size of the header
         */
        public static int logRecordMinSize(int p_logFormat,
                bool holux) {
        int bits = p_logFormat;
        int index = 0;
        int total = 0;
        int[] byteSizes;
        if (holux) {
            byteSizes = logFmtByteSizesHolux;
        } else {
            byteSizes = logFmtByteSizes;
        }
        do {
            if ((bits & 1) != 0) {
                switch (index) {
                    case 14: //FMT_ELEVATION_IDX:
                    case 15: //FMT_AZIMUTH_IDX:
                    case 16: //FMT_SNR_IDX:
                    // These fields do not contribute to the minimum size
                    break;
                default:
                    // Other fields contribute
                    try {
                        total += byteSizes[index];
                    } catch (Exception e) {
                        // TODO: Check when this happens.
                      //  Vm.debug(Txt.C_BAD_LOG_FORMAT);
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                    break;
                }
            }
            index++;
        } while ((bits >= 1) /*!= 0*/);
        return total;
    }

        /**
         * Calculate the log record size
         * 
         * @param logFormat
         * @param holux
         *            When true, then the log record size is calculated for a Holux
         *            M241 device.
         * @param sats
         *            Estimated number of satellites per record.
         * @return The estimated record size.
         */
        public static int logRecordSize(int logFormat, bool holux,
                int sats)
        {
            int cnt = 0;
            int[] byteSizes;
            if (holux)
            {
                byteSizes = logFmtByteSizesHolux;
            }
            else
            {
                byteSizes = logFmtByteSizes;
            }
            if ((logFormat & (1 << FMT_SID_IDX)) != 0)
            {
                cnt += byteSizes[FMT_SID_IDX];
                cnt += (logFormat & (1 << FMT_ELEVATION_IDX)) != 0 ? byteSizes[FMT_ELEVATION_IDX]
                        : 0;
                cnt += (logFormat & (1 << FMT_AZIMUTH_IDX)) != 0 ? byteSizes[FMT_AZIMUTH_IDX]
                        : 0;
                cnt += (logFormat & (1 << FMT_SNR_IDX)) != 0 ? byteSizes[FMT_SNR_IDX]
                        : 0;
                cnt *= sats - 1;
            }
            return cnt + logRecordMinSize(logFormat, holux);
        }

        public static int logRecordMaxSize(int p_logFormat,
                bool holux)
        {
            return logRecordSize(p_logFormat, holux, FMT_MAX_SATS);
        }

        /**
         * Next entries are elated to <code>PMTK_API_Q_NMEA_OUTPUT</code> and
         * similar.
         * 
         */

        /**
         * Number of NMEA sentence types
         */
        public static int C_NMEA_SEN_COUNT = 19;
        public static String[] NMEA_STRINGS = { "GLL", // 0 // GPGLL
            // interval -
            // Geographic
            // Position -
            // Latitude
            // longitude
            "RMC", // 1 // GPRMC interval - Recommended Min. specic GNSS
            // sentence
            "VTG", // 2 / / GPVTG interval - Course Over Ground and Ground
            // Speed
            "GGA", // 3 / / GPGGA interval - GPS Fix Data
            "GSA", // 4 / / GPGSA interval - GNSS DOPS and Active Satellites
            "GSV", // 5 / / GPGSV interval - GNSS Satellites in View
            "GRS", // 6 / / GPGRS interval - GNSS Range Residuals
            "GST", // 7 // GPGST interval - GNSS Pseudorange Error Statistics
            "?8", // 8
            "?9", // 9
            "?10", // 10
            "?11", // 11
            "?12", // 12
            "MALM", // 13 // PMTKALM interval - GPS almanac information
            "MEPH", // 14 // PMTKEPH interval - GPS ephemeris information
            "MDGP", // 15 // PMTKDGP interval - GPS differential correction
            // information
            "MDBG", // 16 // PMTKDBG interval – MTK debug information
            "ZDA", // 17 // GPZDA interval – Time & Date
            "MCHN", // 18 // PMTKCHN interval – GPS channel status
    };
        public static int NMEA_SEN_GLL_IDX = 0; // GPGLL interval -
        // Geographic Position -
        // Latitude longitude
        public static int NMEA_SEN_RMC_IDX = 1; // GPRMC interval -
        // Recommended Min. specic
        // GNSS sentence
        public static int NMEA_SEN_VTG_IDX = 2; // GPVTG interval - Course
        // Over Ground and Ground
        // Speed
        public static int NMEA_SEN_GGA_IDX = 3; // GPGGA interval - GPS Fix
        // Data
        public static int NMEA_SEN_GSA_IDX = 4; // GPGSA interval - GNSS
        // DOPS and Active
        // Satellites
        public static int NMEA_SEN_GSV_IDX = 5; // GPGSV interval - GNSS
        // Satellites in View
        public static int NMEA_SEN_GRS_IDX = 6; // GPGRS interval - GNSS
        // Range Residuals
        public static int NMEA_SEN_GST_IDX = 7; // GPGST interval - GNSS
        // Pseudorange Error
        // Statistics
        public static int NMEA_SEN_MALM_IDX = 13; // PMTKALM interval - GPS
        // almanac information
        public static int NMEA_SEN_MEPH_IDX = 14; // PMTKEPH interval - GPS
        // ephemeris information
        public static int NMEA_SEN_MDGP_IDX = 15; // PMTKDGP interval - GPS
        // differential correction
        // information
        public static int NMEA_SEN_MDBG_IDX = 16; // PMTKDBG interval – MTK
        // debug information
        public static int NMEA_SEN_ZDA_IDX = 17; // GPZDA interval – Time &
        // Date
        public static int NMEA_SEN_MCHN_IDX = 18; // PMTKCHN interval – GPS
        // channel status

        public static int SPI_Y2_MAN_ID_MASK = 0xff00;
        public static int SPI_Y2_DEV_ID_MASK = 0x00ff;

        /* SPI flash manufacturer ID's */
        public static int SPI_MAN_ID_AMD = 0x01;
        public static int SPI_MAN_ID_FUJITSU = 0x04;
        public static int SPI_MAN_ID_ST_M25P10 = 0x10;
        public static int SPI_MAN_ID_ST_M25P20 = 0x11;
        public static int SPI_MAN_ID_EON = 0x1C;
        public static int SPI_MAN_ID_MITSUBISHI = 0x1C;
        public static int SPI_MAN_ID_ATMEL = 0x1F;
        public static int SPI_MAN_ID_STMICROELECTRONICS = 0x20;
        public static int SPI_MAN_ID_CATALYST = 0x31;
        public static int SPI_MAN_ID_SYNCMOS = 0x40;
        public static int SPI_MAN_ID_INTEL = 0x89;
        public static int SPI_MAN_ID_HYUNDAI = 0xAD;
        public static int SPI_MAN_ID_SST = 0xBF; // Silicon Storage
        // Technology
        public static int SPI_MAN_ID_MACRONIX = 0xC2; // MX
        public static int SPI_MAN_ID_WINBOND = 0xDA;

        // #define MX_29F002 0xB0
        // +/* MX25L chips are SPI, first byte of device id is memory type,
        // + second byte of device id is log(bitsize)-9 */
        // +#define MX_25L512 0x2010 /* 2^19 kbit or 2^16 kByte */
        // +#define MX_25L1005 0x2011
        // +#define MX_25L2005 0x2012
        // +#define MX_25L4005 0x2013 /* MX25L4005{,A} */
        // +#define MX_25L8005 0x2014
        // +#define MX_25L1605 0x2015 /* MX25L1605{,A,D} */
        // +#define MX_25L3205 0x2016 /* MX25L3205{,A} */
        // +#define MX_25L6405 0x2017 /* MX25L3205{,D} */
        // +#define MX_25L1635D 0x2415
        // +#define MX_25L3235D 0x2416

        public static int VALID_NO_FIX_MASK = 0x0001;
        public static int VALID_SPS_MASK = 0x0002;
        public static int VALID_DGPS_MASK = 0x0004;
        public static int VALID_PPS_MASK = 0x0008;
        public static int VALID_RTK_MASK = 0x0010;
        public static int VALID_FRTK_MASK = 0x0020;
        public static int VALID_ESTIMATED_MASK = 0x0040;
        public static int VALID_MANUAL_MASK = 0x0080;
        public static int VALID_SIMULATOR_MASK = 0x0100;

        public static int NO_ERROR = 0;
        public static int ERROR_COULD_NOT_OPEN = -1;
        public static int ERROR_NO_FILES_WERE_CREATED = -2;
        public static int ERROR_READING_FILE = -3;
        /**
         * The requested output format is unknown.
         */
        public static int ERROR_UNKNOWN_OUTPUT_FORMAT = -4;
    }
}