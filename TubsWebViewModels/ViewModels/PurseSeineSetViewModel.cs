// -----------------------------------------------------------------------
// <copyright file="PurseSeineSetViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
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
    /// PS-3 form ViewModel.
    /// </summary>
    public class PurseSeineSetViewModel
    {
        public PurseSeineSetViewModel()
        {
            ByCatch = new List<SetCatch>(8);
            TargetCatch = new List<SetCatch>(16);
            //AllCatch = new List<SetCatch>(16);
        }

        // Use Knockout to help with common codes
        public IList<string> TargetSpecies = new List<string>() { "SKJ", "YFT", "BET" };        
        public IList<string> BooleanValues = new List<string> { String.Empty, "YES", "NO" };
        public IList<string> FateCodes = new List<string>
        {
            "RWW",
            "RHG",
            "RGG",
            "RPT",
            "RCC",
            "ROR",
            "RFR",
            "DFR",
            "DTS",
            "DGD",
            "DVF",
            "DUS",
            "DSD",
            "DWD",
            "DPQ",
            "DPA",
            "DPD",
            "DPU",
            "DOR",
            "ESC"
        };
        // TODO Not sure about ByCatch yet...
        
        // UX state
        public string TripNumber { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public int VersionNumber { get; set; }

        public int TripId { get; set; }
        public int ActivityId { get; set; }
        public int SetId { get; set; }

        // Sequence of this set during the trip
        public int SetNumber { get; set; }
        public int MaxSets { get; set; }
        public int NextSet { get; set; }
        public int PreviousSet { get; set; }

        public DateTime? LogbookDate { get; set; }
        public string LogbookTime { get; set; }

        public DateTime? SkiffOff { get; set; }

        // New DateOnly fields are to fix hole in PS-3 implementation
        // CLC 12/07/13
        public DateTime? WinchOnDateOnly { get; set; }
        public DateTime? RingsUpDateOnly { get; set; }
        public DateTime? BeginBrailingDateOnly { get; set; }
        public DateTime? EndBrailingDateOnly { get; set; }
        public DateTime? EndOfSetDateOnly { get; set; }

        [Display(Name = "Start of Set")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string SkiffOffTimeOnly { get; set; }

        [Display(Name = "Begin Pursing")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string WinchOnTimeOnly { get; set; }

        [Display(Name = "End Pursing")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string RingsUpTimeOnly { get; set; }

        [Display(Name = "Begin Brailing")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string BeginBrailingTimeOnly { get; set; }

        [Display(Name = "End Brailing")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string EndBrailingTimeOnly { get; set; }

        [Display(Name = "End of Set")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string EndOfSetTimeOnly { get; set; }

        public decimal? WeightOnboardObserved { get; set; }
        public decimal? WeightOnboardFromLog { get; set; }

        public decimal? RetainedTonnageObserved { get; set; }
        public decimal? RetainedTonnageFromLog { get; set; }

        public decimal? NewOnboardTotalObserved { get; set; }
        public decimal? NewOnboardTotalFromLog { get; set; }

        [Display(Name = "Tons of Tuna")]
        public decimal? TonsOfTunaObserved { get; set; }

        [Display(Name = "Sum of Brail 1")]
        public decimal? SumOfBrail1 { get; set; }

        [Display(Name = "Sum of Brail 2")]
        public decimal? SumOfBrail2 { get; set; }

        // Used to help with calculation/validation
        public decimal? SizeOfBrail1 { get; set; }
        public decimal? SizeOfBrail2 { get; set; }

        [Display(Name = "Total Catch")]
        public decimal? TotalCatch { get; set; }

        public string ContainsSkipjack { get; set; }
        public string ContainsYellowfin { get; set; }
        public string ContainsLargeYellowfin { get; set; }
        public string ContainsBigeye { get; set; }
        public string ContainsLargeBigeye { get; set; }

        public int? SkipjackPercentage { get; set; }

        public int? YellowfinPercentage { get; set; }
        public int? LargeYellowfinPercentage { get; set; }
        public int? LargeYellowfinCount { get; set; }

        public int? BigeyePercentage { get; set; }
        public int? LargeBigeyePercentage { get; set; }
        public int? LargeBigeyeCount { get; set; }

        public decimal? TonsOfSkipjackObserved { get; set; }
        public decimal? TonsOfYellowfinObserved { get; set; }
        public decimal? TonsOfBigeyeObserved { get; set; }

        // This is from old form versions.  2009 form versions have
        // more detailed information
        public string LargeSpecies { get; set; }
        public int? LargeSpeciesPercentage { get; set; }
        public int? LargeSpeciesCount { get; set; }

        public List<SetCatch> ByCatch { get; set; }
        public List<SetCatch> TargetCatch { get; set; }

        // This is used to display data -- The normal display doesn't
        // differentiate between target and bycatch
        // For what it's worth, this could probably be replaced by a read-only property
        // that combines ByCatch and TargetCatch
        // That might even simplify the mapping...
        [JsonIgnore]
        public IEnumerable<SetCatch> AllCatch
        {
            get
            {
                return (
                    this.TargetCatch ?? Enumerable.Empty<SetCatch>()
                ).Union(
                    this.ByCatch ?? Enumerable.Empty<SetCatch>()
                );
            }
        }
        //public List<SetCatch> AllCatch { get; set; }

        [Display(Name = "Recovered Tag Count")]
        public int? RecoveredTagCount { get; set; }
        public string Comments { get; set; }

        [JsonIgnore]
        public IEnumerable<int> DeletedCatchIds
        {
            get
            {
                var all = (
                    this.TargetCatch ?? Enumerable.Empty<SetCatch>()
                ).Union(
                    this.ByCatch ?? Enumerable.Empty<SetCatch>()
                );

                foreach (var c in all)
                {
                    if (null == c)
                        continue;

                    if (c._destroy && c.Id != default(Int32))
                        yield return c.Id;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class SetCatch
        {
            public int Id { get; set; }            
            public string SpeciesCode { get; set; }
            public string FateCode { get; set; }
            public decimal? ObservedWeight { get; set; }
            public int? ObservedCount { get; set; }
            public decimal? LogbookWeight { get; set; }
            public int? LogbookCount { get; set; }
            public string Comments { get; set; }
            public bool NeedsFocus { get; set; }
            public bool _destroy { get; set; }
        }
    }
}