// -----------------------------------------------------------------------
// <copyright file="ElectronicsProfile.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Mapping.Profiles
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
    using AutoMapper;
    using DAL = Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common; // For DateTime 'Merge'
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ElectronicsProfile : Profile
    {
        /*
         * I'm aware that this is awful, terrible, gross.  However, due to time
         * pressure (and a large set of legacy data), this is probably the easiest
         * way to manage this.
         */
        public const int GpsId = 21;
        public const int DepthSounderId = 14;
        public const int TrackPlotterId = 51;
        public const int SstId = 47;
        public const int BirdRadarId = 5;
        public const int SonarId = 48;
        public const int GpsBuoyId = 89;
        public const int EchoSoundingBuoyId = 99;
        public const int NetDepthId = 127;
        public const int DopplerCurrentMeterId = 17;
        public const int VmsId = 53;
        
        protected override void Configure()
        {
            base.Configure();

            CreateMap<DAL.Entities.ElectronicDevice, ElectronicsViewModel.DeviceModel>()
                .ForMember(d => d.DeviceName, o => o.MapFrom(s => s.DeviceType.Category))
                .ForMember(d => d.Installed, o => o.MapFrom(s => s.IsInstalled))
                ;

            CreateMap<DAL.Entities.Trip, ElectronicsViewModel>()
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.Gps, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == GpsId).FirstOrDefault()))
                .ForMember(d => d.DepthSounder, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == DepthSounderId).FirstOrDefault()))
                .ForMember(d => d.TrackPlotter, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == TrackPlotterId).FirstOrDefault()))
                .ForMember(d => d.SstGauge, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == SstId).FirstOrDefault()))
                .ForMember(d => d.BirdRadar, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == BirdRadarId).FirstOrDefault()))
                .ForMember(d => d.Sonar, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == SonarId).FirstOrDefault()))
                .ForMember(d => d.GpsBuoys, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == GpsBuoyId).FirstOrDefault()))
                .ForMember(d => d.EchoSoundingBuoy, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == EchoSoundingBuoyId).FirstOrDefault()))
                .ForMember(d => d.NetDepthInstrumentation, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == NetDepthId).FirstOrDefault()))
                .ForMember(d => d.DopplerCurrentMeter, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == DopplerCurrentMeterId).FirstOrDefault()))
                .ForMember(d => d.Vms, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.Id == VmsId).FirstOrDefault()))
                .AfterMap((s,d) => 
                {

                })
                ;
        }
    }
}
