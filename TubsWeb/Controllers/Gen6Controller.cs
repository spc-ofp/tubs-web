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
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Models;
    
    public class Gen6Controller : SuperController
    {
        //
        // GET: /Gen6/
        public ActionResult Index(int id, int pageNumber)
        {
            ViewBag.TripId = id;
            var repo = new TubsRepository<PollutionEvent>(MvcApplication.CurrentSession);
            var events = repo.FilterBy(e => e.Trip.Id == id);
            int maxPages = events.Count();
            if (pageNumber > maxPages)
            {
                pageNumber = maxPages;
            }
            ViewBag.MaxPages = maxPages;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.Title = String.Format("GEN-6 page {0} of {1}", pageNumber, maxPages);
            var pollutionEvent = events.Skip(pageNumber - 1).Take(1).FirstOrDefault();
            return View(pollutionEvent);
        }

        public ActionResult List(int id)
        {
            ViewBag.TripId = id;
            var repo = new TubsRepository<Trip>(MvcApplication.CurrentSession);
            var trip = repo.FindBy(id);
            IList<PollutionEvent> events =  null == trip ? 
                new List<PollutionEvent>() : 
                trip.PollutionEvents.ToList(); // Push into a "real" list instead of an NHibernate/Iesi collection

            if (null != trip)
            {
                ViewBag.Title = String.Format("GEN-6 events for trip {0}", trip.ToString());
            }
            return View(events);
        }

    }
}
