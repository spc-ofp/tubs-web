// -----------------------------------------------------------------------
// <copyright file="Ps4Controller.cs" company="Secretariat of the Pacific Community">
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
    using TubsWeb.ViewModels;

    /// <summary>
    /// 
    /// </summary>
    public class Ps4Controller : AbstractSetController
    {
        /*
         * List of all PS-4 pages for a trip:
         * /Trip/{tripId}/PS-4/
         * 
         * List of all PS-4 pages for a set:
         * /Trip/{tripId}/PS-4/{setNumber}/List
         * 
         * Direct link to a PS-4 page
         * /Trip/{tripId}/PS-4/{setNumber}/{pageNumber}/Index
         * 
         * Edit link to a PS-4 page
         * /Trip/{tripId}/PS-4/{setNumber}/{pageNumber}/Edit
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter(TripType=typeof(PurseSeineTrip))]
        public ActionResult List(Trip tripId)
        {
            var vm = Mapper.Map<PurseSeineTrip, TripSamplingViewModel>(tripId as PurseSeineTrip);
            return View(vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="setNumber"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Index(Trip tripId, int setNumber, int pageNumber)
        {
            var trip = tripId as PurseSeineTrip;
            int maxSets = trip.FishingSets.Count();
            if (setNumber > maxSets)
            {
                // This isn't like set, where someone can use
                // setNumber = 999 to get to the last set.
                // For now, redirect to List, but a better response is probably called for
                return RedirectToAction("List", new { tripId = trip.Id });
            }

            var repo = TubsDataService.GetRepository<PurseSeineSet>(MvcApplication.CurrentSession);

            var fset = 
                repo.FilterBy(
                    s => s.Activity.Day.Trip.Id == trip.Id && s.SetNumber == setNumber
                ).FirstOrDefault();

            // Shouldn't happen, but then again...
            if (null == fset)
                return RedirectToAction("List", new { tripId = trip.Id });

            // We can (and should) be lenient on page number
            int maxPages = fset.SamplingHeaders.Count;

            if (pageNumber > maxPages)
                pageNumber = maxPages;

            var header = fset.SamplingHeaders.Skip(pageNumber - 1).Take(1).FirstOrDefault();
            if (null == header)
                return RedirectToAction("List", new { tripId = trip.Id });

            var vm = Mapper.Map<LengthSamplingHeader, LengthFrequencyViewModel>(header);

            if (IsApiRequest())
                return GettableJsonNetData(vm);

            return View(vm);
        }

        

    }
}
