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

    public class Gen2LandedViewModel : Gen2ViewModel
    {
        // Send down to client for use with Knockout.js
        public IList<string> SexCodes = new List<string>() { "M", "F", "I", "U" };

        // Landed on deck
        public string LandedConditionCode { get; set; }

        public string LandedConditionDescription { get; set; }

        public string LandedHandling { get; set; }

        public decimal? LandedLength { get; set; }

        public string LandedLengthCode { get; set; }

        public string LandedSexCode { get; set; }

        public string DiscardedConditionCode { get; set; }

        public string DiscardedConditionDescription { get; set; }

        public string RetrievedTagNumber { get; set; }

        public string RetrievedTagType { get; set; }

        public string RetrievedTagOrganization { get; set; }

        public string PlacedTagNumber { get; set; }

        public string PlacedTagType { get; set; }

        public string PlacedTagOrganization { get; set; }
    }
}
