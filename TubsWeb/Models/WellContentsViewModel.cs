// -----------------------------------------------------------------------
// <copyright file="WellContentsViewModel.cs" company="Secretariat of the Pacific Community">
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
    /// WellContentsViewModel captures the well contents data collected
    /// by the observer on PS-1, page 2.
    /// </summary>
    public class WellContentsViewModel
    {
        public WellContentsViewModel()
        {
            FuelWells = new List<Well>();
            WaterWells = new List<Well>();
            OtherWells = new List<Well>();
        }
        
        // UX state
        public string TripNumber { get; set; }

        public int TripId { get; set; }
        public int WellId { get; set; }
        public decimal? FishStorageCapacity { get; set; }

        public IList<Well> FuelWells { get; set; }
        public IList<Well> WaterWells { get; set; }
        public IList<Well> OtherWells { get; set; }

        public class Well
        {
            public int ItemId { get; set; }
            public int? WellNumber { get; set; }
            public string Location { get; set; }
            public decimal? Capacity { get; set; }
            public string Comments { get; set; }
        }
    }
}