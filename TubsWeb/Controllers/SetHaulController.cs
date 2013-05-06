// -----------------------------------------------------------------------
// <copyright file="SetHaulController.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Routing;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models;
    using TubsWeb.Models.ExtensionMethods;

    /// <summary>
    /// SetHaulController manages LL-2/3 data.
    /// As it's for LL-2/3, it only works for Long Line trips
    /// </summary>
    public class SetHaulController : SuperController
    {

        /// <summary>
        /// NeedsRedirect validates the setNumber param for a trip based on the
        /// action and the number of sets already existing for this trip.
        /// 
        /// The logic is as follows:
        /// - Index:  Don't care -- Index will cap dayNumber @ maxDays
        /// - Add:  If dayNumber is a day that already exists, push to Edit.
        ///         If dayNumber isn't the appropriate value based on maxDays,
        ///         redirect to Add with the appropriate value
        /// - Edit: If dayNumber is past the end of the trip, redirect to edit
        ///         with the final dayNumber
        /// </summary>
        /// <param name="tripId">obstrip_id</param>
        /// <param name="setNumber">Index (one based) of set number for this trip</param>
        /// <param name="maxSets">Number of sets already added to this trip</param>
        /// <returns></returns>
        internal Tuple<bool, RouteValueDictionary> NeedsRedirect(int tripId, int setNumber, int maxSets)
        {
            var needsRedirect = false;
            // Fill values that don't change
            var rvd = new RouteValueDictionary(
                new { controller = "SetHaul", tripId = tripId }
            );

            if (IsAdd())
            {
                if (setNumber <= maxSets)
                {
                    // Redirect to edit
                    rvd["setNumber"] = setNumber;
                    rvd["action"] = "Edit";
                    needsRedirect = true;
                }
                else if (setNumber > (maxSets + 1))
                {
                    // Redirect to add, but with correct dayNumber
                    rvd["setNumber"] = (maxSets + 1);
                    rvd["action"] = "Add";
                    needsRedirect = true;
                }
            }
            else if (IsEdit())
            {
                if (setNumber > maxSets)
                {
                    rvd["setNumber"] = maxSets;
                    rvd["action"] = "Edit";
                    needsRedirect = true;
                }
            }

            return Tuple.Create(needsRedirect, rvd);
        }
        
        
        public ActionResult List(Trip tripId)
        {
            var trip = tripId as LongLineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var repo = TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession);
            var sets =
                TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession)
                    .FilterBy(s => s.Trip.Id == tripId.Id)
                    .OrderBy(s => s.SetNumber);

            ViewBag.Title = String.Format("Sets for {0}", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            return View(sets);
        }

        internal ActionResult ViewActionImpl(Trip tripId, int setNumber)
        {
            var trip = tripId as LongLineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var sets =
                TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession)
                    .FilterBy(s => s.Trip.Id == tripId.Id)
                    .OrderBy(s => s.SetNumber);

            // This LINQ method results in a trip to the database
            // (select count(1) from ... where obstrip_id = ?)
            // It's also worth mentioning that if an Add/Edit has
            // a 'bad' setNumber param, we'll be running this query twice
            // I expect SQL Server to be able to handle that...
            int maxSets = sets.Count();
            var checkpoint = NeedsRedirect(tripId.Id, setNumber, maxSets);
            if (checkpoint.Item1)
                return new RedirectToRouteResult(checkpoint.Item2);

            // One minor point.  If the user passes in a completely crazy dayNumber for Index
            // we'll re-interpret based on intent.
            if (IsIndex())
            {
                if (setNumber < 1) { setNumber = 1; }
                if (setNumber > maxSets) { setNumber = maxSets; }
            }

            // Based on NeedsRedirect, we should be okay -- the
            // setNumber should be perfect for the action
            var set = sets.Skip(setNumber - 1).Take(1).FirstOrDefault() as LongLineSet;

            return View(CurrentAction(), set);
        }

        public ActionResult Index(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

        [EditorAuthorize]
        public ActionResult Add(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

        [EditorAuthorize]
        public ActionResult Edit(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

    }
}
