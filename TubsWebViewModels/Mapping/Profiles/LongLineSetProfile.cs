// -----------------------------------------------------------------------
// <copyright file="LongLineSetProfile.cs" company="Secretariat of the Pacific Community">
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
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using DAL = Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common; // For DateTime 'Merge'
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    
    public class LongLineSetProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            // ViewModel to Entity
            // TODO

            // Entity to ViewModel
            CreateMap<DAL.Entities.LongLineSetHaulEvent, LongLineSetViewModel.Position>()
                .ForMember(d => d.Latitude, o => o.MapFrom(s => s.Latitude))
                .ForMember(d => d.Longitude, o => o.MapFrom(s => s.Longitude))
                .ForMember(d => d.LocalTime, o => o.MapFrom(s => s.LogTimeOnly))
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())                
                ;

            CreateMap<DAL.Entities.LongLineSetHaulNote, LongLineSetViewModel.Comment>()
                .ForMember(d => d.LocalTime, o => o.MapFrom(s => s.LogTimeOnly))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.Comments))
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())
                ;

            CreateMap<DAL.Entities.LongLineSet, LongLineSetViewModel>()
                .ForMember(d => d.HasNext, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.HasPrevious, o => o.Ignore())
                .ForMember(d => d.MaxSets, o => o.Ignore())
                .ForMember(d => d.NextSet, o => o.Ignore())
                .ForMember(d => d.PreviousSet, o => o.Ignore())
                .ForMember(d => d.BooleanValues, o => o.Ignore())
                .ForMember(d => d.VelocityUnits, o => o.Ignore())
                .ForMember(d => d.SetId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Trip.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.Trip.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Trip.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.TotalBaskets, o => o.MapFrom(s => s.TotalBasketCount))
                .ForMember(d => d.TotalHooks, o => o.MapFrom(s => s.TotalHookCount))
                .ForMember(d => d.WasTdrDeployed, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.TdrDeployed))
                .ForMember(d => d.IsTargetingTuna, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.IsTargetingTuna))
                .ForMember(d => d.IsTargetingSwordfish, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.IsTargetingSwordfish))
                .ForMember(d => d.IsTargetingShark, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.IsTargetingSharks))
                .ForMember(d => d.TotalObservedBaskets, o => o.MapFrom(s => s.TotalBasketsObserved))
                .ForMember(d => d.HasGen3Event, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.Gen3Events))
                .ForMember(d => d.ShipsTime, o => o.MapFrom(s => s.LocalTime))
                .ForMember(d => d.ShipsDate, o => o.MapFrom(s => s.SetDateOnly))
                .ForMember(d => d.UtcDate, o => o.MapFrom(s => s.UtcSetDateOnly))
                .ForMember(d => d.UtcTime, o => o.MapFrom(s => s.UtcSetTimeOnly))
                .ForMember(d => d.UnusualDetails, o => o.MapFrom(s => s.Details))
                .ForMember(d => d.StartEndPositionsObserved, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.AllPositionsDirectlyObserved))
                .ForMember(d => d.StartOfSet, o => o.MapFrom(s => s.EventList.Where(e => e.ActivityType == HaulActivityType.StartOfSet).FirstOrDefault()))
                .ForMember(d => d.EndOfSet, o => o.MapFrom(s => s.EventList.Where(e => e.ActivityType == HaulActivityType.EndOfSet).FirstOrDefault()))
                .ForMember(d => d.StartOfHaul, o => o.MapFrom(s => s.EventList.Where(e => e.ActivityType == HaulActivityType.StartOfHaul).FirstOrDefault()))
                .ForMember(d => d.IntermediateHaulPositions, o => o.MapFrom(s => s.EventList.Where(e => e.ActivityType == null)))
                .ForMember(d => d.EndOfHaul, o => o.MapFrom(s => s.EventList.Where(e => e.ActivityType == HaulActivityType.EndOfHaul).FirstOrDefault()))
                .ForMember(d => d.Comments, o => o.MapFrom(s => s.NotesList))
                .AfterMap((s, d) => 
                {
                    if (null != s)
                    {
                        d.MaxSets = ((Spc.Ofp.Tubs.DAL.Entities.LongLineTrip)s.Trip).FishingSets.Count;
                    }
                })
                ;
        }
    }
}
