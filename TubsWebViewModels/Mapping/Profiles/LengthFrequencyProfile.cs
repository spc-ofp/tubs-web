// -----------------------------------------------------------------------
// <copyright file="LengthFrequencyProfile.cs"  company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using AutoMapper;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    using DAL = Spc.Ofp.Tubs.DAL;

    /// <summary>
    /// AutoMapper profile for the conversion of PS-4 entity to/from
    /// MVC ViewModel.
    /// </summary>
    public class LengthFrequencyProfile : Profile
    {
        // This is a trivial implementation and doesn't call for a full blown
        // AutoMapper resolver
        internal int? WhichBrailResolver(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return
                "Brail 1".Equals(value, StringComparison.InvariantCultureIgnoreCase) ? 1 :
                "Brail 2".Equals(value, StringComparison.InvariantCultureIgnoreCase) ? 2 :
                (int?)null;
        }
        
        protected override void Configure()
        {
            base.Configure();
            // ViewModel to Entity
            CreateMap<LengthFrequencyViewModel.Brail, DAL.Entities.BrailRecord>()
                .ForMember(d => d.Sequence, o => o.MapFrom(s => s.Number))
                ;

            CreateMap<LengthFrequencyViewModel, DAL.Entities.LengthSamplingHeader>()
                // Ignore entity relationships
                .ForMember(d => d.Set, o => o.Ignore())
                // Standard ignores
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                // Legacy data (no longer collected)
                .ForMember(d => d.FormId, o => o.Ignore())
                .ForMember(d => d.Comments, o => o.Ignore()) // WTF?!
                // Custom properties
                .ForMember(d => d.SamplingProtocolComments, o => o.MapFrom(s => s.ProtocolComments))
                .ForMember(d => d.OtherSamplingCode, o => o.MapFrom(s => s.OtherCode))
                .ForMember(d => d.TotalBrailCount, o => o.MapFrom(s => s.TotalBrails))
                .ForMember(d => d.FishPerBrail, o => o.MapFrom(s => s.GrabTarget))
                .ForMember(d => d.BrailNumber, o => o.MapFrom(s => WhichBrailResolver(s.WhichBrail)))
                .ForMember(d => d.SampledBrailNumber, o => o.MapFrom(s => s.SpillBrailNumber))
                .ForMember(d => d.NumberOfFishMeasured, o => o.MapFrom(s => s.SpillNumberFishMeasured))
                .ForMember(d => d.SamplingProtocol, o => o.ResolveUsing<SamplingProtocolCodeResolver>().FromMember(s => s.SampleType))
                // Same property name, requires a resolver
                .ForMember(d => d.CalibratedThisSet, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.CalibratedThisSet))
                // Caller's responsibility
                .ForMember(d => d.Brails, o => o.Ignore())
                ;

            // Entity to ViewModel
            CreateMap<DAL.Entities.BrailRecord, LengthFrequencyViewModel.Brail>()
                .ForMember(d => d.Number, o => o.MapFrom(s => s.Sequence))
                ;            

            CreateMap<DAL.Entities.LengthSamplingHeader, LengthFrequencyViewModel>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.BrailId, o => o.MapFrom(s => null != s.Brails ? s.Brails.Id : 0))
                .ForMember(d => d.SetId, o => o.MapFrom(s => s.Set.Id))                
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Set.Activity.Day.Trip.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.Set.Activity.Day.Trip.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Set.Activity.Day.Trip.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.SetNumber, o => o.MapFrom(s => s.Set.SetNumber))
                .ForMember(d => d.SpillNumberFishMeasured, o => o.MapFrom(s => s.NumberOfFishMeasured))
                .ForMember(d => d.GrabTarget, o => o.MapFrom(s => s.FishPerBrail))
                .ForMember(d => d.SpillBrailNumber, o => o.MapFrom(s => s.SampledBrailNumber))
                .ForMember(d => d.CalibratedThisSet, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.CalibratedThisSet))
                .ForMember(d => d.SampleType, o => o.ResolveUsing<SamplingProtocolResolver>().FromMember(s => s.SamplingProtocol))
                .ForMember(d => d.OtherCode, o => o.MapFrom(s => s.OtherSamplingCode))
                .ForMember(d => d.ProtocolComments, o => o.MapFrom(s => s.SamplingProtocolComments))
                .ForMember(d => d.TotalBrails, o => o.MapFrom(s => s.TotalBrailCount))               
                .ForMember(d => d.Brails, o => o.MapFrom(s => null != s.Brails ? s.Brails.BrailRecords : Enumerable.Empty<DAL.Entities.BrailRecord>()))
                .ForMember(d => d.WhichBrail, o => o.MapFrom(s => s.BrailNumber.HasValue ? string.Format("Brail {0}", s.BrailNumber.Value) : string.Empty))
                // Ignore UI properties
                .ForMember(d => d.BooleanValues, o => o.Ignore())
                .ForMember(d => d.OtherCodes, o => o.Ignore())
                .ForMember(d => d.MeasuringInstruments, o => o.Ignore())
                .ForMember(d => d.SampleTypes, o => o.Ignore())
                .ForMember(d => d.ActionName, o => o.Ignore())
                .ForMember(d => d.BrailNumbers, o => o.Ignore())
                // AfterMap
                .ForMember(d => d.HasNext, o => o.Ignore())
                .ForMember(d => d.HasPrevious, o => o.Ignore())
                .AfterMap((s,d) => 
                {
                    // If necessary, sort Brails on the destination by Sequence number
                    d.HasNext = (d.PageNumber.HasValue && d.PageCount.HasValue) && d.PageNumber < d.PageCount;
                    d.HasPrevious = d.PageNumber.HasValue && d.PageNumber.Value > 1;
                })
                ;

        }
    }
}
