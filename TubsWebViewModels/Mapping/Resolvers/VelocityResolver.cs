// -----------------------------------------------------------------------
// <copyright file="VelocityResolver.cs" company="Secretariat of the Pacific Community">
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

    public sealed class VelocityCodeResolver : ValueResolver<string, DAL.Common.UnitOfMeasure?>
    {
        protected override DAL.Common.UnitOfMeasure? ResolveCore(string source)
        {
            source = null == source ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            return
                "m/s".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.MetersPerSecond :
                "kts".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.Knots :
                (DAL.Common.UnitOfMeasure?)null;

        }
    }

    public sealed class VelocityTypeResolver : ValueResolver<DAL.Common.UnitOfMeasure?, string>
    {
        protected override string ResolveCore(DAL.Common.UnitOfMeasure? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case DAL.Common.UnitOfMeasure.MetersPerSecond:
                    return "m/s";
                case DAL.Common.UnitOfMeasure.Knots:
                    return "kts";
                default:
                    return null;
            }
        }
    }
}
