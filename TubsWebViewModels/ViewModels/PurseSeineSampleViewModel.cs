// -----------------------------------------------------------------------
// <copyright file="LengthFrequencyViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
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

    /// <summary>
    /// Class representing a single biological sample.
    /// </summary>
    public sealed class PurseSeineSampleViewModel
    {
        /// <summary>
        /// Entity primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Sample sequence number
        /// </summary>
        [Range(1, 120)]
        public int Number { get; set; }

        /// <summary>
        /// FAO species code
        /// </summary>
        [MaxLength(3)]
        [Required]
        public string SpeciesCode { get; set; }

        [Required]
        [Range(1, 300)]
        public int? Length { get; set; }
    }
}
