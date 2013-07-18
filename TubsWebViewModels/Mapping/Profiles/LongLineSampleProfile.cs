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
    using System.Linq;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL.Common; // For DateTime 'Merge'
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    using DAL = Spc.Ofp.Tubs.DAL;

    /// <summary>
    /// AutoMapper profile for the conversion of LL-4 entity to/from
    /// MVC ViewModel. 
    /// </summary>
    public class LongLineSampleProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            // ViewModel to Entity
            CreateMap<LongLineSampleViewModel.LongLineCatchDetail, DAL.Entities.LongLineCatch>()
                // Ignore entity relationships
                .ForMember(d => d.FishingSet, o => o.Ignore())
                // Standard ignores
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                // Not in the UI
                .ForMember(d => d.EstimatedWeight, o => o.Ignore())
                .ForMember(d => d.EstimatedWeightReliability, o => o.Ignore())
                .ForMember(d => d.Spare1, o => o.Ignore())
                .ForMember(d => d.GonadStage, o => o.Ignore())
                // Custom properties
                .ForMember(d => d.LandedConditionCode, o => o.MapFrom(s => s.CaughtCondition))
                .ForMember(d => d.DiscardedConditionCode, o => o.MapFrom(s => s.DiscardedCondition))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.DateOnly.Merge(s.TimeOnly)))
                // Property names match.  C# is able to convert the string to an enum value without issue
                //.ForMember(d => d.SexCode, o => o.ResolveUsing<null>().FromMember(s => s.SexCode))
                //.ForMember(d => d.FateCode, o => o.ResolveUsing<null>().FromMember(s => s.FateCode))
                //.ForMember(d => d.LengthCode, o => o.ResolveUsing<null>().FromMember(s => s.LengthCode))
                ;

            CreateMap<LongLineSampleViewModel, DAL.Entities.LongLineCatchHeader>()
                // Ignore entity relationships
                .ForMember(d => d.FishingSet, o => o.Ignore())
                .ForMember(d => d.Samples, o => o.MapFrom(s => s.Details.Where(d => null != d && !d._destroy)))
                ;

            // Entity to ViewModel
            CreateMap<DAL.Entities.LongLineCatch, LongLineSampleViewModel.LongLineCatchDetail>()
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())
                .ForMember(d => d.CaughtCondition, o => o.MapFrom(s => s.LandedConditionCode))
                .ForMember(d => d.DiscardedCondition, o => o.MapFrom(s => s.DiscardedConditionCode))
                // Property names match.  C# is able to convert the enum to a string without issue
                //.ForMember(d => d.SexCode, o => o.ResolveUsing<null>().FromMember(s => s.SexCode))
                //.ForMember(d => d.FateCode, o => o.ResolveUsing<null>().FromMember(s => s.FateCode))
                //.ForMember(d => d.LengthCode, o => o.ResolveUsing<null>().FromMember(s => s.LengthCode))
                ;

            CreateMap<DAL.Entities.LongLineCatchHeader, LongLineSampleViewModel>()
                // Knockout UI details
                .ForMember(d => d.MeasuringInstruments, o => o.Ignore())
                .ForMember(d => d.ConditionCodes, o => o.Ignore())
                .ForMember(d => d.FateCodes, o => o.Ignore())
                .ForMember(d => d.SexCodes, o => o.Ignore())
                .ForMember(d => d.LengthCodes, o => o.Ignore())
                .ForMember(d => d.WeightCodes, o => o.Ignore())
                .ForMember(d => d.HasNext, o => o.Ignore())
                .ForMember(d => d.HasPrevious, o => o.Ignore())
                .ForMember(d => d.ActionName, o => o.Ignore())
                .ForMember(d => d.SetCount, o => o.Ignore())
                // Custom properties
                .ForMember(d => d.MeasuringInstrument, o => o.ResolveUsing<MeasuringInstrumentTypeResolver>().FromMember(s => s.FishingSet.MeasuringInstrument))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.Samples))
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.FishingSet.Trip.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => s.FishingSet.Trip.SpcTripNumber))
                .ForMember(d => d.SetId, o => o.MapFrom(s => s.FishingSet.Id))
                .ForMember(d => d.SetNumber, o => o.MapFrom(s => s.FishingSet.SetNumber.HasValue ? s.FishingSet.SetNumber.Value : 0))
                .ForMember(d => d.SetDate, o => o.MapFrom(s => s.FishingSet.SetDate))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.FishingSet.Trip.Version == WorkbookVersion.v2009 ? 2009 : 2007))
                // AfterMap
                .ForMember(d => d.HaulDate, o => o.Ignore())
                .AfterMap((s, d) => 
                {
                    // Set a reasonable default
                    d.HaulDate = s.FishingSet.SetDateOnly;
                    if (null != s.FishingSet.EventList)
                    {
                        var startOfHaul = s.FishingSet.EventList.Where(e => null != e && HaulActivityType.StartOfHaul == e.ActivityType).FirstOrDefault();
                        if (null != startOfHaul)
                        {
                            d.HaulDate = startOfHaul.LogDateOnly;
                        }
                    }
                })
                ;
        }
    }
}
