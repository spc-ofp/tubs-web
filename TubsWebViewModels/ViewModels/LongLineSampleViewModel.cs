// -----------------------------------------------------------------------
// <copyright file="LongLineSampleViewModel.cs" company="Secretariat of the Pacific Community">
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
    /// ViewModel for the LL-4 form.
    /// </summary>
    public sealed class LongLineSampleViewModel
    {
        #region Knockout UI lists
        public IList<string> ConditionCodes = new List<string>()
        {
            String.Empty,
            "A0",
            "A1",
            "A2",
            "A3",
            "A4",
            "A5",
            "A6",
            "A7",
            "A8",
            "D",
            "D1",
            "D2",
            "D3",
            "D4",
            "U",
            "U1",
            "U2",
            "U3",
            "U4"
        };

        public IList<string> FateCodes = new List<string>
        {
            String.Empty,
            "RGG",
            "RGT",
            "RWW",
            "RPT",
            "RFR",
            "RHG",
            "RSD",            
            "RCC",
            "RGO",
            "ROR",
            "DFR",
            "DGD",
            "DSD",
            "DWD",
            "DUS",
            "DDL",
            "DSO",
            "DCF",
            "DTS",
            "DPQ",
            "DPA",
            "DPD",
            "DPU",
            "DOR",
            "ESC"
        };

        public IList<string> SexCodes = new List<string>() { String.Empty, "M", "F", "I", "U" };

        // TODO: Backfill additional codes to other forms?
        public IList<string> LengthCodes = new List<string>() { String.Empty, "TL", "UF", "LF", "PF", "TW", "CL", "NM" };

        public IList<string> WeightCodes = new List<string>
        {
            String.Empty,
            "WW", // Whole weight
            "GG", // Gilled and gutted
            "GH", // Gutted and headed
            "GT", // Gilled, gutted, and tailed
            "GX", // Gutted, headed, and tailed
            "GO", // Gutted only (gills left in)
            "FW", // Fillets weight
            "TW", // Trunk weight
            "SF"  // Shark fin
        };

        public IList<string> MeasuringInstruments = new List<string>
        {
            String.Empty,
            "Board",
            "Aluminum Caliper",
            "Ruler",
            "Deck Tape",
            "Wooden Caliper"
        };
        #endregion

        public LongLineSampleViewModel()
        {
            // A single LL-4 form has ~ 30 lines.
            // The average number of line items per set is 50
            this.Details = new List<LongLineCatchDetail>(50);
        }

        // UX state
        public int SetId { get; set; }
        public int SetNumber { get; set; }
        public int SetCount { get; set; }
        public DateTime? SetDate { get; set; }
        public DateTime? HaulDate { get; set; }
        public int TripId { get; set; }
        public string TripNumber { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public int VersionNumber { get; set; }
        public string ActionName { get; set; }

        public string MeasuringInstrument { get; set; }

        public IList<LongLineCatchDetail> Details { get; set; }

        [JsonIgnore]
        public IEnumerable<int> DeletedCatch
        {
            get
            {
                return from d in this.Details
                       where d != null && d._destroy
                       select d.Id;
            }
        }

        
        /// <summary>
        /// LL-4 line item
        /// </summary>
        public sealed class LongLineCatchDetail
        {
            public int Id { get; set; }

            /// <summary>
            /// Position of this sample on the form.
            /// </summary>
            public int SampleNumber { get; set; }

            /// <summary>
            /// Date of catch (day only)
            /// </summary>
            public DateTime? DateOnly { get; set; }

            /// <summary>
            /// Catch time (time of day only)
            /// </summary>
            public string TimeOnly { get; set; }

            /// <summary>
            /// Date and time of catch
            /// </summary>
            public DateTime? Date { get; set; }

            /// <summary>
            /// Position within the basket
            /// </summary>
            public int? HookNumber { get; set; }

            /// <summary>
            /// Catch species
            /// </summary>
            public string SpeciesCode { get; set; }

            /// <summary>
            /// Condition when caught
            /// </summary>
            public string CaughtCondition { get; set; }

            /// <summary>
            /// Condition on discard/release
            /// </summary>
            public string DiscardedCondition { get; set; }

            /// <summary>
            /// Length (cm)
            /// </summary>
            public int? Length { get; set; }

            /// <summary>
            /// Code describing length measurement
            /// </summary>
            public string LengthCode { get; set; }

            /// <summary>
            /// Weight (kg)
            /// </summary>
            public decimal? Weight { get; set; }

            /// <summary>
            /// Code describing the weight measurement
            /// </summary>
            public string WeightCode { get; set; }

            /// <summary>
            /// Final fate for this fish
            /// </summary>
            public string FateCode { get; set; }

            /// <summary>
            /// Sex of this fish
            /// </summary>
            public string SexCode { get; set; }

            /// <summary>
            /// Comments and/or tag number(s)
            /// </summary>
            public string Comments { get; set; }

            public bool _destroy { get; set; }

            public bool NeedsFocus { get; set; }
        }
    }
}
