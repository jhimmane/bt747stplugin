using System;
using System.Collections.Generic;
using System.Xml;

using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.BT747
{
    class Plugin : IPlugin
    {
        public Plugin()
        {
            instance = this;
        }

        #region IPlugin Members

        public Guid Id
        {
            get { return new Guid("41d1922d-7388-443a-bdd5-3e8acc6e2aef"); }
        }

        public IApplication Application
        {
            get { return application; }
            set { application = value; }
        }

        public string Name
        {
            get { return "SportTracks BT747 Device Plugin"; }
        }

        public string BT747LastPosition = "00000000";


        public string Version
        {
            get { return GetType().Assembly.GetName().Version.ToString(3); }
        }

        public void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            BT747LastPosition = pluginNode.GetAttribute(xmlTags.BT747Lastposition);             
        }

        public void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            pluginNode.SetAttribute(xmlTags.BT747Lastposition, BT747LastPosition); 
        }

        #endregion


        public static Plugin Instance
        {
            get { return instance; }
        }

       
        #region Private members

        private static Plugin instance = null;
        private IApplication application;
        
        
        #endregion

        private class xmlTags
        {
            public const string BT747Lastposition = "BT747Lastposition";
        } 
    }
}
