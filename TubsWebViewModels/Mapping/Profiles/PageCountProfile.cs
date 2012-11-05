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
                .Include<DAL.Entities.PurseSeineTrip, PurseSeinePageCountViewModel>()
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => s.SpcTripNumber))
                .ForMember(d => d.Gen1Count, o => o.MapFrom(s => s.PageCounts.Where(x => x.FormName == FormNames.GEN1).Sum(x => x.FormCount)))
                .ForMember(d => d.Gen2Count, o => o.MapFrom(s => s.PageCounts.Where(x => x.FormName == FormNames.GEN2).Sum(x => x.FormCount)))
                .ForMember(d => d.Gen3Count, o => o.MapFrom(s => s.PageCounts.Where(x => x.FormName == FormNames.GEN3).Sum(x => x.FormCount)))
                .ForMember(d => d.Gen6Count, o => o.MapFrom(s => s.PageCounts.Where(x => x.FormName == FormNames.GEN6).Sum(x => x.FormCount)))
                ;

            CreateMap<DAL.Entities.PurseSeineTrip, PurseSeinePageCountViewModel>()
               .ForMember(d => d.Ps1Count, o => o.MapFrom(s => s.PageCounts.Where(x => x.FormName == FormNames.PS1).Sum(x => x.FormCount)))
               .ForMember(d => d.Ps2Count, o => o.MapFrom(s => s.PageCounts.Where(x => x.FormName == FormNames.PS2).Sum(x => x.FormCount)))
               .ForMember(d => d.Ps3Count, o => o.MapFrom(s => s.PageCounts.Where(x => x.FormName == FormNames.PS3).Sum(x => x.FormCount)))
               .ForMember(d => d.Ps4Count, o => o.MapFrom(s => s.PageCounts.Where(x => x.FormName == FormNames.PS4).Sum(x => x.FormCount)))
               .ForMember(d => d.Ps5Count, o => o.MapFrom(s => s.PageCounts.Where(x => x.FormName == FormNames.PS5).Sum(x => x.FormCount)))
               .ForMember(d => d.Gen5Count, o => o.MapFrom(s => s.PageCounts.Where(x => x.FormName == FormNames.GEN5).Sum(x => x.FormCount)))
               ;

        }
    }
}
