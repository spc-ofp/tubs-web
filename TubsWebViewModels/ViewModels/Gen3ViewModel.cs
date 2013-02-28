// -----------------------------------------------------------------------
// <copyright file="Gen3ViewModel.cs" company="Secretariat of the Pacific Community">
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
    using System.Collections;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Originally, this was intended for 2009 GEN-3 forms.  With some bending and squinting,
    /// it can be made to work for all revisions of the GEN-3.
    /// </summary>
    public class Gen3ViewModel
    {
        private static IDictionary<string, int> SortOrder = new Dictionary<string, int>()
        {
            { "RS-A", 0 },
            { "RS-B", 1 },
            { "RS-C", 2 },
            { "RS-D", 3 },
            { "NR-A", 4 },
            { "NR-B", 5 },
            { "NR-C", 6 },
            { "NR-D", 7 },
            { "NR-E", 8 },
            { "NR-F", 9 },
            { "NR-G", 10 },
            { "WC-A", 11 },
            { "WC-B", 12 },
            { "WC-C", 13 },
            { "LP-A", 14 },
            { "LP-B", 15 },
            { "LC-A", 16 },
            { "LC-B", 17 },
            { "LC-C", 18 },
            { "LC-D", 19 },
            { "LC-E", 20 },
            { "LC-F", 21 },
            { "SI-A", 22 },
            { "SI-B", 23 },
            { "PN-A", 24 },
            { "PN-B", 25 },
            { "PN-C", 26 },
            { "PN-D", 27 },
            { "PN-E", 28 },
            { "SS-A", 29 },
            { "SS-B", 30 }
        };
        
        public Gen3ViewModel()
        {
            Incidents = new List<Incident>(31);
            IncidentByCode = new Dictionary<string, int>(31);
            Notes = new List<Note>(6);
        }

        public void PrepareIncidents()
        {
            if (2009 == this.VersionNumber)
            {
                if (this.Incidents.Count != 31)
                {
                    var ilist = this.EmptyList;
                    for (int i = 0; i < ilist.Count; i++)
                    {
                        var qcode = ilist[i].QuestionCode;
                        if (this.IncidentByCode.ContainsKey(qcode))
                        {
                            // Copy into ilist
                            ilist[i] = this.Incidents[this.IncidentByCode[qcode]];
                        }
                    }

                    // Replace existing list with display-ready list
                    this.Incidents = ilist;

                }

                // Place Incidents into correct display order
                ArrayList.Adapter((IList)this.Incidents).Sort();

                this.IndexIncidents();
            }
        }

        public void IndexIncidents()
        {
            lock (this)
            {
                this.IncidentByCode.Clear();
                if (null != this.Incidents)
                {
                    for (int i = 0; i < this.Incidents.Count; i++)
                    {
                        var incident = this.Incidents[i];
                        if (null != incident && !String.IsNullOrEmpty(incident.QuestionCode))
                        {
                            this.IncidentByCode.Add(incident.QuestionCode, i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// EmptyList is a collection of v2009 question codes in correct display order
        /// </summary>
        [JsonIgnore]
        public IList<Incident> EmptyList
        {
            get
            {
                IList<Incident> incidents = new List<Incident>(31);
                foreach (var code in QuestionCodes)
                {
                    if (null == code) { continue; }
                    incidents.Add(new Incident() { Id = 0, Answer = null, JournalPage = null, QuestionCode = code });
                }
                return incidents;
            }
        }

        [JsonIgnore]
        public Dictionary<string, int> IncidentByCode { get; set; }
        
        public string TripNumber { get; set; }

        public int TripId { get; set; }

        // Pre-2009 trips only
        public int MonitorId { get; set; }

        // 2007 or 2009
        public int VersionNumber { get; set; }

        // Use Knockout to help with common codes
        public IList<string> BooleanValues = new List<string> { null, "YES", "NO" };
        [JsonIgnore]
        private static IList<string> QuestionCodes = new List<string>
        {
            "RS-A",
            "RS-B",
            "RS-C",
            "RS-D",
            "NR-A",
            "NR-B",
            "NR-C",
            "NR-D",
            "NR-E",
            "NR-F",
            "NR-G",
            "WC-A",
            "WC-B",
            "WC-C",
            "LP-A",
            "LP-B",
            "LC-A",
            "LC-B",
            "LC-C",
            "LC-D",
            "LC-E",
            "LC-F",
            "SI-A",
            "SI-B",
            "PN-A",
            "PN-B",
            "PN-C",
            "PN-D",
            "PN-E",
            "SS-A",
            "SS-B"
        };

        public IList<Incident> Incidents { get; set; }

        public IList<Note> Notes { get; set; }

        public class Incident : IComparable
        {
            // Zero for 2007 incidents
            public int Id { get; set; }
            public string QuestionCode { get; set; }
            public string Answer { get; set; }
            // Null for 2007 incidents
            public int? JournalPage { get; set; }
            public bool _destroy { get; set; }

            public int CompareTo(object obj)
            {
                if (!(obj is Incident))
                {
                    throw new ArgumentException(string.Format("Cannot compare Incident to {0}", obj.GetType()));
                }
                if (null == obj) { return 1; }
                Incident rhs = obj as Incident;
                int sortOrdinal = -1;
                if (SortOrder.ContainsKey(this.QuestionCode))
                {
                    sortOrdinal = SortOrder[this.QuestionCode];
                }

                int objSortOrdinal = -1;
                if (SortOrder.ContainsKey(rhs.QuestionCode))
                {
                    objSortOrdinal = SortOrder[rhs.QuestionCode];
                }

                return sortOrdinal.CompareTo(objSortOrdinal);
            }
        }

        public class Note
        {
            public int Id { get; set; }
            public DateTime? Date { get; set; }
            public string Comments { get; set; }
            public bool _destroy { get; set; }
        }
    }
}
