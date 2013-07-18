// -----------------------------------------------------------------------
// <copyright file="TripSamplingViewModel.cs" company="Secretariat of the Pacific Community">
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
    using System.Collections;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// TripSamplingViewModel manages a list of all the PS-4 forms associated
    /// with a given trip.
    /// </summary>
    public sealed class TripSamplingViewModel
    {
        public TripSamplingViewModel()
        {
            this.Headers = new List<SampleSummary>(48);
        }
        
        public string TripNumber { get; set; }

        public int TripId { get; set; }

        public IList<SampleSummary> Headers { get; set; }

        /// <summary>
        /// SampleSummary is a single line item in TripSamplingViewModel.
        /// </summary>
        public sealed class SampleSummary
        {
            /// <summary>
            /// PS-4 page number.  This is for the entire trip, not just a single
            /// set.
            /// </summary>
            public int PageNumber { get; set; }

            /// <summary>
            /// Set number.
            /// </summary>
            public int SetNumber { get; set; }

            /// <summary>
            /// Set date and time.
            /// </summary>
            public DateTime? SetDate { get; set; }

            /// <summary>
            /// Sampling protocol in use for this page.
            /// </summary>
            public string SampleType { get; set; }

            /// <summary>
            /// Number of samples recorded for this page.
            /// </summary>
            public int SampleCount { get; set; }

        }
    }
}
