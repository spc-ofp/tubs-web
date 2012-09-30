// -----------------------------------------------------------------------
// <copyright file="FishingSetController.cs" company="Secretariat of the Pacific Community">
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
    using TubsWeb.Models.ExtensionMethods;

    public class FishingSetController : SuperController
    {
        internal Tuple<bool, RouteValueDictionary> NeedsRedirect(int tripId, int setNumber, int maxSets)
        {
            var needsRedirect = false;
            // Fill values that don't change
            var rvd = new RouteValueDictionary(
                new { controller = "FishingSet", tripId = tripId }
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
                else if (setNumber > (setNumber + 1))
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
        
        /// <summary>
        /// Check to see if this is the "Add" action
        /// </summary>
        /// <returns>true if this is the "Add" action, false otherwise</returns>
        internal bool IsAdd()
        {
            return "Add".Equals(CurrentAction(), StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check to see if this is the "Edit" action
        /// </summary>
        /// <returns>true if this is the "Edit" action, false otherwise</returns>
        internal bool IsEdit()
        {
            return "Edit".Equals(CurrentAction(), StringComparison.InvariantCultureIgnoreCase);
        }

        internal ActionResult ViewActionImpl(Trip tripId, int setNumber)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var sets = TubsDataService.GetRepository<PurseSeineSet>(
                MvcApplication.CurrentSession).FilterBy(
                    s => s.Activity.Day.Trip.Id == tripId.Id);

            // This LINQ method results in a trip to the database
            // (select count(1) from ... where obstrip_id = ?)
            // It's also worth mentioning that if an Add/Edit has
            // a 'bad' setNumber param, we'll be running this query twice
            // I expect SQL Server to be able to handle that...
            int maxSets = sets.Count();
            var checkpoint = NeedsRedirect(tripId.Id, setNumber, maxSets);
            if (checkpoint.Item1)
                return new RedirectToRouteResult(checkpoint.Item2);

            // Based on NeedsRedirect, we should be okay -- the
            // setNumber should be perfect for the action
            var fset = (
                from s in sets
                where s.SetNumber == setNumber
                select s).FirstOrDefault();
            var fsvm = fset.AsViewModel();

            // Fill in what the extension method couldn't/didn't
            fsvm.MaxSets = maxSets;
            fsvm.NextSet = setNumber + 1;
            fsvm.PreviousSet = setNumber - 1;
            fsvm.HasNext = setNumber < maxSets;
            fsvm.HasPrevious = setNumber > 1;

            fsvm.TripId = trip.Id;
            fsvm.TripNumber = (trip.SpcTripNumber ?? "This Trip").Trim();
            fsvm.VersionNumber = trip.Version == WorkbookVersion.v2009 ? 2009 : 2007;

            if (IsApiRequest())
                return GettableJsonNetData(fsvm);

            return View(CurrentAction(), fsvm);
        }


        public ActionResult List(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ViewBag.Title = String.Format("Sets for {0}", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";

            var repo = new TubsRepository<PurseSeineSet>(MvcApplication.CurrentSession);
            // Push the projection into a List so that it's not the NHibernate collection implementation
            var sets = repo.FilterBy(s => s.Activity.Day.Trip.Id == tripId.Id).ToList<PurseSeineSet>();
            sets.Sort(delegate(PurseSeineSet s1, PurseSeineSet s2)
            {
                return Comparer<int?>.Default.Compare(s1.SetNumber, s2.SetNumber);
            });
            return View(sets);
        }

        public ActionResult Edit(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

        public ActionResult Add(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

        public ActionResult Index(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }
        
        //
        // GET: /FishingSet/
        public ActionResult IndexEx(Trip tripId, int setNumber)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            var repo = new TubsRepository<PurseSeineSet>(MvcApplication.CurrentSession);
            var sets = repo.FilterBy(s => s.Activity.Day.Trip.Id == tripId.Id);
            int maxSets = sets.Count();
            if (setNumber > maxSets)
            {
                setNumber = maxSets;
            }
            ViewBag.MaxSets = maxSets;
            ViewBag.CurrentSet = setNumber;
            // TODO Add trip number to title?
            ViewBag.Title = String.Format("Set {0} of {1} for {2}", setNumber, maxSets, tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            // This depends on set number!
            var set = (
                from s in sets
                where s.SetNumber == setNumber
                select s).FirstOrDefault();
            return View(set);
        }

        /*
         * http://stackoverflow.com/questions/10472111/graphael-charts-with-dynamic-data-from-database-using-asp-net-mvc3
         * Probably have to use HighCharts as Raphael just doesn't have any documentation
         */

        public JsonResult LengthFrequency(int id, string speciesCode)
        {
            var repo = new TubsRepository<PurseSeineSet>(MvcApplication.CurrentSession);
            var fset = repo.FindBy(id);

            var returnValue = (
                from x in Enumerable.Range(10, 191)
                select new int[]{ x, 0 }
            ).ToList();

            // Return a list with lengths but zero counts if no data
            if (null == fset || null == fset.SamplingHeaders || 0 == fset.SamplingHeaders.Count)
                return GettableJsonNetData(returnValue);         

            var lengths =
                from h in fset.SamplingHeaders
                from d in h.Samples
                where d.Length.HasValue && d.SpeciesCode == speciesCode
                select d.Length.Value;

            var grouped =
                lengths.GroupBy(i => i).Select(i => new int[]{ i.Key, i.Count() });

            foreach (var kvp in grouped.ToList())
            {
                returnValue[kvp[0]] = kvp;
            }

            return GettableJsonNetData(returnValue);
        }

    }
}
