// -----------------------------------------------------------------------
// <copyright file="Gen2SightingViewModel.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// Gen2SummaryViewModel is a lightweight object used when displaying
    /// a list of GEN-2 entities.
    /// </summary>
    public class Gen2SummaryViewModel
    {
        public int PageNumber { get; set; }
        public DateTime ShipsDate { get; set; }
        public string SpeciesCode { get; set; }
        public string InteractionType { get; set; } 
    }
}
