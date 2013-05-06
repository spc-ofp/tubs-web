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
    
    public class RouteConfig
    {
        // TODO Add documentation to all these strings
        public static string Ps1 = "Ps1";
        public static string TripInfo = "TripInfo"; // LL-1
        public static string Electronics = "Electronics";
        public static string SafetyInspection = "SafetyInspection";
        public static string Crew = "Crew";
        public static string Auxiliaries = "Auxiliaries";
        public static string VesselDetails = "VesselDetails";
        // Revision of route for GEN-1
        public static string Gen1Sightings = "Gen1Sightings";
        public static string EditGen1Sightings = "EditGen1Sightings";
        public static string Gen1Transfers = "Gen1Transfers";
        public static string EditGen1Transfers = "EditGen1Transfers";
        // TODO:  This will go away...
        public static string Gen1 = "Gen1";
        public static string Gen2Details = "Gen2Details";
        public static string Gen2 = "Gen2";
        public static string Gen3 = "Gen3";
        public static string Gen5 = "Gen5";
        public static string Gen6Details = "Gen6Details";
        public static string Gen6 = "Gen6";
        public static string Sets = "Sets";
        public static string SetHaul = "SetHaul";
        public static string LengthSamples = "LengthSamples";
        public static string LengthFrequencyByTrip = "LengthFrequencyByTrip";
        public static string SeaDayById = "SeaDayById";
        public static string SeaDays = "SeaDays";
        public static string Gear = "Gear";
        public static string WellContents = "WellContents";
        public static string PageCount = "PageCount";       
        public static string TripDetails = "TripDetails";
        public static string TripDefault = "TripDefault";
        public static string TripList = "TripList";
        public static string Default = "Default";

        // Fix for MVC bug (that's still not fixed in MVC4!)
        // http://stackoverflow.com/questions/780643/asp-net-mvc-html-actionlink-keeping-route-value-i-dont-want
        public static string MyTrips = "MyTrips";
        public static string MyOpenTrips = "MyOpenTrips";
        public static string CreateTrip = "CreateTrip";

        public const string IsPositiveInteger = @"^\d+$";

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapReportingRoute(); // TODO Check that this doesn't break anything

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

            routes.MapRoute(
                name: Electronics,
                url: "Trip/{tripId}/Electronics/{action}",
                defaults: new { controller = "Electronics", action = "List" },
                constraints: new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                SafetyInspection,
                "Trip/{tripId}/SafetyInspection/{action}",
                new { controller = "SafetyInspection", action = "Index" },
                new { tripId = IsPositiveInteger }
            );

            routes.MapRoute(
                Crew,
                "Trip/{tripId}/Crew/{action}",
                new { controller = "Crew", action = "Index" },
                new { tripId = IsPositiveInteger }
            );

            // For now, route vessel attributes through Trip since there's a 1:1 relationship
            // and it's PS only.
            routes.MapRoute(
                Auxiliaries,
                "Trip/{tripId}/Auxiliaries/{action}",
                new { controller = "Auxiliaries", action = "Index" },
                new { tripId = IsPositiveInteger }
            );

            // As with vessel attributes, hang this off of Trip
            // Alternately, we could hang this off Vessel and allow users to see the progression of
            // attributes/details over time.
            // Second alternate is to move attributes and details into a ViewModel and handle them
            // elsewhere.
            routes.MapRoute(
                VesselDetails,
                "Trip/{tripId}/VesselDetails/{action}",
                new { controller = "VesselDetails", action = "Index" },
                new { tripId = IsPositiveInteger }
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

            // TODO This is going to be tricky with the Gen2Details routing
            // TODO Change the default action to Index 
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

            // TODO Change the default action to Index
            routes.MapRoute(
                Gen6,
                "Trip/{tripId}/GEN-6/{action}/{pageNumber}",
                new { controller = "Gen6", action = "List", pageNumber = UrlParameter.Optional },
                new { tripId = IsPositiveInteger, pageNumber = IsPositiveInteger }
            );

            // Even though Set is subordinate to day, allow link directly to list of sets
            // and to a particular set number
            routes.MapRoute(
                name: Sets,
                url: "Trip/{tripId}/Sets/{setNumber}/{action}",
                defaults: new { controller = "FishingSet", action = "List", setNumber = UrlParameter.Optional },
                constraints: new { tripId = IsPositiveInteger, setNumber = IsPositiveInteger }
            );

            /*
            routes.MapRoute(
                SetDetails,
                "Trip/{tripId}/Sets/{setNumber}/{action}",
                new { controller = "FishingSet", action = "Index" },
                new { tripId = @"\d+", setNumber = @"\d+" }
            );

            routes.MapRoute(
                Sets,
                "Trip/{tripId}/Sets/",
                new { controller = "FishingSet", action = "List" },
                new { tripId = @"\d+" }
            );
            */

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

            /*
            // dayNumber is not an ID, it's a number between 1 and the number of sea days in the trip
            // FIXME:  Add another route that gets directly to SeaDay by Id
            routes.MapRoute(
                SeaDayDetails,
                "Trip/{tripId}/Days/{dayNumber}",
                new { controller = "SeaDay", action = "Index" },
                new { tripId = @"\d+", dayNumber = @"\d+" }
            );

            routes.MapRoute(
                SeaDayViewModel,
                "Trip/{tripId}/EditDay/{dayNumber}",
                new { controller = "SeaDay", action = "EditDay" },
                new { tripId = @"\d+", dayNumber = @"\d+" }
            );

            routes.MapRoute(
                SeaDays,
                "Trip/{tripId}/Days/{action}",
                new { controller = "SeaDay", action = "List" },
                new { tripId = @"\d+" }
            );
            */
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

            // Trip/{tripId}/Days/{dayNumber}/{action}
            // action in (Index, Edit, Add)

            // Trip/{tripId}/DayById/{dayId}/{action}
            // action in (Index, Edit)

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

            // Milestone!  First Long Line route!
            routes.MapRoute(
                name: SetHaul,
                url: "Trip/{tripId}/SetHaul/{setNumber}/{action}",
                defaults: new { controller = "SetHaul", action = "List", setNumber = UrlParameter.Optional },
                constraints: new { tripId = IsPositiveInteger, setNumber = IsPositiveInteger }
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