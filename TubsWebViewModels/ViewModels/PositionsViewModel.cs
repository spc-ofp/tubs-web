// -----------------------------------------------------------------------
// <copyright file="PositionsViewModel.cs" company="Secretariat of the Pacific Community">
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
    /// ViewModel for representing all the observer-recorded points in an entire
    /// trip as a GeoJSON FeatureCollection.
    /// </summary>
    public sealed class PositionsViewModel : BaseGeoViewModel
    {
        public PositionsViewModel()
        {
            this.Properties = new Dictionary<string, object>();
            this.Positions = new List<ObservedPosition>();
        }

        public IList<ObservedPosition> Positions { get; set; }

        public decimal[] BoundingBox
        {
            get
            {
                if (null == this.Positions || this.Positions.Count < 2)
                    return DEFAULT_BOUNDING_BOX;

                var longitudes = this.Positions.Select(x => x.Longitude);
                var latitudes = this.Positions.Select(x => x.Latitude);

                return new[] {
                    longitudes.Min(),
                    latitudes.Min(),
                    longitudes.Max(),
                    latitudes.Max()
                };
            }
        }

        public object GeoJson
        {
            get
            {
                return new
                {
                    type = "FeatureCollection",
                    bbox = this.BoundingBox,
                    features = this.Positions.Select(p => p.GeoJson),
                    properties = this.Properties
                };
            }
        }

        /// <summary>
        /// ObservedPosition represents a single observed position within a trip.
        /// Latitude and longitude are first-class properties.  Everything else goes
        /// into the "Properties" grab-bag.
        /// </summary>
        public sealed class ObservedPosition
        {
            public ObservedPosition()
            {
                this.Properties = new Dictionary<string, object>();
            }

            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public IDictionary<string, object> Properties { get; set; }

            public object GeoJson
            {
                get
                {
                    return new
                    {
                        type = "Feature",
                        geometry = new
                        {
                            type = "Point",
                            coordinates = new[] { this.Longitude, this.Latitude }                            
                        },
                        properties = this.Properties
                    };
                }
            }
        }
    }
}
