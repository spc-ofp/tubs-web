﻿// -----------------------------------------------------------------------
// <copyright file="VesselController.cs" company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Models.ExtensionMethods;

    /// <summary>
    /// Lookup-only controller for adding Ajax autocomplete capabilities.
    /// </summary>
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class VesselController : SuperController
    {                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="term">Vessel search term</param>
        /// <returns></returns>
        public JsonResult Find(string term)
        {
            var repo = TubsDataService.GetRepository<Vessel>(MvcApplication.CurrentSession);
            var vessels = (
                from vessel in repo.FilterBy(v => v.Name.Contains(term))
                select new
                {
                    id = vessel.Id,
                    label = vessel.Name.Trim(),
                    value = vessel.Name.Trim(),
                    GearType = TubsExtensions.GearCodeFromVesselType(vessel.TypeCode)
                }
            );
            return GettableJsonNetData(vessels.ToList());
        }

    }
}
