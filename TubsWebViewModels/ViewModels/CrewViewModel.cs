// -----------------------------------------------------------------------
// <copyright file="CrewViewModel.cs" company="Secretariat of the Pacific Community">
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
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Newtonsoft.Json;
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

        public string TripNumber { get; set; }

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

        // List, and not IList because we want to use AddRange
        // elsewhere
        public List<CrewMemberModel> Hands { get; set; }

        [JsonIgnore]
        public IEnumerable<CrewMemberModel> Deleted
        {
            get
            {
                return this.Hands.Where(c =>
                    c != null &&
                    c.Id != default(Int32) && // Ignore entities with a zero for Id, as they're not in the database
                    (c._destroy || !c.IsFilled) // If someone blanked the name (or name && comments), we'll count that as a delete
                );
            }
        }
        
        public class CrewMemberModel
        {
            public CrewMemberModel()
            {
            }
            
            public CrewMemberModel(JobType job)
            {
                this.Job = job;
            }
            
            public int Id { get; set; }

            public JobType? Job { get; set; }

            [StringLength(50, ErrorMessage = "Name must 50 or fewer characters")]
            public string Name { get; set; }

            [Range(0, 99, ErrorMessage = "Experience must be between 0 and 99 years")]
            public int? Years { get; set; }

            [StringLength(2, ErrorMessage = "Nationality code must be 2 characters")]
            public string Nationality { get; set; }

            public string Comments { get; set; }

            public bool _destroy { get; set; }

            [JsonIgnore]
            public bool IsFilled
            {
                get
                {
                    if (this.Job.HasValue && this.Job.Value != JobType.Crew)
                    {
                        return
                            !string.IsNullOrWhiteSpace(this.Name) ||
                            !string.IsNullOrWhiteSpace(this.Comments);
                    }
                    return !string.IsNullOrWhiteSpace(this.Name);
                }
            }

            [JsonIgnore]
            public string Experience
            {
                get
                {
                    return
                        this.Years.HasValue ?
                            this.Years.Value == 1 ?
                            String.Format("{0} year", this.Years.Value) :
                            String.Format("{0} years", this.Years.Value) :
                            "None or unknown";
                }
            }

            
        }
    }

    
}