// -----------------------------------------------------------------------
// <copyright file="LogonAuthorize.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
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
    using System.Collections.Generic;
    using System.Web.Mvc;
    using TubsWeb.Controllers;
    
    /// <summary>
    /// LogonAuthorize filter based off implementation here:
    /// http://blogs.msdn.com/b/rickandy/archive/2011/05/02/securing-your-asp-net-mvc-3-application.aspx
    /// </summary>
    public class LogonAuthorize : AuthorizeAttribute
    {
        private static ISet<string> ActionWhiteList = new HashSet<string>()
        {
            "LogOn",
            "LogOff",
            "Register"
        };
        
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool isAccountController = filterContext.Controller is AccountController;
            // TODO Could also be in required values?
            string actionName = filterContext.RequestContext.RouteData.Values["action"].ToString();

            // Only AccountController can allow anonymous access
            if (!isAccountController)
                base.OnAuthorization(filterContext);

            if (isAccountController && !ActionWhiteList.Contains(actionName))            
                base.OnAuthorization(filterContext);
        }
    }
}