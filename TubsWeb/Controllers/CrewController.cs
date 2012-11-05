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
    using AutoMapper;
    using NHibernate;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models.ExtensionMethods;
    using TubsWeb.ViewModels;

    /// <summary>
    /// 
    /// </summary>
    public class CrewController : SuperController
    {
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var cvm = Mapper.Map<PurseSeineTrip, CrewViewModel>(trip);
            string formatString = 
                IsEdit() ? 
                    "Edit crew list for {0}" :
                    "Crew list for {0}";

            ViewBag.Title = String.Format(formatString, tripId.ToString());

            if (IsApiRequest())
                return GettableJsonNetData(cvm);

            return View(CurrentAction(), cvm);           
        }

        /*
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
        */

        //
        // GET: /Crew/
        public ActionResult Index(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        [EditorAuthorize]
        public ActionResult Edit(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        // This should only ever be hit by the Knockout function, but we'll still probably want
        // to cater for straight up HTML Forms
        // This is the first test of the manual transaction attribute.
        // The transaction is committed in the middle so that we can be sure
        // that what's sent to the client is what's in the database.
        [HttpPost]
        [HandleTransactionManually]
        [EditorAuthorize]
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

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<Crew> repo = TubsDataService.GetRepository<Crew>(MvcApplication.CurrentSession);
                // Deletes first
                cvm.Deleted.ToList().ForEach(c => repo.DeleteById(c.Id));

                // AsCrewList strips out any crew marked _destroy or that don't have details
                var crewlist = cvm.AsCrewList();

                crewlist.ToList().ForEach(c =>
                {
                    c.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    c.Trip = tripId;
                });

                repo.Add(crewlist);
                // Flush to database
                xa.Commit();
            }
            if (IsApiRequest())
            {
                using (var repo = TubsDataService.GetRepository<Trip>(false))
                {
                    var trip = repo.FindById(tripId.Id) as PurseSeineTrip;
                    cvm = Mapper.Map<PurseSeineTrip, CrewViewModel>(trip);
                }
                return GettableJsonNetData(cvm);
            }
            else
            {
                return RedirectToAction("Index", "Crew", new { tripId = tripId.Id });
            }
        }

        // http://xhalent.wordpress.com/2011/02/05/using-unobtrusive-ajax-forms-in-asp-net-mvc3/
    }
}
