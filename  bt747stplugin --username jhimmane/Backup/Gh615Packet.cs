using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh615Packet
    {
        public class Header
        {
            public DateTime StartTime;
            public TimeSpan TotalTime;
            public Int32 TotalDistanceMeters;
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
            public Int32 Latitude; // Degrees * 1000000
            public Int32 Longitude; // Degrees * 1000000
            public Int16 Altitude; // Meters
            public Int16 Speed; // Kilometers per hour * 100
            public Byte HeartRate;
            public Int16 IntervalTime; // Seconds * 10
        }

        public class Response
        {
            public byte CommandId;
            public Int16 PacketLength;
            public byte[] PacketData;
            public byte Checksum;
        }

        public static IList<TrackFileHeader> UnpackTrackHeaders(byte[] payload)
        {
            int numHeaders = payload.Length / 24;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i*24;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, payload, trackStart);
                header.TrackPointCount = ReadInt16(payload, trackStart + 20);
                header.TrackPointIndex = ReadInt16(payload, trackStart + 22);
                headers.Add(header);
            }
            return headers;
        }

        public static TrackFileSection UnpackTrackSection(byte[] payload)
        {
            if (payload.Length < 26) return null;

            TrackFileSection section = new TrackFileSection();
            ReadHeader(section, payload, 0);
            section.TrackPointCount = ReadInt16(payload, 20);
            section.StartPointIndex = ReadInt16(payload, 22);
            section.EndPointIndex = ReadInt16(payload, 24);
            int offset = 26;
            while (offset < payload.Length)
            {
                TrackPoint point = new TrackPoint();
                point.Latitude = ReadInt32(payload, offset);
                point.Longitude = ReadInt32(payload, offset + 4);
                point.Altitude = ReadInt16(payload, offset + 8);
                point.Speed = ReadInt16(payload, offset + 10);
                point.HeartRate = payload[offset + 12];
                point.IntervalTime = ReadInt16(payload, offset + 13);
                section.TrackPoints.Add(point);
                offset += 15;
            }
            return section;
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

        private Gh615Packet()
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
