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
        public bool HasPrevious { get; set; }
        public int VersionNumber { get; set; }

        // This is dynamic, as the list changes based on version
        public IList<string> ActivityCodes { get; set; }

        // No change between 2007 and 2009
        public IList<int?> DetectionCodes { get; set; }
        public IList<int?> AssociationCodes { get; set; }
        public IList<string> SeaCodes { get; set; }

        public int TripId { get; set; }
        public int DayId { get; set; }
        
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
        public bool? HasGen3Event { get; set; }

        [Display(Name = "Diary Page")]
        public string DiaryPage { get; set; }

        public IList<SeaDayEvent> Events { get; protected set; }

        /// <summary>
        /// ViewModel for PS-2 line item.
        /// </summary>
        public class SeaDayEvent
        {
            public int EventId { get; set; }

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

            public bool NeedsFocus { get; set; }

            public bool HasSet { get; set; }
        }
    }
}