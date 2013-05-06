// -----------------------------------------------------------------------
// <copyright file="TripInfoController.cs" company="Secretariat of the Pacific Community">
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
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    /// <summary>
    /// TripInfoController is for working with LL-1 form information.
    /// On the purse seine side, this is called Ps1Controller.
    /// "Ll1Controller" is a name too poor to consider.
    /// This may be further renamed to LongLineTripInfoController
    /// (with Ps1Controller renamed to PurseSeineTripInfoController)
    /// but that is work for another day.
    /// </summary>
    public class TripInfoController : SuperController
    {
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            var trip = tripId as LongLineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var tivm = Mapper.Map<LongLineTrip, LongLineTripInfoViewModel>(trip);

            if (IsApiRequest())
                return GettableJsonNetData(tivm);

            // If we wanted to, we could set up the new NavPills here...
            return View(CurrentAction(), tivm);
        }

        //
        // GET: /Trip/{tripId}/LL-1
        public ActionResult Index(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        [EditorAuthorize]
        public ActionResult Edit(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

    }
}
