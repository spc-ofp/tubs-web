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
    using System.Linq;
    using Newtonsoft.Json;
    using Spc.Ofp.Tubs.DAL.Common;

    /// <summary>
    /// Page counts for a trip.
    /// </summary>
    public class PageCountViewModel
    {
        public int TripId { get; set; }
        public string TripNumber { get; set; }

        public int? Gen1Count { get; set; }
        public int? Gen2Count { get; set; }
        public int? Gen3Count { get; set; }
        public int? Gen6Count { get; set; }

    }
}
