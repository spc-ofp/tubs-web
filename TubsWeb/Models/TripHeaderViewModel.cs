// -----------------------------------------------------------------------
// <copyright file="TripHeaderViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Models
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
    using System.Linq;
    using System.Web;
    using System.ComponentModel.DataAnnotations;
    
    /// <summary>
    /// TripHeaderViewModel holds the minimum set of data required to consider an entity a Trip.
    /// </summary>
    public class TripHeaderViewModel
    {
        public static List<GearSelection> GearTypes = new List<GearSelection>()
        {
            new GearSelection() { Code = "L", Label = "Long line" },
            new GearSelection() { Code = "S", Label = "Purse seine" },
            new GearSelection() { Code = "P", Label = "Pole and Line" }
        };
        
        public int Id { get; set; }

        [Required]
        [Display(Name = "Vessel")]
        public int VesselId { get; set; }
        
        [Display(Name = "Vessel")]
        public string VesselName { get; set; }

        public string GearCode { get; set; }

        [Required]
        [Display(Name = "Observer")]
        public string ObserverCode { get; set; }

        [Display(Name = "Observer Name")]
        public string ObserverFullName { get; set; }

        [Required]
        [Display(Name = "Trip Number")]
        public string TripNumber { get; set; }

        [Display(Name = "Program Code")]
        public string ProgramCode { get; set; }

        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }

        [Required]
        [Display(Name = "Departure Port")]
        public string DeparturePortCode { get; set; }

        [Display(Name = "Departure Port")]
        public string DeparturePortName { get; set; }

        [Required]
        [Display(Name = "Return Port")]
        public string ReturnPortCode { get; set; }

        [Display(Name = "Return Port")]
        public string ReturnPortName { get; set; }

        [Required]
        [Display(Name = "Departure Date")]
        public DateTime DepartureDate { get; set; }

        [Required]
        [Display(Name = "Return Date")]
        public DateTime ReturnDate { get; set; }

        [Required]
        [Display(Name = "Version")]
        public string Version { get; set; }

        public int SeaDays { get; set; }

        public int SetCount { get; set; }

        public string EnteredBy { get; set; }

        public DateTime? EnteredDate { get; set; }

        public class GearSelection
        {
            public string Code { get; set; }
            public string Label { get; set; }
        }
    }
}