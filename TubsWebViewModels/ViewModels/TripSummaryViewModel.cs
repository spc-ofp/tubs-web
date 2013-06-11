// -----------------------------------------------------------------------
// <copyright file="TripSummaryViewModel.cs" company="Secretariat of the Pacific Community">
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
    using System.Text;

    /// <summary>
    /// TripSummaryViewModel is a flattened representation of the Trip
    /// entity suitable for viewing in a summary (hence the name).
    /// It also includes additional information to help construct
    /// navigational views.
    /// </summary>
    public class TripSummaryViewModel
    {
        // Primary key
        public int Id { get; set; }

        public string ProgramCode { get; set; }

        public string StaffCode { get; set; }

        public string ObserverName { get; set; }

        public string TripNumber { get; set; }

        public string DeparturePortCode { get; set; }

        public string DeparturePort { get; set; }

        public string ReturnPortCode { get; set; }

        public string ReturnPort { get; set; }

        public DateTime? DepartureDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public string GearCode { get; set; }

        public string Version { get; set; }

        public string VesselName { get; set; }

        public string VesselFlag { get; set; }

        public string AlternateTripNumber { get; set; }

        public string EnteredBy { get; set; }

        public DateTime? EnteredDate { get; set; }

        public DateTime? ClosedDate { get; set; }

        public string EntryComments { get; set; }

        // 0 for Longline trips
        public int SeaDayCount { get; set; }

        // 0 for Longline trips
        public int ExpectedSeaDayCount { get; set; }

        public int SetCount { get; set; }

        public int ExpectedSetCount { get; set; }

        // GEN-1 sightings
        public int SightingCount { get; set; }

        // GEN-1 transfers
        public int TransferCount { get; set; }

        // GEN-2 interactions
        public int InteractionCount { get; set; }

        // No need to count GEN-3 violations, just presence
        // of an appropriate GEN-3 object
        public bool HasGen3 { get; set; }

        // GEN-5 FAD sightings
        public int Gen5Count { get; set; }

        // Affects map/position audit visibility
        public bool HasPositions { get; set; }

        public bool HasCrew { get; set; }

        public bool IsReadOnly
        {
            get
            {
                return this.ClosedDate.HasValue;
            }
        }

        public string SpcTripNumber
        {
            get
            {
                return String.Format("{0} / {1}", this.StaffCode.Trim(), this.TripNumber.Trim());
            }
        }
    }
}
