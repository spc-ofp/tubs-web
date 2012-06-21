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
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// TripHeaderViewModel holds the minimum set of data required to consider an entity a Trip.
    /// </summary>
    public class TripHeaderViewModel /* : IValidatableObject */
    {
        /*
         * After much experimentation, determined to use the following convention for
         * this model:
         * 
         * For fields that don't belong in the edit form:
         * [Display(AutoGenerateField = false)]
         * 
         * For fields that represent a foreign key to another entity
         * (e.g. filled via Ajax lookup)
         * [Editable(false)]
         */

        public static List<GearSelection> GearTypes = new List<GearSelection>()
        {
            new GearSelection() { Code = "L", Label = "Long line" },
            new GearSelection() { Code = "S", Label = "Purse seine" },
            new GearSelection() { Code = "P", Label = "Pole and Line" }
        };
        
        [Editable(false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Vessel")]
        [Editable(false)]
        public int VesselId { get; set; }
        
        [Display(Name = "Vessel")]
        public string VesselName { get; set; }

        [Required]
        [Display(Name = "Gear Code")]
        public string GearCode { get; set; }

        [Required]
        [Display(Name = "Observer")]
        [Editable(false)]
        public string ObserverCode { get; set; }

        [Display(Name = "Observer")]
        public string ObserverFullName { get; set; }

        [Required]
        [Display(Name = "Trip Number")]
        public string TripNumber { get; set; }

        [Required]
        [Display(Name = "Program Code")]
        [EnumDataType(typeof(Spc.Ofp.Tubs.DAL.Common.ObserverProgram))]
        public string ProgramCode { get; set; }

        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }

        [Required]
        [Display(Name = "Departure Port")]
        [Editable(false)]
        public string DeparturePortCode { get; set; }

        [Display(Name = "Departure Port")]
        public string DeparturePortName { get; set; }

        [Required]
        [Display(Name = "Return Port")]
        [Editable(false)]
        public string ReturnPortCode { get; set; }

        [Display(Name = "Return Port")]
        public string ReturnPortName { get; set; }

        [Required]
        [Display(Name = "Departure Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode=true, DataFormatString = "{0:dd-MM-yy HH:mm")]
        public DateTime? DepartureDate { get; set; }

        [Required]
        [Display(Name = "Return Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yy HH:mm")]
        public DateTime? ReturnDate { get; set; }

        [Required]
        [Display(Name = "Version")]
        public string Version { get; set; }

        // AutoGenerateField will keep this from showing up in the editor template
        // altogether (which is what we want)
        [Display(Name = "Sea Days", AutoGenerateField = false)]
        public int SeaDays { get; set; }

        [Display(Name = "Set Count", AutoGenerateField = false)]
        public int SetCount { get; set; }

        [Display(Name = "Entered By", AutoGenerateField = false)]
        public string EnteredBy { get; set; }

        [Display(Name = "Entered Date", AutoGenerateField = false)]
        [DataType(DataType.DateTime)]
        public DateTime? EnteredDate { get; set; }

        public class GearSelection
        {
            public string Code { get; set; }
            public string Label { get; set; }
        }

        /*
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!this.DepartureDate.HasValue)
            {
                yield return new ValidationResult("Departure Date is required", new string[] { "DepartureDate" });
            }
            if (!this.ReturnDate.HasValue)
            {
                yield return new ValidationResult("Return Date is required", new string[] { "ReturnDate" });
            }
            if (this.VesselId == default(int))
            {
                yield return new ValidationResult("Vessel is required", new string[] { "VesselId" });
            }
        }
        */
    }
}