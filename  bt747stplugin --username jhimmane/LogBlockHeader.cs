/*
	(c)2008 Thomas Mohme
	tmohme at sourceforge.net

	This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


//import java.io.DataInputStream;
//import java.io.IOException;

/**
 * A LogBlockHeader primarily provides the engine with an initial LogFormat.
 * @author Thomas Mohme
 */

namespace ZoneFiveSoftware.SportTracks.Device.BT747
{

    class LogBlockHeader
    {
        public static int LOG_BLOCK_HEADER_SIZE
                                                = 20	// header_info
                                                + 32    // sector_status
                                                + 454	// 0xFF
                                                + 6;    // 0x2AFFBBBBBBBB

        public int count = 0xffff;
        public LogFormat format = null;
        public int fix = 0xffff;
        public int period = 0xffff;
        public int distance = 0xffff;
        public int speed = 0xffff;

        public LogBlockHeader(string headerString)
        {
            //System.Diagnostics.Debug.WriteLine("BT747: LogBlockHeader: Start");
            //System.Diagnostics.Debug.WriteLine("BT747: LogBlockHeader: Headerstring:\n" + headerString);
            
            int n = int.Parse(ReverseBytes(headerString.Substring(0, 4)), System.Globalization.NumberStyles.HexNumber);
            //System.Diagnostics.Debug.WriteLine("BT747: Header: ");
            if (n != 0xffff)
            {
                this.count = n;
            }
            this.format = new LogFormat(long.Parse(ReverseBytes(headerString.Substring(4, 8)), System.Globalization.NumberStyles.HexNumber));
            this.fix = int.Parse(ReverseBytes(headerString.Substring(12, 4)), System.Globalization.NumberStyles.HexNumber);
            this.period = int.Parse(ReverseBytes(headerString.Substring(16, 4)), System.Globalization.NumberStyles.HexNumber);
            this.distance = int.Parse(ReverseBytes(headerString.Substring(20, 4)), System.Globalization.NumberStyles.HexNumber); ;
            this.speed = int.Parse(ReverseBytes(headerString.Substring(24, 4)), System.Globalization.NumberStyles.HexNumber);

        } // constructor LogBlockHeader(byte[])

        public static string ReverseBytes(string x)
        {
            char[] charArray = new char[x.Length];
            int len = x.Length - 1;

            for (int i = 0; i <= len; i = i + 2)
            {
                charArray[i] = x[len - i - 1];
                charArray[i + 1] = x[len - i];
            }
            return new string(charArray);
        }



        public LogFormat getFormat()
        {
            return this.format;
        } // LogFormat getFormat()
        /*
            public String toString() {
                StringBuilder sb = new StringBuilder(100);
                sb.append(this.getClass().getName());
                if (this.count != null) {
                    sb.append(": ");
                    sb.append(this.count);
                } else {
                    sb.append(":Unknown number of");
                }
                sb.append(" records in format ");
                sb.append(this.format);
                sb.append(" (");
                sb.append(this.fix);
                sb.append("), p=");
                sb.append(this.period/10.0);
                sb.append("s, d=");
                sb.append(this.distance/10.0);
                sb.append("m, s=");
                sb.append(this.speed/10.0);
                sb.append("km/h");

                return sb.toString();
            }*/
    } // end of class LogBlockHeader
}