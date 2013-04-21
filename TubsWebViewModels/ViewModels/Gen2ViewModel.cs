// -----------------------------------------------------------------------
// <copyright file="Gen2ViewModel.cs" company="Secretariat of the Pacific Community">
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

    public abstract class Gen2ViewModel
    {
        // Condition codes used in Landed and Gear interactions
        public IList<string> ConditionCodes = new List<string>()
        {
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

        // Common data fields
        public string TripNumber { get; set; }

        public int TripId { get; set; }

        public int Id { get; set; }

        public string ShipsDate { get; set; }

        public string ShipsTime { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string SpeciesCode { get; set; }

        public string SpeciesDescription { get; set; }

    }
}