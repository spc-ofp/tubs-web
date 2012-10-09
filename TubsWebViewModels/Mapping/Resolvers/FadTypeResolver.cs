// -----------------------------------------------------------------------
// <copyright file="FadTypeResolver.cs" company="Secretariat of the Pacific Community">
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
    using AutoMapper;
    using Tubs = Spc.Ofp.Tubs.DAL.Common;

    /// <summary>
    /// FadTypeResolver converts numeric form code to
    /// Enum value for data access layer.
    /// </summary>
    public class FadTypeResolver : ValueResolver<int?, Tubs.FadType?>
    {
        protected override Tubs.FadType? ResolveCore(int? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case 1:
                    return Tubs.FadType.ManMadeFad;
                case 2:
                    return Tubs.FadType.ManMadeNonFad;
                case 3:
                    return Tubs.FadType.NaturalTreeOrLog;
                case 4:
                    return Tubs.FadType.ConvertedTreeOrLog;
                case 5:
                    return Tubs.FadType.Debris;
                case 6:
                    return Tubs.FadType.DeadAnimal;
                case 7:
                    return Tubs.FadType.AnchoredRaftFadOrPayao;
                case 8:
                    return Tubs.FadType.AnchoredTreeOrLogs;
                case 9:
                    return Tubs.FadType.Other;
                case 10:
                    return Tubs.FadType.ManMadeFadChanged;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// ObjectCodeResolver converts data access layer enum value to
    /// numeric form code.
    /// </summary>
    public class ObjectCodeResolver : ValueResolver<Tubs.FadType?, int?>
    {
        protected override int? ResolveCore(Tubs.FadType? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case Tubs.FadType.ManMadeFad:
                    return 1;
                case Tubs.FadType.ManMadeNonFad:
                    return 2;
                case Tubs.FadType.NaturalTreeOrLog:
                    return 3;
                case Tubs.FadType.ConvertedTreeOrLog:
                    return 4;
                case Tubs.FadType.Debris:
                    return 5;
                case Tubs.FadType.DeadAnimal:
                    return 6;
                case Tubs.FadType.AnchoredRaftFadOrPayao:
                    return 7;
                case Tubs.FadType.AnchoredTreeOrLogs:
                    return 8;
                case Tubs.FadType.Other:
                    return 9;
                case Tubs.FadType.ManMadeFadChanged:
                    return 10;
                default:
                    return null;
            }
        }
    }
}
