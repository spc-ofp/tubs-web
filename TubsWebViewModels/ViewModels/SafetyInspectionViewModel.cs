// -----------------------------------------------------------------------
// <copyright file="SafetyInspectionViewModel.cs" company="Secretariat of the Pacific Community">
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

    public sealed class SafetyInspectionViewModel
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
