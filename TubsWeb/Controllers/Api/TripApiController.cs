// -----------------------------------------------------------------------
// <copyright file="TripApiController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers.Api
{
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
    
    public class TripApiController : ApiController
    {
        /*
        // Simple version
        // Tested with conditions /api/trip?filter=ProgramCode+le+PGOB&$orderby=DepartureDate+desc
        [Queryable(PageSize=15)]
        public IQueryable<TripHeader> Get()
        {
            var repo = TubsDataService.GetRepository<TripHeader>(MvcApplication.CurrentSession);
            return repo.All();
        }
        */

        // Implementation from here:
        // http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options
        public PageResult<TripHeader> Get(ODataQueryOptions<TripHeader> options)
        {
            // TODO May want to limit query options
            ODataQuerySettings settings = new ODataQuerySettings()
            {
                PageSize = 15
            };

            var repo = TubsDataService.GetRepository<TripHeader>(MvcApplication.CurrentSession);
            IQueryable results = options.ApplyTo(repo.All(), settings);

            return new PageResult<TripHeader>(
                results as IEnumerable<TripHeader>,
                Request.GetNextPageLink(),
                Request.GetInlineCount()
            );
        }

        [AcceptVerbs("GET", "HEAD")]
        public TripHeader GetTrip(int id)
        {
            var repo = TubsDataService.GetRepository<TripHeader>(MvcApplication.CurrentSession);
            var trip = repo.FindById(id);
            if (null == trip)
            {
                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(response);
            }
            return trip;
        }
    }
}
