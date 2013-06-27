// -----------------------------------------------------------------------
// <copyright file="Security.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
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

    /// <summary>
    /// Until such time as I feel like implementing a real security filter, this will serve to
    /// make implementing update security easier.
    /// </summary>
    public sealed class Security
    { 
        /// <summary>
        /// List of principals granted edit permissions.
        /// The raw 'Editor' role is for non-AD implementations
        /// </summary>
        public const string EditRoles = @"SPC\AL_DB-OFP-Tubs_Entry, NOUMEA\OFP Data Entry, NOUMEA\OFP Data Admin, Editor";
    }
}