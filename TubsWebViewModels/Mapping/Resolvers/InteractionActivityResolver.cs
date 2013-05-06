// -----------------------------------------------------------------------
// <copyright file="InteractionActivityResolver.cs" company="Secretariat of the Pacific Community">
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

    public class InteractionActivityResolver : ValueResolver<string, InteractionActivity?>
    {
        protected override InteractionActivity? ResolveCore(string source)
        {
            source = null == source ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            return
                "Setting".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? InteractionActivity.Setting :
                "Hauling".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? InteractionActivity.Hauling :
                "Searching".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? InteractionActivity.Searching :
                "Transiting".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? InteractionActivity.Transit :
                "Transit".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? InteractionActivity.Transit :
                "Other".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? InteractionActivity.Other :
                (InteractionActivity?)null;
        }
    }

    public class InteractionDescriptionResolver : ValueResolver<InteractionActivity?, string>
    {
        protected override string ResolveCore(InteractionActivity? source)
        {
            if (!source.HasValue)
                return null;

            if (source.Value == InteractionActivity.None)
                return String.Empty;

            if (source.Value == InteractionActivity.Transit)
                return "Transiting";

            return source.Value.ToString();
        }
    }
}
