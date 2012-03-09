// -----------------------------------------------------------------------
// <copyright file="Gen2Controller.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    
    public class Gen2Controller : SuperController
    {
        private ActionResult Load(Trip tripId, int pageNumber, string titleFormat, bool createNew = false)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            int maxPages = tripId.Interactions.Count;
            if (pageNumber > maxPages)
            {
                pageNumber = maxPages;
            }

            ViewBag.MaxPages = maxPages;
            ViewBag.CurrentPage = pageNumber;
            var interaction = tripId.Interactions.Skip(pageNumber - 1).Take(1).FirstOrDefault();

            ViewBag.Title = String.Format(titleFormat, pageNumber, maxPages, tripId.ToString());
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            AddMinMaxDates(tripId);
            return
                createNew ?
                    View(actionName, interaction ?? new SpecialSpeciesInteraction()) :
                    View(actionName, interaction);
        }
        
        //
        // GET: /Gen2/
        public ActionResult List(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format("GEN-2 events for trip {0}", tripId.ToString());
            AddMinMaxDates(tripId);
            return View(tripId);
        }

        // GET: /Trip/{tripId}/GEN-2/{pageNumber}
        public ActionResult Index(Trip tripId, int pageNumber)
        {
            return Load(tripId, pageNumber, "GEN-2 Page {0} of {1} for {2}");
        }

        public ActionResult Edit(Trip tripId, int pageNumber)
        {
            return Load(tripId, pageNumber, "Edit GEN-2 Page {0} of {1} for {2}", true);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public PartialViewResult AddInteraction(Trip tripId, SpecialSpeciesInteraction interaction)
        {
            var repo = new TubsRepository<SpecialSpeciesInteraction>(MvcApplication.CurrentSession);
            // TODO Check that interaction date between trip start/end dates

            if (ModelState.IsValid)
            {
                // Fix for something that MVC is doing to/for us.
                if (!"S".Equals(interaction.SgType, StringComparison.InvariantCultureIgnoreCase))
                {
                    interaction.InteractionActivity = null;
                }
                if (!"L".Equals(interaction.SgType, StringComparison.InvariantCultureIgnoreCase))
                {
                    interaction.InteractionId = null;
                }
                interaction.Trip = tripId;
                interaction.EnteredBy = User.Identity.Name;
                interaction.EnteredDate = DateTime.Now;
                repo.Add(interaction);
            }

            var interactions = repo.FilterBy(t => t.Trip.Id == tripId.Id);
            return PartialView("_Interactions", interactions);
        }

    }
}
