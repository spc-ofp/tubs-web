// -----------------------------------------------------------------------
// <copyright file="Ps1Controller.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Mvc;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;

    public class Ps1Controller : SuperController
    {
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var ps1vm = Mapper.Map<PurseSeineTrip, Ps1ViewModel>(trip);

            if (IsApiRequest())
                return GettableJsonNetData(ps1vm);

            // If we wanted to, we could set up the new NavPills here...
            return View(CurrentAction(), ps1vm);
        }
        
        //
        // GET: /Trip/{tripId}/PS-1
        public ActionResult Index(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        public ActionResult Edit(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        [HttpPost]
        //[Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId, Ps1ViewModel ps1vm)
        {
            // TODO:  Needs AutoMapper to go from VM to objects...
            return null;
        }

    }
}
