// -----------------------------------------------------------------------
// <copyright file="SeaDayController.cs" company="Secretariat of the Pacific Community">
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

    public class SeaDayController : SuperController
    {
        //
        // GET: /SeaDay/
        public ActionResult List(int tripId)
        {
            // Enable link back to trip without a ViewModel
            ViewBag.TripId = tripId;

            var repo = new TubsRepository<SeaDay>(MvcApplication.CurrentSession);
            
            // Push the projection into a List so that it's not the NHibernate collection implementation
            var days = repo.FilterBy(d => d.Trip.Id == tripId).ToList<SeaDay>();
            if (null == days || days.Count < 1)
            {
                ViewBag.Title = String.Format("Sea days for tripId {0}", tripId);
            }
            else
            {
                var trip = days.First<SeaDay>().Trip;
                ViewBag.Title = String.Format("Sea days for {0}", trip.ToString());
            }
            return View(days);
        }

        public ActionResult Index(int tripId, int dayNumber)
        {
            // Enable link back to trip without a ViewModel
            ViewBag.TripId = tripId;

            var repo = new TubsRepository<SeaDay>(MvcApplication.CurrentSession);

            var days = repo.FilterBy(d => d.Trip.Id == tripId);
            int maxDays = days.Count();
            if (dayNumber > maxDays)
            {
                dayNumber = maxDays;
            }
            ViewBag.MaxDays = maxDays;
            ViewBag.CurrentDay = dayNumber;
            ViewBag.Title = String.Format("Sea day {0} of {1}", dayNumber, maxDays);
            var day = days.Skip(dayNumber - 1).Take(1).FirstOrDefault();
            return View(day);
        }

        //
        // GET: /SeaDay/1
        public ActionResult IndexEx(int dayId)
        {
            var repo = new TubsRepository<SeaDay>(MvcApplication.CurrentSession);
            var day = repo.FindBy(dayId);
            if (null == day)
            {
                ViewBag.Title = String.Format("No sea day with Id {0}", dayId);
            }
            else
            {
                ViewBag.Title = String.Format("Sea day {0}", dayId); // FIXME Need a better title
            }
            return View(day);
        }

    }
}
