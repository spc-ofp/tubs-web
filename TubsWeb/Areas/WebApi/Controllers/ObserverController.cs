﻿// -----------------------------------------------------------------------
// <copyright file="ObserverController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Areas.WebApi.Controllers
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.OData;
    using System.Web.Http.OData.Query;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;

    /// <summary>
    /// Read-only OData implementation for observer entities.
    /// </summary>
    /// <remarks>
    /// Initially, this was implemented using the EntitySetController in
    /// the Microsoft OData stack, but that had some issues.  This works,
    /// so we'll run with it.
    /// </remarks>
    public class ObserverController : ApiController
    {
        /// <summary>
        /// OData endpoint for listing/searching observers.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("GET", "HEAD")]
        public PageResult<Observer> Get(ODataQueryOptions<Observer> options)
        {
            // Implementation from here:
            // http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options

            // TODO May want to limit additional query options
            ODataQuerySettings settings = new ODataQuerySettings()
            {
                PageSize = 15
            };

            var repo = TubsDataService.GetRepository<Observer>(MvcApplication.CurrentSession);
            IQueryable results = options.ApplyTo(repo.All(), settings);

            // Not sure how to get the appropriate count
            // what is wanted is repo.All().Where(???).Count();
            return new PageResult<Observer>(
                results as IEnumerable<Observer>,
                Request.GetNextPageLink(),
                Request.GetInlineCount()
            );
        }

        /// <summary>
        /// OData endpoint for getting a specific observer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs("GET", "HEAD")]
        public Observer GetObserver(string id)
        {
            var repo = TubsDataService.GetRepository<Observer>(MvcApplication.CurrentSession);
            var observer = repo.FindById(id);
            if (null == observer)
            {
                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(response);
            }
            return observer;
        }
    }
}
