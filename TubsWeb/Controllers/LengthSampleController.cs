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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using AutoMapper;
    using DoddleReport;
    using DoddleReport.Web;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
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

        internal ReportResult AllSamples(PurseSeineTrip trip)
        {
            var repo = TubsDataService.GetRepository<LengthSample>(MvcApplication.CurrentSession);
            var samples = repo.FilterBy(s => s.Header.Set.Activity.Day.Trip.Id == trip.Id);

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
            report.TextFields.Title = string.Format("Length frequency summary for trip {0}", trip.SpcTripNumber);
            return new ReportResult(report);
        }

        internal ReportResult AllSamples(LongLineTrip trip)
        {
            var repo = TubsDataService.GetRepository<LongLineCatch>(MvcApplication.CurrentSession);
            var samples = repo.FilterBy(s => s.FishingSet.Trip.Id == trip.Id).OrderBy(s => s.FishingSet.SetNumber);

            // It would be cool if I could use LINQ like I do for Purse Seine, but NHibernate doesn't like the
            // cool LINQ project necessary to find the Lat/Lon for the start of set
            // Latitude = s.FishingSet.EventList.Where(sh => sh.ActivityType == Spc.Ofp.Tubs.DAL.Common.HaulActivityType.StartOfSet).First().Latitude

            IList<LengthSampleLineItem> items = new List<LengthSampleLineItem>(samples.Count());
            foreach (var sample in samples)
            {
                string latitude = String.Empty;
                string longitude = String.Empty;
                string eez = String.Empty;

                var startOfSet = sample.FishingSet.EventList.Where(sh => sh.ActivityType == Spc.Ofp.Tubs.DAL.Common.HaulActivityType.StartOfSet).FirstOrDefault();
                if (null != startOfSet)
                {
                    latitude = startOfSet.Latitude;
                    longitude = startOfSet.Longitude;
                    eez = startOfSet.EezCode;
                }
                items.Add(new LengthSampleLineItem() 
                { 
                    SetDate = sample.FishingSet.SetDate,
                    Latitude = latitude,
                    Longitude = longitude,
                    Eez = eez,
                    SetNumber = sample.FishingSet.SetNumber.HasValue ? sample.FishingSet.SetNumber.Value : -1,
                    SequenceNumber = sample.SampleNumber.HasValue ? sample.SampleNumber.Value : -1, // All 1 for obstrip_id 4177?
                    SpeciesCode = sample.SpeciesCode,
                    Length = sample.Length.HasValue ? sample.Length.Value : -1
                });
                
            }

            var report = new Report(items.ToReportSource());
            report.TextFields.Title = string.Format("Length frequency summary for trip {0}", trip.SpcTripNumber);
            return new ReportResult(report);
        }

        // GET: "Trip/{tripId}/LengthFrequency.xlsx" 
        public ReportResult AllSamples(Trip tripId)
        {
            return
                tripId is PurseSeineTrip ? AllSamples(tripId as PurseSeineTrip) :
                tripId is LongLineTrip ? AllSamples(tripId as LongLineTrip) :
                new ReportResult(new Report());
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
