﻿// -----------------------------------------------------------------------
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Electronic equipment ViewModel for data recorded on PS-1 and LL-1.
    /// </summary>
    public sealed class ElectronicsViewModel
    {
        #region Knockout Lists
        public IList<string> BooleanValues = new List<string> { null, "YES", "NO" };
        public IList<string> UsageCodes = new List<string> { String.Empty, "ALL", "TRA", "OIF", "SIF", "RAR", "BRO", "NOL" };
        public IList<string> DeviceTypes = new List<string>
        {
            "Other",
            "Bird Radar",
            "Sonar",            
            "Net Depth Instrumentation",
            "Doppler Current Meter",
            "Radio Beacon Direction Finder",
            "Bathythermograph"
        };
        public IList<string> BuoyTypes = new List<string>
        {
            "GPS Buoys",
            "Echo Sounding Buoy",
        };
        public IList<string> SatelliteSystems = new List<string> { String.Empty, "INMARSAT", "Iridium", "Argos" };

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
        // PK for CommunicationServices Entity
        public int ServiceId { get; set; }

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


        public CommunicationServices Communications { get; set; }

        public InformationServices Info { get; set; }

        /// <summary>
        /// This read-only property assists with finding devices that the client-side
        /// script marked for deletion.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<int> DeletedDevices
        {
            get
            {
                var deletedBuoys =
                    from b in this.Buoys
                    where b != null && b._destroy
                    select b.Id;

                var deletedVms =
                    from v in this.Vms
                    where v != null && v._destroy
                    select v.Id;

                var deletedOther =
                    from d in this.OtherDevices
                    where d != null && d._destroy
                    select d.Id;

                // Who doesn't love LINQ?
                return deletedBuoys.Union(deletedVms).Union(deletedOther);
            }
        }

        [JsonIgnore]
        public IEnumerable<DeviceCategory> Categories
        {
            // TODO:  It might make sense to force the name property for these devices
            // Existing code sets the data, but it seems a bit fiddly for my tastes.
            get
            {
                if (null != this.Gps && this.Gps.HasData)
                    yield return this.Gps;

                if (null != this.TrackPlotter && this.TrackPlotter.HasData)
                    yield return this.TrackPlotter;

                if (null != this.DepthSounder && this.DepthSounder.HasData)
                    yield return this.DepthSounder;

                if (null != this.SstGauge && this.SstGauge.HasData)
                    yield return this.SstGauge;
            }
        }

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
            // TODO: Do we need the Name property?
            public string Name { get; set; }

            public string Usage { get; set; }

            [JsonIgnore]
            public bool HasData
            {
                get
                {
                    return
                        this.Id != default(int) ||
                        !String.IsNullOrEmpty(this.IsInstalled) ||
                        !String.IsNullOrEmpty(this.Usage);
                }
            }
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

        public sealed class CommunicationServices
        {
            public string HasSatellitePhone { get; set; }
            public string SatellitePhoneNumber { get; set; }

            public string HasMobilePhone { get; set; }
            public string MobilePhoneNumber { get; set; }

            public string HasFax { get; set; }
            public string FaxNumber { get; set; }

            public string HasEmail { get; set; }
            public string EmailAddress { get; set; }
        }

        public sealed class InformationServices
        {
            public string HasWeatherFax { get; set; }

            public string HasSatelliteMonitor { get; set; }

            public string HasOther { get; set; }

            public string HasPhytoplanktonService { get; set; }
            public string PhytoplanktonUrl { get; set; }

            public string HasSeaSurfaceTemperatureService { get; set; }
            public string SeaSurfaceTemperatureUrl { get; set; }

            public string HasSeaHeightService { get; set; }
            public string SeaHeightServiceUrl { get; set; }
        }
    }
}