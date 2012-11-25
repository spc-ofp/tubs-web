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
    using System.Collections.Generic;

    /// <summary>
    /// Gen3ViewModel is a view model for the v2009 workbook GEN-3.
    /// </summary>
    public class Gen3ViewModel
    {
        public Gen3ViewModel()
        {
            Incidents = new List<Incident>(6);
            Notes = new List<Note>(6);
        }
        
        public string TripNumber { get; set; }

        public int TripId { get; set; }

        // Use Knockout to help with common codes
        public IList<string> BooleanValues = new List<string> { null, "YES", "NO" };

        public IList<Incident> Incidents { get; set; }

        public IList<Note> Notes { get; set; }

        public class Incident
        {
            public int Id { get; set; }
            public string QuestionCode { get; set; }
            public string Answer { get; set; }
            public int? JournalPage { get; set; }
        }

        public class Note
        {
            public int Id { get; set; }
            public DateTime? Date { get; set; }
            public string Comments { get; set; }
        }
    }
}
