// -----------------------------------------------------------------------
// <copyright file="Gen3Profile.cs" company="Secretariat of the Pacific Community">
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
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Gen3Profile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            CreateMap<Gen3Answer, Gen3ViewModel.Incident>()
                ;

            CreateMap<Gen3Detail, Gen3ViewModel.Note>()
                .ForMember(d => d.Date, o => o.MapFrom(s => s.DetailDate))
                ;

            CreateMap<Trip, Gen3ViewModel>()
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => s.TripNumber))
                .ForMember(d => d.Incidents, o => o.MapFrom(s => s.Gen3Answers))
                .ForMember(d => d.Notes, o => o.MapFrom(s => s.Gen3Details))
                .ForMember(d => d.BooleanValues, o => o.Ignore())
                ;

        }
    }
}
