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
    using System.Web.Mvc;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models.ExtensionMethods;
    using TubsWeb.ViewModels;

    /// <summary>
    /// 
    /// </summary>
    public class CrewController : SuperController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            var cvm = Mapper.Map<PurseSeineTrip, CrewViewModel>(tripId as PurseSeineTrip);
            string formatString = 
                IsEdit() ? 
                    "{0}: Edit Crew" :
                    "{0}: Crew";

            ViewBag.Title = String.Format(formatString, tripId.SpcTripNumber);

            if (IsApiRequest())
                return GettableJsonNetData(cvm);

            return View(CurrentAction(), cvm);           
        }

        /// <summary>
        /// MVC Action for displaying crew associated with a purse seine trip.
        /// </summary>
        /// <example>
        /// GET /Trip/{tripId}/Crew
        /// </example>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter(TripType=typeof(PurseSeineTrip))]
        public ActionResult Index(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        /// <summary>
        /// MVC Action for displaying the crew edit form.
        /// </summary>
        /// <example>
        /// GET /Trip/{tripId}/Crew/Edit
        /// </example>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [EditorAuthorize]
        [ValidTripFilter(TripType=typeof(PurseSeineTrip))]
        public ActionResult Edit(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        /// <summary>
        /// MVC Action for updating crew list for a purse seine trip.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="cvm">ViewModel representing all crewmembers.</param>
        /// <returns></returns>
        [HttpPost]        
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, CrewViewModel cvm)
        {
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
                cvm.Deleted.ToList().ForEach(id => repo.DeleteById(id));

                // AsCrewList strips out any crew marked _destroy or that don't have details
                var crewlist = cvm.AsCrewList();

                crewlist.ToList().ForEach(c =>
                {
                    c.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    c.Trip = tripId;
                    repo.Save(c);
                });

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

            return RedirectToAction("Index", "Crew", new { tripId = tripId.Id });
        }
    }
}
