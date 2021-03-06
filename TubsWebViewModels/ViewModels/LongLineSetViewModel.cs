﻿// -----------------------------------------------------------------------
// <copyright file="LongLineSetViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.ViewModels
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
    using Newtonsoft.Json;

    /// <summary>
    /// ViewModel for Long Line LL-2/3 form.
    /// </summary>
    public class LongLineSetViewModel
    {
        public LongLineSetViewModel()
        {
            this.IntermediateHaulPositions = new List<Position>(16);
            this.Baits = new List<Bait>(5);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setNumber"></param>
        /// <param name="maxSets"></param>
        public void SetNavDetails(int setNumber, int maxSets)
        {
            this.HasPrevious = setNumber > 1;
            this.HasNext = setNumber < maxSets;
            this.NextSet = setNumber + 1;
            this.PreviousSet = setNumber - 1;
        }

        // Use Knockout to help with common codes
        public IList<string> BooleanValues = new List<string> { null, "YES", "NO" };
        public IList<string> VelocityUnits = new List<string> { String.Empty, "m/s", "kts" };
        
        // UX state
        public string TripNumber { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public int VersionNumber { get; set; }
        public string ActionName { get; set; }

        public int TripId { get; set; }
        public int SetId { get; set; }

        // Sequence of this set during the trip
        public int SetNumber { get; set; }
        public int MaxSets { get; set; }
        public int NextSet { get; set; }
        public int PreviousSet { get; set; }

        public int? HooksPerBasket { get; set; }
        public int? TotalBaskets { get; set; }
        public int? TotalHooks { get; set; }
        public int? FloatlineLength { get; set; }

        public decimal? LineSettingSpeed { get; set; }
        public string LineSettingSpeedUnit { get; set; }
        public int? BranchlineSetInterval { get; set; }
        public decimal? DistanceBetweenBranchlines { get; set; }
        public decimal? BranchlineLength { get; set; }

        public decimal? VesselSpeed { get; set; }

        public int? SharkLineCount { get; set; }
        public int? SharkLineLength { get; set; }
        public string WasTdrDeployed { get; set; }

        public bool? IsTargetingTuna { get; set; }
        public bool? IsTargetingSwordfish { get; set; }
        public bool? IsTargetingShark { get; set; }

        // Mitigation (v2009)
        public string HasToriPoles { get; set; }
        public string HasBirdCurtain { get; set; }
        public string HasWeightedLines { get; set; }
        public string HasUnderwaterChute { get; set; }

        public List<Bait> Baits { get; set; }

        public int? LightStickCount { get; set; }

        public int? TotalObservedBaskets { get; set; }

        [Display(Name = "GEN-3 Event?")]
        public string HasGen3Event { get; set; }

        [Display(Name = "Diary Page")]
        public string DiaryPage { get; set; }


        // Full date/time for start of set
        public DateTime? ShipsDate { get; set; }

        [Display(Name = "Ship's Time")]
        [RegularExpression(
            @"^[0-2]\d[0-5]\d",
            ErrorMessage = "Ship's time must be a valid 24 hour time")]
        public string ShipsTime { get; set; }

        public DateTime? UtcDate { get; set; }
        [RegularExpression(
            @"^[0-2]\d[0-5]\d",
            ErrorMessage = "UTC time must be a valid 24 hour time")]
        public string UtcTime { get; set; }

        public string UnusualDetails { get; set; }

        // Were all "Start" and "End" positions observed directly?
        public string StartEndPositionsObserved { get; set; }

        public Position StartOfSet { get; set; }
        public Position EndOfSet { get; set; }

        public Position StartOfHaul { get; set; }
        public Position EndOfHaul { get; set; }
        public IList<Position> IntermediateHaulPositions { get; set; }

        public IList<Comment> Comments { get; set; }

        // Entries to be deleted will have a '_destroy' property of true
        [JsonIgnore]
        public IEnumerable<Position> KeeperPositions
        {
            get
            {
                if (null != this.StartOfSet && !this.StartOfSet._destroy)
                    yield return this.StartOfSet;

                if (null != this.EndOfSet && !this.EndOfSet._destroy)
                    yield return this.EndOfSet;

                if (null != this.StartOfHaul && !this.StartOfHaul._destroy)
                    yield return this.StartOfHaul;

                if (null != this.EndOfHaul && !this.EndOfHaul._destroy)
                    yield return this.EndOfHaul;

                foreach (var position in this.IntermediateHaulPositions)
                {
                    if (null != position && !position._destroy)
                    {
                        yield return position;
                    }
                }
            }
        }

        // Entries to be deleted will have a '_destroy' property of true
        [JsonIgnore]
        public IEnumerable<Position> DeletedPositions
        {
            get
            {
                if (null != this.StartOfSet && this.StartOfSet._destroy)
                    yield return this.StartOfSet;

                if (null != this.EndOfSet && this.EndOfSet._destroy)
                    yield return this.EndOfSet;

                if (null != this.StartOfHaul && this.StartOfHaul._destroy)
                    yield return this.StartOfHaul;

                if (null != this.EndOfHaul && this.EndOfHaul._destroy)
                    yield return this.EndOfHaul;

                foreach (var position in this.IntermediateHaulPositions)
                {
                    if (null != position && position._destroy)
                    {
                        yield return position;
                    }
                }
            }
        }

        [JsonIgnore]
        public IEnumerable<Comment> KeeperComments
        {
            get
            {
                foreach (var comment in this.Comments)
                {
                    if (null != comment && !comment._destroy)
                    {
                        yield return comment;
                    }
                }
            }
        }

        [JsonIgnore]
        public IEnumerable<Comment> DeletedComments
        {
            get
            {
                if (null == this.Comments || 0 == this.Comments.Count)
                    return Enumerable.Empty<Comment>();
                return this.Comments.Where(c => null != c && c._destroy);
            }
        }

        // Bait data is stored in the set table using columns with numeric suffixes
        // There should really be an l_bait table that links back to l_set_id, but
        // I'm not going to go there -- my replacement can deal with it
        public class Bait
        {
            public string Species { get; set; }
            public int? Weight { get; set; }
            public string Hooks { get; set; }
            public string DyedBlue { get; set; }

            [JsonIgnore]
            public bool Show
            {
                get
                {
                    return !String.IsNullOrEmpty(this.Species) ||
                           this.Weight.HasValue ||
                           !String.IsNullOrEmpty(this.DyedBlue) ||
                           !String.IsNullOrEmpty(this.Hooks);
                           
                }
            }
        }

        /// <summary>
        /// Class representing a single Set/Haul position line item.
        /// </summary>
        public class Position
        {
            public int Id { get; set; }

            public DateTime? DateOnly { get; set; }

            [RegularExpression(
                @"^(20|21|22|23|[01]\d)[0-5]\d$",
                ErrorMessage = "Local time must be a valid 24 hour time")]
            public string LocalTime { get; set; }

            [RegularExpression(
                @"^[0-8]\d{3}\.?\d{3}[NnSs]$",
                ErrorMessage = "Latitude must be of the form ddmm.mmmN or ddmm.mmmS")]
            public string Latitude { get; set; }

            [RegularExpression(
                @"^[0-1]\d{4}\.?\d{3}[EeWw]$",
                ErrorMessage = "Longitude must be of the form dddmm.mmmE or dddmm.mmmW")]
            public string Longitude { get; set; }

            [StringLength(2, ErrorMessage = "EEZ Code must be 2 characters")]
            public string EezCode { get; set; }
            public bool _destroy { get; set; } // Knockout UX
            public bool NeedsFocus { get; set; } // Knockout UX

            /// <summary>
            /// It would be better if the client managed this for us, removing the
            /// placeholder entities that, if missing, cause the Knockout DOM binding
            /// to puke all over itself.
            /// </summary>
            [JsonIgnore]
            public bool IsEmpty
            {
                get
                {
                    // DateOnly is going to be filled automatically, so
                    // having a value is no indicator that this record
                    // needs saving
                    return
                        0 == this.Id &&
                        String.IsNullOrEmpty(this.LocalTime) &&
                        String.IsNullOrEmpty(this.Latitude) &&
                        String.IsNullOrEmpty(this.Longitude) &&
                        String.IsNullOrEmpty(this.EezCode);
                }
            }
        }

        /// <summary>
        /// Class representing a single Set/Haul comment line item.
        /// </summary>
        public class Comment
        {
            public int Id { get; set; }

            public DateTime? DateOnly { get; set; }

            [RegularExpression(
                @"^(20|21|22|23|[01]\d)[0-5]\d$",
                ErrorMessage = "Local time must be a valid 24 hour time")]
            public string LocalTime { get; set; }
            public string Details { get; set; }
            public bool _destroy { get; set; } // Knockout UX
            public bool NeedsFocus { get; set; } // Knockout UX

            /// <summary>
            /// It would be better if the client managed this for us, removing the
            /// placeholder entities that, if missing, cause the Knockout DOM binding
            /// to puke all over itself.
            /// </summary>
            [JsonIgnore]
            public bool IsEmpty
            {
                get
                {
                    // DateOnly is going to be filled automatically, so
                    // having a value is no indicator that this record
                    // needs saving
                    return
                        0 == this.Id &&
                        String.IsNullOrEmpty(this.LocalTime) &&
                        String.IsNullOrEmpty(this.Details);
                }
            }
        }

    }
}
