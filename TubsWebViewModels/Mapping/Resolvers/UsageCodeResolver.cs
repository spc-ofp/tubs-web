// -----------------------------------------------------------------------
// <copyright file="UsageCodeResolver.cs" company="Secretariat of the Pacific Community">
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

    public class UsageCodeTextResolver : ValueResolver<string, UsageCode?>
    {
        protected override UsageCode? ResolveCore(string source)
        {
            source = null == source ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            UsageCode? value = null;
            try
            {
                value = (UsageCode)Enum.Parse(typeof(UsageCode), source, true);
            }
            catch (ArgumentException) { /* Don't care.  Initializing it to null handles what ArgumentException would tell us */}
            return value;
        }
    }

    /// <summary>
    /// UsageCodeResolver converts a UsageCode enumeration value into it's
    /// string representation.  Luckily, enum values exactly match string values.
    /// </summary>
    public class UsageCodeResolver : ValueResolver<UsageCode?, string>
    {
        protected override string ResolveCore(UsageCode? source)
        {
            if (!source.HasValue)
                return String.Empty;

            return source.Value.ToString();
        }
    }
}
