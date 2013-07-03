// -----------------------------------------------------------------------
// <copyright file="Gen2LandedViewModel.cs" company="Secretariat of the Pacific Community">
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
    /// Summary of Long Line catch for a trip.
    /// Used in navigation
    /// </summary>
    public class LongLineCatchSummaryViewModel
    {
        /// <summary>
        /// Trip primary key
        /// </summary>
        public int TripId { get; set; }

        /// <summary>
        /// Location of the set within the trip
        /// </summary>
        public int SetNumber { get; set; }

        /// <summary>
        /// Date/time of the set
        /// </summary>
        public DateTime? SetDate { get; set; }

        /// <summary>
        /// Number of samples from LL-4 form.
        /// </summary>
        public int SampleCount { get; set; }
    }
}
