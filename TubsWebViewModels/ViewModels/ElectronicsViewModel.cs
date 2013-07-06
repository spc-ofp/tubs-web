// -----------------------------------------------------------------------
// <copyright file="ElectronicsViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.ViewModels
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
    using Newtonsoft.Json;

    /// <summary>
    /// Electronic equipment ViewModel for data recorded on PS-1 and LL-1.
    /// </summary>
    public sealed class ElectronicsViewModel
    {
        #region Knockout Lists
        // TODO UsageCode

        // TODO Named device types (PS and LL all together)
        #endregion


        public ElectronicsViewModel()
        {
            Buoys = new List<DeviceModel>();
            Vms = new List<DeviceModel>();
            OtherDevices = new List<DeviceModel>();
        }

        // Necessary for navigation
        public int TripId { get; set; }
        public string TripNumber { get; set; }

        public DeviceCategory Gps { get; set; }
        public DeviceCategory TrackPlotter { get; set; }
        public DeviceCategory DepthSounder { get; set; }
        public DeviceCategory SstGauge { get; set; }

        // These devices represent those with associated buoys.
        // Buoys have additional user interface requirements, so that shows up only in this section.
        public List<DeviceModel> Buoys { get; set; }

        // As with buoy, the VMS UI has different fields and so needs a different section.
        public List<DeviceModel> Vms { get; set; }

        // Everything else
        public List<DeviceModel> OtherDevices { get; set; }

        /// <summary>
        /// Electronic device category.
        /// </summary>
        /// <remarks>
        /// These devices represent electronics that are:
        /// a) Expected to be onboard nearly all the time
        /// b) At or near the pinnacle of their development
        /// As such, there's not a high value in tracking specific
        /// makes and models, merely the use of this category of equipment in fishing.
        /// 
        /// Per conversation with Peter Sharples on 05/07/2013.
        /// </remarks>
        public sealed class DeviceCategory
        {
            public int Id { get; set; }

            public string IsInstalled { get; set; }

            // GPS, Track Plotter, Depth Sounder, SST Gauge
            public string Name { get; set; }

            public string Usage { get; set; }
        }

        /// <summary>
        /// Detailed electronic device record.
        /// </summary>
        public sealed class DeviceModel
        {
            public int Id  { get; set; }

            public string DeviceType { get; set; }
            
            // This should only be filled for DeviceType "Other"
            public string Description { get; set; }

            public string IsInstalled { get; set; }

            public string Usage { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string Comments { get; set; }

            // Only for buoy gear
            public int? BuoyCount { get; set; }

            // Only for VMS gear
            public string SealsIntact { get; set; }
            public string SystemDescription { get; set; }

            // Knockout UI integration
            public bool _destroy { get; set; }
            public bool NeedsFocus { get; set; }
        }
    }
}