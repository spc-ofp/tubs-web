// -----------------------------------------------------------------------
// <copyright file="PageCountViewModel.cs" company="Secretariat of the Pacific Community">
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
    using Newtonsoft.Json;

    /// <summary>
    /// Page counts for a trip.
    /// </summary>
    public class PageCountViewModel
    {
        public PageCountViewModel()
        {
            PageCounts = new List<PageCount>(8);
            FormKeys = new List<string>()
            {
                String.Empty,
                "PS1",
                "PS2",
                "PS3",
                "PS4",
                "PS5",
                "GEN1",
                "GEN2",
                "GEN3",
                "GEN5",
                "GEN6",
                "LL1",
                "LL2",
                "LL3/4"
            };
        }
        
        public int TripId { get; set; }
        public string TripNumber { get; set; }
        public IList<PageCount> PageCounts { get; set; }

        public IList<string> FormKeys { get; set; }

        public class PageCount
        {
            public int Id { get; set; }
            public string Key { get; set; }
            public int? Value { get; set; }
            public bool _destroy { get; set; }
        }

    }
}
