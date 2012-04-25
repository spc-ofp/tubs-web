// -----------------------------------------------------------------------
// <copyright file="Gen6Controller.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    
    public class Gen6Controller : SuperController
    {
        private PollutionEvent FindByTripAndPageNumber(Trip tripId, int pageNumber)
        {
            var events = tripId.PollutionEvents;
            int maxPages = events.Count();
            if (pageNumber > maxPages)
            {
                pageNumber = maxPages;
            }

            ViewBag.MaxPages = maxPages;
            ViewBag.CurrentPage = pageNumber;
            return events.Skip(pageNumber - 1).Take(1).FirstOrDefault();
        }
        
        
        //
        // GET: /Gen6/
        public ActionResult Index(Trip tripId, int pageNumber)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            var pollutionEvent = FindByTripAndPageNumber(tripId, pageNumber);
            ViewBag.Title = String.Format("GEN-6 page {0} of {1}", ViewBag.CurrentPage, ViewBag.MaxPages);
            return View(pollutionEvent);
        }

        public ActionResult List(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format("GEN-6 events for trip {0}", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";

            return View(tripId.PollutionEvents ?? new List<PollutionEvent>());
        }

        // The following methods are for an investigation of this method of Ajax/Modal dialog
        // http://xhalent.wordpress.com/2011/05/25/master-details-with-dialog-in-asp-net-mvc-and-unobstrusive-ajax/
        // On further reflection, GEN-6 isn't a great candidate for this

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Add(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            AddMinMaxDates(tripId);
            ViewBag.Title = String.Format("Edit GEN-6 event for trip {0}", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            var pevent = new PollutionEvent();
            pevent.Trip = tripId;
            return View(pevent);
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId, int pageNumber)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            AddMinMaxDates(tripId);
            ViewBag.Title = String.Format("Edit GEN-6 page {1} for trip {0}", tripId.ToString(), pageNumber);
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            var pollutionEvent = FindByTripAndPageNumber(tripId, pageNumber);
            return View(pollutionEvent);
        }

    }
}
