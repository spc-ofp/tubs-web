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
                // TODO Add line/hook information when it's present in VM
                ;
 
            CreateMap<DAL.Entities.LongLineGear, LongLineTripInfoViewModel.RefrigerationMethod>()
                .ForMember(d => d.HasBlastFreeze, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasBlastFreezer))
                .ForMember(d => d.HasIce, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasIce))
                .ForMember(d => d.HasRefrigeratedBrine, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasRefrigeratedSeawater))
                .ForMember(d => d.HasChilledSeawater, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasChilledSeawater))
                .ForMember(d => d.HasOther, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.HasOtherStorage))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.OtherStorageDescription))
                ;

            // TODO Lifejacket provided is 'Y/N/O'
            CreateMap<DAL.Entities.SafetyInspection, LongLineTripInfoViewModel.SafetyInspection>()
                .ForMember(d => d.LifejacketSizeOk, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.LifejacketSizeOk))
                .ForMember(d => d.Epirb406Count, o => o.MapFrom(s => s.Epirb1.Count))
                .ForMember(d => d.Epirb406Expiration, o => o.MapFrom(s => s.Epirb1.Expiration))
                .ForMember(d => d.OtherEpirbType, o => o.MapFrom(s => s.Epirb2.BeaconType))
                .ForMember(d => d.OtherEpirbCount, o => o.MapFrom(s => s.Epirb2.Count))
                .ForMember(d => d.OtherEpirbExpiration, o => o.MapFrom(s => s.Epirb2.Expiration))
                .ForMember(d => d.LifeRaft1Capacity, o => o.MapFrom(s => s.Raft1.Capacity))
                .ForMember(d => d.LifeRaft1Inspection,
                    o => o.MapFrom(s => s.Raft1.InspectionDate.HasValue ? s.Raft1.InspectionDate.Value.ToString("mm/yy") : string.Empty))
                .ForMember(d => d.LifeRaft1LastOrDue, o => o.MapFrom(s => s.Raft1.LastOrDue))
                //
                .ForMember(d => d.LifeRaft2Capacity, o => o.MapFrom(s => s.Raft2.Capacity))
                .ForMember(d => d.LifeRaft2Inspection,
                    o => o.MapFrom(s => s.Raft2.InspectionDate.HasValue ? s.Raft2.InspectionDate.Value.ToString("mm/yy") : string.Empty))
                .ForMember(d => d.LifeRaft2LastOrDue, o => o.MapFrom(s => s.Raft2.LastOrDue))
                //
                .ForMember(d => d.LifeRaft3Capacity, o => o.MapFrom(s => s.Raft3.Capacity))
                .ForMember(d => d.LifeRaft3Inspection,
                    o => o.MapFrom(s => s.Raft3.InspectionDate.HasValue ? s.Raft3.InspectionDate.Value.ToString("mm/yy") : string.Empty))
                .ForMember(d => d.LifeRaft3LastOrDue, o => o.MapFrom(s => s.Raft3.LastOrDue))
                //
                .ForMember(d => d.LifeRaft4Capacity, o => o.MapFrom(s => s.Raft4.Capacity))
                .ForMember(d => d.LifeRaft4Inspection,
                    o => o.MapFrom(s => s.Raft4.InspectionDate.HasValue ? s.Raft4.InspectionDate.Value.ToString("mm/yy") : string.Empty))
                .ForMember(d => d.LifeRaft4LastOrDue, o => o.MapFrom(s => s.Raft4.LastOrDue))
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
                .ForMember(d => d.BooleanValues, o => o.Ignore())
                .ForMember(d => d.LengthUnits, o => o.Ignore())
                .ForMember(d => d.UsageValues, o => o.Ignore())
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
                        d.Inspection = new LongLineTripInfoViewModel.SafetyInspection();
                    if (null == d.Gear)
                        d.Gear = new LongLineTripInfoViewModel.FishingGear();
                    if (null == d.Characteristics)
                        d.Characteristics = new LongLineTripInfoViewModel.VesselCharacteristics();
                    if (null == d.Nationality)
                        d.Nationality = new LongLineTripInfoViewModel.CrewNationality();
                    if (null == d.Refrigeration)
                        d.Refrigeration = new LongLineTripInfoViewModel.RefrigerationMethod();

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
