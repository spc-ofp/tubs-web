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
    /// AutoMapper profile for creating a Positions ViewModel from a trip.
    /// </summary>
    public sealed class PositionsProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();
            // No need for ViewModel to Entity

            // Entity to ViewModel
            CreateMap<DAL.Entities.Pushpin, PositionsViewModel.ObservedPosition>()
                .ForMember(d => d.Latitude, o => o.MapFrom(s => s.Latitude.Value))
                .ForMember(d => d.Longitude, o => o.MapFrom(s => s.Longitude.Value))
                // AfterMap
                .ForMember(d => d.Properties, o => o.Ignore())
                .AfterMap((s,d) => 
                {
                    if (s.Timestamp.HasValue)
                        d.Properties.Add(new KeyValuePair<string, object>("timestamp", s.Timestamp.Value.ToString("yyyy-MM-ddTHH:mm:ss")));
                    d.Properties.Add(new KeyValuePair<string, object>("form", s.FormName));
                    d.Properties.Add(new KeyValuePair<string, object>("description", s.Description));
                    d.Properties.Add(new KeyValuePair<string, object>("eventKey", s.EventKey));
                })
                ;

            CreateMap<DAL.Entities.Trip, PositionsViewModel>()
                // I think this is a bug in AutoMapper.  BoundingBox is a read-only property
                // and shouldn't wig out on not being set...
                .ForMember(d => d.BoundingBox, o => o.Ignore())
                // AfterMap
                .ForMember(d => d.Properties, o => o.Ignore())
                .ForMember(d => d.Positions, o => o.MapFrom(s => s.Pushpins.Where(p => null != p && p.Longitude.HasValue && p.Latitude.HasValue)))
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
