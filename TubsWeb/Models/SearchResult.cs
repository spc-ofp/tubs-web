// -----------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Models
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

    /// <summary>
    /// Value object for returning search results.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Trip detail link
        /// </summary>
        public string DetailUrl { get; set; }

        /// <summary>
        /// Vessel name
        /// </summary>
        public string VesselName { get; set; }

        /// <summary>
        /// Port of departure
        /// </summary>
        public string DeparturePort { get; set; }

        /// <summary>
        /// Trip start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Return port
        /// </summary>
        public string ReturnPort { get; set; }

        /// <summary>
        /// Trip end date
        /// </summary>
        public DateTime ReturnDate { get; set; }
    }
}