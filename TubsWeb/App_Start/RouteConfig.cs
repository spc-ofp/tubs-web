// -----------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb
{
    using System.Web.Mvc;
    using System.Web.Routing;
    
    public class RouteConfig
    {
        // TODO Add documentation to all these strings
        public static string Electronics = "Electronics";
        public static string SafetyInspection = "SafetyInspection";
        public static string Crew = "Crew";
        public static string Auxiliaries = "Auxiliaries";
        public static string VesselDetails = "VesselDetails";
        public static string Gen1 = "Gen1";
        public static string Gen2Details = "Gen2Details";
        public static string Gen2 = "Gen2";
        public static string Gen3 = "Gen3";
        public static string Gen6Details = "Gen6Details";
        public static string Gen6 = "Gen6";
        //public static string SetDetails = "SetDetails";
        public static string Sets = "Sets";
        public static string LengthSamples = "LengthSamples";
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

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

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
                constraints: new { tripId = @"\d+" }
            );

            routes.MapRoute(
                SafetyInspection,
                "Trip/{tripId}/SafetyInspection/{action}",
                new { controller = "SafetyInspection", action = "Index" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                Crew,
                "Trip/{tripId}/Crew/{action}",
                new { controller = "Crew", action = "Index" },
                new { tripId = @"\d+" }
            );

            // For now, route vessel attributes through Trip since there's a 1:1 relationship
            // and it's PS only.
            routes.MapRoute(
                Auxiliaries,
                "Trip/{tripId}/Auxiliaries/{action}",
                new { controller = "Auxiliaries", action = "Index" },
                new { tripId = @"\d+" }
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
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                Gen1,
                "Trip/{tripId}/GEN-1/{action}",
                new { controller = "Gen1", action = "Index" },
                new { tripId = @"\d+" }
            );

            // A trip has zero to n GEN-2 forms, and each GEN-2 has a "Page X of Y"
            // field...
            routes.MapRoute(
                Gen2Details,
                "Trip/{tripId}/GEN-2/{pageNumber}/{action}",
                new { controller = "Gen2", action = "Index" },
                new { tripId = @"\d+", pageNumber = @"\d+" }
            );

            // TODO This is going to be tricky with the Gen2Details routing
            // TODO Change the default action to Index 
            routes.MapRoute(
                Gen2,
                "Trip/{tripId}/GEN-2/{action}",
                new { controller = "Gen2", action = "List" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                Gen3,
                "Trip/{tripId}/GEN-3/{action}",
                new { controller = "Gen3", action = "Index" },
                new { tripId = @"\d+" }
            );

            // Link to particular GEN-6 page has to come first
            // due to route precedence
            routes.MapRoute(
                Gen6Details,
                "Trip/{tripId}/GEN-6/{pageNumber}",
                new { controller = "Gen6", action = "Index" },
                new { tripId = @"\d+", pageNumber = @"\d+" }
            );

            // TODO Change the default action to Index
            routes.MapRoute(
                Gen6,
                "Trip/{tripId}/GEN-6/{action}/{pageNumber}",
                new { controller = "Gen6", action = "List", pageNumber = UrlParameter.Optional },
                new { tripId = @"\d+", pageNumber = @"\d+" }
            );

            // Even though Set is subordinate to day, allow link directly to list of sets
            // and to a particular set number
            routes.MapRoute(
                name: Sets,
                url: "Trip/{tripId}/Sets/{setNumber}/{action}",
                defaults: new { controller = "FishingSet", action = "List", setNumber = UrlParameter.Optional },
                constraints: new { tripId = @"\d+", setNumber = @"\d+" }
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
                new { tripId = @"\d+", setNumber = @"\d+", pageNumber = @"\d+" }
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
                constraints: new { tripId = @"\d+", dayNumber = @"\d+" }
            );

            routes.MapRoute(
                name: SeaDayById,
                url: "Trip/{tripId}/DayById/{dayId}/{action}",
                defaults: new { controller = "SeaDay", action = "Index" },
                constraints: new { tripId = @"\d+", dayId = @"\d+" }
            );

            // Trip/{tripId}/Days/{dayNumber}/{action}
            // action in (Index, Edit, Add)

            // Trip/{tripId}/DayById/{dayId}/{action}
            // action in (Index, Edit)

            routes.MapRoute(
                Gear,
                "Trip/{tripId}/Gear/{action}",
                new { controller = "Gear", action = "Index" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                WellContents,
                "Trip/{tripId}/WellContent/{action}",
                new { controller = "WellContent", action = "Index" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                PageCount,
                "Trip/{tripId}/PageCount/{action}",
                new { controller = "PageCount", action = "Index" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                TripDetails,
                "Trip/{tripId}/{action}",
                new { controller = "Trip", action = "Details" },
                new { tripId = @"\d+" }
            );

            // Can this route replace the fairly generic routes?
            // Doesn't look like it...
            routes.MapRoute(
                TripDefault,
                "Trip/{tripId}/{controller}/{action}",
                new { action = "Index" },
                new { tripId = @"\d+" }
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