// -----------------------------------------------------------------------
// <copyright file="Gen5ViewModel.cs" company="Secretariat of the Pacific Community">
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
    using System.ComponentModel.DataAnnotations;

    public class Gen5ViewModel
    {
        // UX state
        public string TripNumber { get; set; }
        public int VersionNumber { get; set; }
        public int TripId { get; set; }

        public IList<int?> OriginCodes = new List<int?> { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        
        // Used for "as Found" and "as Left"
        public IList<int?> DescriptionCodes =
            new List<int?> { null, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // Main Materials
        public IList<int?> MaterialCodes =
            new List<int?> { null, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // FAD Attachments
        public IList<int?> AttachmentCodes =
            new List<int?> { null, 11, 12, 13, 14, 15, 16, 17 };

        public int Id { get; set; }

        public int ActivityId { get; set; }

        [Display(Name = "Object Number")]
        public int? ObjectNumber { get; set; }

        [Display(Name = "Origin of FAD")]
        public int? OriginCode { get; set; }

        public DateTime? DeploymentDate { get; set; }

        [RegularExpression(@"^[0-8]\d{3}\.?\d{3}[NnSs]$")]
        public string Latitude { get; set; }

        [RegularExpression(@"^[0-1]\d{4}\.?\d{3}[EeWw]$")]
        public string Longitude { get; set; }

        public bool? SsiTrapped { get; set; }

        public int? AsFoundCode { get; set; }

        public int? AsLeftCode { get; set; }

        public IList<int> MainMaterials { get; set; }

        public IList<int> Attachments { get; set; }

        public int? Depth { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public string BuoyNumber { get; set; }

        public string Markings { get; set; }

        public string Comments { get; set; }
 
    }
}