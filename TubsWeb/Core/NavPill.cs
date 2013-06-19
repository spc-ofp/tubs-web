// -----------------------------------------------------------------------
// <copyright file="NavPill.cs" company="Secretariat of the Pacific Community">
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
    
    /// <summary>
    /// Entity representing a navigation list item in the user interface.
    /// </summary>
    public class NavPill
    {
        /// <summary>
        /// String displayed in user interface.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Navigation link target.
        /// </summary>
        public string Href { get; set; }

        // TODO Come up with a good collection for this
        /// <summary>
        /// Collection of attributes to be applied to navigation link.
        /// </summary>
        public IDictionary<string, string> Attributes { get; set; }
    }
}