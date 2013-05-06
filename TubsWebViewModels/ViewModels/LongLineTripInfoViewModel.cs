// -----------------------------------------------------------------------
// <copyright file="LongLineTripInfoViewModel.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// LongLineTripInfoViewModel collects most of the entities (see below) that are sourced
    /// from the LL-1 forms of an observer workbook.  Exceptions are made
    /// for potentially high-volume or complex data entry sections like
    /// Electronics.
    /// 
    /// Entities:
    /// LongLineGear
    /// SafetyInspection
    /// </summary>
    public class LongLineTripInfoViewModel
    {
        public string TripNumber { get; set; }
        public int VersionNumber { get; set; }
        public int TripId { get; set; }

        // Use Knockout to help with common codes
        public IList<string> BooleanValues = new List<string> { null, "YES", "NO" };
        public IList<string> LengthUnits = new List<string> { String.Empty, "M", "F" };
        public IList<string> UsageValues = new List<string> { String.Empty, "ALL", "TRA", "OIF", "SIF", "RAR", "BRO", "NOL" };

        public string VesselDeparturePort { get; set; }
        public string VesselDepartureDate { get; set; }

        public VesselCharacteristics Characteristics { get; set; }
        public CrewNationality Nationality { get; set; }

        public FishingGear Gear { get; set; }
        public RefrigerationMethod Refrigeration { get; set; }

        // PS-1 has two spots for comments
        // LL-1 only has one spot for comments (on the second page)
        // but it's the whole second page
        public string Comments { get; set; }

        public string HasWasteDisposal { get; set; }
        public string WasteDisposalDescription { get; set; }

        public SafetyInspection Inspection { get; set; }

        public class VesselCharacteristics
        {
            public string Owner { get; set; }
            public string RegistrationNumber { get; set; }
            public string Ircs { get; set; }

            public string Captain { get; set; }
            public string CaptainDocument { get; set; }
            public string CaptainDocumentNumber { get; set; }
            // AKA vessel flag
            public string CountryCode { get; set; }

            public string Master { get; set; }
            public string MasterDocument { get; set; }
            public string MasterDocumentNumber { get; set; }
            public decimal? HoldCapacity { get; set; }

            public string PermitNumbers { get; set; }
            public decimal? Length { get; set; }
            public string LengthUnits { get; set; }
            public decimal? GrossTonnage { get; set; }
            
        }

        public class CrewNationality
        {
            public string CaptainCountryCode { get; set; }
            public string MasterCountryCode { get; set; }

            // There are 4 nationality groups on the LL-1 form
            // As of right now, there's no place in TUBS for this to
            // go.
            public string GroupOneCountryCode { get; set; }
            public int? GroupOneCount { get; set; }

            public string GroupTwoCountryCode { get; set; }
            public int? GroupTwoCount { get; set; }

            public string GroupThreeCountryCode { get; set; }
            public int? GroupThreeCount { get; set; }

            public string GroupFourCountryCode { get; set; }
            public int? GroupFourCount { get; set; }
        }

        public class FishingGear
        {
            public int Id { get; set; }
            
            public string HasMainlineHauler { get; set; }
            public string MainlineHaulerUsage { get; set; }

            public string HasBranchlineHauler { get; set; }
            public string BranchlineHaulerUsage { get; set; }

            public string HasLineShooter { get; set; }
            public string LineShooterUsage { get; set; }

            public string HasBaitThrower { get; set; }
            public string BaitThrowerUsage { get; set; }

            public string HasBranchlineAttacher { get; set; }
            public string BranchlineAttacherUsage { get; set; }

            public string HasWeighingScales { get; set; }
            public string WeighingScalesUsage { get; set; }

            // LL-1 has spot for one new bit of equipment that doesn't fall
            // into the other categories
            public string Description { get; set; }
            public string HasOther { get; set; }
            public string OtherUsage { get; set; }

            // TODO:  Add mainline, branchline, and hook details
        }

        public class RefrigerationMethod
        {
            public string HasBlastFreeze { get; set; }
            public string HasIce { get; set; }
            public string HasRefrigeratedBrine { get; set; }
            public string HasChilledSeawater { get; set; }
            public string HasOther { get; set; }
            public string Description { get; set; }
        }

        public class SafetyInspection
        {
            public int Id { get; set; }

            public string LifejacketProvided { get; set; }
            public string LifejacketSizeOk { get; set; }
            public string LifejacketAvailability { get; set; }

            public int? BuoyCount { get; set; }

            public int? Epirb406Count { get; set; }
            public string Epirb406Expiration { get; set; }

            public string OtherEpirbType { get; set; }
            public int? OtherEpirbCount { get; set; }
            public string OtherEpirbExpiration { get; set; }

            public int? LifeRaft1Capacity { get; set; }
            public string LifeRaft1Inspection { get; set; }
            public string LifeRaft1LastOrDue { get; set; }

            public int? LifeRaft2Capacity { get; set; }
            public string LifeRaft2Inspection { get; set; }
            public string LifeRaft2LastOrDue { get; set; }

            public int? LifeRaft3Capacity { get; set; }
            public string LifeRaft3Inspection { get; set; }
            public string LifeRaft3LastOrDue { get; set; }

            public int? LifeRaft4Capacity { get; set; }
            public string LifeRaft4Inspection { get; set; }
            public string LifeRaft4LastOrDue { get; set; }
        }
    }
}
