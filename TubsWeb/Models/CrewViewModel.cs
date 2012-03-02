// -----------------------------------------------------------------------
// <copyright file="CrewViewModel.cs" company="Secretariat of the Pacific Community">
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
    using System.ComponentModel.DataAnnotations;
    using Spc.Ofp.Tubs.DAL.Common;
    
    /// <summary>
    /// CrewViewModel holds the crew in a means suitable for display
    /// </summary>
    public class CrewViewModel
    {
        public CrewViewModel()
        {
            this.Hands = new List<CrewMemberModel>();
        }

        public int TripId { get; set; }
        public CrewMemberModel Captain { get; set; }
        public CrewMemberModel Navigator { get; set; }
        public CrewMemberModel Mate { get; set; }
        public CrewMemberModel ChiefEngineer { get; set; }
        public CrewMemberModel AssistantEngineer { get; set; }
        public CrewMemberModel DeckBoss { get; set; }
        public CrewMemberModel Cook { get; set; }
        public CrewMemberModel HelicopterPilot { get; set; }
        public CrewMemberModel SkiffMan { get; set; }
        public CrewMemberModel WinchMan { get; set; }

        public List<CrewMemberModel> Hands { get; set; }
        
        public class CrewMemberModel
        {
            public int Id { get; set; }

            public JobType? Job { get; set; }

            public string Name { get; set; }

            [StringLength(2)]
            public string Nationality { get; set; }

            [Range(0, 99)]
            public int? Years { get; set; }

            [Range(0, 99)]
            public int? Months { get; set; }

            public string Experience { get; set; }

            public string Comments { get; set; }
        }
    }

    
}