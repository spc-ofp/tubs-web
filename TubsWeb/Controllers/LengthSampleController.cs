// -----------------------------------------------------------------------
// <copyright file="LengthSampleController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
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
    using DoddleReport;
    using DoddleReport.Web;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using AutoMapper;
    using TubsWeb.ViewModels;

    public class LengthSampleController : SuperController
    {
        //
        // GET: "Trip/{tripId}/Samples/{setNumber}/Page/{pageNumber}",
        public ActionResult Index(Trip tripId, int setNumber, int? pageNumber)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }
            
            var repo = TubsDataService.GetRepository<LengthSamplingHeader>(MvcApplication.CurrentSession);
            pageNumber = pageNumber ?? 1; // Or should this be set as a default in the route?
            var header = repo.FilterBy(h => h.Set.Activity.Day.Trip.Id == tripId.Id && h.Set.SetNumber == setNumber && h.PageNumber == pageNumber).FirstOrDefault();
            var lfvm = Mapper.Map<LengthSamplingHeader, LengthFrequencyViewModel>(header);
            return View(lfvm);
        }

        public ReportResult AllSamples(Trip tripId)
        {
            var repo = TubsDataService.GetRepository<LengthSample>(MvcApplication.CurrentSession);
            var samples = repo.FilterBy(s => s.Header.Set.Activity.Day.Trip.Id == tripId.Id);

            var items =
                from s in samples
                orderby s.Header.Set.SetNumber
                select new LengthSampleLineItem
                {
                    SetDate = s.Header.Set.Activity.LocalTime,
                    Latitude = s.Header.Set.Activity.Latitude, // TODO Numeric
                    Longitude = s.Header.Set.Activity.Longitude, // TODO Numeric
                    Eez = s.Header.Set.Activity.EezCode,
                    Association = s.Header.Set.Activity.SchoolAssociation.HasValue ? s.Header.Set.Activity.SchoolAssociation.Value.ToString() : "Unknown",
                    SetNumber = s.Header.Set.SetNumber.HasValue ? s.Header.Set.SetNumber.Value : -1,
                    SequenceNumber = s.SequenceNumber.HasValue ? s.SequenceNumber.Value : -1,
                    SpeciesCode = s.SpeciesCode,
                    Length = s.Length.HasValue ? s.Length.Value : -1
                };

            var report = new Report(items.ToReportSource());
            report.TextFields.Title = string.Format("Length frequency summary for trip {0}", tripId.SpcTripNumber);
            return new ReportResult(report);
        }

        /// <summary>
        /// LengthSampleLineItem represents a single row of length-frequency data
        /// </summary>
        public class LengthSampleLineItem
        {
            public DateTime? SetDate { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Eez { get; set; }
            public string Association { get; set; }
            public int SetNumber { get; set; }
            public int SequenceNumber { get; set; }
            public string SpeciesCode { get; set; }
            public int Length { get; set; }
        }

    }
}
