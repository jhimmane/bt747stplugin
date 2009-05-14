using System;
using System.Collections.Generic;
using System.Text;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.BT747
{
    class ImportJob
    {
        public ImportJob(string sourceDescription, DeviceConfigurationInfo configInfo, IJobMonitor monitor, IImportResults importResults)
        {
            this.sourceDescription = sourceDescription.Replace(Environment.NewLine, " ");
            this.configInfo = configInfo;
            this.monitor = monitor;
            this.importResults = importResults;
        }

        public bool Import()
        {
            BT747Device device = new BT747Device(configInfo);
            try
            {
                monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;
                device.Open(configInfo.portNumber);

                monitor.PercentComplete = 0;
                IList<BT747Packet.TrackFileSection> sections = device.ReadNMEATracks(monitor, configInfo.trackChange, configInfo.ImportOnlyNew);
                if (sections == null) return false;
                AddActivities(importResults, sections);
                return true;
            }
            finally
            {
                device.Close();
            }
        }
        
        private void AddActivities(IImportResults importResults, IList<BT747Packet.TrackFileSection> trackSections)
        {
            //System.Diagnostics.Debug.WriteLine("AddActivities - start", "BT747");
            
            IActivity activity = null;
            foreach (BT747Packet.TrackFileSection section in trackSections)
            {
                if (section.TrackPointCount > 1)
                {
                    //pointTime = section.StartTime.ToUniversalTime().AddHours(configInfo.HoursAdjustment);
                    activity = importResults.AddActivity(section.StartTime);
                    activity.Metadata.Source = string.Format(CommonResources.Text.Devices.ImportJob_ActivityImportSource, sourceDescription);
                    activity.TotalTimeEntered = TimeSpan.FromTicks(section.TrackPoints[section.TrackPointCount - 1].PointTime.Ticks - section.StartTime.Ticks);
                    activity.TotalDistanceMetersEntered = System.Convert.ToSingle(section.TotalDistanceMeters);
                    activity.GPSRoute = new GPSRoute();
                    activity.HeartRatePerMinuteTrack = new NumericTimeDataSeries();

                    if (activity != null)
                    {
                        foreach (BT747Packet.TrackPoint point in section.TrackPoints)
                        {
                            float latitude = System.Convert.ToSingle(point.Latitude);
                            float longitude = System.Convert.ToSingle(point.Longitude);
                            float elevation = point.Altitude;
                            activity.GPSRoute.Add(point.PointTime, new GPSPoint(latitude, longitude, elevation));
                        }
                    }
                }
            }  
        }

        private string sourceDescription;
        private DeviceConfigurationInfo configInfo;
        private IJobMonitor monitor;
        private IImportResults importResults;
    }
}
