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
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;

    public class FishingSetController : SuperController
    {

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
        
        //
        // GET: /FishingSet/
        public ActionResult Index(Trip tripId, int setNumber)
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

    }
}
