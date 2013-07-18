// -----------------------------------------------------------------------
// <copyright file="TripSummaryProfile.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
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
    using System.Linq;
    using AutoMapper;
    using TubsWeb.ViewModels;
    using DAL = Spc.Ofp.Tubs.DAL;

    /// <summary>
    /// AutoMapper profile for the conversion of Trip to Trip summary.
    /// </summary>
    public class TripSummaryProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();
            // There is no need for a ViewModel to Entity transform

            // Entity to ViewModel
            CreateMap<DAL.Entities.Trip, TripSummaryViewModel>()
                .Include<DAL.Entities.PurseSeineTrip, TripSummaryViewModel>()
                .Include<DAL.Entities.LongLineTrip, TripSummaryViewModel>()
                .ForMember(d => d.StaffCode, o => o.MapFrom(s => s.Observer.StaffCode))
                .ForMember(d => d.ObserverName, o => o.MapFrom(s => s.Observer.ToString()))
                .ForMember(d => d.ReturnPort, o => o.MapFrom(s => s.ReturnPort.Name))
                .ForMember(d => d.ReturnPortCode, o => o.MapFrom(s => s.ReturnPort.PortCode))
                .ForMember(d => d.DeparturePort, o => o.MapFrom(s => s.DeparturePort.Name))
                .ForMember(d => d.DeparturePortCode, o => o.MapFrom(s => s.DeparturePort.PortCode))                
                .ForMember(d => d.VesselName, o => o.MapFrom(s => s.Vessel.Name))
                .ForMember(d => d.VesselFlag, o => o.MapFrom(s => s.Vessel.RegisteredCountryCode))
                .ForMember(d => d.Version, o => o.MapFrom(s => s.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.EntryComments, o => o.MapFrom(s => s.Comments))
                .ForMember(d => d.HasGen3, o => o.MapFrom(s => (null != s.TripMonitor) || (s.Gen3Answers.Count > 0)))
                .ForMember(d => d.SightingCount, o => o.MapFrom(s => s.Sightings.Count()))
                .ForMember(d => d.TransferCount, o => o.MapFrom(s => s.Transfers.Count()))
                .ForMember(d => d.InteractionCount, o => o.MapFrom(s => s.Interactions.Count()))
                .ForMember(d => d.HasPositions, o => o.MapFrom(s => s.Pushpins.Count()))
                .ForMember(d => d.GearCode, o => o.Ignore()) // Set in submap
                .ForMember(d => d.VesselDays, o => o.Ignore()) // PS only
                .ForMember(d => d.Cpue, o => o.Ignore()) // LL and PS, but only present on PS trips currently
                .ForMember(d => d.SeaDayCount, o => o.Ignore())
                .ForMember(d => d.ExpectedSeaDayCount, o => o.Ignore())
                .ForMember(d => d.SetCount, o => o.Ignore())
                .ForMember(d => d.ExpectedSetCount, o => o.Ignore())
                .ForMember(d => d.HasCrew, o => o.Ignore())
                .ForMember(d => d.Gen5Count, o => o.Ignore())
                ;

            CreateMap<DAL.Entities.PurseSeineTrip, TripSummaryViewModel>()
                .ForMember(d => d.Cpue, o => o.MapFrom(s => s.Cpue))
                .ForMember(d => d.VesselDays, o => o.MapFrom(s => s.VesselDays))
                .ForMember(d => d.GearCode, o => o.UseValue("S"))
                .ForMember(d => d.SeaDayCount, o => o.MapFrom(s => s.SeaDays.Count()))
                .ForMember(d => d.HasCrew, o => o.MapFrom(s => s.Crew.Any()))
                // TODO: This should also take into account # of PS-2 pages
                .ForMember(d => d.ExpectedSeaDayCount, o => o.MapFrom(s => s.LengthInDays))
                .ForMember(d => d.SetCount, o => o.MapFrom(s => s.FishingSets.Count()))
                .ForMember(d => d.Gen5Count, o => o.MapFrom(s => s.FishAggregatingDevices.Count()))
                ;

            CreateMap<DAL.Entities.LongLineTrip, TripSummaryViewModel>()
                .ForMember(d => d.GearCode, o => o.UseValue("L"))
                .ForMember(d => d.SetCount, o => o.MapFrom(s => s.FishingSets.Count()))
                ;
        }
    }
}
