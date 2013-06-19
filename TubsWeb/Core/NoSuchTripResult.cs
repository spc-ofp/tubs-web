// -----------------------------------------------------------------------
// <copyright file="NoSuchTripResult.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
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
    using System.Text;
    using System.Web.Mvc;

    /// <summary>
    /// ActionResult used when the user has asked for a trip that doesn't exist.
    /// </summary>
    public class NoSuchTripResult : ActionResult
    {
        /// <summary>
        /// log4net logger.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(NoSuchTripResult));
        
        /// <summary>
        /// Return a response
        /// </summary>
        /// <param name="context">Context in which the action executes.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = 404;
            Logger.WarnFormat(
                "NoSuchTripResult for action [{0}] in controller [{1}]",
                context.RouteData.GetRequiredString("controller"),
                context.RouteData.GetRequiredString("action"));

            StringBuilder builder = new StringBuilder(512);
            foreach (var routeKey in context.RouteData.Values.Keys)
            {
                builder.AppendFormat("Key: {0}\tValue: {1}\n", routeKey, context.RouteData.Values[routeKey].ToString());
            }
            Logger.WarnFormat("RouteValueDictionary Contents:\n{0}", builder.ToString());
            new ViewResult { ViewName = "TripNotFound" }.ExecuteResult(context);
        }
    }
}