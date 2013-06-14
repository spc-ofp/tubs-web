// -----------------------------------------------------------------------
// <copyright file="TestingAreaRegistration.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Areas.Testing
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
    /// The 'Testing' area holds online unit tests (QUnit/Jasmine/whatever)
    /// </summary>
    public class TestingAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// Testing is as good an area name as any other
        /// </summary>
        public override string AreaName
        {
            get
            {
                return "Testing";
            }
        }

        /// <summary>
        /// Simple registration.  No fancy routes, just one controller with
        /// a bunch of actions.
        /// </summary>
        /// <param name="context"></param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Testing_default",
                "Testing/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
