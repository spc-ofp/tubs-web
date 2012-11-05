// -----------------------------------------------------------------------
// <copyright file="LengthUnitResolver.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// AutoMapper resolver for converting string unit of measure into a DAL enumeration
    /// value.
    /// </summary>
    public class LengthUnitResolver : ValueResolver<string, DAL.Common.UnitOfMeasure?>
    {
        protected override DAL.Common.UnitOfMeasure? ResolveCore(string source)
        {
            source = null == source ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            return
                "km".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.Kilometers :
                "Nm".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.NauticalMiles :
                "cm".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.Centimeters :
                "in".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.Inches :
                "Y".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.Yards :                
                "Yards".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.Yards :
                "m".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.Meters :
                "Meters".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.Meters :
                "F".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.Fathoms :
                "Fathoms".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? DAL.Common.UnitOfMeasure.Fathoms :
                (DAL.Common.UnitOfMeasure?)null;
        }
    }

    /// <summary>
    /// AutoMapper resolver for converting a DAL enumeration value into a string representation
    /// </summary>
    public class UnitOfMeasureResolver : ValueResolver<DAL.Common.UnitOfMeasure?, string>
    {
        protected override string ResolveCore(DAL.Common.UnitOfMeasure? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case DAL.Common.UnitOfMeasure.Centimeters:
                    return "cm";
                case DAL.Common.UnitOfMeasure.Fathoms:
                    return "F";
                case DAL.Common.UnitOfMeasure.Inches:
                    return "in";
                case DAL.Common.UnitOfMeasure.Kilometers:
                    return "km";
                case DAL.Common.UnitOfMeasure.Meters:
                    return "m";
                case DAL.Common.UnitOfMeasure.NauticalMiles:
                    return "Nm";
                case DAL.Common.UnitOfMeasure.Yards:
                    return "Y";
                default:
                    return null;
            }
        }
    }
}
