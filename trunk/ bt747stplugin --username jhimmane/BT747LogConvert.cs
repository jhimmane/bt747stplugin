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
     * This class is used to convert the binary log to a new format. Basically this
     * class interprets the log and creates a {@link GPSRecord}. The
     * {@link GPSRecord} is then sent to the {@link GPSFile} class object to write
     * it to the output.
     * 
     * @author Mario De Weerd
     */
    public class BT747LogConvert
    { //implements GPSLogConvert {
        private int minRecordSize;
        private int maxRecordSize;
        private int logFormat;
        //private File m_File = null;
        private long timeOffsetSeconds = 0;
        protected bool passToFindFieldsActivatedInLog = false;
        protected int activeFileFields = 0;
        private bool isConvertWGL84ToMSL = false; // If true,remove geoid
        // difference from
        // height

        private int satIdxOffset;
        private int satRecSize;
        private bool holux = false;
        private bool nextPointIsWayPt = false;

        private int badrecord_count = 0;

        private void updateLogFormat(GPSFile gpsFile, int newLogFormat)
        {
            int bits = newLogFormat;
            int index = 0;
            int total = 0;
            int[] byteSizes;

            byteSizes = BT747Constants.logFmtByteSizes;

            satRecSize = 0;

            logFormat = newLogFormat;
            activeFileFields |= logFormat;
            // if (!passToFindFieldsActivatedInLog) {
            //    gpsFile.writeLogFmtHeader(GPSRecord.getLogFormatRecord(logFormat));
            //}
            // if((logFormat&0x80000000)!=0) {
            // holux=true;
            // }
            minRecordSize = BT747Constants.logRecordMinSize(logFormat, holux);
            maxRecordSize = BT747Constants.logRecordMaxSize(logFormat, holux);
            do
            {
                if ((bits & 1) != 0)
                {
                    switch (index)
                    {
                        case BT747Constants.FMT_LATITUDE_IDX:
                        case BT747Constants.FMT_LONGITUDE_IDX:
                            total += byteSizes[index];
                            break;
                        case BT747Constants.FMT_HEIGHT_IDX:
                            total += byteSizes[index];
                            break;

                        case BT747Constants.FMT_SID_IDX:
                        case BT747Constants.FMT_ELEVATION_IDX:
                        case BT747Constants.FMT_AZIMUTH_IDX:
                        case BT747Constants.FMT_SNR_IDX:
                            satRecSize += byteSizes[index];
                            break;
                        case BT747Constants.FMT_RCR_IDX:
                        case BT747Constants.FMT_MILLISECOND_IDX:
                        case BT747Constants.FMT_DISTANCE_IDX:

                            // These fields do not contribute to the sat offset
                            break;
                        default:
                            // Other fields contribute
                            total += byteSizes[index];
                            break;
                    }
                }
                index++;
            } while ((bits >= 1) != 0);
            satIdxOffset = total;
        }

        /**
         * The size of the file read buffer
         */
        private static int BUF_SIZE = 0x800;

        /**
         * Parse the binary input file and convert it.
         * 
         * @return non zero in case of err. The error text can be retrieved using
         *         {@link #getErrorInfo()}.
         * @param gpsFile -
         *            object doing actual write to files
         * 
         */
        public int parseFile(GPSFile gpsFile)
        {
            GPSRecord gpsRec = new GPSRecord();
            byte[] bytes = new byte[BUF_SIZE];
            int sizeToRead;
            int nextAddrToRead;
            int recCount;
            int fileSize;
            int satCntIdx;
            int satcnt;

            recCount = 0;
            logFormat = 0;
            nextAddrToRead = 0;
            nextPointIsWayPt = false;
            badrecord_count = 0;
            try
            {
                fileSize = m_File.getSize();
            }
            catch (Exception e)
            {
                // TODO: handle exception
                fileSize = 0;
            }
            while (nextAddrToRead < fileSize)
            {
                int okInBuffer = -1; // Last ending position in buffer

                /*******************************************************************
                 * Read data from the raw data file into the local buffer.
                 */
                // Determine size to read
                if ((nextAddrToRead & 0xFFFF) < 0x200)
                {
                    // Read the header
                    nextAddrToRead = (nextAddrToRead & 0xFFFF0000);
                }
                int endOfBlock = (nextAddrToRead & 0xFFFF0000) | 0xFFFF;
                sizeToRead = endOfBlock + 1 - nextAddrToRead;
                if (sizeToRead > BUF_SIZE)
                {
                    sizeToRead = BUF_SIZE;
                }
                if ((sizeToRead + nextAddrToRead) > fileSize)
                {
                    sizeToRead = fileSize - nextAddrToRead;
                }

                /* Read the bytes from the file */
                int readResult;
                bool continueInBuffer = true;
                int offsetInBuffer = 0;
                int newLogFormat;

                try
                {
                    m_File.setPos(nextAddrToRead);
                }
                catch (Exception e)
                {
                    // TODO: handle exception
                }

                if ((nextAddrToRead & 0xFFFF) == 0)
                {
                    /***************************************************************
                     * This is the header. Only 20 bytes are read - just enough to
                     * get the log format.
                     */
                    if (sizeToRead >= 20)
                    {
                        readResult = 0;
                        // Read header (20 bytes is enough)
                        try
                        {
                            readResult = m_File.readBytes(bytes, 0, 20);
                        }
                        catch (Exception e)
                        {
                            // TODO: handle exception
                        }
                        if (readResult != 20)
                        {
                            errorInfo = m_File.getPath() + "|" + m_File.lastError;
                            return BT747Constants.ERROR_READING_FILE;
                        }
                        newLogFormat = (0xFF & bytes[2]) << 0
                                | (0xFF & bytes[3]) << 8 | (0xFF & bytes[4]) << 16
                                | (0xFF & bytes[5]) << 24;
                        logMode = (0xFF & bytes[6]) << 0 | (0xFF & bytes[7]) << 8;

                        logPeriod = (0xFF & bytes[8]) << 0 | (0xFF & bytes[9]) << 8
                                | (0xFF & bytes[10]) << 16
                                | (0xFF & bytes[11]) << 24;

                        logDistance = (0xFF & bytes[12]) << 0
                                | (0xFF & bytes[13]) << 8
                                | (0xFF & bytes[14]) << 16
                                | (0xFF & bytes[15]) << 24;

                        logSpeed = (0xFF & bytes[16]) << 0
                                | (0xFF & bytes[17]) << 8
                                | (0xFF & bytes[18]) << 16
                                | (0xFF & bytes[19]) << 24;
                        if (logPeriod != 0)
                        {
                            rcr_mask |= BT747Constants.RCR_TIME_MASK;
                        }
                        else
                        {
                            rcr_mask &= ~BT747Constants.RCR_TIME_MASK;
                        }
                        if (logDistance != 0)
                        {
                            rcr_mask |= BT747Constants.RCR_DISTANCE_MASK;
                        }
                        else
                        {
                            rcr_mask &= ~BT747Constants.RCR_DISTANCE_MASK;
                        }
                        if (logSpeed != 0)
                        {
                            rcr_mask |= BT747Constants.RCR_SPEED_MASK;
                        }
                        else
                        {
                            rcr_mask &= ~BT747Constants.RCR_SPEED_MASK;
                        }

                        if (newLogFormat == 0xFFFFFFFF)
                        {
                            // TODO: Treat error
                            if (logFormat == 0)
                            {
                                newLogFormat = 0x8000001D; // Supposing holux M-241
                            }
                            else
                            {
                                newLogFormat = logFormat;
                            }
                        }
                        if (newLogFormat != logFormat)
                        {
                            updateLogFormat(gpsFile, newLogFormat);
                        }
                    }
                    nextAddrToRead += 0x200;
                    continueInBuffer = false;
                }
                else
                {
                    /***************************************************************
                     * Not reading header - reading data.
                     */
                    try
                    {
                        readResult = m_File.readBytes(bytes, 0, sizeToRead);
                    }
                    catch (Exception e)
                    {
                        // TODO: handle exception
                        readResult = 0;
                    }
                    if (readResult != sizeToRead)
                    {
                        errorInfo = m_File.getPath() + "|" + m_File.lastError;
                        return BT747Constants.ERROR_READING_FILE;
                    }
                    nextAddrToRead += sizeToRead;
                }
                /*******************************************************************
                 * Data read from file into local buffer
                 ******************************************************************/

                /*******************************************************************
                 * Interpret the data read in the local buffer
                 */
                while (continueInBuffer)
                {
                    bool lookForRecord = true;

                    while (lookForRecord && (sizeToRead - 16 > offsetInBuffer) // Enough
                        // bytes
                        // in
                        // buffer
                    )
                    {
                        int nbrBytes;
                        nbrBytes = getSpecialRecord(bytes, offsetInBuffer, gpsFile);
                        lookForRecord = (nbrBytes != 0);
                        offsetInBuffer += nbrBytes;
                    }

                    /***************************************************************
                     * Look for a record
                     */
                    bool foundRecord = false;
                    bool foundAnyRecord = false;
                    int satRecords;

                    if ((sizeToRead > offsetInBuffer + minRecordSize
                            + (holux ? 1 : 2)) // Enough bytes in buffer
                    )
                    { // As long as record may fit in data still to read.
                        int indexInBuffer = offsetInBuffer;
                        int checkSum = 0;
                        int allFF = 0xFF; // If 0xFF, all bytes are FF.
                        foundRecord = false;
                        satcnt = 0;
                        satCntIdx = 0;
                        satRecords = 0;

                        /***********************************************************
                         * Get some satellite record information.
                         */
                        if ((logFormat & (1 << BT747Constants.FMT_SID_IDX)) != 0)
                        {
                            satCntIdx = offsetInBuffer + satIdxOffset;
                            satcnt = (0xFF & bytes[satCntIdx + 2]) << 0
                                    | (0xFF & bytes[satCntIdx + 3]) << 8;
                            if ((satcnt > 32) || (satcnt < 0))
                            {
                                // TODO: handle error [but ok when end of block or
                                // end of file]
                                satcnt = 32;
                            }
                            if (satcnt != 0)
                            {
                                satRecords = satcnt * satRecSize - 4;
                            }
                        }

                        /***********************************************************
                         * Skip minimum number of bytes in a record.
                         */
                        if ((minRecordSize + satRecords + offsetInBuffer) <= (sizeToRead - 2))
                        {
                            // Record fits in buffer
                            int cnt;
                            cnt = minRecordSize + satRecords + offsetInBuffer
                                    - indexInBuffer;
                            while (cnt-- > 0)
                            {
                                allFF &= bytes[indexInBuffer];
                                checkSum ^= bytes[indexInBuffer++];
                            }

                            if ((allFF != 0xFF)
                                    && ((!holux && ((bytes[indexInBuffer] == '*') && ((checkSum & 0xFF) == (0xFF & bytes[indexInBuffer + 1])))) || (holux && ((checkSum & 0xFF) == (0xFF & bytes[indexInBuffer])))))
                            {
                                if (!holux)
                                {
                                    indexInBuffer += 2; // Point just past end ('*'
                                    // and checksum).
                                }
                                else
                                {
                                    indexInBuffer += 1;
                                }

                                int recIdx = offsetInBuffer;

                                offsetInBuffer = indexInBuffer;
                                okInBuffer = indexInBuffer;
                                foundRecord = true;

                                int rcrIdx; // Offset to first field after sat data.
                                if (!holux)
                                {
                                    rcrIdx = offsetInBuffer
                                            - 2
                                            - ((((logFormat & (1 << BT747Constants.FMT_DISTANCE_IDX)) != 0) ? BT747Constants.logFmtByteSizes[BT747Constants.FMT_DISTANCE_IDX]
                                                    : 0)
                                                    + (((logFormat & (1 << BT747Constants.FMT_MILLISECOND_IDX)) != 0) ? BT747Constants.logFmtByteSizes[BT747Constants.FMT_MILLISECOND_IDX]
                                                            : 0) + (((logFormat & (1 << BT747Constants.FMT_RCR_IDX)) != 0) ? BT747Constants.logFmtByteSizes[BT747Constants.FMT_RCR_IDX]
                                                    : 0));
                                }
                                else
                                {
                                    rcrIdx = offsetInBuffer
                                            - 1
                                            - ((((logFormat & (1 << BT747Constants.FMT_DISTANCE_IDX)) != 0) ? BT747Constants.logFmtByteSizesHolux[BT747Constants.FMT_DISTANCE_IDX]
                                                    : 0)
                                                    + (((logFormat & (1 << BT747Constants.FMT_MILLISECOND_IDX)) != 0) ? BT747Constants.logFmtByteSizesHolux[BT747Constants.FMT_MILLISECOND_IDX]
                                                            : 0) + (((logFormat & (1 << BT747Constants.FMT_RCR_IDX)) != 0) ? BT747Constants.logFmtByteSizesHolux[BT747Constants.FMT_RCR_IDX]
                                                    : 0));
                                }

                                recCount++;
                                // System.out.println(recCount);

                                foundAnyRecord = true;

                                /***************************************************
                                 * Get all the information in the record.
                                 */
                                gpsRec.recCount = recCount;
                                if (!passToFindFieldsActivatedInLog)
                                {
                                    // Only interpret fields if not looking for
                                    // logFormat changes only

                                    // Vm.debug(Convert.unsigned2hex(nextAddrToRead-sizeToRead+recIdx,
                                    // 8)); // record start position
                                    // Vm.debug("Offset:"+recIdx+"
                                    // "+offsetInBuffer);
                                    bool valid;

                                    // Retrieve the record from the file (in
                                    // buffer).
                                    valid = getRecord(bytes, gpsRec, recIdx,
                                            rcrIdx, satcnt);
                                    if (valid)
                                    {
                                        gpsFile.writeRecord(gpsRec);
                                    }
                                    else
                                    {
                                        badrecord_count++;
                                    }
                                }
                                /***************************************************
                                 * Information from record retrieved
                                 **************************************************/
                            }
                            else
                            {
                                // Problem in checksum, data format, ... .
                                if (allFF != 0xFF)
                                {
                                    badrecord_count++;
                                }
                            }
                        }
                        else
                        {
                            continueInBuffer = false;
                        }
                        lookForRecord = foundRecord;
                    } // End if (or while previously) for possible good record.

                    if (!foundAnyRecord && continueInBuffer)
                    {
                        if (sizeToRead > offsetInBuffer + maxRecordSize
                                + (holux ? 1 : 2))
                        { // TODO: recover when 16
                            // bytes available too.
                            // Did not find any record - expected at least one.
                            // Try to recover.
                            offsetInBuffer++;
                        }
                        else
                        {
                            // There is not enough data in the buffer, we'll need to
                            // get some more.
                            continueInBuffer = false;
                        }
                    }
                } /* ContinueInBuffer */
                if (okInBuffer > 0)
                {
                    nextAddrToRead -= (sizeToRead - okInBuffer);
                }
            } /* nextAddrToRead<fileSize */
            return BT747Constants.NO_ERROR;
        }

        public void setTimeOffset(long offset)
        {
            timeOffsetSeconds = offset;
        }

        public void setConvertWGS84ToMSL(bool b)
        {
            isConvertWGL84ToMSL = b;
        }

        private String errorInfo;

        public String getErrorInfo()
        {
            return errorInfo;
        }

        public int toGPSFile(String fileName, GPSFile gpsFile,
                int Card)
        {
            int error = BT747Constants.NO_ERROR;
            if (File.isAvailable())
            {
                try
                {
                    m_File = new File(fileName, File.READ_ONLY, Card);
                    errorInfo = fileName + "|" + m_File.lastError;
                }
                catch (Exception e)
                {
                    // TODO: handle exception
                    e.printStackTrace();
                }
                if (m_File == null || !m_File.isOpen())
                {
                    errorInfo = fileName;
                    if (m_File != null)
                    {
                        errorInfo += "|" + m_File.lastError;
                    }
                    error = BT747Constants.ERROR_COULD_NOT_OPEN;
                    m_File = null;
                }
                else
                {
                    passToFindFieldsActivatedInLog = gpsFile
                            .needPassToFindFieldsActivatedInLog();
                    if (passToFindFieldsActivatedInLog)
                    {
                        activeFileFields = 0;
                        error = parseFile(gpsFile);
                        gpsFile
                                .setActiveFileFields(GPSRecord.getLogFormatRecord(activeFileFields));
                    }
                    passToFindFieldsActivatedInLog = false;
                    if (error == BT747Constants.NO_ERROR)
                    {
                        do
                        {
                            error = parseFile(gpsFile);
                        } while (error == BT747Constants.NO_ERROR
                                && gpsFile.nextPass());
                    }
                    gpsFile.finaliseFile();
                    if (gpsFile.getFilesCreated() == 0)
                    {
                        error = BT747Constants.ERROR_NO_FILES_WERE_CREATED;
                    }
                }

                if (m_File != null)
                {
                    try
                    {
                        m_File.close();
                    }
                    catch (Exception e)
                    {
                        // TODO: handle exception
                    }
                }
            }
            return error;
        }

        /**
         * @param holux
         *            The holux to set.
         */
        public void setHolux(bool holux)
        {
            this.holux = holux;
        }

        private int logMode = 0;
        private int rcr_mask = 0; // Default RCR based on log settings
        private int logSpeed = 0;
        private int logDistance = 0;
        private int logPeriod = 0;

        /**
         * Tries to find a special record at the indicated offset.
         * 
         * @return int / number of bytes found
         */
        private int getSpecialRecord(byte[] bytes, int offsetInBuffer,
                GPSFile gpsFile)
        {
            int newLogFormat;
            int nbrBytesDone = 0;
            if (((0xFF & bytes[offsetInBuffer + 0]) == 0xAA)
                    && ((0xFF & bytes[offsetInBuffer + 1]) == 0xAA)
                    && ((0xFF & bytes[offsetInBuffer + 2]) == 0xAA)
                    && ((0xFF & bytes[offsetInBuffer + 3]) == 0xAA)
                    && ((0xFF & bytes[offsetInBuffer + 4]) == 0xAA)
                    && ((0xFF & bytes[offsetInBuffer + 5]) == 0xAA)
                    && ((0xFF & bytes[offsetInBuffer + 6]) == 0xAA)
                    && ((0xFF & bytes[offsetInBuffer + 12]) == 0xBB)
                    && ((0xFF & bytes[offsetInBuffer + 13]) == 0xBB)
                    && ((0xFF & bytes[offsetInBuffer + 14]) == 0xBB)
                    && ((0xFF & bytes[offsetInBuffer + 15]) == 0xBB))
            {
                int value = (0xFF & bytes[offsetInBuffer + 8]) << 0
                        | (0xFF & bytes[offsetInBuffer + 9]) << 8
                        | (0xFF & bytes[offsetInBuffer + 10]) << 16
                        | (0xFF & bytes[offsetInBuffer + 11]) << 24;
                // There is a special operation here
                switch (0xFF & bytes[offsetInBuffer + 7])
                {
                    case 0x02: // logBitMaskChange
                        newLogFormat = value;
                        if (newLogFormat != logFormat)
                        {
                            updateLogFormat(gpsFile, newLogFormat);
                        }
                        // bt747.sys.Vm.debug("Log format set to
                        // :"+Convert.unsigned2hex(value, 8));
                        break;
                    case 0x03: // log Period change
                        logPeriod = value;
                        if (value != 0)
                        {
                            rcr_mask |= BT747Constants.RCR_TIME_MASK;
                        }
                        else
                        {
                            rcr_mask &= ~BT747Constants.RCR_TIME_MASK;
                        }
                        // bt747.sys.Vm.debug("Log period set to :"+value);
                        break;
                    case 0x04: // log distance change
                        logDistance = value;
                        if (value != 0)
                        {
                            rcr_mask |= BT747Constants.RCR_DISTANCE_MASK;
                        }
                        else
                        {
                            rcr_mask &= ~BT747Constants.RCR_DISTANCE_MASK;
                        }
                        // bt747.sys.Vm.debug("Log distance set to :"+value);
                        break;
                    case 0x05: // log speed change
                        logSpeed = value;
                        if (value != 0)
                        {
                            rcr_mask |= BT747Constants.RCR_SPEED_MASK;
                        }
                        else
                        {
                            rcr_mask &= ~BT747Constants.RCR_SPEED_MASK;
                        }
                        // bt747.sys.Vm.debug("Log speed set to :"+value);
                        break;
                    case 0x06: // value: 0x0106= logger on 0x0107= logger off 0x104=??
                        logMode = value;
                        // bt747.sys.Vm.debug("Logger off :"+value);
                        break;
                    case 0x07: // value: 0x0106= logger on 0x0107= logger off 0x104=??
                        logMode = value;
                        // bt747.sys.Vm.debug("Logger off :"+value);
                        break;
                    default:
                        break; // Added to set SW breakpoint to discover other records.

                }
                // No data: on/off
                nbrBytesDone += 16;
            }
            else if (((0xFF & bytes[offsetInBuffer + 0]) == 'H')
                    && ((0xFF & bytes[offsetInBuffer + 1]) == 'O')
                    && ((0xFF & bytes[offsetInBuffer + 2]) == 'L')
                    && ((0xFF & bytes[offsetInBuffer + 3]) == 'U')
                    && ((0xFF & bytes[offsetInBuffer + 4]) == 'X'))
            {

                // No data: on/off
                if (!holux)
                {
                    holux = true; // currently set like this
                    updateLogFormat(gpsFile, logFormat);
                }
                nbrBytesDone += 16;
                if (// ((0xFF&bytes[offsetInBuffer+5])=='G')
                    // &&((0xFF&bytes[offsetInBuffer+6])=='R')
                    // &&((0xFF&bytes[offsetInBuffer+7])=='2')
                    // &&((0xFF&bytes[offsetInBuffer+8])=='4')
                    // &&((0xFF&bytes[offsetInBuffer+9])=='1')
                    // &&
                ((0xFF & bytes[offsetInBuffer + 10]) == 'W')
                        && ((0xFF & bytes[offsetInBuffer + 11]) == 'A')
                        && ((0xFF & bytes[offsetInBuffer + 12]) == 'Y')
                        && ((0xFF & bytes[offsetInBuffer + 13]) == 'P')
                        && ((0xFF & bytes[offsetInBuffer + 14]) == 'N')
                        && ((0xFF & bytes[offsetInBuffer + 15]) == 'T'))
                {
                    nextPointIsWayPt = true;
                    // Vm.debug("Holux Waypoint");
                }
            }
            return nbrBytesDone;
        }

        /**
         * Tries to find a normal record at the indicated offset.
         * 
         * @return true if success
         */
        private bool getRecord(
                byte[] bytes, // The data string
                GPSRecord gpsRec, int startIdx, int rcrIdx,
                int satcnt)
        {
            int recIdx;
            bool valid;
            int satidx;
            int idx;
            recIdx = startIdx;
            valid = true;

            if ((logFormat & (1 << BT747Constants.FMT_UTC_IDX)) != 0)
            {
                gpsRec.logPeriod = logPeriod;
                gpsRec.logDistance = logDistance;
                gpsRec.logSpeed = logSpeed;
                gpsRec.utc = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8
                        | (0xFF & bytes[recIdx++]) << 16
                        | (0xFF & bytes[recIdx++]) << 24;
                if (gpsRec.utc == 0xFFFFFFFF)
                {
                    valid = false;
                }
                gpsRec.utc += timeOffsetSeconds;
            }
            else
            {
                gpsRec.utc = 1000; // Value after earliest date
            }
            if ((logFormat & (1 << BT747Constants.FMT_VALID_IDX)) != 0)
            {
                gpsRec.valid = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8;
            }
            else
            {
                gpsRec.valid = 0xFFFF;
            }
            if ((logFormat & (1 << BT747Constants.FMT_LATITUDE_IDX)) != 0)
            {
                if (!holux)
                {
                    long latitude = (0xFFL & bytes[recIdx++]) << 0
                            | (0xFFL & bytes[recIdx++]) << 8
                            | (0xFFL & bytes[recIdx++]) << 16
                            | (0xFFL & bytes[recIdx++]) << 24
                            | (0xFFL & bytes[recIdx++]) << 32
                            | (0xFFL & bytes[recIdx++]) << 40
                            | (0xFFL & bytes[recIdx++]) << 48
                            | (0xFFL & bytes[recIdx++]) << 56;
                    gpsRec.latitude = Convert.longBitsToDouble(latitude);
                }
                else
                {
                    int latitude = (0xFF & bytes[recIdx++]) << 0
                            | (0xFF & bytes[recIdx++]) << 8
                            | (0xFF & bytes[recIdx++]) << 16
                            | (0xFF & bytes[recIdx++]) << 24;
                    gpsRec.latitude = Convert.toFloatBitwise(latitude);
                }
                if (gpsRec.latitude > 90.00 || gpsRec.latitude < -90.00)
                {
                    valid = false;
                }
            }
            if ((logFormat & (1 << BT747Constants.FMT_LONGITUDE_IDX)) != 0)
            {
                if (!holux)
                {
                    long longitude = (0xFFL & bytes[recIdx++]) << 0
                            | (0xFFL & bytes[recIdx++]) << 8
                            | (0xFFL & bytes[recIdx++]) << 16
                            | (0xFFL & bytes[recIdx++]) << 24
                            | (0xFFL & bytes[recIdx++]) << 32
                            | (0xFFL & bytes[recIdx++]) << 40
                            | (0xFFL & bytes[recIdx++]) << 48
                            | (0xFFL & bytes[recIdx++]) << 56;
                    gpsRec.longitude = Convert.longBitsToDouble(longitude);
                }
                else
                {
                    int longitude = (0xFF & bytes[recIdx++]) << 0
                            | (0xFF & bytes[recIdx++]) << 8
                            | (0xFF & bytes[recIdx++]) << 16
                            | (0xFF & bytes[recIdx++]) << 24;
                    gpsRec.longitude = Convert.toFloatBitwise(longitude);// *1.0;
                }
                if (gpsRec.longitude > 180.00 || gpsRec.latitude < -180.00)
                {
                    valid = false;
                }
            }
            if ((logFormat & (1 << BT747Constants.FMT_HEIGHT_IDX)) != 0)
            {
                if (!holux)
                {
                    int height = (0xFF & bytes[recIdx++]) << 0
                            | (0xFF & bytes[recIdx++]) << 8
                            | (0xFF & bytes[recIdx++]) << 16
                            | (0xFF & bytes[recIdx++]) << 24;
                    gpsRec.height = Convert.toFloatBitwise(height);
                }
                else
                {
                    int height =

                    (0xFF & bytes[recIdx++]) << 8 | (0xFF & bytes[recIdx++]) << 16
                            | (0xFF & bytes[recIdx++]) << 24;
                    gpsRec.height = Convert.toFloatBitwise(height);
                }
                if (isConvertWGL84ToMSL
                        && ((logFormat & (1 << BT747Constants.FMT_LATITUDE_IDX)) != 0)
                        && ((logFormat & (1 << BT747Constants.FMT_LONGITUDE_IDX)) != 0)
                        && valid)
                {
                    gpsRec.height -= Conv.wgs84Separation(gpsRec.latitude,
                            gpsRec.longitude);
                }
            }
            if ((logFormat & (1 << BT747Constants.FMT_SPEED_IDX)) != 0)
            {
                int speed = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8
                        | (0xFF & bytes[recIdx++]) << 16
                        | (0xFF & bytes[recIdx++]) << 24;
                gpsRec.speed = Convert.toFloatBitwise(speed);
            }
            if ((logFormat & (1 << BT747Constants.FMT_HEADING_IDX)) != 0)
            {
                int heading = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8
                        | (0xFF & bytes[recIdx++]) << 16
                        | (0xFF & bytes[recIdx++]) << 24;
                gpsRec.heading = Convert.toFloatBitwise(heading);
            }
            if ((logFormat & (1 << BT747Constants.FMT_DSTA_IDX)) != 0)
            {
                gpsRec.dsta = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8;
            }
            if ((logFormat & (1 << BT747Constants.FMT_DAGE_IDX)) != 0)
            {
                gpsRec.dage = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8
                        | (0xFF & bytes[recIdx++]) << 16
                        | (0xFF & bytes[recIdx++]) << 24;
            }
            if ((logFormat & (1 << BT747Constants.FMT_PDOP_IDX)) != 0)
            {
                gpsRec.pdop = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8;
            }
            if ((logFormat & (1 << BT747Constants.FMT_HDOP_IDX)) != 0)
            {
                gpsRec.hdop = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8;
            }
            if ((logFormat & (1 << BT747Constants.FMT_VDOP_IDX)) != 0)
            {
                gpsRec.vdop = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8;
            }
            if ((logFormat & (1 << BT747Constants.FMT_NSAT_IDX)) != 0)
            {
                gpsRec.nsat = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8;
            }
            idx = 0;
            satidx = 0;
            if (rcrIdx - recIdx > 0)
            {
                idx = (0xFF & bytes[recIdx + 2]) << 0
                        | (0xFF & bytes[recIdx + 3]) << 8;
                gpsRec.sid = new int[idx];
                gpsRec.sidinuse = new bool[idx];
                gpsRec.ele = new int[idx];
                gpsRec.azi = new int[idx];
                gpsRec.snr = new int[idx];
                if (idx == 0)
                {
                    recIdx += 4;
                }
            }
            if (satcnt == idx)
            {
                while (idx-- > 0)
                {
                    if ((logFormat & (1 << BT747Constants.FMT_SID_IDX)) != 0)
                    {
                        gpsRec.sid[satidx] = (0xFF & bytes[recIdx++]) << 0;
                        gpsRec.sidinuse[satidx] = ((0xFF & bytes[recIdx++]) << 0) != 0;
                        // if(false) {
                        // // satcnt is not used - skipping with iffalse)
                        // satcnt=
                        // (0xFF&bytes[recIdx++])<<0
                        // |(0xFF&bytes[recIdx++])<<8;
                        // } else {
                        recIdx += 2;
                        // }
                    }
                    if ((logFormat & (1 << BT747Constants.FMT_ELEVATION_IDX)) != 0)
                    {
                        gpsRec.ele[satidx] = (0xFF & bytes[recIdx++]) << 0
                                | (0xFF & bytes[recIdx++]) << 8;
                    }
                    if ((logFormat & (1 << BT747Constants.FMT_AZIMUTH_IDX)) != 0)
                    {
                        gpsRec.azi[satidx] = (0xFF & bytes[recIdx++]) << 0
                                | (0xFF & bytes[recIdx++]) << 8;
                    }
                    if ((logFormat & (1 << BT747Constants.FMT_SNR_IDX)) != 0)
                    {
                        gpsRec.snr[satidx] = (0xFF & bytes[recIdx++]) << 0
                                | (0xFF & bytes[recIdx++]) << 8;
                    }
                    satidx++;
                }
            }
            else
            {
                Vm.debug("Problem in sat decode");
            }
            // Vm.debug("Offset1:"+recIdx+" "+rcrIdx);
            if (recIdx != rcrIdx)
            {
                Vm.debug("Problem in sat decode (end idx)");
            }
            recIdx = rcrIdx; // Sat information limit is rcrIdx
            if ((logFormat & (1 << BT747Constants.FMT_RCR_IDX)) != 0)
            {
                gpsRec.rcr = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8;
            }
            else
            {
                gpsRec.rcr = rcr_mask; // For filter
            }
            if (nextPointIsWayPt)
            {
                gpsRec.rcr |= BT747Constants.RCR_BUTTON_MASK;
                nextPointIsWayPt = false;
            }
            if ((logFormat & (1 << BT747Constants.FMT_MILLISECOND_IDX)) != 0)
            {
                gpsRec.milisecond = (0xFF & bytes[recIdx++]) << 0
                        | (0xFF & bytes[recIdx++]) << 8;
            }
            else
            {
                gpsRec.milisecond = 0;
            }
            if ((logFormat & (1 << BT747Constants.FMT_DISTANCE_IDX)) != 0)
            {
                long distance = (0xFFL & bytes[recIdx++]) << 0
                        | (0xFFL & bytes[recIdx++]) << 8
                        | (0xFFL & bytes[recIdx++]) << 16
                        | (0xFFL & bytes[recIdx++]) << 24
                        | (0xFFL & bytes[recIdx++]) << 32
                        | (0xFFL & bytes[recIdx++]) << 40
                        | (0xFFL & bytes[recIdx++]) << 48
                        | (0xFFL & bytes[recIdx++]) << 56;
                gpsRec.distance = Convert.longBitsToDouble(distance);
            }

            return valid;

        }
    }
}
