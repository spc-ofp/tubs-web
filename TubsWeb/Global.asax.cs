// -----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
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
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using NHibernate;
    using Spc.Ofp.Tubs.DAL;
    using TubsWeb.Core;
    
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        // Use the following string keys for stashing objects in the current request
        public const string ISessionKey = "current.session";
        public const string ITransactionKey = "current.transaction";
        public const string IsEnrolledInTransactionKey = "current.transaction-enrolled";

        public MvcApplication()
        {
            /*
             * This implementation is per Ayende:
             * http://ayende.com/blog/4101/do-you-need-a-framework
             * Essentially, what happens is that each Request gets a new Session on creation,
             * and when the request is finished, the Session is disposed of.  NHibernate Sessions
             * are very lightweight, so there's no significant overhead to this.
             */
            BeginRequest += delegate
            {
                if (!HttpContext.Current.Items.Contains(ISessionKey))
                {
                    CurrentSession = TubsDataService.GetSession();
                }
            };
            EndRequest += delegate
            {
                if (null != CurrentSession)
                {
                    CurrentSession.Dispose();
                    HttpContext.Current.Items.Remove(ITransactionKey);
                    HttpContext.Current.Items.Remove(ISessionKey);
                }
            };
        }

        public static ISession CurrentSession
        {
            get { return (ISession)HttpContext.Current.Items[ISessionKey]; }
            set { HttpContext.Current.Items[ISessionKey] = value; }
        }

        public static ITransaction CurrentTransaction
        {
            get { return (ITransaction)HttpContext.Current.Items[ITransactionKey]; }
            set { HttpContext.Current.Items[ITransactionKey] = value; }
        }

        public static bool IsEnrolledInTransaction
        {
            get { return (bool)HttpContext.Current.Items[IsEnrolledInTransactionKey];  }
            set { HttpContext.Current.Items[IsEnrolledInTransactionKey] = value; }
        }
        
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //An authorize attribute filter takes the place of allow roles in web.config
            //filters.Add(new AuthorizeAttribute { Roles = @"NOUMEA\OFP Users" });
            // The following filter is only for non-AD authentication
            filters.Add(new LogonAuthorize());
            filters.Add(new ElmahHandledErrorLoggerFilter());
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");            

            // TODO This should default to Index
            routes.MapRoute(
                "Electronics",
                "Trip/{tripId}/Electronics/{action}",
                new { controller = "Electronics", action = "List" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                "SafetyInspection",
                "Trip/{tripId}/SafetyInspection/{action}",
                new { controller = "SafetyInspection", action = "Index" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                "Crew",
                "Trip/{tripId}/Crew/{action}",
                new { controller = "Crew", action = "Index" },
                new { tripId = @"\d+" }
            );

            // For now, route vessel attributes through Trip since there's a 1:1 relationship
            // and it's PS only.
            routes.MapRoute(
                "Auxiliaries",
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
                "VesselDetails",
                "Trip/{tripId}/VesselDetails/{action}",
                new { controller = "VesselDetails", action = "Index" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                "Gen1",
                "Trip/{tripId}/GEN-1/{action}",
                new { controller = "Gen1", action = "Index" },
                new { tripId = @"\d+" }
            );

            // A trip has zero to n GEN-2 forms, and each GEN-2 has a "Page X of Y"
            // field...
            routes.MapRoute(
                "Gen2Details",
                "Trip/{tripId}/GEN-2/{pageNumber}/{action}",
                new { controller = "Gen2", action = "Index" },
                new { tripId = @"\d+", pageNumber = @"\d+" }
            );

            // TODO This is going to be tricky with the Gen2Details routing
            // TODO Change the default action to Index 
            routes.MapRoute(
                "Gen2",
                "Trip/{tripId}/GEN-2/{action}",
                new { controller = "Gen2", action = "List" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                "Gen3",
                "Trip/{tripId}/GEN-3/{action}",
                new { controller = "Gen3", action = "Index" },
                new { tripId = @"\d+" }
            );

            // Link to particular GEN-6 page has to come first
            // due to route precedence
            routes.MapRoute(
                "Gen6Details",
                "Trip/{tripId}/GEN-6/{pageNumber}",
                new { controller = "Gen6", action = "Index" },
                new { tripId = @"\d+", pageNumber = @"\d+" }
            );

            // TODO Change the default action to Index
            routes.MapRoute(
                "Gen6",
                "Trip/{tripId}/GEN-6/{action}/{pageNumber}",
                new { controller = "Gen6", action = "List", pageNumber = UrlParameter.Optional },
                new { tripId = @"\d+", pageNumber = @"\d+" }
            );

            // Even though Set is subordinate to day, allow link directly to list of sets
            // and to a particular set number
            routes.MapRoute(
                "SetDetails",
                "Trip/{tripId}/Sets/{setNumber}",
                new { controller = "FishingSet", action = "Index" },
                new { tripId = @"\d+", setNumber = @"\d+" }
            );

            routes.MapRoute(
                "Sets",
                "Trip/{tripId}/Sets/",
                new { controller = "FishingSet", action = "List" },
                new { tripId = @"\d+" }
            );

            // Although length samples are subordinate to Sets, they'll be available at a higher level
            // for a more readable URL.
            routes.MapRoute(
                "LengthSamples",
                "Trip/{tripId}/Samples/{setNumber}/Page/{pageNumber}",
                new { controller = "LengthSample", action = "Index", pageNumber = UrlParameter.Optional },
                new { tripId = @"\d+", setNumber = @"\d+", pageNumber = @"\d+" }
            );


            // dayNumber is not an ID, it's a number between 1 and the number of sea days in the trip
            // FIXME:  Add another route that gets directly to SeaDay by Id
            routes.MapRoute(
                "SeaDayDetails",
                "Trip/{tripId}/Days/{dayNumber}",
                new { controller = "SeaDay", action = "Index" },
                new { tripId = @"\d+", dayNumber = @"\d+" }
            );

            routes.MapRoute(
                "SeaDays",
                "Trip/{tripId}/Days/{action}",
                new { controller = "SeaDay", action = "List" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                "Gear",
                "Trip/{tripId}/Gear/{action}",
                new { controller = "Gear", action = "Index" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                "WellContents",
                "Trip/{tripId}/WellContent/{action}",
                new { controller = "WellContent", action = "Index" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                "PageCount",
                "Trip/{tripId}/PageCount/{action}",
                new { controller = "PageCount", action = "Index" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                "TripDetails",
                "Trip/{tripId}/{action}",
                new { controller = "Trip", action = "Details" },
                new { tripId = @"\d+" }
            );

            // Can this route replace the fairly generic routes?
            // Doesn't look like it...
            routes.MapRoute(
                "TripDefault",
                "Trip/{tripId}/{controller}/{action}",
                new { action = "Index" },
                new { tripId = @"\d+" }
            );

            routes.MapRoute(
                "TripList",
                "Trip/",
                new { controller = "Trip", action = "Index" }
            );

            // TODO Wire up JSON outputs in controllers
            routes.MapRoute(
                "ApiRoute",
                "api/{controller}/{action}",
                new { controller = "Trip", action = "Index" } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            // Trip is the only model worth binding.
            ModelBinderProviders.BinderProviders.Add(new TripModelBinderProvider());
            // Turns out DateTime needs binding too for client workstation installed culture issues
            //ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            // Hard to get Log4net working if you don't call this -- D'Oh!
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}