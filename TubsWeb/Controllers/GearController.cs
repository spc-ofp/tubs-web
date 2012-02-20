// -----------------------------------------------------------------------
// <copyright file="GearController.cs" company="Secretariat of the Pacific Community">
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
    using System.Web;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    
    public class GearController : SuperController
    {
        //
        // GET: /Gear/
        public ActionResult Index(int id)
        {
            ViewBag.TripId = id;
            var repo = new TubsRepository<Gear>(MvcApplication.CurrentSession);
            var gear = repo.FilterBy(g => g.Trip.Id == id).FirstOrDefault();
            ViewBag.Title =
                null == gear ?
                    "No gear recorded for this trip" :
                    String.Format("Gear for trip {0} / {1}", gear.Trip.Observer.StaffCode, gear.Trip.TripNumber);

            return View(gear);
        }

    }
}
