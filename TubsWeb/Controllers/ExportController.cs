// -----------------------------------------------------------------------
// <copyright file="ExportController.cs" company="Secretariat of the Pacific Community">
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
    using DoddleReport;
    using DoddleReport.Web;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;

    /// <summary>
    /// ExportController manages a simple export of a trip out to
    /// Excel.
    /// </summary>
    public class ExportController : SuperController
    {
        
        internal decimal? Sum(decimal? lhs, decimal? rhs)
        {
            if (!lhs.HasValue && !rhs.HasValue)
                return null;

            decimal lhsv = lhs.HasValue ? lhs.Value : 0.0M;
            decimal rhsv = rhs.HasValue ? rhs.Value : 0.0M;
            return lhsv + rhsv;
        }

        internal ReportResult Summary(LongLineTrip trip)
        {
            return new ReportResult(new Report()); 
        }
        
        internal ReportResult Summary(PurseSeineTrip trip)
        {
            // Empty spreadsheet
            if (null == trip || null == trip.SeaDays || 0 == trip.SeaDays.Count)
                return new ReportResult(new Report());

            var activities =
                from d in trip.SeaDays
                from a in d.Activities
                select new ActivityLogLineItem
                {
                    ShipsTime = a.LocalTime.Value,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    Activity = a.ActivityType.HasValue ? a.ActivityType.Value.ToString() : String.Empty,
                    WindDirection = a.WindDirection,
                    WindSpeed = a.WindSpeed,
                    SeaCode = a.SeaCode.HasValue ? a.SeaCode.Value.ToString() : String.Empty,
                    Comments = a.Comments
                };

            var sets =
                from d in trip.SeaDays
                from a in d.Activities
                where a.ActivityType == ActivityType.Fishing && null != a.FishingSet
                select new SetLogLineItem
                {
                    SetNumber = a.FishingSet.SetNumber,
                    VesselLogDate = a.LocalTime.Value,
                    SkiffOff = a.FishingSet.SkiffOff,
                    WinchOn = a.FishingSet.WinchOn,
                    RingsUp = a.FishingSet.RingsUp,
                    BrailStart = a.FishingSet.BeginBrailing,
                    BrailEnd = a.FishingSet.EndBrailing,
                    EndOfSet = a.FishingSet.EndOfSet,
                    SumOfBrails = Sum(a.FishingSet.SumOfBrail1, a.FishingSet.SumOfBrail2), // Regular addition operator doesn't work as expected
                    TotalCatch = a.FishingSet.TotalCatch
                };

            // From here (see jshannon99 response of 9 November)
            // http://doddlereport.codeplex.com/discussions/283107
            var writer = new DoddleReport.OpenXml.ExcelReportWriter();
            var activityReport = new Report(activities.ToReportSource(), writer);
            var setReport = new Report(sets.ToReportSource(), writer);  
         
            // Set up some labels
            activityReport.TextFields.Title = trip.ToString();
            activityReport.RenderHints["SheetName"] = "Activity Log";
            setReport.RenderHints["SheetName"] = "Set Log";

            // TODO:  Consider adding confidentiality footers to the report
            activityReport.AppendReport(setReport);
            return new ReportResult(activityReport, writer);
        }

        public ReportResult Summary(Trip tripId)
        {
            return
                tripId is PurseSeineTrip ? Summary(tripId as PurseSeineTrip) :
                tripId is LongLineTrip ? Summary(tripId as LongLineTrip) :
                new ReportResult(new Report());
        }

        /// <summary>
        /// ActivityLogLineItem is an internal class to simplify
        /// Excel output.  DoddleReports picks up header names from
        /// the property name.
        /// TODO: Add association and beacon
        /// </summary>
        public class ActivityLogLineItem
        {
            public DateTime ShipsTime { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Activity { get; set; }
            public int? WindDirection { get; set; }
            public int? WindSpeed { get; set; }
            public string SeaCode { get; set; }
            public string Comments { get; set; }
        }

        /// <summary>
        /// SetLogLineItem is an internal class to simplify
        /// Excel output.  DoddleReports picks up header names from
        /// the property name.
        /// TODO: Replace SumOfBrails with brail count
        /// </summary>
        public class SetLogLineItem
        {
            public int? SetNumber { get; set; }
            public DateTime VesselLogDate { get; set; }
            public DateTime? SkiffOff { get; set; }
            public DateTime? WinchOn { get; set; }
            public DateTime? RingsUp { get; set; }
            public DateTime? BrailStart { get; set; }
            public DateTime? BrailEnd { get; set; }
            public DateTime? EndOfSet { get; set; }
            public decimal? SumOfBrails { get; set; }
            public decimal? TotalCatch { get; set; }
        }
    }
}