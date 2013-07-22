// -----------------------------------------------------------------------
// <copyright file="TrackViewModel.cs" company="Secretariat of the Pacific Community">
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
    /// ViewModel for representing a vessels track over an entire trip as a single
    /// GeoJSON LineString.
    /// </summary>
    public sealed class TrackViewModel : BaseGeoViewModel
    {        
        public TrackViewModel()
        {
            this.Positions = new List<decimal[]>();
            this.Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// An ordered collection of positions in longitude/latitude pairs.
        /// </summary>
        /// <remarks>
        /// NOTE:  Longitude _then_ Latitude (like KML, not like people say it in English)
        /// </remarks>
        public IList<decimal[]> Positions { get; set; }        

        /// <summary>
        /// Read-only property that gives a box that bounds all properties in the
        /// collection of positions.
        /// </summary>
        public decimal[] BoundingBox
        {
            get
            {
                if (null == this.Positions || this.Positions.Count < 2)
                    return DEFAULT_BOUNDING_BOX;

                var longitudes = this.Positions.Select(x => x[0]);
                var latitudes = this.Positions.Select(x => x[1]);
                return new[] {
                    longitudes.Min(),
                    latitudes.Min(),
                    longitudes.Max(),
                    latitudes.Max()
                };
            }
        }

        /// <summary>
        /// Read-only property that gives an object that the NewtonSoft JSON
        /// serializer can convert into valid GeoJSON.
        /// </summary>
        /// <example>
        /// var json = JsonConvert.SerializeObject(track.GeoJson);
        /// </example>
        public object GeoJson
        {
            get
            {
                return new
                {
                    type = "Feature",
                    bbox = this.BoundingBox,
                    geometry = new
                    {
                        type = "LineString",
                        coordinates = this.Positions
                    },
                    properties = this.Properties
                };
            }
        }
    }
}
