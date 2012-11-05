// -----------------------------------------------------------------------
// <copyright file="Ps1ViewModelProfile.cs" company="Secretariat of the Pacific Community">
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
    using System.Globalization;
    using AutoMapper;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    using DAL = Spc.Ofp.Tubs.DAL;

    /// <summary>
    /// AutoMapper profile
    /// </summary>
    public class Ps1ViewModelProfile : Profile
    {
        public static DAL.Entities.RaftResult MakeRaft(int? capacity, string date, string lastOrDue)
        {
            bool hasDate = !string.IsNullOrEmpty(date);
            bool hasLastOrDue = !string.IsNullOrEmpty(lastOrDue);

            if (!hasDate && !hasLastOrDue && !capacity.HasValue)
                return null;

            var raft = new DAL.Entities.RaftResult
            {
                Capacity = capacity,                
            };

            // mm/yy
            if (hasDate)
            {
                DateTime idate;
                if (DateTime.TryParseExact(date, "mm/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out idate))
                {
                    raft.InspectionDate = idate;
                }
            }

            if (hasLastOrDue)
            {
                lastOrDue = lastOrDue.ToUpper();
                var first = lastOrDue.Substring(0, 1).ToCharArray()[0];
                if (first == 'L' || first == 'D')
                {
                    raft.LastOrDue = first;
                }
            }           

            return raft;
        }
        
        protected override void Configure()
        {
            base.Configure();

            CreateMap<Ps1ViewModel.FishingGear, DAL.Entities.PurseSeineGear>()
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.NetDepthInMeters, o => o.Ignore()) // AfterMap (?)
                .ForMember(d => d.NetLengthInMeters, o => o.Ignore()) // AfterMap (?)
                .ForMember(d => d.MeshSizeInCm, o => o.Ignore()) // AfterMap (?)
                .ForMember(d => d.Trip, o => o.Ignore()) // Managed by parent
                .ForMember(d => d.PowerblockPower, o => o.Ignore()) // No longer captured
                .ForMember(d => d.PowerblockSpeed, o => o.Ignore()) // No longer captured
                .ForMember(d => d.PurseWinchPower, o => o.Ignore()) // No longer captured
                .ForMember(d => d.PurseWinchSpeed, o => o.Ignore()) // No longer captured
                .ForMember(d => d.NetHangRatio, o => o.Ignore())    // No longer captured
                .ForMember(d => d.Brail1Size, o => o.MapFrom(s => s.Brail1Capacity))
                .ForMember(d => d.Brail2Size, o => o.MapFrom(s => s.Brail2Capacity))
                .ForMember(d => d.MeshSize, o => o.MapFrom(s => s.NetMeshSize))
                .ForMember(d => d.NetStrips, o => o.MapFrom(s => s.NetStripCount))
                .ForMember(d => d.NetDepthUnit, o => o.ResolveUsing<LengthUnitResolver>().FromMember(s => s.NetDepthUnits))
                .ForMember(d => d.MeshSizeUnits, o => o.ResolveUsing<LengthUnitResolver>().FromMember(s => s.NetMeshUnits))
                .ForMember(d => d.NetLengthUnits, o => o.ResolveUsing<LengthUnitResolver>().FromMember(s => s.NetLengthUnits))
                ;

            CreateMap<Ps1ViewModel.VesselCharacteristics, DAL.Entities.PurseSeineVesselAttributes>()
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.VesselLength, o => o.Ignore()) // This should come out of Vessel which is read-only
                .ForMember(d => d.VesselTonnage, o => o.Ignore()) // This should come out of Vessel which is read-only
                .ForMember(d => d.Trip, o => o.Ignore()) // Managed by parent
                .ForMember(d => d.TowboatCount, o => o.Ignore()) // No longer captured
                .ForMember(d => d.LightCount, o => o.Ignore())   // No longer captured
                .ForMember(d => d.HelicopterRegistrationNumber, o => o.MapFrom(s => s.HelicopterRegistration))
                //.ForMember(d => d.VesselLength, o => o.MapFrom(s => s.Length))
                //.ForMember(d => d.VesselTonnage, o => o.MapFrom(s => s.GrossTonnage))
                .ForMember(d => d.HasTenderBoats, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.TenderBoatAnswer))                
                .ForMember(d => d.HelicopterRangeUnit, o => o.ResolveUsing<LengthUnitResolver>().FromMember(s => s.HelicopterRangeUnits))
                .ForMember(d => d.VesselLengthUnits, o => o.ResolveUsing<LengthUnitResolver>().FromMember(s => s.LengthUnits))
                ;

            CreateMap<Ps1ViewModel.SafetyInspection, DAL.Entities.SafetyInspection>()
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.LifejacketProvided, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.LifejacketProvided))
                .ForMember(d => d.LifejacketSizeOk, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.LifejacketSizeOk))
                .ForMember(d => d.Trip, o => o.Ignore()) // Managed by parent
                .ForMember(d => d.Epirb1, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Epirb2, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Raft1, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Raft2, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Raft3, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Raft4, o => o.Ignore()) // AfterMap
                .AfterMap((s,d) => 
                {
                    d.Epirb1 = new DAL.Entities.EpirbResult
                    {
                        BeaconType = "406",
                        Count = s.Epirb406Count,
                        Expiration = s.Epirb406Expiration
                    };

                    d.Epirb2 = new DAL.Entities.EpirbResult
                    {
                        BeaconType = s.OtherEpirbType ?? "Unknown",
                        Count = s.OtherEpirbCount,
                        Expiration = s.OtherEpirbExpiration
                    };

                    d.Raft1 = MakeRaft(s.LifeRaft1Capacity, s.LifeRaft1Inspection, s.LifeRaft1LastOrDue);
                    d.Raft2 = MakeRaft(s.LifeRaft2Capacity, s.LifeRaft2Inspection, s.LifeRaft2LastOrDue);
                    d.Raft3 = MakeRaft(s.LifeRaft3Capacity, s.LifeRaft3Inspection, s.LifeRaft3LastOrDue);
                    d.Raft4 = MakeRaft(s.LifeRaft4Capacity, s.LifeRaft4Inspection, s.LifeRaft4LastOrDue);
                })
                ;
        }
    }
}
