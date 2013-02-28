// -----------------------------------------------------------------------
// <copyright file="SeaDayViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Models
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
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Newtonsoft.Json;

    /// <summary>
    /// ViewModel for PS-2 data.
    /// VERY IMPORTANT NOTE!!!!!!!!
    /// DON'T USE ARRAYS IN A VIEW MODEL!!!!!
    /// The default model binder freaks out when it trys to fill an array, and the
    /// stack trace is worthless.
    /// 
    /// Second Note:  Dates are crazy in MVC when you don't use WebApi
    /// Although this has been fixed for now, here's a note about using attributes to control JSON
    /// serialization.
    /// http://stackoverflow.com/questions/10527001/asp-net-mvc-controller-json-datetime-serialization-vs-newtonsoft-json-datetime-s
    /// 
    /// Third Note:  Properties added/removed from root level view model are automatically managed
    /// by knockout.mapping plugin.  Properties added to SeaDayEvent are _NOT_ managed and need
    /// to be reflected in the vm.seaday.js file.  Failure to do so will result in LINQ errors
    /// </summary>
    public class SeaDayViewModel
    {
        #region v2009 Activity Codes
        public static IList<string> v2009ActivityCodes = new List<string>()
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10D",
            "10R",
            "11",
            "12",
            "13",
            "14",
            "15R",
            "15D",
            "16",
            "17",
            "H1",
            "H2"
        };
        #endregion

        #region v2007 Activity Codes
        public static IList<string> v2007ActivityCodes = new List<string>()
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10D",
            "10R",
            "11",
            "12",
            "13",
            "14",
            "15R",
            "15D",
            "16",
            "H1",
            "H2"
        };
        #endregion

        public SeaDayViewModel()
        {
            Events = new List<SeaDayEvent>(8);
            DetectionCodes = new List<int?>() { null, 1, 2, 3, 4, 5, 6, 7 };
            AssociationCodes = new List<int?>() { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SeaCodes = new List<string>() { string.Empty, "C", "S", "M", "R", "V" };
        }

        // UX state
        public string TripNumber { get; set; }
        public bool HasNext { get; set; }
        public int PreviousDay { get; set; }
        public bool HasPrevious { get; set; }
        public int NextDay { get; set; }
        public int VersionNumber { get; set; }
        public string ActionName { get; set; }

        // Again UX state, to let the display path know if
        // this object exists in the database.
        [JsonIgnore]
        public bool IsEmpty
        {
            get
            {
                return
                    (null == Events) || (Events.Count == 0) &&
                    !ShipsDate.HasValue;
            }
        }

        // This is dynamic, as the list changes based on version
        public IList<string> ActivityCodes { get; set; }

        // No change between 2007 and 2009
        public IList<int?> DetectionCodes { get; set; }
        public IList<int?> AssociationCodes { get; set; }
        public IList<string> SeaCodes { get; set; }

        public int TripId { get; set; }
        public int DayId { get; set; }
        public int DayNumber { get; set; }
        public int MaxDays { get; set; }
        
        [Required]
        [Display(Name = "Ship's Date")]
        public DateTime? ShipsDate { get; set; }

        [Required]
        [Display(Name = "Ship's Time")]
        [RegularExpression(
            @"^[0-2]\d[0-5]\d",
            ErrorMessage = "Ship's time must be a valid 24 hour time")]
        public string ShipsTime { get; set; }

        [Display(Name = "UTC Date")]
        public DateTime? UtcDate { get; set; }

        [Display(Name = "UTC Time")]
        [RegularExpression(
            @"^[0-2]\d[0-5]\d",
            ErrorMessage = "UTC time must be a valid 24 hour time")]
        public string UtcTime { get; set; }

        [Display(Name = "Anchored with NO school")]
        [Range(0, Int32.MaxValue)]
        public int? AnchoredWithNoSchool { get; set; }

        [Display(Name = "Anchored with school")]
        [Range(0, Int32.MaxValue)]
        public int? AnchoredWithSchool { get; set; }

        [Display(Name = "Free floating with NO school")]
        [Range(0, Int32.MaxValue)]
        public int? FreeFloatingWithNoSchool { get; set; }

        [Display(Name = "Free floating with school")]
        [Range(0, Int32.MaxValue)]
        public int? FreeFloatingWithSchool { get; set; }

        [Display(Name = "Free Schools")]
        [Range(0, Int32.MaxValue)]
        public int? FreeSchool { get; set; }

        [Display(Name = "GEN-3 Event?")]
        public string HasGen3Event { get; set; }

        [Display(Name = "Diary Page")]
        public string DiaryPage { get; set; }

        public IList<SeaDayEvent> Events { get; protected set; }

        public bool NeedsDates()
        {
            return !this.ShipsDate.HasValue || !this.UtcDate.HasValue;
        }

        // Entries to be deleted will have a '_destroy' property of true
        [JsonIgnore]
        public IEnumerable<SeaDayEvent> Keepers
        {
            get
            {
                foreach (var evt in this.Events)
                {
                    if (null != evt && !evt._destroy)
                    {
                        yield return evt;
                    }
                }
                //return this.Events.Where(e => (e != null && !e._destroy)) ?? Enumerable.Empty<SeaDayEvent>();
            }
        }

        [JsonIgnore]
        public IEnumerable<SeaDayEvent> Deleted
        {
            get
            {
                return this.Events.Where(e => e != null && e._destroy);
            }
        }

        /// <summary>
        /// ViewModel for PS-2 line item.
        /// </summary>
        public class SeaDayEvent
        {
            public int EventId { get; set; }

            public int Gen5Id { get; set; }

            // Re-use the RoR integration in Knockout
            public bool _destroy { get; set; }
            
            [Required]
            [RegularExpression(
                @"^[0-2]\d[0-5]\d", 
                ErrorMessage = "Time must be a valid 24 hour time")]
            public string Time { get; set; }

            [Required]
            [RegularExpression(
                @"^[0-8]\d{3}\.?\d{3}[NnSs]$",
                ErrorMessage = "Latitude must be of the form ddmm.mmmN or ddmm.mmmS")]
            public string Latitude { get; set; }

            [Required]
            [RegularExpression(
                @"^[0-1]\d{4}\.?\d{3}[EeWw]$",
                ErrorMessage = "Longitude must be of the form dddmm.mmmE or dddmm.mmmW")]
            public string Longitude { get; set; }

            [StringLength(2, ErrorMessage = "EEZ Code must be 2 characters")]
            public string EezCode { get; set; }

            public string ActivityCode { get; set; }

            public int? WindSpeed { get; set; }

            [Range(0, 360)]
            public int? WindDirection { get; set; }

            [RegularExpression(
                @"[CcSsMmRrVv]",
                ErrorMessage = "Sea code must be C, S, M, R, or V")]
            public string SeaCode { get; set; }

            public int? DetectionCode { get; set; }

            public int? AssociationCode { get; set; }

            public string FadNumber { get; set; }

            public string BuoyNumber { get; set; }

            public string Comments { get; set; }

            // With the new GEN-5 form, it's possible
            // to have other records associated with
            // an activity.  So just use a generic
            // 'IsLocked' param.
            // Changing this to 'false' is enough
            // of a positive action
            public bool IsLocked { get; set; }

            public bool HasSet { get; set; }

            public bool HasGen5 { get; set; }

            // TODO:  Add any other dependent data record flags

            public bool NeedsFocus { get; set; }

            // Display only, so no need to include in JSON
            [JsonIgnore]
            public string ActivityIconPath { get; set; }

            // Display only, so no need to include in JSON
            [JsonIgnore]
            public string SpeedAndDirection
            {
                get
                {
                    return string.Format(
                        "{0} kts @ {1} &deg;",
                        (this.WindSpeed.HasValue ? this.WindSpeed.Value.ToString() : "UNK"),
                            (this.WindDirection.HasValue ? this.WindDirection.Value.ToString() : "UNK"));
                }
            }
        }
    }
}