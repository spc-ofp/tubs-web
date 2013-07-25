// -----------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb
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
    using System.Web.Routing;
    using DoddleReport.Web;
    
    /// <summary>
    /// Configure MVC routes.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// MVC route name for PS-1 form.
        /// </summary>
        public static string Ps1 = "Ps1";

        /// <summary>
        /// MVC route name for LL-1 form.
        /// </summary>
        public static string TripInfo = "TripInfo";

        /// <summary>
        /// MVC route name for electronics equipment detail
        /// on the PS-1/LL-1 form.
        /// </summary>
        public static string Electronics = "Electronics";

        /// <summary>
        /// MVC route name for the crew detail from the PS-1 form.
        /// </summary>
        public static string Crew = "Crew";

        /// <summary>
        /// MVC route name for vessel sightings (GEN-1)
        /// </summary>
        public static string Gen1Sightings = "Gen1Sightings";

        /// <summary>
        /// MVC route name for editing vessel sightings (GEN-1)
        /// </summary>
        public static string EditGen1Sightings = "EditGen1Sightings";

        /// <summary>
        /// MVC route name for catch transfer (GEN-1)
        /// </summary>
        public static string Gen1Transfers = "Gen1Transfers";

        /// <summary>
        /// MVC route name for editing catch transfer (GEN-1)
        /// </summary>
        public static string EditGen1Transfers = "EditGen1Transfers";

        /// <summary>
        /// MVC route for displaying all GEN-1 data.
        /// </summary>
        public static string Gen1 = "Gen1";

        /// <summary>
        /// MVC route name for special species interaction (GEN-2)
        /// </summary>
        public static string Gen2 = "Gen2";

        /// <summary>
        /// MVC route name for special species interaction details.
        /// </summary>
        public static string Gen2Details = "Gen2Details";

        /// <summary>
        /// MVC route name for compliance form (GEN-3)
        /// </summary>
        public static string Gen3 = "Gen3";

        /// <summary>
        /// MVC route name for FAD form (GEN-5).
        /// </summary>
        public static string Gen5 = "Gen5";

        /// <summary>
        /// MVC route name for pollution form (GEN-6).
        /// </summary>
        public static string Gen6 = "Gen6";

        /// <summary>
        /// MVC route name for pollution details.
        /// </summary>
        public static string Gen6Details = "Gen6Details";

        /// <summary>
        /// MVC route name for purse seine sets.
        /// </summary>
        public static string Sets = "Sets";

        /// <summary>
        /// MVC route name for list of LL-4 forms for a trip.
        /// </summary>
        public static string LongLineSampleList = "LongLineSampleList";

        /// <summary>
        /// MVC route name for long line sets (LL-4)
        /// </summary>
        public static string LongLineSets = "LongLineSets";

        /// <summary>
        /// MVC route name for long line set/haul (LL-2/3 form)
        /// </summary>
        public static string SetHaul = "SetHaul";

        /// <summary>
        /// MVC route name for all PS-4 forms associated with a trip
        /// </summary>
        public static string Ps4List = "Ps4List";

        public static string Ps4ByPage = "Ps4ByPage";


        public static string LengthSamples = "LengthSamples";


        public static string LengthFrequencyByTrip = "LengthFrequencyByTrip";

        /// <summary>
        /// MVC route name for Doddle Reports Excel export of trip.
        /// </summary>
        public static string ExcelTripSummary = "ExcelTripSummary";

        public static string SeaDayById = "SeaDayById";

        /// <summary>
        /// MVC route name for purse seine sea days.
        /// </summary>
        public static string SeaDays = "SeaDays";

        /// <summary>
        /// MVC route name for
        /// </summary>
        public static string Gear = "Gear";

        /// <summary>
        /// MVC route name for
        /// </summary>
        public static string WellContents = "WellContents";

        /// <summary>
        /// MVC route name for workbook page counts.
        /// </summary>
        public static string PageCount = "PageCount";

        /// <summary>
        /// MVC route name for trip details.
        /// </summary>
        public static string TripDetails = "TripDetails";

        /// <summary>
        /// MVC route name for trip map.
        /// </summary>
        public static string TripMap = "TripMap";

        /// <summary>
        /// MVC route name for
        /// </summary>
        public static string TripDefault = "TripDefault";

        /// <summary>
        /// MVC route name for standard list of trips.
        /// </summary>
        public static string TripList = "TripList";

        /// <summary>
        /// MVC route name for the RSS feed of trips.
        /// </summary>
        public static string RssFeed = "RssFeed";

        /// <summary>
        /// MVC route name for the default route.
        /// </summary>
        public static string Default = "Default";

        /// <summary>
        /// MVC route name for trips entered by current user.
        /// This is to fix an MVC bug that is still not fixed in MVC4
        /// http://stackoverflow.com/questions/780643/asp-net-mvc-html-actionlink-keeping-route-value-i-dont-want
        /// </summary>
        public static string MyTrips = "MyTrips";

        /// <summary>
        /// MVC route name for open trips entered by current user.
        /// </summary>
        public static string MyOpenTrips = "MyOpenTrips";

        /// <summary>
        /// MVC route name for creating a new trip.
        /// </summary>
        public static string CreateTrip = "CreateTrip";

        /// <summary>
        /// MVC route name for trip track with a client-selectable file extension.
        /// </summary>
        public static string TrackWithExtension = "TrackWithExtension";

        /// <summary>
        /// MVC route name for trip positions with a client-selectable file extension.
        /// </summary>
        public static string PositionsWithExtension = "PositionsWithExtension";

        /// <summary>
        /// MVC route name for full trip data as KML (or KMZ).
        /// </summary>
        public static string FullTripKml = "FullTripKml";

        /// <summary>
        /// Regular expression constraint for use with database primary key specifications
        /// in MVC routes.
        /// </summary>
        public const string IsPositiveInteger = @"^\d+$";

        /// <summary>
        /// This is an acceptable starting point.  If we really want a case insensitive
        /// constraint, we actually have to create a new constraint class.
        /// http://stackoverflow.com/questions/13008904/can-i-create-an-asp-mvc-route-that-only-accepts-uppercase-in-the-url
        /// </summary>
        /// <remarks>
        /// Some people, when confronted with a problem, think 
        /// “I know, I'll use regular expressions.”   Now they have two problems.
        /// -jwz
        /// </remarks>
        public const string IsMappingExtension = @"^(kml|KML|kmz|KMZ|json|JSON|geojson|GEOJSON)$";

        /// <summary>
        /// Register MVC routes in the application.
        /// </summary>
        /// <param name="routes">Existing application route collection.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapReportingRoute(); // For DoddleReports

            routes.MapRoute(
                name: MyOpenTrips,
                url: "Trip/MyOpenTrips",
                defaults: new { controller = "Trip", action = "MyOpenTrips" }
            );

            routes.MapRoute(
                name: MyTrips,
                url: "Trip/MyTrips",
                defaults: new { controller = "Trip", action = "MyTrips" }
            );

            routes.MapRoute(
                name: CreateTrip,
                url: "Trip/Create",
                defaults: new { controller = "Trip", action = "Create" }
            );

            // I'm fairly certain that the DoddleReports installer fiddled web.config
            // so that routes that include a file extension get routed through MVC
            routes.MapRoute(
                name: TrackWithExtension,
                url: "Trip/{tripId}/track.{extension}",
                defaults: new { controller = "Map", action = "Track" },
                constraints: new { tripId = IsPositiveInteger, extension = IsMappingExtension }
            );

            routes.MapRoute(
                name: PositionsWithExtension,
                url: "Trip/{tripId}/positions.{extension}",
                defaults: new { controller = "Map", action = "Positions" },
                constraints: new { tripId = IsPositiveInteger, extension = IsMappingExtension }
            );

            // For this one, we can set a default extension, preferring compression for what
            // is likely to be a large file.
            routes.MapRoute(
                name: FullTripKml,
                url: "Trip/{tripId}/full_trip.{extension}",
                defaults: new { controller = "Map", action = "AllData", extension = "kmz" },
                constraints: new { tripId = IsPositiveInteger, extension = IsMappingExtension }
            );

            routes.MapRoute(
                name: TripMap,
                url: "Trip/{tripId}/Map",
                defaults: new { controller = "Map", action = "Index" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: Electronics,
                url: "Trip/{tripId}/Electronics/{action}",
                defaults: new { controller = "Electronics", action = "Index" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: Crew,
                url: "Trip/{tripId}/Crew/{action}",
                defaults: new { controller = "Crew", action = "Index" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: Ps1,
                url: "Trip/{tripId}/PS-1/{action}",
                defaults: new { controller = "Ps1", action = "Index" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: TripInfo,
                url: "Trip/{tripId}/LL-1/{action}",
                defaults: new { controller = "TripInfo", action = "Index" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: Gen1Sightings,
                url: "Trip/{tripId}/Sightings",
                defaults: new { controller = "Gen1", action = "Sightings" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: EditGen1Sightings,
                url: "Trip/{tripId}/Sightings/Edit",
                defaults: new { controller = "Gen1", action = "EditSightings" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: Gen1Transfers,
                url: "Trip/{tripId}/Transfers",
                defaults: new { controller = "Gen1", action = "Transfers" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: EditGen1Transfers,
                url: "Trip/{tripId}/Transfers/Edit",
                defaults: new { controller = "Gen1", action = "EditTransfers" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                Gen1,
                "Trip/{tripId}/GEN-1/{action}",
                new { controller = "Gen1", action = "Index" },
                new { tripId = IsPositiveInteger }
            );

            // A trip has zero to n GEN-2 forms, and each GEN-2 has a "Page X of Y"
            // field...
            routes.MapRoute(
                Gen2Details,
                "Trip/{tripId}/GEN-2/{pageNumber}/{action}",
                new { controller = "Gen2", action = "Index" },
                new { tripId = IsPositiveInteger, pageNumber = IsPositiveInteger }
            );

            routes.MapRoute(
                Gen2,
                "Trip/{tripId}/GEN-2/{action}",
                new { controller = "Gen2", action = "List" },
                new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                Gen3,
                "Trip/{tripId}/GEN-3/{action}",
                new { controller = "Gen3", action = "Index" },
                new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: Gen5,
                url: "Trip/{tripId}/GEN-5/{action}",
                defaults: new { controller = "Gen5", action = "Index" },
                constraints: new { tripId = IsPositiveInteger }
            );

            // Link to particular GEN-6 page has to come first
            // due to route precedence
            routes.MapRoute(
                Gen6Details,
                "Trip/{tripId}/GEN-6/{pageNumber}",
                new { controller = "Gen6", action = "Index" },
                new { tripId = IsPositiveInteger, pageNumber = IsPositiveInteger }
            );

            routes.MapRoute(
                Gen6,
                "Trip/{tripId}/GEN-6/{action}/{pageNumber}",
                new { controller = "Gen6", action = "List", pageNumber = UrlParameter.Optional },
                new { tripId = IsPositiveInteger, pageNumber = IsPositiveInteger }
            );

            routes.MapRoute(
                name: LongLineSets,
                url: "Trip/{tripId}/LL-4/{setNumber}/{action}",
                defaults: new  { controller = "LongLineSampling", action = "List", setNumber = UrlParameter.Optional },
                constraints: new { tripId = IsPositiveInteger, setNumber = IsPositiveInteger }
            );

            routes.MapRoute(
                name: LongLineSampleList,
                url: "Trip/{tripId}/LL-4/",
                defaults: new  { controller = "LongLineSampling", action = "List" },
                constraints: new { tripId = IsPositiveInteger }
            );

            // Even though Set is subordinate to day, allow link directly to list of sets
            // and to a particular set number
            routes.MapRoute(
                name: Sets,
                url: "Trip/{tripId}/Sets/{setNumber}/{action}",
                defaults: new { controller = "FishingSet", action = "List", setNumber = UrlParameter.Optional },
                constraints: new { tripId = IsPositiveInteger, setNumber = IsPositiveInteger }
            );

            routes.MapRoute(
                name: "Ps4ByPageAndColumn",
                url: "Trip/{tripId}/PS-4/Pages/{pageNumber}/Column/{columnNumber}",
                defaults: new { controller = "Ps4", action = "Column" },
                constraints: new { tripId = IsPositiveInteger, pageNumber = IsPositiveInteger, columnNumber = IsPositiveInteger }
            );

            routes.MapRoute(
                name: Ps4ByPage,
                url: "Trip/{tripId}/PS-4/Pages/{pageNumber}",
                defaults: new { controller = "Ps4", action = "Page" },
                constraints: new { tripId = IsPositiveInteger, pageNumber = IsPositiveInteger }
            );

            routes.MapRoute(
                name: Ps4List,
                url: "Trip/{tripId}/PS-4/Pages",
                defaults: new { controller = "Ps4", action = "Index" },
                constraints: new { tripId = IsPositiveInteger }
            );


            // Although length samples are subordinate to Sets, they'll be available at a higher level
            // for a more readable URL.
            routes.MapRoute(
                LengthSamples,
                "Trip/{tripId}/Samples/{setNumber}/Page/{pageNumber}",
                new { controller = "LengthSample", action = "Index", pageNumber = UrlParameter.Optional },
                new { tripId = IsPositiveInteger, setNumber = IsPositiveInteger, pageNumber = IsPositiveInteger }
            );

            routes.MapRoute(
                name: LengthFrequencyByTrip,
                url: "Trip/{tripId}/LengthFrequency.xlsx",
                defaults: new { controller = "LengthSample", action = "AllSamples" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: ExcelTripSummary,
                url: "Trip/{tripId}/TripSummary.xlsx",
                defaults: new { controller = "Export", action = "Summary" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                name: SeaDays,
                url: "Trip/{tripId}/Days/{dayNumber}/{action}",
                defaults: new { controller = "SeaDay", action = "List", dayNumber = UrlParameter.Optional},
                constraints: new { tripId = IsPositiveInteger, dayNumber = IsPositiveInteger }
            );

            routes.MapRoute(
                name: SeaDayById,
                url: "Trip/{tripId}/DayById/{dayId}/{action}",
                defaults: new { controller = "SeaDay", action = "Index" },
                constraints: new { tripId = IsPositiveInteger, dayId = IsPositiveInteger }
            );

            routes.MapRoute(
                Gear,
                "Trip/{tripId}/Gear/{action}",
                new { controller = "Gear", action = "Index" },
                new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                WellContents,
                "Trip/{tripId}/WellContent/{action}",
                new { controller = "WellContent", action = "Index" },
                new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                PageCount,
                "Trip/{tripId}/PageCount/{action}",
                new { controller = "PageCount", action = "Index" },
                new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                TripDetails,
                "Trip/{tripId}/{action}",
                new { controller = "Trip", action = "Details" },
                new { tripId = IsPositiveInteger }
            );

            // TODO: Consider changing route to LL-2?  (Problem is that the form is LL-2/3, and '/' isn't great in a route URL)
            routes.MapRoute(
                name: SetHaul,
                url: "Trip/{tripId}/SetHaul/{setNumber}/{action}",
                defaults: new { controller = "SetHaul", action = "List", setNumber = UrlParameter.Optional },
                constraints: new { tripId = IsPositiveInteger, setNumber = IsPositiveInteger }
            );

            routes.MapRoute(
                name: RssFeed,
                url: "Trip/Rss",
                defaults: new { controller = "Trip", action = "Rss" }
            );

            // Can this route replace the fairly generic routes?
            // Doesn't look like it...
            routes.MapRoute(
                TripDefault,
                "Trip/{tripId}/{controller}/{action}",
                new { action = "Index" },
                new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                TripList,
                "Trip/",
                new { controller = "Trip", action = "Index" }
            );

            routes.MapRoute(
                Default, // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }
    }
}