// -----------------------------------------------------------------------
// <copyright file="SearchCriteria.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
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

    /// <summary>
    /// 
    /// </summary>
    public class SearchCriteria
    {
        public string ObserverName { get; set; }

        public string ProgramCode { get; set; }
        
        public string VesselName { get; set; }

        public DateTime? StartDate { get; set; }

        public string DeparturePort { get; set; }

        public DateTime? EndDate { get; set; }

        public string ReturnPort { get; set; }

        public bool IsValid()
        {
            return
                !string.IsNullOrEmpty(this.ObserverName) ||
                !string.IsNullOrEmpty(this.ProgramCode) ||
                !string.IsNullOrEmpty(this.VesselName) ||
                !string.IsNullOrEmpty(this.DeparturePort) ||
                !string.IsNullOrEmpty(this.ReturnPort) ||
                this.StartDate.HasValue ||
                this.EndDate.HasValue;
        }
    }

    public class SearchResult
    {
        public string DetailUrl { get; set; }

        public string VesselName { get; set; }

        public string DeparturePort { get; set; }

        public DateTime? StartDate { get; set; }

        public string ReturnPort { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}