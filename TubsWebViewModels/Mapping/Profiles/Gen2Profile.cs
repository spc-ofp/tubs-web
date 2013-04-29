// -----------------------------------------------------------------------
// <copyright file="Gen2Profile.cs" company="Secretariat of the Pacific Community">
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
    using DAL = Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;

    class Gen2Profile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            // ViewModel to DAL
            CreateMap<Gen2ViewModel, DAL.Entities.Interaction>()
                .Include<Gen2GearViewModel, DAL.Entities.GearInteraction>()
                .Include<Gen2SightingViewModel, DAL.Entities.SightingInteraction>()
                .Include<Gen2LandedViewModel, DAL.Entities.LandedInteraction>()
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.Trip, o => o.Ignore())
                .ForMember(d => d.EezId, o => o.Ignore()) // Not in UI
                .ForMember(d => d.LandedTimeOnly, o => o.MapFrom(s => s.ShipsTime))
                .ForMember(d => d.LandedDateOnly, o => o.MapFrom(s => s.ShipsDate))
                .ForMember(d => d.LandedDate, o => o.MapFrom(s => s.ShipsDate.Merge(s.ShipsTime)))
                ;

            CreateMap<Gen2LandedViewModel, DAL.Entities.LandedInteraction>()
                ;

            // Details contains the union of "StartOfInteraction" and "EndOfInteraction"
            CreateMap<Gen2GearViewModel, DAL.Entities.GearInteraction>()
                .ForMember(d => d.InteractionId, o => o.MapFrom(s => s.VesselActivity)) // need a resolver from string to InteractionActivity
                .ForMember(d => d.InteractionOther, o => o.MapFrom(s => s.VesselActivityDescription))
                .ForMember(d => d.Details, o => o.Ignore())
                .AfterMap((s,d) => 
                {
                    if (null != s.StartOfInteraction)
                    {
                        foreach (var item in s.StartOfInteraction)
                        {
                            // Don't bother to convert entities being destroyed
                            if (null != item && !item._destroy)
                            { 
                                var detail = Mapper.Map<Gen2GearViewModel.SpeciesGroup, DAL.Entities.GearInteractionDetail>(item);
                                detail.StartOrEnd = "START";
                                d.AddDetail(detail);
                            }
                        }
                    }

                    if (null != s.EndOfInteraction)
                    {
                        foreach (var item in s.EndOfInteraction)
                        {
                            // Don't bother to convert entities being destroyed
                            if (null != item && !item._destroy)
                            {
                                var detail = Mapper.Map<Gen2GearViewModel.SpeciesGroup, DAL.Entities.GearInteractionDetail>(item);
                                detail.StartOrEnd = "END";
                                d.AddDetail(detail);
                            }
                        }
                    }
                })
                ;

            CreateMap<Gen2SightingViewModel, DAL.Entities.SightingInteraction>()
                .ForMember(d => d.SightingDistanceInNm, o => o.Ignore()) // AfterMap if at all
                .ForMember(d => d.SightingCount, o => o.MapFrom(s => s.NumberSighted))
                .ForMember(d => d.SightingAdultCount, o => o.MapFrom(s => s.NumberOfAdults))
                .ForMember(d => d.SightingJuvenileCount, o => o.MapFrom(s => s.NumberOfJuveniles))
                .ForMember(d => d.InteractionActivity, o => o.MapFrom(s => s.VesselActivity)) // need a resolver from string to InteractionActivity
                .ForMember(d => d.InteractionIfOther, o => o.MapFrom(s => s.VesselActivityDescription))
                ;

            CreateMap<Gen2GearViewModel.SpeciesGroup, DAL.Entities.GearInteractionDetail>()
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.Header, o => o.Ignore())
                .ForMember(d => d.Number, o => o.MapFrom(s => s.Count))
                .ForMember(d => d.StartOrEnd, o => o.Ignore()) // Managed in ViewModel via two separate lists
                ;

            // DAL to ViewModel
            CreateMap<DAL.Entities.Interaction, Gen2ViewModel>()
                .Include<DAL.Entities.GearInteraction, Gen2GearViewModel>()
                .Include<DAL.Entities.SightingInteraction, Gen2SightingViewModel>()
                .Include<DAL.Entities.LandedInteraction, Gen2LandedViewModel>()
                .ForMember(d => d.InteractionType, o => o.Ignore()) // MVC model binder plumbing       
                .ForMember(d => d.ConditionCodes, o => o.Ignore()) // UI details
                .ForMember(d => d.Activities, o => o.Ignore()) // UI details
                .ForMember(d => d.HasNext, o => o.Ignore()) // UI details
                .ForMember(d => d.HasPrevious, o => o.Ignore()) // UI details
                .ForMember(d => d.PreviousPage, o => o.Ignore()) // UI details
                .ForMember(d => d.NextPage, o => o.Ignore()) // UI details
                .ForMember(d => d.ActionName, o => o.Ignore()) // UI details
                .ForMember(d => d.PageNumber, o => o.Ignore()) // UI details
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.Trip.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.ShipsDate, o => o.MapFrom(s => s.LandedDateOnly))
                .ForMember(d => d.ShipsTime, o => o.MapFrom(s => s.LandedTimeOnly))
            ;

            CreateMap<DAL.Entities.GearInteraction, Gen2GearViewModel>()
                .ForMember(d => d.VesselActivity, o => o.MapFrom(s => s.InteractionId))
                .ForMember(d => d.VesselActivityDescription, o => o.MapFrom(s => s.InteractionDescription))
                .ForMember(d => d.StartOfInteraction, o => o.MapFrom(s => s.Details.Where(d => d.StartOrEnd == "START")))
                .ForMember(d => d.EndOfInteraction, o => o.MapFrom(s => s.Details.Where(d => d.StartOrEnd == "END")))
                ;

            CreateMap<DAL.Entities.SightingInteraction, Gen2SightingViewModel>()
                .ForMember(d => d.NumberSighted, o => o.MapFrom(s => s.SightingCount))
                .ForMember(d => d.NumberOfAdults, o => o.MapFrom(s => s.SightingAdultCount))
                .ForMember(d => d.NumberOfJuveniles, o => o.MapFrom(s => s.SightingJuvenileCount))
                .ForMember(d => d.DistanceUnits, o => o.Ignore()) // UI details
                .ForMember(d => d.VesselActivity, o => o.MapFrom(s => s.InteractionActivity))
                .ForMember(d => d.VesselActivityDescription, o => o.MapFrom(s => s.InteractionIfOther))
                ;

            CreateMap<DAL.Entities.LandedInteraction, Gen2LandedViewModel>()
                .ForMember(d => d.SexCodes, o => o.Ignore()) // UI details
                .ForMember(d => d.LengthCodes, o => o.Ignore()) // UI details
                ;

            CreateMap<DAL.Entities.GearInteractionDetail, Gen2GearViewModel.SpeciesGroup>()
                .ForMember(d => d.Count, o => o.MapFrom(s => s.Number))
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())
                ;
        }
    }
}
