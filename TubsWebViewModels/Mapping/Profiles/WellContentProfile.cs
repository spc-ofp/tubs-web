// -----------------------------------------------------------------------
// <copyright file="WellContentProfile.cs" company="Secretariat of the Pacific Community">
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
    public class WellContentProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            CreateMap<DAL.Entities.PurseSeineTrip, WellContentViewModel>()
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => s.SpcTripNumber))
                .ForMember(d => d.Locations, o => o.Ignore())
                .ForMember(d=>d.WellContentItems,o=>o.MapFrom(s=>s.WellContent))
                .ForMember(d => d.Contents, o => o.Ignore())
                ;

            CreateMap<DAL.Entities.PurseSeineWellContent, WellContentViewModel.WellContentItem>()
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())
                .ForMember(d => d.Capacity, o => o.MapFrom(s => s.WellCapacity))
                .ForMember(d => d.Comment, o => o.MapFrom(s => s.Comments))
                .ForMember(d => d.Content, o => o.MapFrom(s => s.FuelOrWater))
                .ForMember(d => d.Location, o => o.ResolveUsing<LocationCodeResolver>().FromMember(s => s.WellLocation));


            CreateMap<WellContentViewModel.WellContentItem, DAL.Entities.PurseSeineWellContent>()
                .ForMember(d=>d.Trip,o=>o.Ignore())
                .ForMember(d=>d.Comments,o=>o.MapFrom(s=>s.Comment))
                .ForMember(d=>d.DctNotes,o=>o.Ignore())
                .ForMember(d=>d.DctScore,o=>o.Ignore())
                .ForMember(d=>d.EnteredBy,o=>o.Ignore()) 
                .ForMember(d=>d.EnteredDate,o=>o.Ignore()) 
                .ForMember(d=>d.FuelOrWater,o=>o.MapFrom(s=>s.Content))
                .ForMember(d=>d.UpdatedBy,o=>o.Ignore())
                .ForMember(d=>d.UpdatedDate,o=>o.Ignore())
                .ForMember(d=>d.WellCapacity,o=>o.MapFrom(s=>s.Capacity))
                .ForMember(d => d.WellLocation, o => o.ResolveUsing<LocationTypeResolver>().FromMember(s => s.Location));

        }
    }
}
