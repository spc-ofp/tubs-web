// -----------------------------------------------------------------------
// <copyright file="Gen1ViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>

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
    using System.Collections.Generic;
    using Spc.Ofp.Tubs.DAL.Entities;
    using Spc.Ofp.Tubs.DAL.Common;
    
    public class Gen1ViewModel
    {
        public static IList<SightedVesselType> SightedVessels = new List<SightedVesselType>()
        {
            SightedVesselType.SinglePurseSeine,
            SightedVesselType.Longline,
            SightedVesselType.PoleAndLine,
            SightedVesselType.Mothership,
            SightedVesselType.Troll,
            SightedVesselType.NetBoat, 
            SightedVesselType.Bunker,
            SightedVesselType.SearchAnchorOrLightBoat,           
            SightedVesselType.FishCarrier,
            SightedVesselType.Trawler,            
            SightedVesselType.LightAircraft,
            SightedVesselType.Helicopter,
            SightedVesselType.Other,
        };

        public static IList<VesselType> TransferVessels = new List<VesselType>()
        {
            VesselType.SinglePurseSeine,
            VesselType.Longline,
            VesselType.PoleAndLine,
            VesselType.Mothership,
            VesselType.Troll,
            VesselType.NetBoat,
            VesselType.Bunker,
            VesselType.FishCarrier,        
            VesselType.Trawler,
            VesselType.Other,
        };

        // TODO Rationalize this list
        public static IList<ActionType> ActionTypes = new List<ActionType>()
        {
            ActionType.AG,
            ActionType.BG,
            ActionType.BR,
            ActionType.DF,
            ActionType.FI,
            ActionType.FS,
            ActionType.NF,
            ActionType.OG,
            ActionType.OR,
            ActionType.PF,
            ActionType.SG,
            ActionType.SR,
            ActionType.TG,
            ActionType.TR,
            ActionType.UL,
            ActionType.WT,
        };

        /// <summary>
        /// List of ActionType values that make sense for the transfer portion of
        /// the GEN-1 form.
        /// </summary>
        public static IList<ActionType> TransferActionTypes = new List<ActionType>()
        {
            ActionType.DF,
            ActionType.TR,
            ActionType.SR,
            ActionType.BR,
            ActionType.TR,
            ActionType.SG,
            ActionType.BG,
            ActionType.OR,
            ActionType.OG
        };
        
        
        public Gen1ViewModel()
        {
            this.Sightings = new List<Sighting>();
            this.Transfers = new List<Transfer>();
        }

        public int TripId { get; set; }
        public IList<Sighting> Sightings { get; set; }
        public IList<Transfer> Transfers { get; set; }
    }
}