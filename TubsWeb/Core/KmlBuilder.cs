// -----------------------------------------------------------------------
// <copyright file="KmlBuilder.cs" company="Secretariat of the Pacific Community">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Google.Kml;
    using Spc.Ofp.Tubs.DAL.Entities;
    
    public static class KmlBuilder
    {
        public const string ACTIVITY_KEY = 
            "Red=Set, Black=Haul, Blue=In Port, Yellow=Other activity, White=Gen1, Green=Gen2, Brown=Gen6";

        public const string ShadedRedDot = "#msn_shaded_dotred";
        public const string ShadedBlueDot = "#msn_shaded_dotblue";
        public const string ShadedYellowDot = "#msn_shaded_dotyellow";
        public const string ShadedWhiteDot = "#msn_shaded_dotwhite";
        public const string ShadedGreenDot = "#msn_shaded_dotgreen";
        public const string ShadedBrownDot = "#msn_shaded_dotbrown";
        public const string ShadedBlackDot = "#msn_shaded_dotblack";

        public static Placemark ToPlacemark(this Pushpin pushpin, string style)
        {
            if (null == pushpin || !pushpin.Latitude.HasValue || !pushpin.Longitude.HasValue || !pushpin.Timestamp.HasValue)
            {
                return null;
            }
            return new Placemark()
            {
                name = pushpin.Description,
                Geometry = new Point(new coordinates((double)pushpin.Latitude.Value, (double)pushpin.Longitude.Value)),
                styleUrl = style,
                TimePrimitive = new SimpleTimeStamp(pushpin.Timestamp.Value.ToString("s"))
            };
        }

        public static string GetStyle(this Pushpin pushpin)
        {
            if (null == pushpin)
                return ShadedYellowDot;

            if ("LL-2".Equals(pushpin.FormName.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                if (pushpin.Description.NullSafeToUpper().Contains("HAUL"))
                    return ShadedBlackDot;

                if (pushpin.Description.NullSafeToUpper().Contains("SET"))
                    return ShadedRedDot;
            }

            if ("GEN-6".Equals(pushpin.FormName.Trim(), StringComparison.InvariantCultureIgnoreCase))
                return ShadedBrownDot;

            if ("GEN-2".Equals(pushpin.FormName.Trim(), StringComparison.InvariantCultureIgnoreCase))
                return ShadedGreenDot;

            if ("GEN-1".Equals(pushpin.FormName.Trim(), StringComparison.InvariantCultureIgnoreCase))
                return ShadedWhiteDot;

            if (pushpin.Description.ToUpper().Contains("IN PORT"))
                return ShadedBlueDot;

            if ("Fishing".Equals(pushpin.Description.Trim(), StringComparison.InvariantCultureIgnoreCase))
                return ShadedRedDot;

            // Anything else
            return ShadedYellowDot;
        }

        private static Folder BuildAllPositions(IEnumerable<Pushpin> positions)
        {
            // Root folder contains another folder will _all_ the positions
            Folder allPositions = new Folder()
            {
                id = Guid.NewGuid().ToString(),
                name = "All Positions",
                visibility = true,
                open = true,
                description = "All Observer Positions"
            };

            // TODO Fix styling
            var allPositionQuery =
                from p in positions
                where p != null
                select p.ToPlacemark(p.GetStyle());

            allPositions.Features.AddRange(allPositionQuery.Where(p => p != null));

            return allPositions;
        }

        private static Folder BuildTripTrack(IEnumerable<Pushpin> positions)
        {
            Folder tripTrack = new Folder()
            {
                id = Guid.NewGuid().ToString(),
                name = "Trip Track",
                visibility = true,
                open = true,
                description = "Trip Track"
            };

            StringBuilder linestringBuilder = new StringBuilder(1024);
            var previousPosition = positions.First();
            foreach (var position in positions.Skip(1))
            {
                linestringBuilder.AppendFormat(
                    "{0},{1},0,{2},{3},0\n",
                    previousPosition.Longitude,
                    previousPosition.Latitude,
                    position.Longitude,
                    position.Latitude);
                previousPosition = position;
            }

            Placemark track = new Placemark()
            {
                name = "TripTrack"
            };
            LineString lineString = new LineString(new coordinates(linestringBuilder.ToString()));
            lineString.tessellate = true;
            track.Geometry = lineString;
            tripTrack.Features.Add(track);

            return tripTrack;
        }

        private static Folder BuildGen1Transfers(IEnumerable<Pushpin> positions)
        {
            Folder folder = new Folder()
            {
                id = Guid.NewGuid().ToString(),
                name = "GEN-1 Transfers",
                visibility = false,
                open = false,
                description = "GEN-1 Transfers"
            };

            var qry =
                from p in positions
                where p != null && "GEN-1".Equals(p.FormName) && p.Description.Contains("Transfer")
                select p.ToPlacemark(ShadedWhiteDot);

            folder.Features.AddRange(qry.Where(p => p != null));
            return folder;
        }

        private static Folder BuildGen1Sightings(IEnumerable<Pushpin> positions)
        {
            Folder folder = new Folder()
            {
                id = Guid.NewGuid().ToString(),
                name = "GEN-1 Sightings",
                visibility = false,
                open = false,
                description = "GEN-1 Sightings"
            };

            var qry =
                from p in positions
                where p != null && "GEN-1".Equals(p.FormName) && p.Description.Contains("Sighting")
                select p.ToPlacemark(ShadedWhiteDot);

            folder.Features.AddRange(qry.Where(p => p != null));
            return folder;
        }

        private static Folder BuildGen2(IEnumerable<Pushpin> positions)
        {
            Folder folder = new Folder()
            {
                id = Guid.NewGuid().ToString(),
                name = "GEN-2 Special Species Interactions",
                visibility = false,
                open = false,
                description = "GEN-2 Special Species Interactions"
            };

            var qry =
                from p in positions
                where p != null && "GEN-2".Equals(p.FormName)
                select p.ToPlacemark(ShadedGreenDot);

            folder.Features.AddRange(qry.Where(p => p != null));
            return folder;
        }

        private static Folder BuildGen6(IEnumerable<Pushpin> positions)
        {
            Folder folder = new Folder()
            {
                id = Guid.NewGuid().ToString(),
                name = "GEN-6 Pollution Incidents",
                visibility = false,
                open = false,
                description = "GEN-6 Pollution Incidents"
            };

            var qry =
                from p in positions
                where p != null && "GEN-6".Equals(p.FormName)
                select p.ToPlacemark(ShadedBrownDot);

            folder.Features.AddRange(qry.Where(p => p != null));
            return folder;
        }
        
        public static Document Build(IEnumerable<Pushpin> positions)
        {
            // TODO See about moving StyleBuilder over
            Folder root = new Folder()
            {
                id = Guid.NewGuid().ToString(),
                name = "Observer Positions",
                visibility = true,
                description = ACTIVITY_KEY
            };

            // Root folder contains another folder with _all_ the positions
            root.Features.Add(BuildAllPositions(positions));

            // Add another folder holding the trip track           
            root.Features.Add(BuildTripTrack(positions));

            // Add GEN-1 transfers
            root.Features.Add(BuildGen1Transfers(positions));

            // Add GEN-1 sightings
            root.Features.Add(BuildGen1Sightings(positions));

            // Add GEN-2 events
            root.Features.Add(BuildGen2(positions));

            // Add GEN-6 events
            root.Features.Add(BuildGen6(positions));

            Document doc = new Document()
            {
                id = Guid.NewGuid().ToString(),
                open = true
            };
            doc.Features.Add(root);
            doc.StyleSelectors.AddRange(KmlStyleBuilder.BuildStyles());
            return doc;
        }
    }
}