// -----------------------------------------------------------------------
// <copyright file="UnitTestController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Areas.Testing.Controllers
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

    /// <summary>
    /// UnitTestController allows the use of bundles and Razor to test
    /// JavaScript inside the application environment.
    /// Inspiration for laying this out in the area came from Jonathan Creamer
    /// http://freshbrewedcode.com/jonathancreamer/2011/12/08/qunit-layout-for-javascript-testing-in-asp-net-mvc3/
    /// </summary>
    public class UnitTestController : Controller
    {
        // TODO Add security to the entire controller
        // web.config transform will lock the entire area on production deployment
        /// <summary>
        /// Runs QUnit tests via QUnit composite plugin
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Used for experimental viewmodels.
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Experimental()
        {
            ViewBag.Title = "Experimental features";
            return View();
        }
        
        /// <summary>
        /// Longline SetHaul QUnit unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult SetHaul()
        {
            ViewBag.Title = "Longline SetHaul unit tests";
            return View();
        }

        /// <summary>
        /// Longline LL-1 form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult TripInfo()
        {
            ViewBag.Title = "LL-1 unit tests";
            return View();
        }

        /// <summary>
        /// Purse Seine PS-1 form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Ps1()
        {
            ViewBag.Title = "PS-1 unit tests";
            return View();
        }

        /// <summary>
        /// Purse Seine crew unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Crew()
        {
            ViewBag.Title = "Purse Seine crew unit tests";
            return View();
        }

        /// <summary>
        /// GEN-5 (FAD) form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Fad()
        {
            ViewBag.Title = "Purse Seine GEN-5 (FAD) unit tests";
            return View();
        }

        /// <summary>
        /// Purse Seine PS-3 form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult FishingSet()
        {
            ViewBag.Title = "Purse Seine fishing set unit tests";
            return View();
        }

        /// <summary>
        /// GEN-2 Sighting form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Gen2Sighting()
        {
            ViewBag.Title = "GEN-2 (Sighting) unit tests";
            return View();
        }

        /// <summary>
        /// GEN-2 Landed on Deck form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Gen2Landed()
        {
            ViewBag.Title = "GEN-2 (Landed) unit tests";
            return View();
        }

        /// <summary>
        /// GEN-2 Interacted with gear form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Gen2GearInteraction()
        {
            ViewBag.Title = "GEN-2 (Interacted With Gear) unit tests";
            return View();
        }

        /// <summary>
        /// GEN-3 (v2009) form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Gen3()
        {
            ViewBag.Title = "GEN-3 (v2009) unit tests";
            return View();
        }

        /// <summary>
        /// GEN-3 (v2007 and earlier) form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Gen3v2007()
        {
            ViewBag.Title = "GEN-3 (v2007) unit tests";
            return View();
        }

        /// <summary>
        /// Page count unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult PageCounts()
        {
            ViewBag.Title = "Page count unit tests";
            return View();
        }

        /// <summary>
        /// PS-2 form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult SeaDay()
        {
            ViewBag.Title = "Purse Seine sea day (PS-2) unit tests";
            return View();
        }

        /// <summary>
        /// GEN-1 Sighting form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Sighting()
        {
            ViewBag.Title = "GEN-1 Sighting unit tests";
            return View();
        }

        /// <summary>
        /// GEN-1 Transfer form unit tests
        /// </summary>
        /// <returns>QUnit view</returns>
        public ActionResult Transfer()
        {
            ViewBag.Title = "GEN-1 Transfer unit tests";
            return View();
        }

    }
}
