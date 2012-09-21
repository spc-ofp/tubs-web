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
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using NHibernate;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models;
    using TubsWeb.Models.ExtensionMethods;

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
            return PartialView(partialName, cmm);
        }

        /// <summary>
        /// Find the first crewmember with the given job in the list of all crew for the trip.
        /// If there is no crewmember, create a new, empty object.
        /// </summary>
        /// <param name="crewlist"></param>
        /// <param name="jobType"></param>
        /// <returns></returns>
        internal static CrewViewModel.CrewMemberModel GetCrewmember(IQueryable<Crew> crewlist, JobType jobType)
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
                    Years = c.YearsExperience
                }
            ).FirstOrDefault<CrewViewModel.CrewMemberModel>() ?? new CrewViewModel.CrewMemberModel(jobType);
        }

        internal static IEnumerable<CrewViewModel.CrewMemberModel> GetDeckHands(IQueryable<Crew> crew)
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
                    Years = c.YearsExperience
                };
        }

        internal static CrewViewModel Fill(IStatelessSession session, int tripId)
        {
            CrewViewModel cvm = new CrewViewModel();
            cvm.TripId = tripId;
            // Without a Trip, we can't meaningfully set TripNumber
            cvm.TripNumber = "This Trip";
            var crewlist = TubsDataService.GetRepository<Crew>(session).FilterBy(c => c.Trip.Id == tripId);
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

        private CrewViewModel Fill(Trip tripId)
        {            
            CrewViewModel cvm = Fill(MvcApplication.CurrentStatelessSession, tripId.Id);
            cvm.TripNumber = tripId.SpcTripNumber ?? "This Trip";
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
            var cvm = Fill(tripId);
            
            if (IsApiRequest())
                return GettableJsonNetData(cvm);
            return View(cvm);
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }
            ViewBag.Title = String.Format("Edit crew list for {0}", tripId.ToString());
            return View(Fill(tripId));
        }

        // This should only ever be hit by the Knockout function, but we'll still probably want
        // to cater for straight up HTML Forms
        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId, CrewViewModel cvm)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }
            if (!ModelState.IsValid)
            {
                if (IsApiRequest())
                {
                    return ModelErrorsResponse();
                }
                ViewBag.Title = String.Format("Edit crew list for {0}", tripId.ToString());
                return View(cvm);
            }

            // Two approaches:  We can save what we can save, or we do this as an all or nothing
            // deal.  For now, we'll do all or nothing...
            var crewlist = cvm.AsCrewList(); // AsCrewList strips out any crew without any details
            // Set the Trip relationship for each crewmember
            crewlist.ToList().ForEach(c => c.Trip = tripId);
            IRepository<Crew> repo = TubsDataService.GetRepository<Crew>(MvcApplication.CurrentSession);                
            repo.Add(crewlist);
            // TODO:  If there are crew that aren't in the list, then those crew members need to be deleted
            if (IsApiRequest())
            {
                Response.StatusCode = (int)HttpStatusCode.NoContent;
                return null;
            }
            else
            {
                return RedirectToAction("Index", "Crew", new { tripId = tripId.Id });
            }
        }

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
