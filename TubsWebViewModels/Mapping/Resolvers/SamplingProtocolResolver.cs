// -----------------------------------------------------------------------
// <copyright file="SamplingProtocolResolver.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL.Common;

    /// <summary>
    /// AutoMapper resolver for converting a SamplingProtocol enumeration value
    /// to a string.
    /// </summary>
    public class SamplingProtocolResolver : ValueResolver<SamplingProtocol?, string>
    {
        protected override string ResolveCore(SamplingProtocol? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case SamplingProtocol.Normal:
                    return "Grab";
                case SamplingProtocol.Spill:
                    return "Spill";
                case SamplingProtocol.Other:
                case SamplingProtocol.SmallFish:
                    return "Other";
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// AutoMapper resolver for converting a string into a SamplingProtocol enumeration value.
    /// </summary>
    public class SamplingProtocolCodeResolver : ValueResolver<string, SamplingProtocol?>
    {
        protected override SamplingProtocol? ResolveCore(string source)
        {
            source = null == source ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            return
                "Grab".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? SamplingProtocol.Normal :
                "Spill".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? SamplingProtocol.Spill :
                "Other".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? SamplingProtocol.Other :
                (SamplingProtocol?)null;
        }
    }
}
