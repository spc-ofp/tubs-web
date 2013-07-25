// -----------------------------------------------------------------------
// <copyright file="MapController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers
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
    using System.Web.Mvc;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    /// <summary>
    /// MapController holds all the mapping-related interactions for a trip.
    /// It can produce the following outputs:
    /// - Full KML (useful in Google Earth)
    /// - KML of just positions
    /// - KML of just track
    /// - GeoJSON of just positions
    /// - GeoJSON of just track
    /// </summary>
    public sealed class MapController : SuperController
    {
        /// <summary>
        /// Uncompressed KML file extension
        /// </summary>
        public const string KML_EXTENSION = "KML";

        /// <summary>
        /// Compressed KML file extension
        /// </summary>
        public const string KMZ_EXTENSION = "KMZ";

        /// <summary>
        /// JSON file extension
        /// </summary>
        public const string JSON_EXTENSION = "JSON";

        /// <summary>
        /// Alternate JSON file extension
        /// </summary>
        public const string GEOJSON_EXTENSION = "GEOJSON";
        
        /// <summary>
        /// Convenience method for determining if a file extension is KML related
        /// </summary>
        /// <param name="extension">File extension to check</param>
        /// <returns>true if file is KML/KMZ, false otherwise</returns>
        internal bool IsKml(string extension)
        {
            return KML_EXTENSION.Equals(extension, StringComparison.InvariantCultureIgnoreCase) ||
                   KMZ_EXTENSION.Equals(extension, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Convenience method for determining if a file extension signifies a request
        /// for compressed output.
        /// </summary>
        /// <param name="extension">File extension to check</param>
        /// <returns>true if request for compressed results, false otherwise</returns>
        internal bool IsCompressed(string extension)
        {
            return KMZ_EXTENSION.Equals(extension, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Convenience method for determining if a file extension is GeoJSON related
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        internal bool IsJson(string extension)
        {
            return JSON_EXTENSION.Equals(extension, StringComparison.InvariantCultureIgnoreCase) ||
                   GEOJSON_EXTENSION.Equals(extension, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Create KML document representing just the trip track.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="isCompressed">Parameter for requesting compressed data</param>
        /// <returns>KML document containing trip track.</returns>
        internal KmlResult TrackKml(Trip tripId, bool isCompressed = false)
        {
            // Exclude any pushpins that won't display nicely
            var pushpins = tripId.Pushpins.Where(p => p.CanDisplay()).ToList();
            // Sort by date (assumes all timestamps have the same base frame of reference for date)
            // which occasionally is not true.
            pushpins.Sort(
                delegate(Pushpin p1, Pushpin p2)
                {
                    return Comparer<DateTime?>.Default.Compare(p1.Timestamp, p2.Timestamp);
                });

            var tripDoc = KmlBuilder.BuildTrack(tripId.Pushpins);
            tripDoc.name = "Trip Track";
            tripDoc.description =
                String.Format(
                    "Positions for trip {0} generated on {1} via URL: [{2}]",
                    tripId.ToString(),
                    DateTime.Now.ToShortDateString(),
                    this.HttpContext.Request.RawUrl);
            return new KmlResult()
            {
                Document = tripDoc,
                IsCompressed = isCompressed
            };
        }

        /// <summary>
        /// Create KML document representing just the trip positions.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="isCompressed">Parameter for requesting compressed data</param>
        /// <returns>KML document containing trip positions.</returns>
        internal KmlResult PositionsKml(Trip tripId, bool isCompressed = false)
        {
            // Exclude any pushpins that won't display nicely
            var pushpins = tripId.Pushpins.Where(p => p.CanDisplay()).ToList();
            // There's no call to sort these positions
            var tripDoc = KmlBuilder.Build(tripId.Pushpins, false);
            tripDoc.name = "All Trip Positions";
            tripDoc.description =
                String.Format(
                    "Positions for trip {0} generated on {1} via URL: [{2}]",
                    tripId.ToString(),
                    DateTime.Now.ToShortDateString(),
                    this.HttpContext.Request.RawUrl);
            return new KmlResult()
            {
                Document = tripDoc,
                IsCompressed = isCompressed
            };
        }

        /// <summary>
        /// MVC Action for rendering the map plugin.
        /// Leaflet doesn't like living in a Bootstrap tab, so this
        /// has been extracted to a separate view.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        public ActionResult Index(Trip tripId)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            ViewBag.Title = String.Format("{0}: Map", tripId.SpcTripNumber);
            ViewBag.TripNumber = tripId.SpcTripNumber;
            // Rather than make network calls, stuff the positions and track
            // into the ViewBag (and then into the page)

            var pvm = Mapper.Map<Trip, PositionsViewModel>(tripId);
            if (null != pvm)
                ViewBag.Positions = pvm.GeoJson;

            var tvm = Mapper.Map<Trip, TrackViewModel>(tripId);
            if (null != tvm)
                ViewBag.Track = tvm.GeoJson;

            return View();
        }

        /// <summary>
        /// MVC Action for returning all trip positions
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="extension">Desired file extension</param>
        /// <returns>All trip positions using the requested file type.</returns>
        public ActionResult Positions(Trip tripId, string extension)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            if (IsKml(extension))
                return PositionsKml(tripId, IsCompressed(extension));

            if (IsJson(extension))
            {
                // This (and the KML method) is the only thing that changes between
                // positions and track.  It would be good to define these as function pointers
                // and then use a common implementation
                var vm = Mapper.Map<Trip, PositionsViewModel>(tripId);

                if (null == vm)
                {
                    // Return a 500 error with the message that
                    // the mapping failed
                }

                return GettableJsonNetData(vm.GeoJson);
            }

            // Caller is asking for a file type (extension) we can't create
            return new HttpNotFoundResult();
        }

        /// <summary>
        /// MVC Action for returning trip track
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="extension">Desired file extension</param>
        /// <returns>Trip track using the requested file type.</returns>
        public ActionResult Track(Trip tripId, string extension)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            if (IsKml(extension))
                return TrackKml(tripId, IsCompressed(extension));

            if (IsJson(extension))
            {
                var vm = Mapper.Map<Trip, TrackViewModel>(tripId);

                if (null == vm)
                {
                    // Return a 500 error with the message that
                    // the mapping failed
                }

                return GettableJsonNetData(vm.GeoJson);
            }

            // Caller is asking for a file type (extension) we can't create
            return new HttpNotFoundResult();
        }
        
        
        /// <summary>
        /// Return all trip positions as KML.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// /// <param name="extension">File extension</param>
        /// <returns>KML document with all trip positions.</returns>
        public ActionResult AllData(Trip tripId, string extension)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            if (!IsKml(extension))
                return new HttpNotFoundResult();

            // Exclude any pushpins that won't display nicely
            var pushpins = tripId.Pushpins.Where(p => p.CanDisplay()).ToList();
            // Sort by date (assumes all timestamps have the same base frame of reference for date)
            // which occasionally is not true.
            pushpins.Sort(
                delegate(Pushpin p1, Pushpin p2)
                {
                    return Comparer<DateTime?>.Default.Compare(p1.Timestamp, p2.Timestamp);
                });

            var tripDoc = KmlBuilder.Build(tripId.Pushpins);
            tripDoc.name = "All Trip Positions";
            tripDoc.description =
                String.Format(
                    "Positions for trip {0} generated on {1} via URL: [{2}]",
                    tripId.ToString(),
                    DateTime.Now.ToShortDateString(),
                    this.HttpContext.Request.RawUrl);
            return new KmlResult()
            {
                Document = tripDoc,
                IsCompressed = IsCompressed(extension)
            };
        }

    }
}
