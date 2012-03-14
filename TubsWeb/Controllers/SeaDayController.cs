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
    using System.Linq;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models.ExtensionMethods;

    public class SeaDayController : SuperController
    {
        //
        // GET: /SeaDay/
        public ActionResult List(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }
            
            ViewBag.IsReadOnly = tripId.IsReadOnly;

            var repo = new TubsRepository<SeaDay>(MvcApplication.CurrentSession);            
            // Push the projection into a List so that it's not the NHibernate collection implementation
            var days = repo.FilterBy(d => d.Trip.Id == tripId.Id).ToList<SeaDay>();
            ViewBag.Title = String.Format("Sea days for {0}", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
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

        public ActionResult AutoFill(Trip tripId)
        {
            if (null == tripId)
            {
                // TODO Figure out if this is really how we want to handle this...
                return new NoSuchTripResult();
            }

            var repo = new TubsRepository<SeaDay>(MvcApplication.CurrentSession);
            if (repo.FilterBy(d => d.Trip.Id == tripId.Id).Count() > 0)
            {
                // Already has at least one day -- don't mess with it
                return JavaScript("alert('One or more days already present');");
            }

            // Figure out how many days are between departure and end date
            TimeSpan span = tripId.ReturnDate.Value.Subtract(tripId.DepartureDate.Value);
            int daysAdded = 0;
            for (int i = 0; i <= span.Days; i++)
            {
                DateTime startDate = tripId.DepartureDate.Value.AddDays(i);
                SeaDay seaDay = tripId.CreateSeaDay(startDate);
                var enteredDate = DateTime.Now;
                if (null != seaDay)
                {
                    seaDay.Trip = tripId;
                    seaDay.EnteredBy = User.Identity.Name;
                    seaDay.EnteredDate = enteredDate;
                    repo.Add(seaDay);
                    daysAdded++;
                }
            }

            return JavaScript(String.Format("alert('Added {0} day(s)');", daysAdded));
        }

    }
}
