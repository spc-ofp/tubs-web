// -----------------------------------------------------------------------
// <copyright file="LongLineSetProfile.cs" company="Secretariat of the Pacific Community">
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
    using DAL = Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common; // For DateTime 'Merge'
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    
    /// <summary>
    /// AutoMapper profile for the conversion of LongLineSet entity to/from
    /// MVC ViewModel.
    /// </summary>
    public class LongLineSetProfile : Profile
    {
        public const decimal KTS_TO_METERS_PER_SECOND = 0.5144M;

        // Ideally, I could re-use YesNoResolver, but it wants context, and results, and blah blah blah
        internal bool? ResolveYesNo(string source)
        {
            return
                "YES".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? true :
                "NO".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? false :
                 (bool?)null;

        }

        internal string ResolveBoolean(bool? source)
        {
            if (!source.HasValue)
                return null;

            return source.Value ? "YES" : "NO";
        }
        
        internal LongLineSetViewModel.Bait Build(string species, int? weight, string hooks, bool? dyed)
        {

            return new LongLineSetViewModel.Bait()
            {
                Species = species,
                Weight = weight,
                Hooks = hooks,
                DyedBlue = ResolveBoolean(dyed)
            };
        }

        /// <summary>
        /// A mismatch between the database and the UI means that there are 4 positions that are guaranteed to
        /// be on the input form that might not have any data on the backend.
        /// This method will exclude these positions if they have no data.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal bool IncludeInMap(LongLineSetViewModel.Position position)
        {
            if (null == position || position._destroy)
                return false;

            return !position.IsEmpty;
        }
        
        protected override void Configure()
        {
            base.Configure();
            
            // ViewModel to Entity
            CreateMap<LongLineSetViewModel.Position, DAL.Entities.LongLineSetHaulEvent>()
                // Ignore entity relationships
                .ForMember(d => d.FishingSet, o => o.Ignore())
                // Standard ignores
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                // Legacy data
                .ForMember(d => d.WindDirection, o => o.Ignore())
                .ForMember(d => d.WindSpeed, o => o.Ignore())
                .ForMember(d => d.SeaCode, o => o.Ignore())
                .ForMember(d => d.CloudCover, o => o.Ignore())
                .ForMember(d => d.Comments, o => o.Ignore())
                // Custom properties
                .ForMember(d => d.LogTimeOnly, o => o.MapFrom(s => s.LocalTime)) 
                .ForMember(d => d.LogDateOnly, o => o.MapFrom(s => s.DateOnly))
                .ForMember(d => d.LogDate, o => o.MapFrom(s => s.DateOnly.Merge(s.LocalTime)))
                // Parent AfterMap
                .ForMember(d => d.Sethaul, o => o.Ignore())
                .ForMember(d => d.ActivityType, o => o.Ignore())
                ;

            CreateMap<LongLineSetViewModel.Comment, DAL.Entities.LongLineSetHaulNote>()
                // Ignore entity relationships
                .ForMember(d => d.FishingSet, o => o.Ignore())
                // Standard ignores
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                // Custom properties
                .ForMember(d => d.LogTimeOnly, o => o.MapFrom(s => s.LocalTime))
                .ForMember(d => d.LogDateOnly, o => o.MapFrom(s => s.DateOnly))
                .ForMember(d => d.LogDate, o => o.MapFrom(s => s.DateOnly.Merge(s.LocalTime)))
                .ForMember(d => d.Comments, o => o.MapFrom(s => s.Details))
                ;

            CreateMap<LongLineSetViewModel, DAL.Entities.LongLineSet>()
                // Ignore entity relationships
                .ForMember(d => d.Trip, o => o.Ignore())
                .ForMember(d => d.CatchList, o => o.Ignore())
                .ForMember(d => d.ConversionFactors, o => o.Ignore())
                .ForMember(d => d.Baskets, o => o.Ignore())
                // Standard ignores
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                // Ignore legacy data
                .ForMember(d => d.TargetSpeciesCode, o => o.Ignore())
                .ForMember(d => d.HookDepthLow, o => o.Ignore())
                .ForMember(d => d.HookDepthHigh, o => o.Ignore())
                .ForMember(d => d.HookCalc, o => o.Ignore())                
                .ForMember(d => d.LocalTime, o => o.Ignore())
                .ForMember(d => d.TdrLength, o => o.Ignore())
                .ForMember(d => d.MeasuringInstrument, o => o.Ignore()) // Is this v2009?
                .ForMember(d => d.BranchlineCount_00to20m, o => o.Ignore())
                .ForMember(d => d.BranchlineCount_20to34m, o => o.Ignore())
                .ForMember(d => d.BranchlineCount_35to50m, o => o.Ignore())
                .ForMember(d => d.BranchlineCount_50to99m, o => o.Ignore())
                .ForMember(d => d.HasRecordedData, o => o.Ignore())
                .ForMember(d => d.WasObserved, o => o.Ignore())               
                .ForMember(d => d.FloatlineHookCount, o => o.Ignore())
                .ForMember(d => d.Strategy, o => o.Ignore())
                .ForMember(d => d.SetId, o => o.Ignore()) // Matches ViewModel property, but not the same thing
                // Not sure
                .ForMember(d => d.LineSettingSpeedMetersPerSecond, o => o.Ignore()) 
                // Custom properties
                // Bait 1
                .ForMember(d => d.BaitSpecies1Code, o => o.Ignore())
                .ForMember(d => d.BaitSpecies1Weight, o => o.Ignore())
                .ForMember(d => d.BaitSpecies1Hooks, o => o.Ignore())
                .ForMember(d => d.BaitSpecies1Dyed, o => o.Ignore())
                // Bait 2
                .ForMember(d => d.BaitSpecies2Code, o => o.Ignore())
                .ForMember(d => d.BaitSpecies2Weight, o => o.Ignore())
                .ForMember(d => d.BaitSpecies2Hooks, o => o.Ignore())
                .ForMember(d => d.BaitSpecies2Dyed, o => o.Ignore())
                // Bait 3
                .ForMember(d => d.BaitSpecies3Code, o => o.Ignore())
                .ForMember(d => d.BaitSpecies3Weight, o => o.Ignore())
                .ForMember(d => d.BaitSpecies3Hooks, o => o.Ignore())
                .ForMember(d => d.BaitSpecies3Dyed, o => o.Ignore())
                // Bait 4
                .ForMember(d => d.BaitSpecies4Code, o => o.Ignore())
                .ForMember(d => d.BaitSpecies4Weight, o => o.Ignore())
                .ForMember(d => d.BaitSpecies4Hooks, o => o.Ignore())
                .ForMember(d => d.BaitSpecies4Dyed, o => o.Ignore())
                // Bait 5
                .ForMember(d => d.BaitSpecies5Code, o => o.Ignore())
                .ForMember(d => d.BaitSpecies5Weight, o => o.Ignore())
                .ForMember(d => d.BaitSpecies5Hooks, o => o.Ignore())
                .ForMember(d => d.BaitSpecies5Dyed, o => o.Ignore())
                
                .ForMember(d => d.Id, o => o.MapFrom(s => s.SetId))
                .ForMember(d => d.TotalBasketCount, o => o.MapFrom(s => s.TotalBaskets))
                .ForMember(d => d.TotalHookCount, o => o.MapFrom(s => s.TotalHooks))
                .ForMember(d => d.EstimatedHookCount, o => o.MapFrom(s => s.TotalHooks)) // Copy into another column
                .ForMember(d => d.TotalHooksObserved, o => o.MapFrom(s => s.TotalHooks)) // Copy into another column
                // Same member name, but needs a resolver
                .ForMember(d => d.LineSettingSpeedUnit, o => o.ResolveUsing<VelocityCodeResolver>().FromMember(s => s.LineSettingSpeedUnit))
                .ForMember(d => d.TdrDeployed, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.WasTdrDeployed))
                .ForMember(d => d.IsTargetingSharks, o => o.MapFrom(s => s.IsTargetingShark))
                .ForMember(d => d.TotalBasketsObserved, o => o.MapFrom(s => s.TotalObservedBaskets))
                .ForMember(d => d.Gen3Events, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasGen3Event))
                .ForMember(d => d.AllPositionsDirectlyObserved, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.StartEndPositionsObserved))
                // v2009 mitigation
                .ForMember(d => d.HasToriPoles, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasToriPoles))
                .ForMember(d => d.HasBirdCurtain, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasBirdCurtain))
                .ForMember(d => d.HasWeightedLines, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasWeightedLines))
                .ForMember(d => d.HasUnderwaterChute, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasUnderwaterChute))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.UnusualDetails))
                .ForMember(d => d.SetDateOnly, o => o.MapFrom(s => s.ShipsDate))
                .ForMember(d => d.SetTimeOnly, o => o.MapFrom(s => s.ShipsTime))
                .ForMember(d => d.SetDate, o => o.MapFrom(s => s.ShipsDate.Merge(s.ShipsTime)))
                .ForMember(d => d.UtcSetDateOnly, o => o.MapFrom(s => s.UtcDate))
                .ForMember(d => d.UtcSetTimeOnly, o => o.MapFrom(s => s.UtcTime))
                .ForMember(d => d.UtcSetDate, o => o.MapFrom(s => s.UtcDate.Merge(s.UtcTime)))
                .ForMember(d => d.NotesList, o => o.MapFrom(s => s.Comments))
                // AfterMap
                .ForMember(d => d.EventList, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    // Attach Set to notes and backfill date properties
                    if (null != d.NotesList)
                    {
                        foreach (var note in d.NotesList)
                        {
                            if (null == note) { continue; }
                            note.FishingSet = d;
                        }
                    }                    
                    
                    // Still unsure whether the ViewModel should call out the special positions
                    // or this complexity should be driven into the UI.
                    // Until that is worked out, this should serve
                    // NOTE: Positions are only mapped from viewmodel to model if they're not marked
                    // for destruction.
                    if (IncludeInMap(s.StartOfSet))
                    {
                        var evt = Mapper.Map<DAL.Entities.LongLineSetHaulEvent>(s.StartOfSet);
                        evt.ActivityType = HaulActivityType.StartOfSet;
                        evt.Sethaul = "S";
                        d.AddEvent(evt);
                    }

                    if (IncludeInMap(s.EndOfSet))
                    {
                        var evt = Mapper.Map<DAL.Entities.LongLineSetHaulEvent>(s.EndOfSet);
                        evt.ActivityType = HaulActivityType.EndOfSet;
                        evt.Sethaul = "S";
                        d.AddEvent(evt);
                    }

                    if (IncludeInMap(s.StartOfHaul))
                    {
                        var evt = Mapper.Map<DAL.Entities.LongLineSetHaulEvent>(s.StartOfHaul);
                        evt.ActivityType = HaulActivityType.StartOfHaul;
                        evt.Sethaul = "H";
                        d.AddEvent(evt);
                    }

                    foreach (var ihp in s.IntermediateHaulPositions)
                    {
                        if (!IncludeInMap(ihp)) { continue; }
                        var evt = Mapper.Map<DAL.Entities.LongLineSetHaulEvent>(ihp);
                        evt.Sethaul = "H";
                        d.AddEvent(evt);
                    }

                    if (IncludeInMap(s.EndOfHaul))
                    {
                        var evt = Mapper.Map<DAL.Entities.LongLineSetHaulEvent>(s.EndOfHaul);
                        evt.ActivityType = HaulActivityType.EndOfHaul;
                        evt.Sethaul = "H";
                        d.AddEvent(evt);
                    }
                    
                    if (null != s)
                    {
                        if (null != s.Baits[0])
                        {
                            d.BaitSpecies1Code = s.Baits[0].Species;
                            d.BaitSpecies1Weight = s.Baits[0].Weight;
                            d.BaitSpecies1Hooks = s.Baits[0].Hooks;
                            d.BaitSpecies1Dyed = ResolveYesNo(s.Baits[0].DyedBlue);
                        }

                        if (null != s.Baits[1])
                        {
                            d.BaitSpecies2Code = s.Baits[1].Species;
                            d.BaitSpecies2Weight = s.Baits[1].Weight;
                            d.BaitSpecies2Hooks = s.Baits[1].Hooks;
                            d.BaitSpecies2Dyed = ResolveYesNo(s.Baits[1].DyedBlue);
                        }

                        if (null != s.Baits[2])
                        {
                            d.BaitSpecies3Code = s.Baits[2].Species;
                            d.BaitSpecies3Weight = s.Baits[2].Weight;
                            d.BaitSpecies3Hooks = s.Baits[2].Hooks;
                            d.BaitSpecies3Dyed = ResolveYesNo(s.Baits[2].DyedBlue);
                        }

                        if (null != s.Baits[3])
                        {
                            d.BaitSpecies4Code = s.Baits[3].Species;
                            d.BaitSpecies4Weight = s.Baits[3].Weight;
                            d.BaitSpecies4Hooks = s.Baits[3].Hooks;
                            d.BaitSpecies4Dyed = ResolveYesNo(s.Baits[3].DyedBlue);
                        }

                        if (null != s.Baits[4])
                        {
                            d.BaitSpecies5Code = s.Baits[4].Species;
                            d.BaitSpecies5Weight = s.Baits[4].Weight;
                            d.BaitSpecies5Hooks = s.Baits[4].Hooks;
                            d.BaitSpecies5Dyed = ResolveYesNo(s.Baits[4].DyedBlue);
                        }

                        // Convert line setting speed to m/s
                        if (d.LineSettingSpeed.HasValue && d.LineSettingSpeedUnit.HasValue)
                        {
                            if (DAL.Common.UnitOfMeasure.MetersPerSecond == d.LineSettingSpeedUnit.Value)
                            {
                                d.LineSettingSpeedMetersPerSecond = d.LineSettingSpeed;
                            }
                            else
                            {
                                d.LineSettingSpeedMetersPerSecond = d.LineSettingSpeed.Value * KTS_TO_METERS_PER_SECOND;
                            }
                        }

                    }

                })
                ;


            // Entity to ViewModel
            CreateMap<DAL.Entities.LongLineSetHaulEvent, LongLineSetViewModel.Position>()                
                .ForMember(d => d.Latitude, o => o.MapFrom(s => s.Latitude))
                .ForMember(d => d.Longitude, o => o.MapFrom(s => s.Longitude))
                .ForMember(d => d.DateOnly, o => o.MapFrom(s => s.LogDateOnly))
                .ForMember(d => d.LocalTime, o => o.MapFrom(s => s.LogTimeOnly))
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())                
                ;

            CreateMap<DAL.Entities.LongLineSetHaulNote, LongLineSetViewModel.Comment>()
                .ForMember(d => d.DateOnly, o => o.MapFrom(s => s.LogDateOnly))
                .ForMember(d => d.LocalTime, o => o.MapFrom(s => s.LogTimeOnly))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.Comments))
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())
                ;

            CreateMap<DAL.Entities.LongLineSet, LongLineSetViewModel>()
                .ForMember(d => d.HasNext, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.HasPrevious, o => o.Ignore())
                .ForMember(d => d.MaxSets, o => o.Ignore())
                .ForMember(d => d.NextSet, o => o.Ignore())
                .ForMember(d => d.PreviousSet, o => o.Ignore())
                .ForMember(d => d.BooleanValues, o => o.Ignore())
                .ForMember(d => d.VelocityUnits, o => o.Ignore())
                .ForMember(d => d.ActionName, o => o.Ignore())
                .ForMember(d => d.Baits, o => o.Ignore()) // AfterMap
                .ForMember(d => d.SetId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Trip.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.Trip.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Trip.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.TotalBaskets, o => o.MapFrom(s => s.TotalBasketCount))
                .ForMember(d => d.TotalHooks, o => o.MapFrom(s => s.TotalHookCount))
                .ForMember(d => d.WasTdrDeployed, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.TdrDeployed))
                .ForMember(d => d.IsTargetingTuna, o => o.MapFrom(s => s.IsTargetingTuna))
                .ForMember(d => d.IsTargetingSwordfish, o => o.MapFrom(s => s.IsTargetingSwordfish))
                .ForMember(d => d.IsTargetingShark, o => o.MapFrom(s => s.IsTargetingSharks))
                .ForMember(d => d.TotalObservedBaskets, o => o.MapFrom(s => s.TotalBasketsObserved))
                .ForMember(d => d.HasGen3Event, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.Gen3Events))
                // Same member name, but needs a resolver
                .ForMember(d => d.LineSettingSpeedUnit, o => o.ResolveUsing<VelocityTypeResolver>().FromMember(s => s.LineSettingSpeedUnit))
                .ForMember(d => d.ShipsTime, o => o.MapFrom(s => s.SetTimeOnly))
                .ForMember(d => d.ShipsDate, o => o.MapFrom(s => s.SetDateOnly))
                .ForMember(d => d.UtcDate, o => o.MapFrom(s => s.UtcSetDateOnly))
                .ForMember(d => d.UtcTime, o => o.MapFrom(s => s.UtcSetTimeOnly))
                .ForMember(d => d.UnusualDetails, o => o.MapFrom(s => s.Details))
                .ForMember(d => d.StartEndPositionsObserved, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.AllPositionsDirectlyObserved))
                .ForMember(d => d.StartOfSet, o => o.MapFrom(s => s.EventList.Where(e => e.ActivityType == HaulActivityType.StartOfSet).FirstOrDefault()))
                .ForMember(d => d.EndOfSet, o => o.MapFrom(s => s.EventList.Where(e => e.ActivityType == HaulActivityType.EndOfSet).FirstOrDefault()))
                .ForMember(d => d.StartOfHaul, o => o.MapFrom(s => s.EventList.Where(e => e.ActivityType == HaulActivityType.StartOfHaul).FirstOrDefault()))
                .ForMember(d => d.IntermediateHaulPositions, o => o.MapFrom(s => s.EventList.Where(e => e.ActivityType == null)))
                .ForMember(d => d.EndOfHaul, o => o.MapFrom(s => s.EventList.Where(e => e.ActivityType == HaulActivityType.EndOfHaul).FirstOrDefault()))
                .ForMember(d => d.Comments, o => o.MapFrom(s => s.NotesList))
                .AfterMap((s, d) => 
                {
                    if (null != s)
                    {
                        d.Baits.Add(Build(s.BaitSpecies1Code, s.BaitSpecies1Weight, s.BaitSpecies1Hooks, s.BaitSpecies1Dyed));
                        d.Baits.Add(Build(s.BaitSpecies2Code, s.BaitSpecies2Weight, s.BaitSpecies2Hooks, s.BaitSpecies2Dyed));
                        d.Baits.Add(Build(s.BaitSpecies3Code, s.BaitSpecies3Weight, s.BaitSpecies3Hooks, s.BaitSpecies3Dyed));
                        d.Baits.Add(Build(s.BaitSpecies4Code, s.BaitSpecies4Weight, s.BaitSpecies4Hooks, s.BaitSpecies4Dyed));
                        d.Baits.Add(Build(s.BaitSpecies5Code, s.BaitSpecies5Weight, s.BaitSpecies5Hooks, s.BaitSpecies5Dyed));
                        
                        // NUnit was complaining about this, so rather than one-line it, be a little more
                        // verbose with some null checks.
                        //d.MaxSets = ((Spc.Ofp.Tubs.DAL.Entities.LongLineTrip)s.Trip).FishingSets.Count;
                        var trip = s.Trip as Spc.Ofp.Tubs.DAL.Entities.LongLineTrip;
                        if (null != trip)
                        {
                            d.MaxSets = trip.FishingSets.Count;
                        }
                        
                    }
                })
                ;
        }
    }
}
