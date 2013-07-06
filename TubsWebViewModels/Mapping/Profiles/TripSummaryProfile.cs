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

        // TODO: This could potentially be a read-only property of Trip
        internal int TripLength(DAL.Entities.Trip trip)
        {
            if (!trip.DepartureDate.HasValue || !trip.ReturnDate.HasValue)
                return 0;

            var dep = trip.DepartureDate.Value;
            var ret = trip.ReturnDate.Value;
            var diff = dep.Subtract(ret);
            return (int)Math.Abs(diff.TotalDays);
        }

        // TODO: This could potentially be a read-only property of PurseSeineTrip
        // TODO: Consider adding an additional read-only property to PurseSeineTrip
        // to return catch by weight for target species
        internal int SetCount(DAL.Entities.PurseSeineTrip trip)
        {
            return (
                from d in trip.SeaDays
                from a in d.Activities
                where a.ActivityType.HasValue && a.ActivityType.Value == Spc.Ofp.Tubs.DAL.Common.ActivityType.Fishing
                select 1
            ).Sum();
        }

        // TODO:  Add this to PurseSeineTrip
        internal int FadCount(DAL.Entities.PurseSeineTrip trip)
        {
            return (
                from d in trip.SeaDays
                from a in d.Activities
                where a.Fad != null
                select 1
            ).Sum();
        }
        
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
                .ForMember(d => d.SeaDayCount, o => o.Ignore())
                .ForMember(d => d.ExpectedSeaDayCount, o => o.Ignore())
                .ForMember(d => d.SetCount, o => o.Ignore())
                .ForMember(d => d.ExpectedSetCount, o => o.Ignore())
                .ForMember(d => d.HasCrew, o => o.Ignore())
                .ForMember(d => d.Gen5Count, o => o.Ignore())
                ;

            CreateMap<DAL.Entities.PurseSeineTrip, TripSummaryViewModel>()
                .ForMember(d => d.GearCode, o => o.UseValue("S"))
                .ForMember(d => d.SeaDayCount, o => o.MapFrom(s => s.SeaDays.Count()))
                .ForMember(d => d.HasCrew, o => o.MapFrom(s => s.Crew.Count()))
                .ForMember(d => d.ExpectedSeaDayCount, o => o.MapFrom(s => TripLength(s)))
                .ForMember(d => d.SetCount, o => o.MapFrom(s => SetCount(s)))
                .ForMember(d => d.Gen5Count, o => o.MapFrom(s => FadCount(s)))
                ;

            CreateMap<DAL.Entities.LongLineTrip, TripSummaryViewModel>()
                .ForMember(d => d.GearCode, o => o.UseValue("L"))
                .ForMember(d => d.SetCount, o => o.MapFrom(s => s.FishingSets.Count()))
                ;
        }
    }
}
