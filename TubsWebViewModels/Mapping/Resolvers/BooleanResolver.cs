// -----------------------------------------------------------------------
// <copyright file="BooleanResolver.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// Boolean resolver converts a nullable boolean to a human readable
    /// yes or no, with 'N/A' provided on null.
    /// </summary>
    public class BooleanResolver : ValueResolver<bool?, string>
    {
        protected override string ResolveCore(bool? source)
        {
            if (!source.HasValue)
                return "N/A";

            return source.Value ? "YES" : "NO";
        }
    }

    /// <summary>
    /// Resolver converts a string yes/no answer to a boolean, with anything
    /// that's not yes or no being converted to null.
    /// </summary>
    public class YesNoResolver : ValueResolver<string, bool?>
    {
        protected override bool? ResolveCore(string source)
        {
            source = null == source ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            return 
                "YES".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? true :
                "NO".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? false :
                (bool?)null;
        }
    }
}
