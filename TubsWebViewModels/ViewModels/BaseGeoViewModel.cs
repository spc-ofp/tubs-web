// -----------------------------------------------------------------------
// <copyright file="BaseGeoViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.ViewModels
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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// BaseGeoViewModel allows the Track and Positions ViewModels to share some
    /// common implementation details.
    /// </summary>
    public abstract class BaseGeoViewModel
    {
        /// <summary>
        /// Minimum longitude value (assuming a +/- 180 model)
        /// </summary>
        public const decimal MIN_LONGITUDE = -180.0M;

        /// <summary>
        /// Maximum latitude value (assuming a +/- 180 model)
        /// </summary>
        public const decimal MAX_LONGITUDE = 180.0M;

        /// <summary>
        /// Minimum latitude value (assuming a +/- 90 model)
        /// </summary>
        public const decimal MIN_LATITUDE = -90.0M;

        /// <summary>
        /// Maximum latitude value (assuming a +/- 90 model)
        /// </summary>
        public const decimal MAX_LATITUDE = 90.0M;

        public const string WGS84_CRS = "urn:ogc:def:crs:OGC:1.3:CRS84";

        /// <summary>
        /// Create a default bounding box that covers the entire world.
        /// </summary>
        public static decimal[] DEFAULT_BOUNDING_BOX = 
        {
            MIN_LONGITUDE,
            MIN_LATITUDE,
            MAX_LONGITUDE,
            MAX_LATITUDE    
        };

        /// <summary>
        /// A key value collection of assorted properties that might be useful on the client side.
        /// </summary>
        public IDictionary<string, object> Properties { get; set; }
    }
}
