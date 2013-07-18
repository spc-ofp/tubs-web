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
    using System.Web.Mvc;
    using System.Web.Routing;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;

    /// <summary>
    /// 
    /// </summary>
    public class Ps4Controller : SuperController
    {
        /*
         * List of all PS-4 pages (Index):
         * /Trip/{tripId}/PS-4/
         * 
         * Sampling details for a given page
         * /Trip/{tripId}/PS-4/Pages/{pageNumber}
         * 
         * Length samples for a given page and column
         * /Trip/{tripId}/PS-4/Pages/{pageNumber}/Column/{columnNumber}
         */


        public ActionResult Index(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var vm = Mapper.Map<PurseSeineTrip, TripSamplingViewModel>(trip);

            return View(vm);
        }

        public ActionResult AddPage(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }
            // At this point, we are unsure of which set we're adding a page for
            // We could fix this by changing the TripSamplingViewModel such that we get
            // something for each set regardless of existence of PS-4


            return View();
        }

        public ActionResult Page(Trip tripId, int pageNumber)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }
            // TODO: Confirm that page number falls within trip


            return View();
        }

        public ActionResult Column(Trip tripId, int pageNumber, int columnNumber)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }
            // TODO: Confirm that page number falls within trip
            // TODO: Confirm that column number falls within number of samples for this page
            return View();
        }

    }
}
