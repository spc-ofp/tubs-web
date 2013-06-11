// -----------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Http;
    using System.Web.Http.OData.Builder;
    using Microsoft.Data.Edm;

    /// <summary>
    /// Configure WebApi support, including OData metadata
    /// </summary>
    public static class WebApiConfig
    {
        // OData routes go into the same table as MVC routes, so ensure uniqueness
        public static string SeaDays = "ApiSeaDays";

        /// <summary>
        /// Regular expression to enforce that a number is a positive integer value.
        /// </summary>
        public const string IsPositiveInteger = @"^\d+$";
        
        public static void Register(HttpConfiguration config)
        {
            // From here
            // http://blogs.msdn.com/b/webdev/archive/2013/01/29/getting-started-with-asp-net-webapi-odata-in-3-simple-steps.aspx
            config.Routes.MapODataRoute(
                routeName: "OData",
                routePrefix: "odata",
                model: GetModel()
            );

            // Support WCF
            // From this Gist, may not be operable
            // https://gist.github.com/bastervrugt/3917081
            // More info:
            // From this StackOverflow question
            // http://stackoverflow.com/questions/12971174/asp-net-web-api-wrong-odata-entitycontainer-schema-namespace
            //config.Routes.MapHttpRoute(
            //    "OData.$metadata", 
            //    "api/$metadata", 
            //    new { Controller = "ODataMetadata", Action = "GetMetadata" }
            //);

            //config.Routes.MapHttpRoute(
            //    "OData.servicedoc", 
            //    "api/", 
            //    new { Controller = "ODataMetadata", Action = "GetServiceDocument" }
            //);

            config.Routes.MapHttpRoute(
                name: SeaDays,
                routeTemplate: "api/trip/{tripId}/day/{dayNumber}",
                defaults: new { Controller = "SeaDay", dayNumber = RouteParameter.Optional },
                constraints: new { tripId = IsPositiveInteger, dayNumber = IsPositiveInteger } 
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
        }

        public static IEdmModel GetModel()
        {
            ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<TubsWeb.ViewModels.TripSummaryViewModel>("Trips");
            modelBuilder.EntitySet<Spc.Ofp.Tubs.DAL.Entities.Observer>("Observers");
            return modelBuilder.GetEdmModel();

        }

    }
}