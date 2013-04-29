// -----------------------------------------------------------------------
// <copyright file="Gen2GearViewModel.cs" company="Secretariat of the Pacific Community">
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
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Gen2GearViewModel : Gen2ViewModel
    {
        public Gen2GearViewModel()
        {
            this.InteractionType = typeof(Gen2GearViewModel).FullName;
            this.StartOfInteraction = new List<SpeciesGroup>(3);
            this.EndOfInteraction = new List<SpeciesGroup>(3);
        }
        
        // Interactions with vessel or gear           
        public string VesselActivity { get; set; } // Also used for sighting

        // For use with VesselActivity == 'Other'
        public string VesselActivityDescription { get; set; } // Also used for sighting

        public IList<SpeciesGroup> StartOfInteraction { get; set; }

        public IList<SpeciesGroup> EndOfInteraction { get; set; }

        public string InteractionDescription { get; set; }

        [JsonIgnore]
        public IEnumerable<SpeciesGroup> Deleted
        {
            get
            {
                return this.StartOfInteraction.Where(e => e != null && e._destroy).Union(this.EndOfInteraction.Where(e => e != null && e._destroy));
            }
        }

        // The GEN-2 allows entry of up to 3 groups of species for an interaction
        // with fishing gear/vessel
        public class SpeciesGroup
        {
            public int Id { get; set; }

            public int? Count { get; set; }

            public string ConditionCode { get; set; }

            public string Description { get; set; }

            public bool _destroy { get; set; }

            public bool NeedsFocus { get; set; }
        }
    }

    
}
