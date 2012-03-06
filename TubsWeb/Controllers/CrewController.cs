// -----------------------------------------------------------------------
// <copyright file="CrewController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

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
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models;
    using TubsWeb.Models.ExtensionMethods;
    using System.Collections.Generic;

    public class CrewController : SuperController
    {
        // This is a strong linkage to the template name.  We can't use the default template engine behavior
        // since there are two templates with this name -- one for display, and one for edit.
        // There _might_ be a fix by changing the view template from something that's shared in "DisplayTemplates"
        // into something that's hosted elsewhere.  That will mean having to provide the partial name where appropriate
        private const string PathToEditPartial = @"~/Views/Shared/EditorTemplates/CrewMemberModel.cshtml";

        private const string PathToDeckHandPartial = @"~/Views/Crew/DeckHand.cshtml";

        private CrewViewModel.CrewMemberModel SaveCrewmember(Trip tripId, CrewViewModel.CrewMemberModel cmm, out bool success)
        {
            success = false;
            if (ModelState.IsValid)
            {
                Crew crew = tripId.CreateCrew();
                if (null != crew)
                {
                    cmm.CopyTo(crew);
                    crew.Trip = tripId;
                    var repo = new TubsRepository<Crew>(MvcApplication.CurrentSession);
                    if (default(int) == crew.Id)
                    {
                        crew.EnteredBy = User.Identity.Name;
                        crew.EnteredDate = DateTime.Now;
                        repo.Add(crew);
                        cmm.Id = crew.Id;
                        success = true;
                    }
                    else
                    {
                        repo.Update(crew, true);
                        success = true;
                    }
                }
            }
            return cmm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="cmm"></param>
        /// <param name="partialName"></param>
        /// <returns></returns>
        private PartialViewResult ModifyCrewmember(Trip tripId, CrewViewModel.CrewMemberModel cmm, string partialName)
        {
            // Not the best way to handle this, but how often will it happen?
            if (null == tripId)
            {
                return PartialView(partialName, cmm);
            }

            bool modelSaved = false;
            cmm = SaveCrewmember(tripId, cmm, out modelSaved);
            ViewData["modelSaved"] = modelSaved;
            return PartialView(partialName, cmm);

            /*
            // Ideally, we'd pass a status message back to the
            // UI.  Unfortunately, jQuery and MVC3 are having issues
            // with escaping data correctly
            bool modelSaved = false;
            if (ModelState.IsValid)
            {
                Crew crew = tripId.CreateCrew();
                if (null != crew)
                {
                    cmm.CopyTo(crew);
                    crew.Trip = tripId;
                    var repo = new TubsRepository<Crew>(MvcApplication.CurrentSession);
                    if (default(int) == crew.Id)
                    {
                        crew.EnteredBy = User.Identity.Name;
                        crew.EnteredDate = DateTime.Now;
                        repo.Add(crew);
                        // Does this work?
                        cmm.Id = crew.Id;
                        modelSaved = true;
                    }
                    else
                    {
                        repo.Update(crew, true);
                        modelSaved = true;
                    }
                }
            }

            ViewData["modelSaved"] = modelSaved;
            return PartialView(partialName, cmm);
            */
        }

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

        /// <summary>
        /// Find the first crewmember with the given job in the list of all crew for the trip.
        /// If there is no crewmember, create a new, empty object.
        /// </summary>
        /// <param name="crewlist"></param>
        /// <param name="jobType"></param>
        /// <returns></returns>
        private static CrewViewModel.CrewMemberModel GetCrewmember(IQueryable<Crew> crewlist, JobType jobType)
        {
            return (
                from c in crewlist
                where c.Job.HasValue && jobType == c.Job.Value
                select new CrewViewModel.CrewMemberModel
                {
                    Id = c.Id,
                    Job = c.Job,
                    Name = c.Name,
                    Nationality = c.CountryCode,
                    Comments = c.Comments,
                    Years = c.YearsExperience,
                    Months = c.MonthsExperience,
                    Experience = Experience(c.YearsExperience, c.MonthsExperience)
                }
            ).FirstOrDefault<CrewViewModel.CrewMemberModel>() ?? new CrewViewModel.CrewMemberModel(jobType);
        }

        private static IEnumerable<CrewViewModel.CrewMemberModel> GetDeckHands(IQueryable<Crew> crew)
        {
            return 
                from c in crew
                where c.Job.HasValue && JobType.Crew == c.Job.Value
                select new CrewViewModel.CrewMemberModel
                {
                    Id = c.Id,
                    Job = JobType.Crew,
                    Name = c.Name,
                    Nationality = c.CountryCode,
                    Comments = c.Comments,
                    Years = c.YearsExperience,
                    Months = c.MonthsExperience,
                    Experience = Experience(c.YearsExperience, c.MonthsExperience)
                };
        }

        private CrewViewModel Fill(int tripId)
        {
            CrewViewModel cvm = new CrewViewModel();
            cvm.TripId = tripId;
            var crewlist = new TubsRepository<Crew>(MvcApplication.CurrentSession).FilterBy(c => c.Trip.Id == tripId);
            cvm.Hands.AddRange(GetDeckHands(crewlist));

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
            return cvm;
        }

        //
        // GET: /Crew/
        public ActionResult Index(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }
            ViewBag.Title = String.Format("Crew list for {0}", tripId.ToString());
            return View(Fill(tripId.Id));
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }
            ViewBag.Title = String.Format("Edit crew list for {0}", tripId.ToString());
            return View(Fill(tripId.Id));
        }

        /*
        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId, CrewViewModel cvm)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }
            
            // Convert CrewViewModel back into list of crew for trip

            // Add/Update as appropriate
            return View(cvm);
        }
        */

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public PartialViewResult EditSingleDeckhand(Trip tripId, CrewViewModel.CrewMemberModel cmm)
        {
            return ModifyCrewmember(tripId, cmm, PathToDeckHandPartial);
        }

        // TODO At some point in the future, change this from the ViewModel directly to the DAL model.
        // However, at this point, it's a PITA because it requires the AbstractBind attribute, which
        // then requires a hidden attribute with the concrete type, which means that the concrete
        // type needs to be fed into ViewModel.  Yeesh!
        //
        // Hint to get started on this came from here:
        // http://xhalent.wordpress.com/2011/02/05/using-unobtrusive-ajax-forms-in-asp-net-mvc3/
        //
        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public PartialViewResult EditSingle(Trip tripId, CrewViewModel.CrewMemberModel cmm)
        {
            return ModifyCrewmember(tripId, cmm, PathToEditPartial);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public PartialViewResult AddDeckHand(Trip tripId, [Bind(Prefix = "hand")] CrewViewModel.CrewMemberModel cmm)
        {
            bool modelSaved = false;
            SaveCrewmember(tripId, cmm, out modelSaved);
            var crewlist = new TubsRepository<Crew>(MvcApplication.CurrentSession).FilterBy(c => c.Trip.Id == tripId.Id);
            var hands = GetDeckHands(crewlist);
            return PartialView("OtherCrew", hands);
        }

    }
}
