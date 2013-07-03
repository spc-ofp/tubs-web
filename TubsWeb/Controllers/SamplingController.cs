// -----------------------------------------------------------------------
// <copyright file="SamplingController.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL.Entities;

    /// <summary>
    /// 
    /// </summary>
    public abstract class SamplingController : SuperController
    {
        /// <summary>
        /// Validate the incoming setNumber parameter
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="setNumber"></param>
        /// <param name="maxSets"></param>
        /// <returns></returns>
        internal Tuple<bool, RouteValueDictionary> NeedsRedirect(int tripId, int setNumber, int maxSets)
        {
            var needsRedirect = false;
            // Fill values that don't change
            // TODO: Confirm CurrentController returns the "friendly" name (e.g. FishingSet)
            // and not the full name (e.g. FishingSetController).
            var rvd = new RouteValueDictionary(
                new { controller = CurrentController(), tripId = tripId }
            );

            if (IsEdit())
            {
                if (setNumber > maxSets)
                {
                    rvd["setNumber"] = maxSets;
                    rvd["action"] = "Edit";
                    needsRedirect = true;
                }
            }

            return Tuple.Create(needsRedirect, rvd);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="setNumber"></param>
        /// <returns></returns>
        internal abstract ActionResult ViewActionImpl(Trip tripId, int setNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="setNumber"></param>
        /// <returns></returns>
        public ActionResult Index(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="setNumber"></param>
        /// <returns></returns>
        public ActionResult Edit(Trip tripId, int setNumber)
        {
            return ViewActionImpl(tripId, setNumber);
        }

    }
}
