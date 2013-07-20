// -----------------------------------------------------------------------
// <copyright file="SightingViewModel.cs" company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// ViewModel representing all the GEN-1 sightings in a single trip.
    /// </summary>
    public class SightingViewModel
    {
        public SightingViewModel()
        {
            Sightings = new List<Sighting>(12);
            ActionCodes = new List<string>() { string.Empty, "FI", "PF", "NF", "DF", "TR", "SR", "BR", "OR", "TG", "SG", "BG", "OG" };
        }
        
        // UX state
        public string TripNumber { get; set; }
        public int VersionNumber { get; set; }
        public int TripId { get; set; }

        // Use Knockout to help with common codes
        public IList<int?> TypeCodes = new List<int?>() { null, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 21, 22, 31 };
        public IList<string> ActionCodes = new List<string>() 
        {
            string.Empty, 
            "FI", 
            "PF", 
            "NF", 
            "DF", 
            "TR", 
            "SR", 
            "BR", 
            "OR", 
            "TG", 
            "SG", 
            "BG", 
            "OG" 
        };

        public List<Sighting> Sightings { get; set; }

        [JsonIgnore]
        public IEnumerable<int> DeletedIds
        {
            get
            {
                foreach (var s in this.Sightings ?? Enumerable.Empty<Sighting>())
                {
                    if (null == s)
                        continue;

                    if (s._destroy && default(int) != s.Id)
                        yield return s.Id;
                }
            }
        }

        /// <summary>
        /// A single sighting from the sightings portion of the GEN-1 form.
        /// </summary>
        public sealed class Sighting
        {
            public int Id { get; set; }
            public DateTime? DateOnly { get; set; }
            public string TimeOnly { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int? VesselId { get; set; } // If we have a match, might as well use it.
            public string Name { get; set; }
            public string Ircs { get; set; }
            public string CountryCode { get; set; }
            public int? TypeCode { get; set; }           
            public int? Bearing { get; set; }
            public decimal? Distance { get; set; }
            public string ActionCode { get; set; }
            public string PhotoFrame { get; set; }
            public string Comments { get; set; }

            // UX state
            public bool _destroy { get; set; }
            public bool NeedsFocus { get; set; }

            // Display only, so no need to include in JSON
            [JsonIgnore]
            public string ActivityIconPath { get; set; }
            [JsonIgnore]
            public string TypeDescription { get; set; }
            [JsonIgnore]
            public string ActionDescription { get; set; }
        }
    }
}