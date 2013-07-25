// -----------------------------------------------------------------------
// <copyright file="WellContentViewModel.cs" company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using Newtonsoft.Json;
    public class WellContentViewModel
    {
        #region Knockout Lists
        public IList<string> Locations = new List<string>
        {
            String.Empty,
            "Port",
            "Starboard",
            "Center"
        };

        public IList<string> Contents = new List<string> 
        {
            String.Empty,
            "Fuel",
            "Water",
            "Other"
        };

        
        #endregion
        
        // Necessary for navigation
        public int TripId { get; set; }
        public string TripNumber { get; set; }
        public IList<WellContentItem> WellContentItems { get; set; }

        [JsonIgnore]
        public IEnumerable<int> DeletedIds
        {
            get
            {
                foreach (var c in WellContentItems)
                {
                    if (null == c)
                        continue;

                    if (c._destroy && c.Id != default(Int32))
                        yield return c.Id;
                }
            }
        }


        public class WellContentItem
        {
            public int Id { get; set; }
            public int WellNumber { get; set; }
            public string Location { get; set; }
            public decimal? Capacity{ get; set; }
            public string Content { get; set; }
            public string Comment { get; set; }

            // Knockout UI integration
            public bool _destroy { get; set; }
            public bool NeedsFocus { get; set; }
        }

    }
}
