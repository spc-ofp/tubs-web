// -----------------------------------------------------------------------
// <copyright file="TripClosureViewModel.cs" company="Secretariat of the Pacific Community">
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
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Lightweight view model for closing trips.
    /// </summary>
    public class TripClosureViewModel
    {
        /// <summary>
        /// Trip primary key
        /// </summary>
        [Display(Name = "Trip Id")]
        public int? TripId { get; set; }

        /// <summary>
        /// Comments about trip entry
        /// </summary>
        [Display(Name = "Entry Comments")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
    }
}