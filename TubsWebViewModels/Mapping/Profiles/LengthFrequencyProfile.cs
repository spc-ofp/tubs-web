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
        protected override void Configure()
        {
            base.Configure();
            // TODO ViewModel to Entity

            // Entity to ViewModel
            CreateMap<DAL.Entities.LengthSample, LengthFrequencyViewModel.Sample>()

                ;

            CreateMap<DAL.Entities.LengthSamplingHeader, LengthFrequencyViewModel>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Set.Activity.Day.Trip.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.Set.Activity.Day.Trip.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Set.Activity.Day.Trip.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.SpillNumberFishMeasured, o => o.MapFrom(s => s.NumberOfFishMeasured))
                .ForMember(d => d.GrabTarget, o => o.MapFrom(s => s.FishPerBrail))
                .ForMember(d => d.SpillBrailNumber, o => o.MapFrom(s => s.SampledBrailNumber))
                .ForMember(d => d.CalibratedThisSet, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.CalibratedThisSet))
                .ForMember(d => d.OtherCode, o => o.Ignore()) // This looks like it's missing on the other side
                /*
                .ForMember(d => d.OtherCode, o => 
                {
                    o.Condition(s => !s.IsSourceValueNull);
                    o.MapFrom(s => s.SamplingProtocol);
                })
                */
                .ForMember(d => d.IsBrail1, o => o.MapFrom(s => s.BrailNumber.HasValue && 1 == s.BrailNumber.Value ? true : (bool?)null))
                .ForMember(d => d.IsBrail2, o => o.MapFrom(s => s.BrailNumber.HasValue && 2 == s.BrailNumber.Value ? true : (bool?)null))
                .ForMember(d => d.StartBrailingTime, o => o.MapFrom(s => s.BrailStartTime))
                .ForMember(d => d.EndBrailingTime, o => o.MapFrom(s => s.BrailEndTime))
                .ForMember(d => d.SamplePageNumber, o => o.MapFrom(s => s.PageNumber))
                .ForMember(d => d.SamplePageTotal, o => o.MapFrom(s => s.PageCount))
                .ForMember(d => d.ProtocolComments, o => o.MapFrom(s => s.SamplingProtocolComments))
                .ForMember(d => d.TotalBrails, o => o.MapFrom(s => s.TotalBrailCount))
                .ForMember(d => d.BrailId, o => o.MapFrom(s => s.Brails.Count > 0 ? s.Brails[0].Id : 0))
                .ForMember(d => d.Brails, o => o.Ignore()) // AfterMap
                .AfterMap((s,d) => 
                {

                })
                ;

        }
    }
}
