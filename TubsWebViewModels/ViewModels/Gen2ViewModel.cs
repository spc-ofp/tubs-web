// -----------------------------------------------------------------------
// <copyright file="Gen2ViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>

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

    public class Gen2ViewModel
    {
        // Send down to client for use with Knockout.js
        public IList<string> SexCodes = new List<string>() { "M", "F", "I", "U" };

        public IList<string> DistanceUnits = new List<string>() { "m", "NM" };

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

        public Gen2ViewModel()
        {
            StartOfInteraction = new List<SpeciesGroup>(3);
            EndOfInteraction = new List<SpeciesGroup>(3);
        }

        // Common data fields
        public string TripNumber { get; set; }

        public int TripId { get; set; }

        public int Id { get; set; }

        public string InteractionType { get; set; }

        public string ShipsDate { get; set; }

        public string ShipsTime { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string SpeciesCode { get; set; }

        public string SpeciesDescription { get; set; }

        // Landed on deck
        public string LandedConditionCode { get; set; }

        public string LandedConditionDescription { get; set; }

        public string LandedHandling { get; set; }

        public int? LandedLength { get; set; }

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

        // Interactions with vessel or gear           
        public string VesselActivity { get; set; } // Also used for sighting

        // For use with VesselActivity == 'Other'
        public string VesselActivityDescription { get; set; } // Also used for sighting

        public IList<SpeciesGroup> StartOfInteraction { get; set; }

        public IList<SpeciesGroup> EndOfInteraction { get; set; }

        public string InteractionDescription { get; set; }

        // Species Sighted
        public int? NumberSighted { get; set; }

        public int? NumberOfAdults { get; set; }

        public int? NumberOfJuveniles { get; set; }

        public string SightingLength { get; set; }

        public decimal? SightingDistance { get; set; }

        public string SightingDistanceUnit { get; set; }

        public string SightingBehavior { get; set; }

        // The GEN-3 allows entry of up to 3 groups of species for an interaction
        // with fishing gear/vessel
        public class SpeciesGroup
        {
            public int Id { get; set; }

            public int? Count { get; set; }

            public string ConditionCode { get; set; }

            public string Description { get; set; }
        }

    }
}