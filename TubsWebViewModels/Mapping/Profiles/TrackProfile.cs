// -----------------------------------------------------------------------
// <copyright file="TrackProfile.cs" company="Secretariat of the Pacific Community">
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
    using TubsWeb.ViewModels;
    using DAL = Spc.Ofp.Tubs.DAL;

    /// <summary>
    /// AutoMapper profile for creating a trip Track ViewModel from a trip.
    /// </summary>
    public sealed class TrackProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();
            // No need for ViewModel to Entity

            // Entity to ViewModel
            CreateMap<DAL.Entities.Trip, TrackViewModel>()
                // I think this is a bug in AutoMapper.  BoundingBox is a read-only property
                // and shouldn't wig out on not being set...
                .ForMember(d => d.BoundingBox, o => o.Ignore())
                // AfterMap
                .ForMember(d => d.Properties, o => o.Ignore())
                // LINQ magic
                .ForMember(d => d.Positions, o => o.MapFrom(s => from p in s.Pushpins
                                                                 orderby p.Timestamp
                                                                 where null != p && p.Latitude.HasValue && p.Longitude.HasValue && p.Timestamp.HasValue
                                                                 select new[] { p.Longitude.Value, p.Latitude.Value })
                )
                .AfterMap((s, d) =>
                {
                    d.Properties.Add(new KeyValuePair<string, object>("obstrip_id", s.Id));
                    d.Properties.Add(new KeyValuePair<string, object>("tripNumber", s.SpcTripNumber ?? "Unknown"));
                    if (!String.IsNullOrEmpty(s.AlternateTripNumber))
                        d.Properties.Add(new KeyValuePair<string, object>("alternateTripNumber", s.AlternateTripNumber));
                    // TODO: Anything else we might want to add.
                })
                ;
        }
    }
}
