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
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LengthFrequencyViewModel
    {
        public LengthFrequencyViewModel()
        {
            this.Brails = new List<Brail>(30);
        }
        
        public int Id { get; set; }
        public int SetId { get; set; }
        public int TripId { get; set; }
        public string TripNumber { get; set; }
        public int VersionNumber { get; set; }

        // Grab
        public int GrabTarget { get; set; }

        // Spill
        public int SpillBrailNumber { get; set; }
        public int SpillNumberFishMeasured { get; set; }

        // Other
        public string OtherCode { get; set; }

        // Play with this -- whatever works with a nullable radio
        public bool IsBrail1 { get; set; }
        public bool IsBrail2 { get; set; }

        public string StartBrailingTime { get; set; }
        public string EndBrailingTime { get; set; }

        public int? SamplePageNumber { get; set; }
        public int? SamplePageTotal { get; set; }

        public string MeasuringInstrument { get; set; }
        public string CalibratedThisSet { get; set; }

        public string ProtocolComments { get; set; }

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
        public List<Brail> Brails { get; set; }
        public List<Sample> Samples { get; set; }

        public class Brail
        {
            public int? Number { get; set; } // 1-30
            public int? Fullness { get; set; } // Code
            public int? Samples { get; set; }
        }

        public class Sample
        {
            public int Id { get; set; }
            public string SpeciesCode { get; set; }
            public int? Length { get; set; }
        }
    }
}
