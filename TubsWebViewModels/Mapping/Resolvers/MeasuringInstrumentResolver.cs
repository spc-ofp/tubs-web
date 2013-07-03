// -----------------------------------------------------------------------
// <copyright file="MeasuringInstrumentResolver.cs" company="Secretariat of the Pacific Community">
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

    public sealed class MeasuringInstrumentTypeResolver : ValueResolver<MeasuringInstrument?, string>
    {
        protected override string ResolveCore(MeasuringInstrument? source)
        {
            if (!source.HasValue)
                return null;

            switch(source.Value)
            {
                case MeasuringInstrument.B:
                    return "Board";
                case MeasuringInstrument.C:
                    return "Aluminum Caliper";
                case MeasuringInstrument.R:
                    return "Ruler";
                case MeasuringInstrument.T:
                    return "Deck Tape";
                case MeasuringInstrument.W:
                    return "Wooden Caliper";
                default:
                    return null;
            }
        }
    }

    public sealed class MeasuringInstrumentCodeResolver : ValueResolver<string, MeasuringInstrument?>
    {
        protected override MeasuringInstrument? ResolveCore(string source)
        {
            source = string.IsNullOrEmpty(source) ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            return
                "Board".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? MeasuringInstrument.B :
                "Aluminum Caliper".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? MeasuringInstrument.C :
                "Ruler".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? MeasuringInstrument.R :
                "Deck Tape".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? MeasuringInstrument.T :
                "Wooden Caliper".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? MeasuringInstrument.W :
                (MeasuringInstrument?)null;

        }
    }
}
