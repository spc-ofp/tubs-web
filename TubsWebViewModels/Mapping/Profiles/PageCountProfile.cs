// -----------------------------------------------------------------------
// <copyright file="PageCountProfile.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL.Common;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PageCountProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            CreateMap<DAL.Entities.Trip, PageCountViewModel>()
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => s.SpcTripNumber))
                .ForMember(d => d.FormKeys, o => o.Ignore())
                ;

            CreateMap<DAL.Entities.PageCount, PageCountViewModel.PageCount>()
                .ForMember(d => d.Key, o => o.MapFrom(s => s.FormName))
                .ForMember(d => d.Value, o => o.MapFrom(s => s.FormCount))
                .ForMember(d => d._destroy, o => o.Ignore())
                ;

            CreateMap<PageCountViewModel.PageCount, DAL.Entities.PageCount>()
                .ForMember(d => d.Trip, o => o.Ignore()) // Caller's problem
                //.ForMember(d => d.RowVersion, o => o.Ignore()) // Not in VM
                .ForMember(d => d.EnteredBy, o => o.Ignore()) // Caller's problem
                .ForMember(d => d.EnteredDate, o => o.Ignore()) // Caller's problem
                .ForMember(d => d.FormName, o => o.ResolveUsing<FormNameResolver>().FromMember(s => s.Key))
                .ForMember(d => d.FormCount, o => o.MapFrom(s => s.Value))
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                ;

        }
    }
}
