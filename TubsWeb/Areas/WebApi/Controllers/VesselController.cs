// -----------------------------------------------------------------------
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
    /// Read-only OData implementation for vessel entities.
    /// </summary>
    public class VesselController : ApiController
    {
        /// <summary>
        /// OData endpoint for listing/searching vessels.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("GET", "HEAD")]
        public PageResult<Vessel> Get(ODataQueryOptions<Vessel> options)
        {
            // Implementation from here:
            // http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options

            // TODO May want to limit additional query options
            ODataQuerySettings settings = new ODataQuerySettings()
            {
                PageSize = 15
            };

            var repo = TubsDataService.GetRepository<Vessel>(MvcApplication.CurrentSession);
            IQueryable results = options.ApplyTo(repo.All(), settings);

            // Not sure how to get the appropriate count
            // what is wanted is repo.All().Where(???).Count();
            return new PageResult<Vessel>(
                results as IEnumerable<Vessel>,
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
        public Vessel GetVessel(int id)
        {
            var repo = TubsDataService.GetRepository<Vessel>(MvcApplication.CurrentSession);
            var vessel = repo.FindById(id);
            if (null == vessel)
            {
                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(response);
            }
            return vessel;
        }
    }
}
