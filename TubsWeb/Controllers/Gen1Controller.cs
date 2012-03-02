// -----------------------------------------------------------------------
// <copyright file="Gen1Controller.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
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
    using TubsWeb.Models;
    using TubsWeb.Core;

    public class Gen1Controller : SuperController
    {

        //
        // GET: /Gen1/
        public ActionResult Index(Trip tripId)
        {
            Gen1ViewModel viewModel = new Gen1ViewModel();
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }
            ViewBag.Title = String.Format("GEN-1 events for trip {0}", tripId.ToString());
            viewModel.TripId = tripId.Id;
            viewModel.Sightings = tripId.Sightings;
            viewModel.Transfers = tripId.Transfers;
            return View(viewModel);
        }

    }
}
