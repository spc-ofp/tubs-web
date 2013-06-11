// -----------------------------------------------------------------------
// <copyright file="TripController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers.Api
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
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.OData;
    using System.Web.Http.OData.Query;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;
    using AutoMapper;
    
    /// <summary>
    /// 
    /// </summary>
    public class TripController : ApiController
    {
        /*
         * Consider this implementation for CORS/OPTIONS support
         * http://blogs.msdn.com/b/carlosfigueira/archive/2012/02/21/implementing-cors-support-in-asp-net-web-apis-take-2.aspx
         * More here
         * http://www.jefclaes.be/2012/09/supporting-options-verb-in-aspnet-web.html
         * Or this
         * http://www.strathweb.com/2013/03/adding-http-head-support-to-asp-net-web-api/
         * Final bit
         * http://zacstewart.com/2012/04/14/http-options-method.html
         */
       

        /// <summary>
        /// Get all trips
        /// </summary>
        /// <remarks>Default page size is 15 items</remarks>
        /// <param name="options">OData options</param>
        /// <example>/api/trip?filter=ProgramCode+le+PGOB&$orderby=DepartureDate+desc</example>
        /// <returns></returns>
        [AcceptVerbs("GET", "HEAD")]
        public PageResult<TripHeader> Get(ODataQueryOptions<TripHeader> options)
        {
            // Implementation from here:
            // http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options
            
            // TODO May want to limit additional query options
            ODataQuerySettings settings = new ODataQuerySettings()
            {
                PageSize = 15
            };

            var repo = TubsDataService.GetRepository<TripHeader>(MvcApplication.CurrentSession);
            IQueryable results = options.ApplyTo(repo.All(), settings);

            // Not sure how to get the appropriate count
            // what is wanted is repo.All().Where(???).Count();
            return new PageResult<TripHeader>(
                results as IEnumerable<TripHeader>,
                Request.GetNextPageLink(),
                Request.GetInlineCount()
            );
        }

        /// <summary>
        /// Get the trip with the specified obstrip_id
        /// </summary>
        /// <param name="id"></param>
        /// <example>/api/trip/5</example>
        /// <returns>A single Trip</returns>
        [AcceptVerbs("GET", "HEAD")]
        public TripSummaryViewModel GetTrip(int id)
        {
            var repo = TubsDataService.GetRepository<Trip>(MvcApplication.CurrentSession);
            var trip = repo.FindById(id);
            if (null == trip)
            {
                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(response);
            }
            return Mapper.Map<Trip, TripSummaryViewModel>(trip);
        }
    }
}
