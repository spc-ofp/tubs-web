﻿// -----------------------------------------------------------------------
// <copyright file="Ps1ViewModel.cs" company="Secretariat of the Pacific Community">
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
    
    /// <summary>
    /// Ps1ViewModel collects most of the entities (see below) that are sourced
    /// from the PS-1 forms of an observer workbook.  Exceptions are made
    /// for potentially high-volume or complex data entry sections like
    /// Electronics and Crew.
    /// 
    /// Entities:
    /// PurseSeineGear
    /// VesselNotes
    /// PurseSeineVesselAttributes
    /// SafetyInspection
    /// </summary>
    public class Ps1ViewModel
    {
        public string TripNumber { get; set; }
        public int VersionNumber { get; set; }
        public int TripId { get; set; }

        // Use Knockout to help with common codes
        public IList<string> AvailabilityValues = new List<string> { null, "YES", "NO", "OWN" };
        public IList<string> BooleanValues = new List<string> { null, "YES", "NO" };
        public IList<string> NetUnits = new List<string> { null, "M", "Y", "F" };
        public IList<string> MeshUnits = new List<string> { null, "CM", "IN" };
        public IList<string> RangeUnits = new List<string> { null, "KM", "NM" };


        public string PermitNumbers { get; set; }
        public string VesselDeparturePort { get; set; }
        public string VesselDepartureDate { get; set; }

        public VesselCharacteristics Characteristics { get; set; }

        public FishingGear Gear { get; set; }

        public string Page1Comments { get; set; }

        public string HasWasteDisposal { get; set; }
        public string WasteDisposalDescription { get; set; }

        public SafetyInspectionViewModel Inspection { get; set; }

        public string Page2Comments { get; set; }

        public class VesselCharacteristics
        {
            public int Id { get; set; }

            public string Owner { get; set; }
            public string RegistrationNumber { get; set; }
            public string CountryCode { get; set; }
            public string Ircs { get; set; }
            public float? Length { get; set; }
            public string LengthUnits { get; set; }
            public float? GrossTonnage { get; set; }

            public int? SpeedboatCount { get; set; }
            public int? AuxiliaryBoatCount { get; set; }
            public string TenderBoatAnswer { get; set; }
            public string SkiffMake { get; set; }
            public int? SkiffPower { get; set; }
            public decimal CruiseSpeed { get; set; }

            public string HelicopterMake { get; set; }
            public string HelicopterModel { get; set; }
            public string HelicopterRegistration { get; set; }
            public int? HelicopterRange { get; set; }
            public string HelicopterRangeUnits { get; set; }
            public string HelicopterColor { get; set; }
            public int? HelicopterServiceOtherCount { get; set; }

        }

        public class FishingGear
        {
            public int Id { get; set; }

            public string PowerblockMake { get; set; }
            public string PowerblockModel { get; set; }
            public string PurseWinchMake { get; set; }
            public string PurseWinchModel { get; set; }
            public int? NetDepth { get; set; }
            public string NetDepthUnits { get; set; }
            public int? NetLength { get; set; }
            public string NetLengthUnits { get; set; }
            public int? NetStripCount { get; set; }
            public int? NetMeshSize { get; set; }
            public string NetMeshUnits { get; set; }
            public decimal? Brail1Capacity { get; set; }
            public decimal? Brail2Capacity { get; set; }
            public string BrailType { get; set; }
        }
        
    }
}