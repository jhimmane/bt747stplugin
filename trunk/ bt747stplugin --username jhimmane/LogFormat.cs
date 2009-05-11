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


/**
 * A LogFormat object is a container for the layout-information of the following LogPackets.
 * <br>
 * This class provides some convenience methods.
 * @author Thomas Mohme
 */

namespace ZoneFiveSoftware.SportTracks.Device.BT747
{

    public class LogFormat
    {
        private long format;

        public LogFormat(long f)
        {
            this.format = f;
        } // constructor LogFormat(int)
            
        public LogFormat(LogFormat other)
        {
            this.format = other.format;
        } // constructor LogFormat(LogFormat)

        public bool hasUTC() 
        {
            //System.Diagnostics.Debug.WriteLine("BT747,LogFormat - hasUTC, format: " + format);
            return ((this.format & (1 << 0)) != 0); 
        }
        public bool hasFix() { return ((this.format & (1 << 1)) != 0); }
        public bool hasLatitude() { return ((this.format & (1 << 2)) != 0); }
        public bool hasLongitude() { return ((this.format & (1 << 3)) != 0); }
        public bool hasHeight() { return ((this.format & (1 << 4)) != 0); }
        public bool hasSpeed() { return ((this.format & (1 << 5)) != 0); }
        public bool hasHeading() { return ((this.format & (1 << 6)) != 0); }
        public bool hasDSta() { return ((this.format & (1 << 7)) != 0); }
        public bool hasDAge() { return ((this.format & (1 << 8)) != 0); }
        public bool hasPDOP() { return ((this.format & (1 << 9)) != 0); }
        public bool hasHDOP() { return ((this.format & (1 << 10)) != 0); }
        public bool hasVDOP() { return ((this.format & (1 << 11)) != 0); }
        public bool hasNSat() { return ((this.format & (1 << 12)) != 0); }
        public bool hasSID() { return ((this.format & (1 << 13)) != 0); }
        public bool hasElevation() { return ((this.format & (1 << 14)) != 0); }
        public bool hasAzimuth() { return ((this.format & (1 << 15)) != 0); }
        public bool hasSNR() { return ((this.format & (1 << 16)) != 0); }
        public bool hasRCR() { return ((this.format & (1 << 17)) != 0); }
        public bool hasMSec() { return ((this.format & (1 << 18)) != 0); }
        public bool hasDist() { return ((this.format & (1 << 19)) != 0); }
        public bool isLowPrecision() { return ((this.format & (1 << 31)) != 0); }


        public void update(int newFormat)
        {
            this.format = newFormat;
        }

        public int getMaxPacketSize()
        {
            int total = 0;
            if (this.hasUTC()) {total += 2 * 4;}
            if (this.hasFix() ) {total += 2 * 2;}
            if (this.hasLatitude()) {total += 2 * 8;}
            if (this.hasLongitude()) {total += 2 * 8;}
            if (this.hasHeight()) {total += 2 * 4;}
            if (this.hasSpeed()) {total += 2 * 4;}
            if (this.hasHeading()){total += 2 * 4;}
            if (this.hasDSta()) {total += 2 * 2;}
            if (this.hasDAge()) {total += 2 * 4;}
            if (this.hasPDOP()) {total += 2 * 2;}
            if (this.hasHDOP()){total += 2 * 2;}
            if (this.hasVDOP()) {total += 2 * 2;}
            if (this.hasNSat()) {
                total += 2 * 2;
                total += 2 * 4 * 16; //16 is the maximum number visible gps satellites (Confirmation?)
            }
            if (this.hasRCR()) {total += 2 * 2;}
            if (this.hasMSec()) {total += 2 * 2;}
            if (this.hasDist()) {total += 2 * 8;}
            return total;
        }
        

        //@Override
      /*  public string toString()
        {
            return "0x" + this.format;
        }*/
    } // end of class LogFormat
}
