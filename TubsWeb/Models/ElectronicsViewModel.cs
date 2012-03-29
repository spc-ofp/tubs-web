// -----------------------------------------------------------------------
// <copyright file="ElectronicsViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Models
{
    /*
     * This file is part of TUBS.
     *
     * TUBS is free software: you can redistribute it and/or modify
     * it under the terms of the GNU Affero General Public License as published by
     * the Free Software Foundation, either version 3 of the License, or
     * (at your option) any later version.
     *  
     * TUBS is distributed in the hope that it will be useful,
     * but WITHOUT ANY WARRANTY; without even the implied warranty of
     * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
     * GNU Affero General Public License for more details.
     *  
     * You should have received a copy of the GNU Affero General Public License
     * along with TUBS.  If not, see <http://www.gnu.org/licenses/>.
     */
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
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
        public DeviceModel Vms { get; set; }

        // Anything else that's not in the above two categories
        public List<DeviceModel> OtherDevices { get; set; }

        public class DeviceModel
        {
            public DeviceModel(string deviceName)
            {
                DeviceName = deviceName;
            }

            public int Id;
            public string DeviceName;
            public string Installed = "N/A";
            public string Usage = "N/A";
            public string Make;
            public string Model;
            public string Comments;
        }
    }
}