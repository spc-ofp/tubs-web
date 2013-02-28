// -----------------------------------------------------------------------
// <copyright file="InteractionTypeResolver.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
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
    /// TODO: Update summary.
    /// </summary>
    public class InteractionTypeResolver : ValueResolver<string, string>
    {
        // Word to S,I,L
        protected override string ResolveCore(string source)
        {
            source = string.IsNullOrEmpty(source) ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            return
                "sighted".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? "S" :
                "interact".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? "I" :
                "landed".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? "L" :
                null;
        }
    }

    public class InteractionCodeResolver : ValueResolver<string, string>
    {
        // S,I,L to Word
        protected override string ResolveCore(string source)
        {
            source = string.IsNullOrEmpty(source) ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            return
                "S".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? "sighted" :
                "I".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? "interact" :
                "L".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? "landed" :
                null;
        }
    }
}
