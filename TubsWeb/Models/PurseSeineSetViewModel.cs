// -----------------------------------------------------------------------
// <copyright file="PurseSeineSetViewModel.cs" company="Secretariat of the Pacific Community">
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

    public class PurseSeineSetViewModel
    {
        public PurseSeineSetViewModel()
        {
             ByCatch = new List<SetCatch>();
             SkjCatch = new List<SetCatch>();
             YftCatch = new List<SetCatch>();
             BetCatch = new List<SetCatch>();
        }
        
        // UX state
        public string TripNumber { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public int VersionNumber { get; set; }
        // It can happen that set activities start during one day and end
        // during another.  If that happens, rather than puke and require
        // a manual SQL operation, allow the data entry tech to confirm
        public bool CrossesDayBoundary { get; set; }

        public int TripId { get; set; }
        public int SetId { get; set; }

        public DateTime? LogbookDate { get; set; }
        public DateTime? LogbookTime { get; set; }

        [Display(Name = "Start of Set")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string SkiffOff { get; set; }

        [Display(Name = "Begin Pursing")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string WinchOn { get; set; }

        [Display(Name = "End Pursing")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string RingsUp { get; set; }

        [Display(Name = "Begin Brailing")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string BeginBrailing { get; set; }

        [Display(Name = "End Brailing")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string EndBrailing { get; set; }

        [Display(Name = "End of Set")]
        [RegularExpression(@"^[0-2]\d[0-5]\d$")]
        public string SkiffOnBoard { get; set; }

        // TODO Much more here, and it will be different for different
        // form versions

        public IList<SetCatch> ByCatch { get; set; }
        public IList<SetCatch> SkjCatch { get; set; }
        public IList<SetCatch> YftCatch { get; set; }
        public IList<SetCatch> BetCatch { get; set; }

        public int? TagCount { get; set; }
        public string Comments { get; set; }

        public class SetCatch
        {
            public int Id { get; set; }
            public string SpeciesCode { get; set; }
            public string FateCode { get; set; }
            public decimal? ObservedWeight { get; set; }
            public int? ObservedCount { get; set; }
            public decimal? LogbookWeight { get; set; }
            public int? LogbookCount { get; set; }
            public string Comments { get; set; }
        }
    }
}