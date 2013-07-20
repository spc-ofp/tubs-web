﻿// -----------------------------------------------------------------------
// <copyright file="TripSamplingProfile.cs" company="Secretariat of the Pacific Community">
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
    /// 
    /// </summary>
    public sealed class TripSamplingProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();
            // ViewModel to Entity

            // Entity to ViewModel
            // If this changes to PurseSeineSet, then we're guaranteed an entry for every set 
            CreateMap<DAL.Entities.LengthSamplingHeader, TripSamplingViewModel.SampleSummary>()
                .ForMember(d => d.SetNumber, o => o.MapFrom(s => s.Set.SetNumber))
                .ForMember(d => d.SetDate, o => o.MapFrom(s => s.Set.SkiffOff))
                .ForMember(d => d.SampleType, o => o.MapFrom(s => s.SamplingProtocol))
                .ForMember(d => d.SampleCount, o => o.MapFrom(s => null == s.Samples ? 0 : s.Samples.Count))
                ;

            CreateMap<DAL.Entities.PurseSeineSet, TripSamplingViewModel.SampleSummary>()
                .ForMember(d => d.SetNumber, o => o.MapFrom(s => s.SetNumber))
                .ForMember(d => d.SetDate, o => o.MapFrom(s => s.SkiffOff))
                .ForMember(d => d.SampleCount, o => o.Ignore()) // Ignore while we fix another problem
                .ForMember(d => d.PageNumber, o => o.Ignore())
                .ForMember(d => d.SampleType, o => o.Ignore())
                // First problem:  Sample Type can now have 3 values:
                // null, where there is no associated SamplingHeader (aka PS-4)
                // A single string value, where there is only one SamplingHeader (uncommon)
                // A list of strings, with one value for each SamplingHeader
                // The third value is complicated even more by the fairly common situation of
                // multiple pages used for "normal" sampling.  For example, if there are 4 PS-4 pages
                // associated with a set, 3 may be for "normal" sampling with the last one for bycatch.
                ;


            CreateMap<DAL.Entities.PurseSeineTrip, TripSamplingViewModel>()
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => s.SpcTripNumber))
                // The trick here was .SelectMany, which merges the IEnumerable<IList<blah>> into a single IEnumerable<blah>
                .ForMember(d => d.Headers, o => o.MapFrom(s => (from day in s.SeaDays
                                                                from act in day.Activities
                                                                where null != act.FishingSet
                                                                select act.FishingSet.SamplingHeaders).SelectMany(x => x)))
                ;
        }
    }
}