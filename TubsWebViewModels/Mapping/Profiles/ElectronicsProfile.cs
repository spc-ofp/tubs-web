﻿// -----------------------------------------------------------------------
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
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    using DAL = Spc.Ofp.Tubs.DAL;

    /// <summary>
    /// AutoMapper profile for the conversion of electronic equipment to/from
    /// MVC ViewModel.
    /// </summary>
    public class ElectronicsProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            // ViewModel to Entity

            
            // Entity to ViewModel
            CreateMap<DAL.Entities.CommunicationServices, ElectronicsViewModel.InformationServices>()
                // Same property names, but use BooleanResolver to get correct string representation
                .ForMember(d => d.HasWeatherFax, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasWeatherFax))
                .ForMember(d => d.HasSatelliteMonitor, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasSatelliteMonitor))
                .ForMember(d => d.HasPhytoplanktonService, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasPhytoplanktonService))
                .ForMember(d => d.HasSeaSurfaceTemperatureService, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasSeaSurfaceTemperatureService))
                .ForMember(d => d.HasSeaHeightService, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasSeaHeightService))
                .ForMember(d => d.HasOther, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasOther))
                ;

            CreateMap<DAL.Entities.CommunicationServices, ElectronicsViewModel.CommunicationServices>()
                // Same property names, but use BooleanResolver to get correct string representation
                .ForMember(d => d.HasSatellitePhone, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasSatellitePhone))
                .ForMember(d => d.HasMobilePhone, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasMobilePhone))
                .ForMember(d => d.HasFax, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasFax))
                .ForMember(d => d.HasEmail, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasEmail))
                ;

            CreateMap<DAL.Entities.ElectronicDevice, ElectronicsViewModel.DeviceCategory>()
                .ForMember(d => d.IsInstalled, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.IsInstalled))
                .ForMember(d => d.Name, o => o.ResolveUsing<DeviceNameResolver>().FromMember(s => s.DeviceType))
                ;

            CreateMap<DAL.Entities.ElectronicDevice, ElectronicsViewModel.DeviceModel>()
                // Ignore Knockout properties
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())
                // Same property name, but needs a resolver
                .ForMember(d => d.IsInstalled, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.IsInstalled))
                .ForMember(d => d.DeviceType, o => o.ResolveUsing<DeviceNameResolver>().FromMember(s => s.DeviceType))
                // Custom property
                .ForMember(d => d.BuoyCount, o => o.MapFrom(s => s.HowMany))
                ;

            /*
             * NOTE:  For older trips, devices which are currently considered ubiquitous were less common.
             * In order to correctly manage the device to category mapping, we'll want to come up with the
             * "best" usage code from all the alternatives.
             * 
             * For example, if there are 3 or 4 GPS devices, and one is ALL, the usage code for the GPS
             * category should be "ALL".  Handle this in the AfterMap by deleting "category" devices and updating
             * usage code as necessary.
             * 
             * This might also be managed via database update, creating an electronic device record with no make, model, or comments content
             * that contains the best usage code.
             * 
             */

            CreateMap<DAL.Entities.Trip, ElectronicsViewModel>()
                .BeforeMap((s, d) =>
                {
                    // Sort Electronics by UsageCode from best to worst.
                    // In this way, .FirstOrDefault should pick up the best case for
                    // category devices
                    if (null != s && null != s.Electronics)
                    {
                        s.SortElectronics();
                    }                   
                })
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.ServiceId, o => o.MapFrom(s => null == s.CommunicationServices ? 0 : s.CommunicationServices.Id))
                // Categories
                .ForMember(d => d.Gps, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType == DAL.Common.ElectronicDeviceType.Gps).FirstOrDefault()))
                .ForMember(d => d.TrackPlotter, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType == DAL.Common.ElectronicDeviceType.TrackPlotter).FirstOrDefault()))
                .ForMember(d => d.DepthSounder, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType == DAL.Common.ElectronicDeviceType.DepthSounder).FirstOrDefault()))
                .ForMember(d => d.SstGauge, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType == DAL.Common.ElectronicDeviceType.SstGauge).FirstOrDefault()))
                // VMS devices
                .ForMember(d => d.Vms, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType == DAL.Common.ElectronicDeviceType.Vms)))
                // Buoy devices
                .ForMember(d => d.Buoys, o => o.MapFrom(s => s.Electronics.Where(e => e.DeviceType.IsBuoy())))
                .ForMember(d => d.Communications, o => o.MapFrom(s => s.CommunicationServices))
                .ForMember(d => d.Info, o => o.MapFrom(s => s.CommunicationServices))
                // Everything else handled in AfterMap
                .ForMember(d => d.OtherDevices, o => o.Ignore())
                .AfterMap((s, d) => 
                {
                    if (null == d.Info)
                        d.Info = new ElectronicsViewModel.InformationServices();

                    if (null == d.Communications)
                        d.Communications = new ElectronicsViewModel.CommunicationServices();

                    foreach (var device in s.Electronics)
                    {
                        // Skip categories, buoys, and VMS, which are all mapped elsewhere
                        if (null == device || device.DeviceType.IsDeviceCategory() || device.DeviceType.IsBuoy() || device.DeviceType == DAL.Common.ElectronicDeviceType.Vms)
                            continue;

                        d.OtherDevices.Add(Mapper.Map<DAL.Entities.ElectronicDevice, ElectronicsViewModel.DeviceModel>(device));
                    }
                })
                ;

        }
    }
}
