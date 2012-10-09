// -----------------------------------------------------------------------
// <copyright file="SetProfile.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL.Common; // For DateTime 'Merge'
    using TubsWeb.ViewModels;
    
    public class SetProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            // ViewModel to Entity
            CreateMap<PurseSeineSetViewModel.SetCatch, DAL.Entities.PurseSeineSetCatch>()
                // Standard ignores
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.FishingSet, o => o.Ignore()) // Handled by parent in AfterMap
                .ForMember(d => d.SpeciesWeightEstimate, o => o.Ignore())
                .ForMember(d => d.SpeciesWeightHigh, o => o.Ignore())
                .ForMember(d => d.SpeciesWeightLow, o => o.Ignore())
                .ForMember(d => d.EstimatedSpeciesCatch, o => o.Ignore())
                .ForMember(d => d.EstimatedSpeciesCount, o => o.Ignore())
                .ForMember(d => d.CatchWeightMethodId, o => o.Ignore())
                .ForMember(d => d.AverageLength, o => o.Ignore())
                .ForMember(d => d.AverageWeightMethodId, o => o.Ignore())
                .ForMember(d => d.CalculatedSpeciesCatch, o => o.Ignore())
                .ForMember(d => d.ContainsLargeFish, o => o.Ignore())                
                .ForMember(d => d.ConditionCode, o => o.Ignore())
                .ForMember(d => d.MetricTonsFromLog, o => o.MapFrom(s => s.LogbookWeight))
                .ForMember(d => d.MetricTonsObserved, o => o.MapFrom(s => s.ObservedWeight))
                .ForMember(d => d.CountObserved, o => o.MapFrom(s => s.ObservedCount))
                .ForMember(d => d.CountFromLog, o => o.MapFrom(s => s.LogbookCount))                
                ;

            CreateMap<PurseSeineSetViewModel, DAL.Entities.PurseSeineSet>()
                .BeforeMap((s, d) =>
                {
                    if (null == s.AllCatch || s.AllCatch.Count == 0)
                    {
                        if (null != s.TargetCatch) { s.AllCatch.AddRange(s.TargetCatch); }
                        if (null != s.ByCatch) { s.AllCatch.AddRange(s.ByCatch); }
                    }
                })
                // Standard ignores
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.Activity, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.SamplingHeaders, o => o.Ignore()) // Not part of this ViewModel                
                .ForMember(d => d.VesselTonnageOnlyFromThisSet, o => o.Ignore()) // Not supported in webapp
                .ForMember(d => d.ContainsLargeTuna, o => o.Ignore()) // Legacy column
                .ForMember(d => d.LargeTunaPercentage, o => o.Ignore()) // Legacy column
                .ForMember(d => d.TonsOfYellowfinAndBigeyeObserved, o => o.Ignore()) // Legacy column
                .ForMember(d => d.TotalTunaAnswer, o => o.Ignore()) // Legacy column
                .ForMember(d => d.PercentageOfTuna, o => o.Ignore()) // Legacy column
                .ForMember(d => d.TonsOfTunaObserved2, o => o.Ignore()) // Legacy column
                .ForMember(d => d.StartOfSetFromLog, o => o.Ignore()) // AfterMap
                .ForMember(d => d.WinchOn, o => o.Ignore()) // AfterMap
                .ForMember(d => d.RingsUp, o => o.Ignore()) // AfterMap
                .ForMember(d => d.BeginBrailing, o => o.Ignore()) // AfterMap
                .ForMember(d => d.EndBrailing, o => o.Ignore()) // AfterMap
                .ForMember(d => d.EndOfSet, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Id, o => o.MapFrom(s => s.SetId))
                .ForMember(d => d.CatchList, o => o.MapFrom(s => s.AllCatch.Where(x => x != null && !x._destroy))) // Don't copy destroyed entities
                .AfterMap((s, d) =>
                {
                    // s.SkiffOff has the DateTime (if it's set)
                    if (s.SkiffOff.HasValue)
                    {
                        var dt = s.SkiffOff.Value.Date;
                        d.SkiffOff = dt.Merge(s.SkiffOffTimeOnly);
                        d.WinchOn = dt.Merge(s.WinchOnTimeOnly);
                        d.RingsUp = dt.Merge(s.RingsUpTimeOnly);
                        d.BeginBrailing = dt.Merge(s.BeginBrailingTimeOnly);
                        d.EndBrailing = dt.Merge(s.EndBrailingTimeOnly);
                        d.EndOfSet = dt.Merge(s.EndOfSetTimeOnly);
                    }
                    // Rebuild the reference to the parent entity for NHibernate
                    if (null != d.CatchList && d.CatchList.Count > 0)
                    {
                        d.CatchList.ToList().ForEach(cl => cl.FishingSet = d);
                    }
                })
                ;

            // Entity to ViewModel
            CreateMap<DAL.Entities.PurseSeineSetCatch, PurseSeineSetViewModel.SetCatch>()
                .ForMember(d => d._destroy, o => o.Ignore()) // Not from database
                .ForMember(d => d.NeedsFocus, o => o.Ignore()) // Not from database
                .ForMember(d => d.ObservedWeight, o => o.MapFrom(s => s.MetricTonsObserved))
                .ForMember(d => d.ObservedCount, o => o.MapFrom(s => s.CountObserved))
                .ForMember(d => d.LogbookWeight, o => o.MapFrom(s => s.MetricTonsFromLog))
                .ForMember(d => d.LogbookCount, o => o.MapFrom(s => s.CountFromLog))
                ;
            
            CreateMap<DAL.Entities.PurseSeineSet, PurseSeineSetViewModel>()
                .ForMember(d => d.CrossesDayBoundary, o => o.Ignore()) // Not from database
                .ForMember(d => d.TargetSpecies, o => o.Ignore()) // Not from database
                .ForMember(d => d.FateCodes, o => o.Ignore()) // Not from database
                .ForMember(d => d.HasNext, o => o.Ignore()) // Caller's responsibility, post AutoMapper
                .ForMember(d => d.HasPrevious, o => o.Ignore()) // Caller's responsibility, post AutoMapper
                .ForMember(d => d.MaxSets, o => o.Ignore()) // Caller's responsibility, post AutoMapper
                .ForMember(d => d.LogbookDate, o => o.Ignore()) // AfterMap
                .ForMember(d => d.LogbookTime, o => o.Ignore()) // AfterMap
                .ForMember(d => d.SizeOfBrail1, o => o.Ignore()) // AfterMap
                .ForMember(d => d.SizeOfBrail2, o => o.Ignore()) // AfterMap
                .ForMember(d => d.SetId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ActivityId, o => o.MapFrom(s => s.Activity.Id))
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Activity.Day.Trip.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.Activity.Day.Trip.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Activity.Day.Trip.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.LogbookDate, o => o.MapFrom(s => s.StartOfSetFromLog)) // TODO Split this in AfterMap
                .ForMember(d => d.NextSet, o => o.MapFrom(s => (s.SetNumber ?? 0) + 1))
                .ForMember(d => d.PreviousSet, o => o.MapFrom(s => (s.SetNumber ?? 0) - 1))
                // LINQ is soooo awesome!
                .ForMember(d => d.AllCatch, o => o.MapFrom(s => s.CatchList))
                .ForMember(d => d.ByCatch, o => o.MapFrom(s => s.CatchList.Where(cl => 
                        cl != null &&
                        cl.SpeciesCode != "BET" &&
                        cl.SpeciesCode != "YFT" &&
                        cl.SpeciesCode != "SKJ"
                    )))
                .ForMember(d => d.TargetCatch, o => o.MapFrom(s => s.CatchList.Where(cl =>
                        cl != null && (
                        cl.SpeciesCode == "BET" ||
                        cl.SpeciesCode == "YFT" ||
                        cl.SpeciesCode == "SKJ")
                    )))
                .AfterMap((s, d) =>
                {
                    var trip = s.Activity.Day.Trip as DAL.Entities.PurseSeineTrip;
                    if (null != trip && null != trip.Gear)
                    {
                        d.SizeOfBrail1 = trip.Gear.Brail1Size;
                        d.SizeOfBrail2 = trip.Gear.Brail2Size;
                    }
                    
                    d.LogbookDate = s.StartOfSetFromLog.HasValue ? s.StartOfSetFromLog.Value.Date : (DateTime?)null;
                    d.LogbookTime = s.StartOfSetFromLog.HasValue ? s.StartOfSetFromLog.Value.ToString("HHmm") : null;
                });
        }
    }
}