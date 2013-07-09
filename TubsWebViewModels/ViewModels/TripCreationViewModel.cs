// -----------------------------------------------------------------------
// <copyright file="TripCreationViewModel.cs" company="Secretariat of the Pacific Community">
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
    /// View Model for creating new trips.
    /// </summary>
    public sealed class TripCreationViewModel
    {
        public IList<string> WorkbookVersions = new List<string>
        {
            String.Empty,
            "v2007",
            "v2009"
        };

        public IList<string> ProgramCodes = new List<string>
        {
            String.Empty
        };
        
        public string ProgramCode { get; set; }

        public string CountryCode { get; set; }

        public string WorkbookVersion { get; set; }

        public string StaffCode { get; set; }

        public string DeparturePort { get; set; }

        public string DeparturePortCode { get; set; }

        public string ReturnPort { get; set; }

        public string ReturnPortCode { get; set; }

        public DateTime? DepartureDate { get; set; }

        public string DepartureTime { get; set; }

        public DateTime? ReturnDate { get; set; }

        public string ReturnTime { get; set; }

        public string TripNumber { get; set; }

        public string VesselName { get; set; }

        public int? VesselId { get; set; }
    }
}
