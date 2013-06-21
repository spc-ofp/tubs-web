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
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    using System.Globalization;

    public class SafetyInspectionProfile : Profile
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

            // ViewModel to Entity
            CreateMap<SafetyInspectionViewModel, DAL.Entities.SafetyInspection>()
                // Ignore entity relationships
                .ForMember(d => d.Trip, o => o.Ignore())
                // Standard ignores
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                // Custom properties
                .ForMember(d => d.LifejacketProvided, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.LifejacketProvided))
                .ForMember(d => d.LifejacketSizeOk, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.LifejacketSizeOk))
                // AfterMap
                .ForMember(d => d.Epirb1, o => o.Ignore())
                .ForMember(d => d.Epirb2, o => o.Ignore())
                .ForMember(d => d.Raft1, o => o.Ignore())
                .ForMember(d => d.Raft2, o => o.Ignore())
                .ForMember(d => d.Raft3, o => o.Ignore())
                .ForMember(d => d.Raft4, o => o.Ignore())
                .AfterMap((s, d) =>
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


            // Entity to ViewModel
            CreateMap<DAL.Entities.SafetyInspection, SafetyInspectionViewModel>()
                .ForMember(d => d.LifejacketAvailability, o => o.MapFrom(s => s.LifejacketAvailability))
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

        }

    }
}
