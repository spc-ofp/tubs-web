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
    using System.Linq;
    using AutoMapper;
    using DAL = Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Gen2Profile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            CreateMap<Gen2ViewModel, DAL.Entities.SpecialSpeciesInteraction>()
                .ForMember(d => d.SgTime, o => o.Ignore()) // AfterMap
                .ForMember(d => d.InteractionId, o => o.Ignore()) // AfterMap
                .ForMember(d => d.InteractionOther, o => o.Ignore()) // AfterMap
                .ForMember(d => d.InteractionActivity, o => o.Ignore()) // AfterMap
                .ForMember(d => d.InteractionIfOther, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Details, o => o.Ignore()) // AfterMap
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.Trip, o => o.Ignore())
                .ForMember(d => d.EezId, o => o.Ignore()) // Not in UI
                .ForMember(d => d.SightingDistanceInNm, o => o.Ignore()) // AfterMap if at all
                .ForMember(d => d.LandedTimeOnly, o => o.MapFrom(s => s.ShipsTime))
                .ForMember(d => d.LandedDateOnly, o => o.MapFrom(s => s.ShipsDate))
                //.ForMember(d => d.LandedDate, o => o.MapFrom(s => s.LandedDateOnly.Merge(s.LandedTimeOnly)))
                .ForMember(d => d.LandedDate, o => o.Ignore())
                .ForMember(d => d.SightingCount, o => o.MapFrom(s => s.NumberSighted))
                .ForMember(d => d.SightingAdultCount, o => o.MapFrom(s => s.NumberOfAdults))
                .ForMember(d => d.SightingJuvenileCount, o => o.MapFrom(s => s.NumberOfJuveniles))
                .ForMember(d => d.SgType, o => o.ResolveUsing<InteractionTypeResolver>().FromMember(s => s.InteractionType))
                .AfterMap((s,d) => 
                {
                    switch (d.SgType)
                    {
                        case "I":
                            // String to InteractionActivity? (<-- nullable, not a question)
                            d.InteractionOther = s.VesselActivityDescription;
                            break;
                        case "S":
                            // String to InteractionActivity? (<-- nullable, not a question)
                            d.InteractionIfOther = s.VesselActivityDescription;
                            break;
                        default:
                            break;
                    }

                    // TODO Is this going to work?
                    // It might be better to have a list of entities that contain both the start and end of the
                    // interaction.  Otherwise we might get them mixed up... (e.g. start for row 1 is paired with end for row 2)
                    if (null != s.StartOfInteraction)
                    {
                        foreach (var item in s.StartOfInteraction)
                        {
                            var detail = Mapper.Map<Gen2ViewModel.SpeciesGroup, DAL.Entities.InteractionDetail>(item);
                            d.Details.Add(detail);
                        }
                    }

                    if (null != s.EndOfInteraction)
                    {
                        foreach (var item in s.EndOfInteraction)
                        {
                            var detail = Mapper.Map<Gen2ViewModel.SpeciesGroup, DAL.Entities.InteractionDetail>(item);
                            d.Details.Add(detail);
                        }
                    }
                })
                ;

            CreateMap<Gen2ViewModel.SpeciesGroup, DAL.Entities.InteractionDetail>()
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

            CreateMap<DAL.Entities.SpecialSpeciesInteraction, Gen2ViewModel>()
                .ForMember(d => d.VesselActivity, o => o.Ignore()) // AfterMap
                .ForMember(d => d.VesselActivityDescription, o => o.Ignore()) // AfterMap
                .ForMember(d => d.SexCodes, o => o.Ignore()) // UI details
                .ForMember(d => d.DistanceUnits, o => o.Ignore()) // UI details
                .ForMember(d => d.ConditionCodes, o => o.Ignore()) // UI details
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.Trip.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.ShipsDate, o => o.MapFrom(s => s.LandedDateOnly))
                .ForMember(d => d.ShipsTime, o => o.MapFrom(s => s.LandedTimeOnly))
                .ForMember(d => d.NumberSighted, o => o.MapFrom(s => s.SightingCount))
                .ForMember(d => d.NumberOfAdults, o => o.MapFrom(s => s.SightingAdultCount))
                .ForMember(d => d.NumberOfJuveniles, o => o.MapFrom(s => s.SightingJuvenileCount))
                .ForMember(d => d.StartOfInteraction, o => o.MapFrom(s => s.Details.Where(d => d.StartOrEnd == "START")))
                .ForMember(d => d.EndOfInteraction, o => o.MapFrom(s => s.Details.Where(d => d.StartOrEnd == "END")))
                .ForMember(d => d.InteractionType, o => o.ResolveUsing<InteractionCodeResolver>().FromMember(s => s.SgType))
                .AfterMap((s,d) => 
                {
                    switch (s.SgType)
                    {
                        case "I":
                            break;
                        case "S":
                            break;
                        default:
                            break;
                    }
                })
                ;

            // TODO This needs more work
            CreateMap<DAL.Entities.InteractionDetail, Gen2ViewModel.SpeciesGroup>()
                .ForMember(d => d.Count, o => o.MapFrom(s => s.Number))
                ;
        }
    }
}
