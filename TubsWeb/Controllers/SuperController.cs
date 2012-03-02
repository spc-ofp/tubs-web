// -----------------------------------------------------------------------
// <copyright file="SuperController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers
{
    using System.Web.Mvc;
    using TubsWeb.Core;

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

    /// <summary>
    /// SuperController adds logging and user error reporting capabilities to the MVC3 Controller.
    /// </summary>
    [UseTransactionsByDefault]
    [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
    public class SuperController : Controller
    {
        // Alert levels
        public const string ALERT_SUCCESS = "alert-success";
        public const string ALERT_DANGER = "alert-danger";
        public const string ALERT_ERROR = "alert-error";
        public const string ALERT_INFO = "alert-info";

        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(SuperController));

        /// <summary>
        /// Flash stores an alert message and level into ViewData.
        /// Note that ViewData doesn't persist across a redirect, so anything stored thus
        /// will be lost if the user is pushed to another page via RedirectToAction(...)
        /// </summary>
        /// <param name="message">Alert message</param>
        /// <param name="level">Alert level</param>
        public void Flash(string message, string level = ALERT_ERROR)
        {
            ViewData["flash"] = message;
            ViewData["flash-level"] = level;
        }
    }
}