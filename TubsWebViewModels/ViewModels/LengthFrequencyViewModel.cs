// -----------------------------------------------------------------------
// <copyright file="LengthFrequencyViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
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
    using Newtonsoft.Json;

    /// <summary>
    /// ViewModel for a single PS-4 form.
    /// TODO: This was migrated from the 'Model' folder of the MVC project and needs some
    /// work to come up to the standards of newer view model implementations.
    /// </summary>
    public class LengthFrequencyViewModel
    {
        #region Knockout lists
        public IList<string> BooleanValues = new List<string> { String.Empty, "YES", "NO" };

        public IList<string> SampleTypes = new List<string>()
        {
            String.Empty,
            "Grab",
            "Spill",
            "Other"
        };

        public IList<string> BrailNumbers = new List<string> { String.Empty, "Brail 1", "Brail 1" };

        public IList<string> OtherCodes = new List<string>()
        {
            String.Empty,
            "DA",
            "DT",
            "BA",
            "BY",
            "LY",
            "BS"
        };

        public IList<string> MeasuringInstruments = new List<string>
        {
            String.Empty,
            "Aluminum Caliper",
            "Measuring Board",
            "Deck Tape",
            "Other"
        };
        #endregion

        public LengthFrequencyViewModel()
        {
            this.Samples = new List<PurseSeineSampleViewModel>(120);
            this.Brails = new List<Brail>(30);
        }
        
        public int Id { get; set; }
        public int SetId { get; set; }
        public int SetNumber { get; set; }
        public int TripId { get; set; }
        public string TripNumber { get; set; }
        public int VersionNumber { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public string ActionName { get; set; }

        public string SampleType { get; set; }
        public string OtherCode { get; set; }
        public string ProtocolComments { get; set; }

        // Grab
        public int GrabTarget { get; set; }

        // Spill
        public int SpillBrailNumber { get; set; }
        public int SpillNumberFishMeasured { get; set; }

        // 'Brail 1' or 'Brail 2'
        public string WhichBrail { get; set; }

        public int? PageNumber { get; set; }
        public int? PageCount { get; set; }

        public string MeasuringInstrument { get; set; }
        public string CalibratedThisSet { get; set; }

        public int? FullBrailCount { get; set; }
        public int? SevenEighthsBrailCount { get; set; }
        public int? ThreeQuartersBrailCount { get; set; }
        public int? TwoThirdsBrailCount { get; set; }
        public int? OneHalfBrailCount { get; set; }
        public int? OneThirdBrailCount { get; set; }
        public int? OneQuarterBrailCount { get; set; }
        public int? OneEighthBrailCount { get; set; }
        public int? TotalBrails { get; set; } // Sum of Full, SevenEighths, etc.
        public decimal? SumOfAllBrails { get; set; }

        public int BrailId { get; set; } // Brail pattern is held in a separate table...
        
        public IList<Brail> Brails { get; set; }

        /// <summary>
        /// All measured fish.
        /// </summary>
        /// <remarks>
        /// With up to 120 items per PS-4 page, this is too big for a Knockout viewmodel.
        /// </remarks>
        [JsonIgnore]
        public List<PurseSeineSampleViewModel> Samples { get; set; }

        [JsonIgnore]
        public bool IsEditor
        {
            get
            {
                return
                    "Add".Equals(ActionName, StringComparison.InvariantCultureIgnoreCase) ||
                    "Edit".Equals(ActionName, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Inner class representing a single brail brought onboard during
        /// a set.
        /// </summary>
        public sealed class Brail
        {
            /// <summary>
            /// Brail number.  This is the sequence number for when the
            /// brail was brought onboard the vessel.
            /// </summary>
            [Range(1, 30)]
            public int Number { get; set; }
            
            /// <summary>
            /// Fullness code (e.g. 1 = Full, 8 = 1/8th full)
            /// </summary>
            [Range(1, 8)]
            public int? Fullness { get; set; }

            /// <summary>
            /// Number of samples collected from this brail.
            /// </summary>
            [Range(0, 100)]
            public int? Samples { get; set; }
        }
    }
}
