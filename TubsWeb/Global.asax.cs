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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using NHibernate;
    using Spc.Ofp.Tubs.DAL;
    
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
             * are very lightweight, so there's no overhead to this.
             */
            BeginRequest += delegate
            {
                CurrentSession = TubsDataService.GetSession();
            };
            EndRequest += delegate
            {
                if (null != CurrentSession)
                {
                    CurrentSession.Dispose();
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
            
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Electronics",
                "Trip/Details/{id}/Electronics/",
                new { controller = "Electronics", action = "List" }
            );

            routes.MapRoute(
                "Crew",
                "Trip/Details/{id}/Crew/",
                new { controller = "Crew", action = "Index" }
            );

            routes.MapRoute(
                "Gen1",
                "Trip/Details/{id}/GEN-1/",
                new { controller = "Gen1", action = "Index" }
            );

            // FIXME This should probably be changed to look more like sets or days
            // in that we don't expose the primary key of the item.
            // A trip has zero to n GEN-2 forms, and each GEN-2 has a "Page X of Y"
            // field...
            routes.MapRoute(
                "Gen2Details",
                "Trip/Details/{id}/GEN-2/Interaction/{interactionId}",
                new { controller = "Gen2", action = "Details" }
            );

            routes.MapRoute(
                "Gen2",
                "Trip/Details/{id}/GEN-2/",
                new { controller = "Gen2", action = "List" }
            );

            routes.MapRoute(
                "Gen3",
                "Trip/Details/{id}/GEN-3/",
                new { controller = "Gen3", action = "Index" }
            );

            // Link to particular GEN-6 page has to come first
            // due to route precedence
            routes.MapRoute(
                "Gen6Details",
                "Trip/Details/{id}/GEN-6/{pageNumber}",
                new { controller = "Gen6", action = "Index" }
            );

            routes.MapRoute(
                "Gen6",
                "Trip/Details/{id}/GEN-6/",
                new { controller = "Gen6", action = "List" }
            );

            // Even though Set is subordinate to day, allow link directly to list of sets
            // and to a particular set number
            routes.MapRoute(
                "SetDetails",
                "Trip/Details/{id}/Sets/{setNumber}",
                new { controller = "FishingSet", action = "Index" }
            );

            routes.MapRoute(
                "Sets",
                "Trip/Details/{id}/Sets/",
                new { controller = "FishingSet", action = "List" }
            );


            // dayNumber is not an ID, it's a number between 1 and the number of sea days in the trip
            // FIXME:  Add another route that gets directly to SeaDay by Id
            routes.MapRoute(
                "SeaDayDetails",
                "Trip/Details/{id}/Days/{dayNumber}",
                new { controller = "SeaDay", action = "Index" }
            );

            routes.MapRoute(
                "SeaDays",
                "Trip/Details/{id}/Days/",
                new { controller = "SeaDay", action = "List" }
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

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}