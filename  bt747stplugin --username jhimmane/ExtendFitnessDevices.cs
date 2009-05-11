using System;
using System.Collections.Generic;
using System.Text;

using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.BT747
{
    class ExtendFitnessDevices : IExtendFitnessDevices
    {
        public IList<IFitnessDevice> FitnessDevices
        {
            get { return new IFitnessDevice[] { new FitnessDevice_BT747() }; }
        }
    }
}
