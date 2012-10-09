// -----------------------------------------------------------------------
// <copyright file="Gen5Controller.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
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
    using System.Net;
    using System.Web.Mvc;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;

    public class Gen5Controller : SuperController
    {
        //
        // GET: /GEN-5/Details
        public ActionResult Index(Trip tripId)
        {
            return View();
        }
        
        //
        // GET: /GEN-5/Details
        public ActionResult Details(Trip tripId, int? fadId, int? activityId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var repo = TubsDataService.GetRepository<Gen5Object>(MvcApplication.CurrentSession);
            Gen5Object fad = null;

            if (fadId.HasValue)
            {
                fad = repo.FindById(fadId.Value);
            }
            else if (activityId.HasValue)
            {
                fad = repo.FilterBy(f => f.Activity.Id == activityId.Value).FirstOrDefault();
            }

            // 
            if (null == fad || fad.Activity.Day.Trip.Id != trip.Id)
            {
                if (IsApiRequest())
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return GettableJsonNetData("Missing or invalid GEN-5 record");
                }
                // TODO Need a better strategy...
                return RedirectToAction("Index", new { tripId = trip.Id });
            }

            var fvm = Mapper.Map<Gen5Object, Gen5ViewModel>(fad);
            return View(fvm);
        }

        public ActionResult Edit(Trip tripId, int fadId)
        {
            return View();
        }

        public ActionResult Add(Trip tripId, int activityId)
        {
            return View();
        }

        

    }
}
