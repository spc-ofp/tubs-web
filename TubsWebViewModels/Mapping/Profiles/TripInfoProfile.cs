// -----------------------------------------------------------------------
// <copyright file="TripInfoProfile.cs" company="Secretariat of the Pacific Community">
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

    public class TripInfoProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            // ViewModel to Entity
            CreateMap<LongLineTripInfoViewModel.FishingGear, DAL.Entities.LongLineGear>()
                // Ignore entity relationships
                .ForMember(d => d.Trip, o => o.Ignore())
                // Standard ignores
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                // Ignore data that will be filled in by another map
                .ForMember(d => d.HasRefrigeratedSeawater, o => o.Ignore())
                .ForMember(d => d.HasIce, o => o.Ignore())
                .ForMember(d => d.HasBlastFreezer, o => o.Ignore())
                .ForMember(d => d.HasChilledSeawater, o => o.Ignore())
                .ForMember(d => d.HasOtherStorage, o => o.Ignore())
                .ForMember(d => d.OtherStorageDescription, o => o.Ignore())
                // Ignore legacy data
                .ForMember(d => d.MainlineMaterialDescription, o => o.Ignore())
                .ForMember(d => d.MainlineComposition, o => o.Ignore())
                .ForMember(d => d.BranchlineMaterial1Description, o => o.Ignore())
                .ForMember(d => d.BranchlineMaterial2Description, o => o.Ignore())
                .ForMember(d => d.BranchlineMaterial3Description, o => o.Ignore())
                .ForMember(d => d.BranchlineMaterial3, o => o.Ignore())
                .ForMember(d => d.BranchlineComposition, o => o.Ignore())
                .ForMember(d => d.MainlineHaulerComments, o => o.Ignore())
                .ForMember(d => d.BranchlineHaulerComments, o => o.Ignore())
                .ForMember(d => d.LineShooterComments, o => o.Ignore())
                .ForMember(d => d.BaitThrowerComments, o => o.Ignore())
                .ForMember(d => d.BranchlineAttacherComments, o => o.Ignore())
                .ForMember(d => d.WeightScalesComments, o => o.Ignore())
                // Custom properties
                .ForMember(d => d.HasMainlineHauler, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasMainlineHauler))
                .ForMember(d => d.MainlineHaulerUsage, o => o.ResolveUsing<UsageCodeTextResolver>().FromMember(s => s.MainlineHaulerUsage))
                .ForMember(d => d.HasBranchlineHauler, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasBranchlineHauler))
                .ForMember(d => d.BranchlineHaulerUsage, o => o.ResolveUsing<UsageCodeTextResolver>().FromMember(s => s.BranchlineHaulerUsage))
                .ForMember(d => d.HasLineShooter, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasLineShooter))
                .ForMember(d => d.LineShooterUsage, o => o.ResolveUsing<UsageCodeTextResolver>().FromMember(s => s.LineShooterUsage))
                .ForMember(d => d.HasBaitThrower, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasBaitThrower))
                .ForMember(d => d.BaitThrowerUsage, o => o.ResolveUsing<UsageCodeTextResolver>().FromMember(s => s.BaitThrowerUsage))
                .ForMember(d => d.HasBranchlineAttacher, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasBranchlineAttacher))
                .ForMember(d => d.BranchlineAttacherUsage, o => o.ResolveUsing<UsageCodeTextResolver>().FromMember(s => s.BranchlineAttacherUsage))
                .ForMember(d => d.HasWeightScales, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasWeighingScales))
                .ForMember(d => d.WeightScalesUsage, o => o.ResolveUsing<UsageCodeTextResolver>().FromMember(s => s.WeighingScalesUsage))
                .ForMember(d => d.BranchlineMaterial1Diameter, o => o.MapFrom(s => s.BranchlineDiameter1))
                .ForMember(d => d.BranchlineMaterial2Diameter, o => o.MapFrom(s => s.BranchlineDiameter2))
                .ForMember(d => d.JapanHookOffsetRingSwivel, o => o.MapFrom(s => s.JapanOffsetRingSwivel))
                .ForMember(d => d.CircleHookOffsetRingSwivel, o => o.MapFrom(s => s.CircleOffsetRingSwivel))
                .ForMember(d => d.OtherHookOffsetRingSwivel, o => o.MapFrom(s => s.OtherOffsetRingSwivel))
                ;

            // In general, one doesn't want to do this, as it bypasses the configuration checking built into AutoMapper
            // However, there are only 6 members here and we don't want to muddy this profile with
            // 75 lines of .Ignore()
            // It's also worth mentioning that this is the AutoMapper team's recommended strategy for this
            // situation
            // http://stackoverflow.com/questions/4367591/automapper-how-to-ignore-all-destination-members-except-the-ones-that-are-mapp
            // See the test "MergeGearAndRefrigeration" for an example of how to
            // use these maps to create a single Gear instance.
            var rmap = CreateMap<LongLineTripInfoViewModel.RefrigerationMethod, DAL.Entities.LongLineGear>();
            rmap.ForAllMembers(o => o.Ignore());
            rmap.ForMember(d => d.HasBlastFreezer, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasBlastFreeze))
                .ForMember(d => d.HasIce, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasIce))
                .ForMember(d => d.HasRefrigeratedSeawater, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasRefrigeratedBrine))
                .ForMember(d => d.HasChilledSeawater, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasChilledSeawater))
                .ForMember(d => d.HasOtherStorage, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.HasOther))
                .ForMember(d => d.OtherStorageDescription, o => o.MapFrom(s => s.Description))
                ;


            // Entity to ViewModel
            CreateMap<DAL.Entities.LongLineGear, LongLineTripInfoViewModel.FishingGear>()
                .ForMember(d => d.HasMainlineHauler, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasMainlineHauler))
                .ForMember(d => d.MainlineHaulerUsage, o => o.ResolveUsing<UsageCodeResolver>().FromMember(s => s.MainlineHaulerUsage))
                .ForMember(d => d.HasBranchlineHauler, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasBranchlineHauler))
                .ForMember(d => d.BranchlineHaulerUsage, o => o.ResolveUsing<UsageCodeResolver>().FromMember(s => s.BranchlineHaulerUsage))
                .ForMember(d => d.HasLineShooter, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasLineShooter))
                .ForMember(d => d.LineShooterUsage, o => o.ResolveUsing<UsageCodeResolver>().FromMember(s => s.LineShooterUsage))
                .ForMember(d => d.HasBaitThrower, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasBaitThrower))
                .ForMember(d => d.BaitThrowerUsage, o => o.ResolveUsing<UsageCodeResolver>().FromMember(s => s.BaitThrowerUsage))
                .ForMember(d => d.HasBranchlineAttacher, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasBranchlineAttacher))
                .ForMember(d => d.BranchlineAttacherUsage, o => o.ResolveUsing<UsageCodeResolver>().FromMember(s => s.BranchlineAttacherUsage))
                .ForMember(d => d.HasWeighingScales, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasWeightScales))
                .ForMember(d => d.WeighingScalesUsage, o => o.ResolveUsing<UsageCodeResolver>().FromMember(s => s.WeightScalesUsage))
                // Not sure where this comes from in Entity
                .ForMember(d => d.Description, o => o.Ignore())
                .ForMember(d => d.HasOther, o => o.Ignore())
                .ForMember(d => d.OtherUsage, o => o.Ignore())
                // FishingGear has no direct link to version
                .ForMember(d => d.ShowOffsetRingSwivel, o => o.MapFrom(s => s.Trip.Version == WorkbookVersion.v2009))
                .ForMember(d => d.JapanOffsetRingSwivel, o => o.MapFrom(s => s.JapanHookOffsetRingSwivel))
                .ForMember(d => d.CircleOffsetRingSwivel, o => o.MapFrom(s => s.CircleHookOffsetRingSwivel))
                .ForMember(d => d.JHookOffsetRingSwivel, o => o.MapFrom(s => s.JHookOffsetRingSwivel))
                .ForMember(d => d.OtherOffsetRingSwivel, o => o.MapFrom(s => s.OtherHookOffsetRingSwivel))
                .ForMember(d => d.BranchlineDiameter1, o => o.MapFrom(s => s.BranchlineMaterial1Diameter))
                .ForMember(d => d.BranchlineDiameter2, o => o.MapFrom(s => s.BranchlineMaterial2Diameter))
                // AfterMap
                .ForMember(d => d.ShowOtherHook, o => o.Ignore())
                .ForMember(d => d.ShowNewGear, o => o.Ignore())
                .AfterMap((s,d) => 
                {
                    d.ShowOtherHook =
                        !String.IsNullOrEmpty(d.OtherHookSize) ||
                        d.OtherHookPercentage.HasValue;

                    d.ShowNewGear =
                        !String.IsNullOrEmpty(d.Description) ||
                        !String.IsNullOrEmpty(d.HasOther) ||
                        !String.IsNullOrEmpty(d.OtherUsage);
                })
                ;
 
            CreateMap<DAL.Entities.LongLineGear, LongLineTripInfoViewModel.RefrigerationMethod>()
                .ForMember(d => d.HasBlastFreeze, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasBlastFreezer))
                .ForMember(d => d.HasIce, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasIce))
                .ForMember(d => d.HasRefrigeratedBrine, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasRefrigeratedSeawater))
                .ForMember(d => d.HasChilledSeawater, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasChilledSeawater))
                .ForMember(d => d.HasOther, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasOtherStorage))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.OtherStorageDescription))
                ;

            CreateMap<DAL.Entities.VesselNotes, LongLineTripInfoViewModel.CrewNationality>()
                .ForMember(d => d.GroupOneCount, o => o.Ignore())
                .ForMember(d => d.GroupOneCountryCode, o => o.Ignore())
                .ForMember(d => d.GroupTwoCount, o => o.Ignore())
                .ForMember(d => d.GroupTwoCountryCode, o => o.Ignore())
                .ForMember(d => d.GroupThreeCount, o => o.Ignore())
                .ForMember(d => d.GroupThreeCountryCode, o => o.Ignore())
                .ForMember(d => d.GroupFourCount, o => o.Ignore())
                .ForMember(d => d.GroupFourCountryCode, o => o.Ignore())
                .ForMember(d => d.CaptainCountryCode, o => o.MapFrom(s => s.CaptainCountryCode))
                .ForMember(d => d.MasterCountryCode, o => o.MapFrom(s => s.MasterCountryCode))
                ;

            CreateMap<DAL.Entities.LongLineTrip, LongLineTripInfoViewModel>()
                // Ignore UI properties
                .ForMember(d => d.AvailabilityValues, o => o.Ignore())
                .ForMember(d => d.BooleanValues, o => o.Ignore())
                .ForMember(d => d.LengthUnits, o => o.Ignore())
                .ForMember(d => d.UsageValues, o => o.Ignore())
                // Custom properties
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.Comments, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Characteristics, o => o.Ignore())
                .ForMember(d => d.Nationality, o => o.MapFrom(s => s.VesselNotes))
                .ForMember(d => d.Refrigeration, o => o.MapFrom(s => s.Gear))
                .AfterMap((s, d) =>
                {
                    if (null == d.Inspection)
                        d.Inspection = new SafetyInspectionViewModel();
                    if (null == d.Gear)
                        d.Gear = new LongLineTripInfoViewModel.FishingGear();
                    if (null == d.Characteristics)
                        d.Characteristics = new LongLineTripInfoViewModel.VesselCharacteristics();
                    if (null == d.Nationality)
                        d.Nationality = new LongLineTripInfoViewModel.CrewNationality();
                    if (null == d.Refrigeration)
                        d.Refrigeration = new LongLineTripInfoViewModel.RefrigerationMethod();

                    // TODO
                    // Could this be replaced with a merging map?
                    // Or do we just say heck-with-it and fill these properties in in the controller?
                    
                    // Characteristics pulls in data from Trip, Trip.VesselNotes, and Trip.Vessel
                    if (null != s)
                    {
                        d.Characteristics.HoldCapacity = s.WellCapacity;

                        if (null != s.Vessel)
                        {
                            d.Characteristics.RegistrationNumber = s.Vessel.RegistrationNumber;
                            d.Characteristics.CountryCode = s.Vessel.RegisteredCountryCode;
                            d.Characteristics.Ircs = s.Vessel.Ircs;
                            d.Characteristics.Length = (decimal?)s.Vessel.Length;
                            d.Characteristics.GrossTonnage = (decimal?)s.Vessel.GrossTonnage;
                             
                        }

                        if (null != s.VesselNotes)
                        {
                            d.Comments = s.VesselNotes.Comments;
                            d.Characteristics.PermitNumbers = s.VesselNotes.Licenses;
                            d.Characteristics.Owner = s.VesselNotes.Owner;
                            d.Characteristics.Captain = s.VesselNotes.Captain;
                        }
                    }
                })               
                ;
        }
    }
}
