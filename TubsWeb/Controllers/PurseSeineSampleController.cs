// -----------------------------------------------------------------------
// <copyright file="PurseSeineSampleController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers
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
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    public class PurseSeineSampleController : SuperController
    {
        /// <summary>
        /// Get samples for a trip.
        /// </summary>
        /// <remarks>
        /// The number of parameters required to get here in a truly RESTful manner
        /// was getting crazy, so in this instance we're skipping to using ID (which is
        /// still sorta restful). 
        /// </remarks>
        /// <param name="headerId">Sampling header primary key</param>
        /// <param name="offset">Offset within the current page</param>
        /// <returns></returns>
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Index(Trip tripId, int headerId, int offset)
        {
            // offset should be 0, 20, 40, 60, 80, or 100
            // anything else and we should puke here
            var repo = TubsDataService.GetRepository<LengthSamplingHeader>(MvcApplication.CurrentSession);
            var header = repo.FindById(headerId);
            // TODO: What happens when header is not found?

            // Second check -- make sure this header is in this trip

            var vm = new SampleColumnViewModel();
            vm.Id = header.Id;
            vm.Offset = offset;
            int maxSequence = offset + 20;
            var samples = header.Samples.Where(s => null != s && s.SequenceNumber > offset && s.SequenceNumber <= maxSequence).Take(20);
            foreach (var sample in samples.OrderBy(s => s.SequenceNumber))
            {
                // First subtraction to get us into the right place within the column
                // Second subtraction moves us from a one based offset to a zero based offset
                var position = (sample.SequenceNumber.Value - offset) - 1;
                if (position < 0 || position > 19)
                {
                    Logger.ErrorFormat("Sample with Id {0} has a strange offset", sample.Id);
                    continue;
                }
                vm.Samples[position] = new PurseSeineSampleViewModel()
                {
                    Id = sample.Id,
                    Number = sample.SequenceNumber.Value,
                    SpeciesCode = sample.SpeciesCode,
                    Length = sample.Length
                };
                    
            }
            return View(vm);
        }

        [HttpPost]
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        [HandleTransactionManually]
        public JsonResult Edit(Trip tripId, int headerId, SampleColumnViewModel scvm)
        {
            throw new NotImplementedException();
        }

    }
}
