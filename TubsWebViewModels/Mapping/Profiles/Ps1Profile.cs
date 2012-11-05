// -----------------------------------------------------------------------
// <copyright file="Ps1Profile.cs" company="Secretariat of the Pacific Community">
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
    using TubsWeb.ViewModels.Resolvers;

    /// <summary>
    /// AutoMapper profile for conversion of data access layer
    /// entities to ViewModel.  Reverse of these mappings found
    /// in Ps1ViewModelProfile
    /// </summary>
    public class Ps1Profile : Profile
    {
        protected override void Configure()
        {
            base.Configure();
            // ViewModel to Entity handled in Ps1ViewModelProfile
            
            // Entity to ViewModel
            CreateMap<DAL.Entities.PurseSeineGear, Ps1ViewModel.FishingGear>()
                .ForMember(d => d.NetStripCount, o => o.MapFrom(s => s.NetStrips))
                .ForMember(d => d.Brail1Capacity, o => o.MapFrom(s => s.Brail1Size))
                .ForMember(d => d.Brail2Capacity, o => o.MapFrom(s => s.Brail2Size))
                .ForMember(d => d.NetMeshSize, o => o.MapFrom(s => s.MeshSize))
                .ForMember(d => d.NetDepthUnits, o => o.ResolveUsing<UnitOfMeasureResolver>().FromMember(s => s.NetDepthUnit))
                .ForMember(d => d.NetMeshUnits, o => o.ResolveUsing<UnitOfMeasureResolver>().FromMember(s => s.MeshSizeUnits))
                ;

            CreateMap<DAL.Entities.PurseSeineVesselAttributes, Ps1ViewModel.VesselCharacteristics>()
                .ForMember(d => d.Owner, o => o.Ignore()) // TODO:  Add to DAL?
                .ForMember(d => d.RegistrationNumber, o => o.Ignore()) // AfterMap
                .ForMember(d => d.CountryCode, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Ircs, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Length, o => o.Ignore()) // AfterMap
                .ForMember(d => d.LengthUnits, o => o.Ignore()) // AfterMap
                .ForMember(d => d.GrossTonnage, o => o.Ignore()) // AfterMap
                .ForMember(d => d.TenderBoatAnswer, o => o.MapFrom(s => s.HasTenderBoats))
                .ForMember(d => d.HelicopterRegistration, o => o.MapFrom(s => s.HelicopterRegistrationNumber))
                .ForMember(d => d.HelicopterRangeUnits, o => o.ResolveUsing<UnitOfMeasureResolver>().FromMember(s => s.HelicopterRangeUnit))
                .AfterMap((s,d) => {
                    if (null != s && null != s.Trip && null != s.Trip.Vessel)
                    {
                        var vessel = s.Trip.Vessel;
                        d.RegistrationNumber = vessel.RegistrationNumber;
                        d.CountryCode = vessel.RegisteredCountryCode;
                        d.Ircs = vessel.Ircs;
                        d.Length = vessel.Length;
                        d.GrossTonnage = vessel.GrossTonnage;
                    }                    
                })
                ;

            // TODO Lifejacket provided is 'Y/N/O'
            CreateMap<DAL.Entities.SafetyInspection, Ps1ViewModel.SafetyInspection>()
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

            CreateMap<DAL.Entities.PurseSeineTrip, Ps1ViewModel>()
                .ForMember(d => d.BooleanValues, o => o.Ignore())
                .ForMember(d => d.NetUnits, o => o.Ignore())
                .ForMember(d => d.MeshUnits, o => o.Ignore())
                .ForMember(d => d.RangeUnits, o => o.Ignore())
                .ForMember(d => d.PermitNumbers, o => o.MapFrom(s => s.VesselNotes != null ? s.VesselNotes.Licenses : null))
                .ForMember(d => d.Page2Comments, o => o.Ignore()) // TODO:  Where?
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.Page1Comments, o => o.MapFrom(s => s.VesselNotes != null ? s.VesselNotes.Comments : null))
                .ForMember(d => d.Characteristics, o => o.MapFrom(s => s.VesselAttributes))
                .AfterMap((s,d) => 
                {
                    if (null == d.Inspection)
                        d.Inspection = new Ps1ViewModel.SafetyInspection();
                    if (null == d.Gear)
                        d.Gear = new Ps1ViewModel.FishingGear();
                    if (null == d.Characteristics)
                        d.Characteristics = new Ps1ViewModel.VesselCharacteristics();
                })
                ;
        }
    }
}
