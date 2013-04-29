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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public abstract class Gen2ViewModel
    {
        // Condition codes used in Landed and Gear interactions
        public IList<string> ConditionCodes = new List<string>()
        {
            String.Empty,
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

        // Vessel activities used in Gear and Sighted interactions
        public IList<string> Activities = new List<string>()
        {
            String.Empty,
            "Setting",
            "Hauling",
            "Searching",
            "Transiting",
            "Other"
        };

        // Common data fields
        public string TripNumber { get; set; }

        public int TripId { get; set; }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Ship's Date")]
        public DateTime? ShipsDate { get; set; }

        [Required]
        [Display(Name = "Ship's Time")]
        [RegularExpression(
            @"^[0-2]\d[0-5]\d",
            ErrorMessage = "Ship's time must be a valid 24 hour time")]
        public string ShipsTime { get; set; }

        [RegularExpression(
                @"^[0-8]\d{3}\.?\d{3}[NnSs]$",
                ErrorMessage = "Latitude must be of the form ddmm.mmmN or ddmm.mmmS")]
        public string Latitude { get; set; }

        [RegularExpression(
                @"^[0-1]\d{4}\.?\d{3}[EeWw]$",
                ErrorMessage = "Longitude must be of the form dddmm.mmmE or dddmm.mmmW")]
        public string Longitude { get; set; }

        public string SpeciesCode { get; set; }

        public string SpeciesDescription { get; set; }

        // This property is for the MVC model binder
        public virtual string InteractionType { get; set; }

        // Used for navigation
        public int PageNumber { get; set; }

        public bool HasNext { get; set; }

        public int NextPage { get; set; }

        public bool HasPrevious { get; set; }

        public int PreviousPage { get; set; }

        public string ActionName { get; set; }

    }
}