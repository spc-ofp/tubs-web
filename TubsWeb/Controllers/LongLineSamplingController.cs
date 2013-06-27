// -----------------------------------------------------------------------
// <copyright file="SamplingController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
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
    using System.Web.Routing;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models;
    using TubsWeb.ViewModels;

    /// <summary>
    /// 
    /// </summary>
    public class LongLineSamplingController : SamplingController
    {

        // TODO: This needs to be fixed for the "missing set" problem that comes up
        // when this data is published to users that don't have viewing privileges
        // for the entire trip
        
        internal override ActionResult ViewActionImpl(Trip tripId, int setNumber)
        {
            // This should never happen, but a little defensive coding goes a long way
            var trip = tripId as LongLineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var sets =
                TubsDataService.GetRepository<LongLineSet>(MvcApplication.CurrentSession)
                    .FilterBy(s => s.Trip.Id == trip.Id);

            // If I was just a _bit_ smarter, I could probably figure out how to extract the
            // following 10 lines into something that can be used between both PS and LL
            // data.


            // This LINQ method results in a trip to the database
            // (select count(1) from ... where obstrip_id = ?)
            // It's also worth mentioning that if an Add/Edit has
            // a 'bad' setNumber param, we'll be running this query twice
            // I expect SQL Server to be able to handle that...
            int maxSets = sets.Count();
            var checkpoint = NeedsRedirect(trip.Id, setNumber, maxSets);
            if (checkpoint.Item1)
                return new RedirectToRouteResult(checkpoint.Item2);

            // One minor point.  If the user passes in a completely crazy setNumber for Index
            // we'll re-interpret based on intent.
            if (IsIndex())
            {
                if (setNumber < 1) { setNumber = 1; }
                if (setNumber > maxSets) { setNumber = maxSets; }
            }

            // Based on NeedsRedirect, we should be okay -- the
            // setNumber should be perfect for the action
            var fset = (
                from s in sets
                where s.SetNumber == setNumber
                select s).FirstOrDefault();

            // This is where it gets tricky.  LL doesn't have a "header" object for all bio samples
            // in a set -- it's just a list of entities hanging off the set.  AutoMapper doesn't work
            // very well in that situation.
            var header = new LongLineCatchHeader();
            header.FishingSet = fset;
            header.Samples = fset.CatchList;

            // TODO Replace with AutoMapper from proxy
            var svm = new LongLineSampleViewModel();

            if (IsApiRequest())
                return GettableJsonNetData(svm);


            return View(CurrentAction(), svm);
        }

        public ActionResult List(Trip tripId)
        {
            return View();
        }

    }
}
