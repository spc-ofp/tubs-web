

namespace TubsWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    
    public class ElectronicsViewModel
    {
        public ElectronicsViewModel()
        {
            OtherDevices = new List<DeviceModel>();
        }

        // Necessary for navigation
        public int TripId { get; set; }

        // Devices for which Make, Model, and Comments are not collected
        public DeviceModel Gps { get; set; }
        public DeviceModel TrackPlotter { get; set; }
        public DeviceModel DepthSounder { get; set; }
        public DeviceModel SstGauge { get; set; }

        // Named devices that the Observer is checking for
        public DeviceModel BirdRadar { get; set; }
        public DeviceModel Sonar { get; set; }
        public DeviceModel GpsBuoys { get; set; }
        public DeviceModel EchoSoundingBuoy { get; set; }
        public DeviceModel NetDepthInstrumentation { get; set; }
        public DeviceModel DopplerCurrentMeter { get; set; }

        // Anything else that's not in the above two categories
        public List<DeviceModel> OtherDevices { get; set; }

        public class DeviceModel
        {
            public DeviceModel(string deviceName)
            {
                DeviceName = deviceName;
            }

            public string DeviceName;
            public string Installed = "N/A";
            public string Usage = "N/A";
            public string Make;
            public string Model;
            public string Comments;
        }
    }
}