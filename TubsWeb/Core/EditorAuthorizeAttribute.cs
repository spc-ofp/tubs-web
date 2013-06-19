// -----------------------------------------------------------------------
// <copyright file="EditorAuthorizeAttribute.cs" company="Secretariat of the Pacific Community">
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
    using System;
    using System.Web.Configuration;
    using System.Web.Mvc;

    /// <summary>
    /// Configurable editor group authorization attribute.
    /// Based on implementation here:
    /// http://weblogs.asp.net/srkirkland/archive/2010/01/04/authorizing-access-via-attributes-in-asp-net-mvc-without-magic-strings.aspx
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EditorAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Get the list of users/groups that are in the editor role.
        /// </summary>
        /// <returns>String representation of all editors.</returns>
        public static string EditorGroups()
        {
            string groups = WebConfigurationManager.AppSettings["EditorGroups"];
            if (string.IsNullOrEmpty(groups))
            {
                groups = @"NT AUTHORITY\Authenticated Users";
            }
            return groups;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public EditorAuthorizeAttribute()
        {
            Roles = EditorGroups();
        }
    }
}