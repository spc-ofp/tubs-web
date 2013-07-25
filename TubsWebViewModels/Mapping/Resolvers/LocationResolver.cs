// -----------------------------------------------------------------------
// <copyright file="LocationResolver.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.ViewModels.Resolvers
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
    using AutoMapper;
    using DAL = Spc.Ofp.Tubs.DAL;

    public sealed class LocationCodeResolver : ValueResolver<string,string>
    {
        protected override string ResolveCore(string source)
        {
            if (source == null)
                return String.Empty;
            switch (source)
            {
                case "P":
                    return "Port";
                case "S":
                    return "Starboard";
                case "C":
                    return "Center";
                default:
                    return String.Empty;
            }
        }
    }

    public sealed class LocationTypeResolver : ValueResolver<string, string>
    {
        protected override string ResolveCore(string source)
        {
            if (source == null)
                return String.Empty;
            switch (source)
            {
                case "Port":
                    return "P";
                case "Starboard":
                    return "S";
                case "Center":
                    return "C";
                default:
                    return String.Empty;
            }
        }
    }
}
