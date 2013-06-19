// -----------------------------------------------------------------------
// <copyright file="PushpinExtensions.cs" company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using Google.Kml;
    using Spc.Ofp.Tubs.DAL.Entities;
    
    /// <summary>
    /// Extension methods for working with the Pushpin entity.
    /// </summary>
    public static class PushpinExtensions
    {
        /// <summary>
        /// Convert a collection of Pushpin entities into a KML track
        /// </summary>
        /// <param name="pins">Collection of Pushpin entities</param>
        /// <param name="formatString">optional date format string</param>
        /// <returns></returns>
        public static Track ToTrack(this IList<Pushpin> pins, string formatString = "yyyy-MM-ddTHH:mm:ssK")
        {
            var track = new Track();
            if (null != pins && pins.Count > 0)
            {
                // They should already be sorted, but we're lenient in what we accept
                var sorted = 
                    pins.Where(p => p.Timestamp.HasValue && p.Latitude.HasValue && p.Longitude.HasValue)
                        .OrderBy(p => p.Timestamp);

                track.Timestamps = (
                    from p in sorted
                    let ts = new FormattedDateTime(p.Timestamp.Value, formatString)
                    select new TimeStamp(ts)
                ).ToList();

                track.Coordinates = (
                    from p in sorted
                    select new coord((double)p.Longitude.Value, (double)p.Latitude.Value)
                ).ToList();
                
            }
            return track;
        }

        /// <summary>
        /// Extension method for determing if a Pushpin should be displayed in
        /// a user interface.
        /// </summary>
        /// <param name="pin">Pushpin object to check</param>
        /// <returns>true if the Pushpin has a latitude, longitude, and timestamp, false otherwise.</returns>
        public static bool CanDisplay(this Pushpin pin)
        {
            if (null == pin)
                return false;
            return pin.Latitude.HasValue && pin.Longitude.HasValue && pin.Timestamp.HasValue;
        }
    }
}