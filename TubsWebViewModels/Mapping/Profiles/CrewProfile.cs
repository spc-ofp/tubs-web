// -----------------------------------------------------------------------
// <copyright file="CrewProfile.cs" company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;

    /// <summary>
    /// AutoMapper profile for the conversion of Purse Seine crew list entity to/from
    /// MVC ViewModel.
    /// </summary>
    public class CrewProfile : Profile
    {

        protected override void Configure()
        {
            base.Configure();

            // ViewModel item to Entity
            CreateMap<CrewViewModel.CrewMemberModel, PurseSeineCrew>()
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.Trip, o => o.Ignore())
                .ForMember(d => d.MonthsExperience, o => o.Ignore())
                .ForMember(d => d.CountryCode, o => o.MapFrom(s => s.Nationality))
                .ForMember(d => d.YearsExperience, o => o.MapFrom(s => s.Years))
                ;

            // Entity to ViewModel
            CreateMap<PurseSeineCrew, CrewViewModel.CrewMemberModel>()
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.Nationality, o => o.MapFrom(s => s.CountryCode))
                .ForMember(d => d.Years, o => o.MapFrom(s => s.YearsExperience))
                ;
            
            CreateMap<PurseSeineTrip, CrewViewModel>()
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.Captain, o => o.MapFrom(s => s.Crew.Where(c => JobType.Captain == c.Job).FirstOrDefault()))
                .ForMember(d => d.Navigator, o => o.MapFrom(s => s.Crew.Where(c => JobType.NavigatorOrMaster == c.Job).FirstOrDefault()))
                .ForMember(d => d.Mate, o => o.MapFrom(s => s.Crew.Where(c => JobType.Mate == c.Job).FirstOrDefault()))
                .ForMember(d => d.ChiefEngineer, o => o.MapFrom(s => s.Crew.Where(c => JobType.ChiefEngineer == c.Job).FirstOrDefault()))
                .ForMember(d => d.AssistantEngineer, o => o.MapFrom(s => s.Crew.Where(c => JobType.AssistantEngineer == c.Job).FirstOrDefault()))
                .ForMember(d => d.DeckBoss, o => o.MapFrom(s => s.Crew.Where(c => JobType.DeckBoss == c.Job).FirstOrDefault()))
                .ForMember(d => d.Cook, o => o.MapFrom(s => s.Crew.Where(c => JobType.Cook == c.Job).FirstOrDefault()))
                .ForMember(d => d.HelicopterPilot, o => o.MapFrom(s => s.Crew.Where(c => JobType.HelicopterPilot == c.Job).FirstOrDefault()))
                .ForMember(d => d.SkiffMan, o => o.MapFrom(s => s.Crew.Where(c => JobType.SkiffMan == c.Job).FirstOrDefault()))
                .ForMember(d => d.WinchMan, o => o.MapFrom(s => s.Crew.Where(c => JobType.WinchMan == c.Job).FirstOrDefault()))
                .ForMember(d => d.Hands, o => o.MapFrom(s => s.Crew.Where(c => JobType.Crew == c.Job)))
                .AfterMap((s,d) => 
                {
                    // Knockout doesn't like null objects
                    if (null == d.Captain)
                        d.Captain = new CrewViewModel.CrewMemberModel(JobType.Captain);
                    if (null == d.Navigator)
                        d.Navigator = new CrewViewModel.CrewMemberModel(JobType.NavigatorOrMaster);
                    if (null == d.Mate)
                        d.Mate = new CrewViewModel.CrewMemberModel(JobType.Mate);
                    if (null == d.ChiefEngineer)
                        d.ChiefEngineer = new CrewViewModel.CrewMemberModel(JobType.ChiefEngineer);
                    if (null == d.AssistantEngineer)
                        d.AssistantEngineer = new CrewViewModel.CrewMemberModel(JobType.AssistantEngineer);
                    if (null == d.DeckBoss)
                        d.DeckBoss = new CrewViewModel.CrewMemberModel(JobType.DeckBoss);
                    if (null == d.Cook)
                        d.Cook = new CrewViewModel.CrewMemberModel(JobType.Cook);
                    if (null == d.HelicopterPilot)
                        d.HelicopterPilot = new CrewViewModel.CrewMemberModel(JobType.HelicopterPilot);
                    if (null == d.SkiffMan)
                        d.SkiffMan = new CrewViewModel.CrewMemberModel(JobType.SkiffMan);
                    if (null == d.WinchMan)
                        d.WinchMan = new CrewViewModel.CrewMemberModel(JobType.WinchMan);
                })
                ;
        }
    }
}
