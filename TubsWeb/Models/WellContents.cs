﻿// -----------------------------------------------------------------------
// <copyright file="WellContents.cs" company="Secretariat of the Pacific Community">
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
    using System.Collections.Generic;
    using Spc.Ofp.Tubs.DAL.Common;
    
    /// <summary>
    /// PS-1 well contents enumeration for use in select box.
    /// TODO: Replace with ViewModel
    /// </summary>
    public static class WellContents
    {
        /// <summary>
        /// List of well content enumeration values.
        /// </summary>
        public static IList<WellContentType> ContentTypes = new List<WellContentType>()
        {
            WellContentType.Fuel,
            WellContentType.Water,
            WellContentType.Other
        };
    }
}