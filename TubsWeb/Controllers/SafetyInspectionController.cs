// -----------------------------------------------------------------------
// <copyright file="SafetyInspectionController.cs" company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Models;

    public class SafetyInspectionController : SuperController
    {
        //
        // GET: /SafetyInspection/
        public ActionResult Index(int id)
        {
            ViewBag.TripId = id;
            var trip = new TubsRepository<Trip>(MvcApplication.CurrentSession).FindBy(id);
            if (null == trip)
            {
                return View("NotFound");
            }
            ViewBag.Title = trip.ToString();
            return View(trip.Inspection);
        }

    }
}
