// <copyright file="CrewController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
namespace TubsWeb.Controllers
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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Models;
    using System.Text;
    using System.ComponentModel;

    public class CrewController : SuperController
    {

        private static string Experience(int? years, int? months)
        {
            if (!years.HasValue && !months.HasValue)
            {
                return "None or unknown";
            }
            StringBuilder sb = new StringBuilder();
            if (years.HasValue)
            {
                sb.Append(years.Value).Append(" years ");
            }
            if (months.HasValue)
            {
                if (sb.Length > 0)
                {
                    sb.Append("and ");
                }
                sb.Append(months.Value).Append(" months");
            }
            return sb.ToString();
        }

        private static CrewViewModel.CrewMemberModel GetCrewmember(IQueryable<Crew> crewlist, JobType jobType)
        {
            return (
                from c in crewlist
                where c.Job.HasValue && jobType == c.Job.Value
                select new CrewViewModel.CrewMemberModel
                {
                    Name = c.Name,
                    Nationality = c.CountryCode,
                    Comments = c.Comments,
                    Experience = Experience(c.YearsExperience, c.MonthsExperience)
                }
            ).FirstOrDefault<CrewViewModel.CrewMemberModel>();
        }

        //
        // GET: /Crew/
        public ActionResult Index(int id)
        {
            var repo = new TubsRepository<Crew>(MvcApplication.CurrentSession);
            var crewlist = repo.FilterBy(c => c.Trip.Id == id);
            CrewViewModel cvm = new CrewViewModel();
            cvm.TripId = id;
            var hands =
                from c in crewlist
                where c.Job.HasValue && JobType.Crew == c.Job.Value
                select new CrewViewModel.CrewMemberModel 
                { 
                    Name = c.Name, 
                    Nationality = c.CountryCode, 
                    Comments = c.Comments, 
                    Experience = Experience(c.YearsExperience, c.MonthsExperience)
                };
            cvm.Hands.AddRange(hands);
            
            // Get named crew members
            cvm.Captain = GetCrewmember(crewlist, JobType.Captain);
            cvm.Navigator = GetCrewmember(crewlist, JobType.NavigatorOrMaster);
            cvm.Mate = GetCrewmember(crewlist, JobType.Mate);
            cvm.ChiefEngineer = GetCrewmember(crewlist, JobType.ChiefEngineer);
            cvm.AssistantEngineer = GetCrewmember(crewlist, JobType.AssistantEngineer);
            cvm.DeckBoss = GetCrewmember(crewlist, JobType.DeckBoss);
            cvm.Cook = GetCrewmember(crewlist, JobType.Cook);
            cvm.HelicopterPilot = GetCrewmember(crewlist, JobType.HelicopterPilot);
            cvm.SkiffMan = GetCrewmember(crewlist, JobType.SkiffMan);
            cvm.WinchMan = GetCrewmember(crewlist, JobType.WinchMan);
            return View(cvm);
        }

    }
}
