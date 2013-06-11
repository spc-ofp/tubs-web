// -----------------------------------------------------------------------
// <copyright file="SeaDayController.cs" company="Secretariat of the Pacific Community">
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
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;

    /// <summary>
    /// 
    /// </summary>
    public class SeaDayController : ApiController
    {
        /// <summary>
        /// Get all days in a trip
        /// </summary>
        /// <param name="tripId">Trip primary key</param>
        /// <param name="options">OData options</param>
        /// <example>/api/trip/70/day?$orderby=ShipsDate+asc</example>
        /// <returns></returns>
        [AcceptVerbs("GET", "HEAD")]
        public PageResult<SeaDayViewModel> Get(int tripId, ODataQueryOptions<TripHeader> options)
        {
            var days = TubsDataService
                .GetRepository<PurseSeineSeaDay>(MvcApplication.CurrentSession)
                .FilterBy(d => d.Trip.Id == tripId);

            var dayViewModels =
                from day in days
                select Mapper.Map<PurseSeineSeaDay, SeaDayViewModel>(day);

            int inlineCount = dayViewModels.Count();
            IQueryable results = options.ApplyTo(dayViewModels);

            // Not sure how to get the appropriate count
            // what is wanted is repo.All().Where(???).Count();
            return new PageResult<SeaDayViewModel>(
                results as IEnumerable<SeaDayViewModel>,
                Request.GetNextPageLink(),
                inlineCount
            );
        }

        /// <summary>
        /// Get the seaday with the given position within the trip
        /// </summary>
        /// <param name="tripId"></param>
        /// /// <param name="dayNumber"></param>
        /// <example>/api/seaday/5</example>
        /// <returns>A single SeaDay</returns>
        [AcceptVerbs("GET", "HEAD")]
        public SeaDayViewModel GetDay(int tripId, int dayNumber)
        {
            var days = TubsDataService
                .GetRepository<SeaDay>(MvcApplication.CurrentSession)
                .FilterBy(d => d.Trip.Id == tripId);

            int maxDays = days.Count();
            // Unlike the regular controller version, this one will bail
            // if maxDays is outside the number of days in the trip
            if (dayNumber > maxDays)
            {
                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(response);
            }

            var day = days.Skip(dayNumber - 1).Take(1).FirstOrDefault() as PurseSeineSeaDay;
            if (null == day)
            {
                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(response);
            }

            var sdvm = Mapper.Map<PurseSeineSeaDay, SeaDayViewModel>(day);
            // Set the few properties on sdvm that aren't set by AutoMapper
            sdvm.SetNavDetails(dayNumber, maxDays);
            return sdvm;
        }
    }
}
