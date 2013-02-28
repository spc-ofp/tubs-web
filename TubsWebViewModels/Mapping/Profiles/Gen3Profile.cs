// -----------------------------------------------------------------------
// <copyright file="Gen3Profile.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Gen3Profile : Profile
    {
        public static Gen3ViewModel.Incident ConvertQuestion(string questionCode, bool? answer)
        {
            // It's possible to do all this in a single statement
            // Unfortunately, it's much less readable.
            var incident = new Gen3ViewModel.Incident()
            {
                QuestionCode = questionCode
            };
            if (answer.HasValue)
            {
                incident.Answer = answer.Value ? "YES" : "NO";
            }
            return incident;
        }
        
        protected override void Configure()
        {
            base.Configure();

            // Pre-2009 mappings
            CreateMap<Gen3ViewModel, TripMonitor>()
                .ForMember(d => d.Trip, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.Id, o => o.MapFrom(s => s.MonitorId))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.Notes))
                .ForMember(d => d.Question1, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 1 ? s.Incidents[0].Answer : null))
                .ForMember(d => d.Question2, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 2 ? s.Incidents[1].Answer : null))
                .ForMember(d => d.Question3, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 3 ? s.Incidents[2].Answer : null))
                .ForMember(d => d.Question4, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 4 ? s.Incidents[3].Answer : null))
                .ForMember(d => d.Question5, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 5 ? s.Incidents[4].Answer : null))
                .ForMember(d => d.Question6, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 6 ? s.Incidents[5].Answer : null))
                .ForMember(d => d.Question7, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 7 ? s.Incidents[6].Answer : null))
                .ForMember(d => d.Question8, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 8 ? s.Incidents[7].Answer : null))
                .ForMember(d => d.Question9, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 9 ? s.Incidents[8].Answer : null))
                .ForMember(d => d.Question10, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 10 ? s.Incidents[9].Answer : null))
                .ForMember(d => d.Question11, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 11 ? s.Incidents[10].Answer : null))
                .ForMember(d => d.Question12, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 12 ? s.Incidents[11].Answer : null))
                .ForMember(d => d.Question13, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 13 ? s.Incidents[12].Answer : null))
                .ForMember(d => d.Question14, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 14 ? s.Incidents[13].Answer : null))
                .ForMember(d => d.Question15, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 15 ? s.Incidents[14].Answer : null))
                .ForMember(d => d.Question16, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 16 ? s.Incidents[15].Answer : null))
                .ForMember(d => d.Question17, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 17 ? s.Incidents[16].Answer : null))
                .ForMember(d => d.Question18, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 18 ? s.Incidents[17].Answer : null))
                .ForMember(d => d.Question19, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 19 ? s.Incidents[18].Answer : null))
                .ForMember(d => d.Question20, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Incidents.Count >= 20 ? s.Incidents[19].Answer : null))
                ;

            CreateMap<Gen3ViewModel.Note, TripMonitorDetail>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Header, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.DetailDate, o => o.MapFrom(s => s.Date))
                ;

            CreateMap<TripMonitor, Gen3ViewModel>()
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Trip.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => s.Trip.SpcTripNumber))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Trip.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.Notes, o => o.MapFrom(s => s.Details))
                .ForMember(d => d.MonitorId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.BooleanValues, o => o.Ignore())
                .ForMember(d => d.IncidentByCode, o => o.Ignore()) // Not used in pre-2009 view model
                .ForMember(d => d.Incidents, o => o.Ignore()) // AfterMap
                .AfterMap((s,d) => 
                {
                    // This is your brain.
                    // This is your brain on numbered properties.
                    // Any questions?
                    d.Incidents.Add(ConvertQuestion("a", s.Question1));
                    d.Incidents.Add(ConvertQuestion("b", s.Question2));
                    d.Incidents.Add(ConvertQuestion("c", s.Question3));
                    d.Incidents.Add(ConvertQuestion("d", s.Question4));
                    d.Incidents.Add(ConvertQuestion("e", s.Question5));
                    d.Incidents.Add(ConvertQuestion("f", s.Question6));
                    d.Incidents.Add(ConvertQuestion("g", s.Question7));
                    d.Incidents.Add(ConvertQuestion("h", s.Question8));
                    d.Incidents.Add(ConvertQuestion("i", s.Question9));
                    d.Incidents.Add(ConvertQuestion("j", s.Question10));
                    d.Incidents.Add(ConvertQuestion("k", s.Question11));
                    d.Incidents.Add(ConvertQuestion("l", s.Question12));
                    d.Incidents.Add(ConvertQuestion("m", s.Question13));
                    d.Incidents.Add(ConvertQuestion("n", s.Question14));
                    d.Incidents.Add(ConvertQuestion("o", s.Question15));
                    d.Incidents.Add(ConvertQuestion("p", s.Question16));
                    d.Incidents.Add(ConvertQuestion("q", s.Question17));
                    d.Incidents.Add(ConvertQuestion("r", s.Question18));
                    d.Incidents.Add(ConvertQuestion("s", s.Question19));
                    d.Incidents.Add(ConvertQuestion("t", s.Question20));
                })
                ;

            CreateMap<TripMonitorDetail, Gen3ViewModel.Note>()
                .ForMember(d => d.Date, o => o.MapFrom(s => s.DetailDate))
                .ForMember(d => d._destroy, o => o.Ignore())
                ;


            // 2009 mappings
            // Converting from the ViewModel to entities will require a "fake" parent object
            // (e.g. RevisedTripMonitor) that can act as a container for the collections of
            // Incident and Note from the UI.
            CreateMap<Gen3ViewModel, MockTripMonitor>()
                .ForMember(d => d.Answers, o => o.MapFrom(s => s.Incidents))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.Notes))
                ;

            CreateMap<Gen3ViewModel.Incident, Gen3Answer>()
                .ForMember(d => d.Trip, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.Answer, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.Answer))
                ;

            CreateMap<Gen3ViewModel.Note, Gen3Detail>()
                .ForMember(d => d.Trip, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.DetailDate, o => o.MapFrom(s => s.Date))
                ;


            CreateMap<Gen3Answer, Gen3ViewModel.Incident>()
                .ForMember(d => d.Answer, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.Answer))
                .ForMember(d => d._destroy, o => o.Ignore())
                ;

            CreateMap<Gen3Detail, Gen3ViewModel.Note>()
                .ForMember(d => d.Date, o => o.MapFrom(s => s.DetailDate))
                .ForMember(d => d._destroy, o => o.Ignore())
                ;

            CreateMap<Trip, Gen3ViewModel>()
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => s.SpcTripNumber))
                .ForMember(d => d.Incidents, o => o.MapFrom(s => s.Gen3Answers))
                .ForMember(d => d.Notes, o => o.MapFrom(s => s.Gen3Details))
                .ForMember(d => d.BooleanValues, o => o.Ignore())
                .ForMember(d => d.IncidentByCode, o => o.Ignore()) // AfterMap
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.MonitorId, o => o.UseValue(0))
                .AfterMap((s,d) => {
                    d.IndexIncidents();
                })
                ;

        }
    }
}
