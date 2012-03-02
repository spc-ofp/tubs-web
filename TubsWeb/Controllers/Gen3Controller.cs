// -----------------------------------------------------------------------
// <copyright file="Gen3Controller.cs" company="Secretariat of the Pacific Community">
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
    
    public class Gen3Controller : SuperController
    {

        //
        // GET: /Gen3/
        public ActionResult Index(int tripId)
        {
            ViewBag.TripId = tripId;
            var repo = new TubsRepository<TripMonitor>(MvcApplication.CurrentSession);
            var gen3 = repo.FilterBy(g => g.Trip.Id == tripId).FirstOrDefault();
            if (null == gen3)
            {
                // No GEN-3
                ViewBag.Title = String.Format("No GEN-3 for tripId {0}", tripId);
                return View("NotFound");
            }
            else if (null == gen3.Trip)
            {
                ViewBag.Title = String.Format("GEN-3 for tripId {0}", tripId);
            }
            else
            {
                ViewBag.Title = String.Format("GEN-3 for trip {0}", gen3.Trip.ToString());
            }
            return View(gen3);
        }

    }
}
