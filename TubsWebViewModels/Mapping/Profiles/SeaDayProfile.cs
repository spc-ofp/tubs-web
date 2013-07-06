// -----------------------------------------------------------------------
// <copyright file="SeaDayProfile.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL.Common;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    using DAL = Spc.Ofp.Tubs.DAL;
    
    /// <summary>
    /// AutoMapper profile for the conversion of PurseSeineSeaDay entities to/from
    /// MVC ViewModels.
    /// </summary>
    public sealed class SeaDayProfile : Profile
    {
        internal string ActivityTime(DAL.Entities.Activity activity)
        {
            if (null == activity)
                return string.Empty;

            // Prefer using the value in the _dtime column
            if (activity.LocalTime.HasValue)
                return activity.LocalTime.Value.ToString("HHmm");

            // I would hope this isn't the case, but legacy data is screwy...
            if (!string.IsNullOrWhiteSpace(activity.LocalTimeTimeOnly))
                return activity.LocalTimeTimeOnly.Trim();

            // No idea!
            return string.Empty;
        }
        
        protected override void Configure()
        {
            base.Configure();

            // ViewModel to Entity
            CreateMap<SeaDayViewModel.SeaDayEvent, DAL.Entities.PurseSeineActivity>()
                // Ignore entity relationships
                .ForMember(d => d.Day, o => o.Ignore()) 
                .ForMember(d => d.Fad, o => o.Ignore())
                .ForMember(d => d.FishingSet, o => o.Ignore())
                .ForMember(d => d.AccessControl, o => o.Ignore())
                // Standard ignores                
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                // Legacy data
                .ForMember(d => d.FishDays, o => o.Ignore())
                .ForMember(d => d.RowVersion, o => o.Ignore())
                // Custom properties
                .ForMember(d => d.Id, o => o.MapFrom(s => s.EventId))
                .ForMember(d => d.Payao, o => o.MapFrom(s => s.FadNumber))
                .ForMember(d => d.Beacon, o => o.MapFrom(s => s.BuoyNumber))
                .ForMember(d => d.ActivityType, o => o.ResolveUsing<ActivityCodeResolver>().FromMember(s => s.ActivityCode))
                .ForMember(d => d.DetectionMethod, o => o.ResolveUsing<DetectionCodeResolver>().FromMember(s => s.DetectionCode))
                .ForMember(d => d.SchoolAssociation, o => o.ResolveUsing<AssociationCodeResolver>().FromMember(s => s.AssociationCode))
                .ForMember(d => d.LocalTimeTimeOnly, o => o.MapFrom(s => s.Time))
                .ForMember(d => d.LocalTimeDateOnly, o => o.Ignore()) // Handled in parent AfterMap
                .ForMember(d => d.LocalTime, o => o.Ignore()) // Handled in parent AfterMap
                .ForMember(d => d.UtcTimeOnly, o => o.Ignore())
                .ForMember(d => d.UtcDateOnly, o => o.Ignore())
                .ForMember(d => d.UtcTime, o => o.Ignore())
                ;

            CreateMap<SeaDayViewModel, DAL.Entities.PurseSeineSeaDay>()
                // Ignore entity relationships
                .ForMember(d => d.Trip, o => o.Ignore())
                // Standard ignores
                .ForMember(d => d.FormId, o => o.Ignore())
                .ForMember(d => d.RowVersion, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.Id, o => o.MapFrom(s => s.DayId))
                // Original implementation was converting all activities, even those marked for deletion
                // TODO May want to confirm that this bug isn't present in other profiles
                .ForMember(d => d.Activities, o => o.MapFrom(s => s.Events.Where(e => null != e && !e._destroy)))
                .ForMember(d => d.FadsNoSchool, o => o.MapFrom(s => s.AnchoredWithNoSchool))
                .ForMember(d => d.FadsWithSchool, o => o.MapFrom(s => s.AnchoredWithSchool))
                .ForMember(d => d.FloatingObjectsNoSchool, o => o.MapFrom(s => s.FreeFloatingWithNoSchool))
                .ForMember(d => d.FloatingObjectsWithSchool, o => o.MapFrom(s => s.FreeFloatingWithSchool))
                .ForMember(d => d.FreeSchools, o => o.MapFrom(s => s.FreeSchool))
                .ForMember(d => d.Gen3Events, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasGen3Event))
                .ForMember(d => d.StartDateOnly, o => o.MapFrom(s => s.ShipsDate))
                .ForMember(d => d.StartTimeOnly, o => o.MapFrom(s => s.ShipsTime))
                .ForMember(d => d.StartOfDay, o => o.MapFrom(s => s.ShipsDate.Merge(s.ShipsTime)))
                .ForMember(d => d.UtcDateOnly, o => o.MapFrom(s => s.UtcDate))
                .ForMember(d => d.UtcTimeOnly, o => o.MapFrom(s => s.UtcTime))
                .ForMember(d => d.UtcStartOfDay, o => o.MapFrom(s => s.UtcDate.Merge(s.UtcTime)))
                .AfterMap((s, d) =>
                {
                    d.Activities.ToList().ForEach(a => 
                    {
                        a.Day = d;
                        a.LocalTime = d.StartDateOnly.Merge(a.LocalTimeTimeOnly);
                        a.LocalTimeDateOnly = d.StartDateOnly;
                    });

                    // TODO  Compute offset from StartOfDay to UtcStartOfDay and apply to UTC
                    // values in activities
                })
                ;

            // Entity to ViewModel
            CreateMap<DAL.Entities.PurseSeineActivity, SeaDayViewModel.SeaDayEvent>()
                .ForMember(d => d.EventId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Time, o => o.MapFrom(s => ActivityTime(s)))
                .ForMember(d => d.ActivityCode, o => o.ResolveUsing<ActivityTypeResolver>().FromMember(s => s.ActivityType))
                .ForMember(d => d.DetectionCode, o => o.ResolveUsing<DetectionMethodResolver>().FromMember(s => s.DetectionMethod))
                .ForMember(d => d.AssociationCode, o => o.ResolveUsing<SchoolAssociationResolver>().FromMember(s => s.SchoolAssociation))
                .ForMember(d => d.FadNumber, o => o.MapFrom(s => s.Payao))
                .ForMember(d => d.BuoyNumber, o => o.MapFrom(s => s.Beacon))
                .ForMember(d => d.HasSet, o => o.MapFrom(s => DAL.Common.ActivityType.Fishing == s.ActivityType && null != s.FishingSet))
                .ForMember(d => d.HasGen5, o => o.MapFrom(s => null != s.Fad))
                .ForMember(d => d.Gen5Id, o => o.MapFrom(s => null != s.Fad ? s.Fad.Id : 0))
                .ForMember(d => d.IsLocked, o => o.MapFrom(s => DAL.Common.ActivityType.Fishing == s.ActivityType && null != s.FishingSet))
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())
                .ForMember(d => d.ActivityIconPath, o => o.Ignore())
                ;

            CreateMap<DAL.Entities.PurseSeineSeaDay, SeaDayViewModel>()
                .ForMember(d => d.DayId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Trip.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => s.Trip.SpcTripNumber))
                .ForMember(d => d.ShipsDate, o => o.MapFrom(s => s.StartDateOnly))
                .ForMember(d => d.ShipsTime, o => o.MapFrom(s => s.StartTimeOnly))
                .ForMember(d => d.UtcDate, o => o.MapFrom(s => s.UtcDateOnly))
                .ForMember(d => d.UtcTime, o => o.MapFrom(s => s.UtcTimeOnly))
                .ForMember(d => d.AnchoredWithNoSchool, o => o.MapFrom(s => s.FadsNoSchool))
                .ForMember(d => d.AnchoredWithSchool, o => o.MapFrom(s => s.FadsWithSchool))
                .ForMember(d => d.FreeFloatingWithNoSchool, o => o.MapFrom(s => s.FloatingObjectsNoSchool))
                .ForMember(d => d.FreeFloatingWithSchool, o => o.MapFrom(s => s.FloatingObjectsWithSchool))
                .ForMember(d => d.FreeSchool, o => o.MapFrom(s => s.FreeSchools))
                .ForMember(d => d.HasGen3Event, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.Gen3Events))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Trip.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.Events, o => o.MapFrom(s => s.Activities.OrderBy(a => a.LocalTime))) // TODO GetTime sorts by LocalTime or LocalTimeTimeOnly
                .ForMember(d => d.HasNext, o => o.Ignore())
                .ForMember(d => d.PreviousDay, o => o.Ignore())
                .ForMember(d => d.HasPrevious, o => o.Ignore())
                .ForMember(d => d.NextDay, o => o.Ignore())
                .ForMember(d => d.ActivityCodes, o => o.Ignore())
                .ForMember(d => d.DetectionCodes, o => o.Ignore())
                .ForMember(d => d.AssociationCodes, o => o.Ignore())
                .ForMember(d => d.SeaCodes, o => o.Ignore())
                .ForMember(d => d.DayNumber, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.MaxDays, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.ActionName, o => o.Ignore()) // MVC routing detail
                .AfterMap((s, d) =>
                {
                    d.ActivityCodes = d.VersionNumber == 2009 ?
                        SeaDayViewModel.v2009ActivityCodes :
                        SeaDayViewModel.v2007ActivityCodes;
                })
                ;
        }
    }
}
