// -----------------------------------------------------------------------
// <copyright file="WebApiAreaRegistration.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>

namespace TubsWeb.Areas.WebApi
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
    /// Area registration for Web API controllers.
    /// </summary>
    public class WebApiAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// Area name.
        /// </summary>
        public override string AreaName
        {
            get
            {
                return "WebApi";
            }
        }

        /// <summary>
        /// RegisterArea implementation is required but does nothing in this instance.
        /// </summary>
        /// <param name="context"></param>
        public override void RegisterArea(AreaRegistrationContext context)
        {           
            // No normal controllers in this area, just WebApi controllers
            /*
            context.MapRoute(
                "WebApi_default",
                "api/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
            */
        }
    }
}
