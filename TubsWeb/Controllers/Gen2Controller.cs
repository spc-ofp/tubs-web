// -----------------------------------------------------------------------
// <copyright file="Gen2Controller.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Models;
    
    public class Gen2Controller : SuperController
    {        
        //
        // GET: /Gen2/
        public ActionResult List(int id)
        {
            ViewBag.TripId = id;
            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var trip = repo.FindBy(id);
            IList<SpecialSpeciesInteraction> interactions =
                null == trip ? new List<SpecialSpeciesInteraction>() : trip.Interactions;
            if (null != trip)
            {
                ViewBag.Title = String.Format("GEN-2 events for trip {0} / {1}", trip.Observer.StaffCode, trip.TripNumber);
                // NOTE:  If list of DAL domain objects is too unwieldy, this is where I'd fill in the
                // view model
            }
            return View(interactions);
        }

        // GET: /Trip/Details/{id}/Interaction/{interactionId}
        public ActionResult Details(int id, int interactionId)
        {
            ViewBag.TripId = id;
            var repo = new TubsRepository<SpecialSpeciesInteraction>(MvcApplication.CurrentSession);
            var interaction = repo.FindBy(interactionId);
            if (null == interaction || null == interaction.Trip || interaction.Trip.Id != id)
            {
                // TODO Work out what to do here
                return View("NotFound");
            }
            // TODO Not sure I like this title...
            ViewBag.Title = String.Format("Special Species Interaction Id {0}", interactionId);
            return View(interaction);
        }

    }
}
