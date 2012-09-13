// -----------------------------------------------------------------------
// <copyright file="SeaDayViewModel.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// ViewModel for PS-2 data.
    /// </summary>
    public class SeaDayViewModel
    {
        public SeaDayViewModel()
        {
            Events = new List<SeaDayEvent>(8);
        }
        
        public DateTime? ShipsDate { get; set; }
        public string ShipsTime { get; set; }
        public DateTime? UtcDate { get; set; }
        public string UtcTime { get; set; }

        public int? AnchoredWithNoSchool { get; set; }
        public int? AnchoredWithSchool { get; set; }
        public int? FreeFloatingWithNoSchool { get; set; }
        public int? FreeFloatingWithSchool { get; set; }
        public int? FreeSchool { get; set; }

        public bool? HasGen3Event { get; set; }
        public int? DiaryPage { get; set; }

        public IList<SeaDayEvent> Events { get; protected set; }

        public class SeaDayEvent
        {
            public string Time { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string EezCode { get; set; }
            public int? ActivityCode { get; set; }
            public int? WindSpeed { get; set; }
            public int? WindDirection { get; set; }
            public string SeaCode { get; set; }
            public int? DetectionCode { get; set; }
            public int? AssociationCode { get; set; }
            public string FadNumber { get; set; }
            public string BuoyNumber { get; set; }
            public string Comments { get; set; }
        }
    }
}