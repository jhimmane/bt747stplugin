using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh615Device
    {
        public void Open()
        {
            port = OpenPort();
        }

        public void Close()
        {
            if (port != null)
            {
                port.Close();
                port = null;
            }
        }

        public IList<Gh615Packet.TrackFileHeader> ReadTrackHeaders(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            Int16[] tracks = new Int16[2];

            byte[] getHeadersPacket = Gh615Packet.GetTrackFileHeaders();
            byte[] data = SendPacket(port, getHeadersPacket).PacketData;
            return Gh615Packet.UnpackTrackHeaders(data);
        }

        public IList<Gh615Packet.TrackFileSection> ReadTracks(IList<Gh615Packet.TrackFileHeader> tracks, IJobMonitor monitor)
        {
            if (tracks.Count == 0) return new Gh615Packet.TrackFileSection[0];

            float totalPoints = 0;
            IList<Int16> trackIndexes = new List<Int16>();
            foreach (Gh615Packet.TrackFileHeader header in tracks)
            {
                totalPoints += header.TrackPointCount;
                trackIndexes.Add(header.TrackPointIndex);
            }
            float pointsRead = 0;

            IList<Gh615Packet.TrackFileSection> trackSections = new List<Gh615Packet.TrackFileSection>();
            byte[] getFilesPacket = Gh615Packet.GetTrackFileSections(trackIndexes);
            byte[] getNextPacket = Gh615Packet.GetNextSection();
            byte[] data = SendPacket(port, getFilesPacket).PacketData;

            monitor.PercentComplete = 0;

            Gh615Packet.TrackFileSection trackSection;
            do
            {
                trackSection = Gh615Packet.UnpackTrackSection(data);
                if (trackSection != null)
                {
                    pointsRead += trackSection.EndPointIndex - trackSection.StartPointIndex + 1;

                    string statusProgress = trackSection.StartTime.ToShortDateString() + " " + trackSection.StartTime.ToShortTimeString();
                    monitor.StatusText = String.Format(CommonResources.Text.Devices.ImportJob_Status_Reading, statusProgress);
                    monitor.PercentComplete = pointsRead / totalPoints;

                    trackSections.Add(trackSection);
                    data = SendPacket(port, getNextPacket).PacketData;
                }
            } while (trackSection != null);

            monitor.PercentComplete = 1;
            return trackSections;
        }

        private SerialPort OpenPort()
        {
            for (int i = 1; i <= 30; i++)
            {
                SerialPort port = null;
                try
                {
                    port = new SerialPort("COM" + i, 57600);
                    port.ReadTimeout = 1000;
                    port.Open();
                    byte[] packet = Gh615Packet.GetSystemConfiguration();
                    byte commandId = Gh615Packet.SendPacketCommandId(packet);
                    Gh615Packet.Response responsePacket = SendPacket(port, packet);
                    if (responsePacket.CommandId == commandId && responsePacket.PacketLength > 1)
                    {
                        return port;
                    }
                    else if (port != null)
                    {
                        port.Close();
                    }
                }
                catch (Exception ex)
                {
                    if (port != null)
                    {
                        port.Close();
                    }
                }
            }
            throw new Exception(CommonResources.Text.Devices.ImportJob_Status_CouldNotOpenDeviceError);
        }

        private static Gh615Packet.Response SendPacket(SerialPort port, byte[] packet)
        {
            Gh615Packet.Response received = new Gh615Packet.Response();

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

        private SerialPort port;
    }
}
