// -----------------------------------------------------------------------
// <copyright file="WellContentController.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    
    public class WellContentController : SuperController
    {
        // TODO What about Longline trips?
        public ActionResult Index(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            AddTripNavbar(tripId);
            ViewBag.Title = String.Format("Well content for trip {0}", tripId.ToString());
            return View(trip);
        }

    }
}
