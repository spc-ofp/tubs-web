// -----------------------------------------------------------------------
// <copyright file="FormNameResolver.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class FormNameResolver : ValueResolver<string, FormNames?>
    {
        protected override FormNames? ResolveCore(string source)
        {
            source = string.IsNullOrEmpty(source) ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            return
                "PS1".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.PS1 :
                "PS2".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.PS2 :
                "PS3".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.PS3 :
                "PS4".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.PS4 :
                "PS5".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.PS5 :
                "GEN1".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.GEN1 :
                "GEN2".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.GEN2 :
                "GEN2S".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.GEN2S :
                "GEN3".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.GEN3 :
                "GEN4".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.GEN4 :
                "GEN5".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? FormNames.GEN5 :
                (FormNames?)null;
        }
    }
}
