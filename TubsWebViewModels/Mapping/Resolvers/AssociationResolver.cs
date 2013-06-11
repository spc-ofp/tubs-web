// -----------------------------------------------------------------------
// <copyright file="AssociationResolver.cs" company="Secretariat of the Pacific Community">
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

    public class SchoolAssociationResolver : ValueResolver<SchoolAssociation?, int?>
    {
        protected override int? ResolveCore(SchoolAssociation? source)
        {
            if (!source.HasValue || source.Value == SchoolAssociation.None)
                return null;

            switch (source.Value)
            {
                case SchoolAssociation.Unassociated:
                    return 1;
                case SchoolAssociation.FeedingOnBaitfish:
                    return 2;
                case SchoolAssociation.DriftingLog:
                    return 3;
                case SchoolAssociation.DriftingRaft:
                    return 4;
                case SchoolAssociation.AnchoredRaft:
                    return 5;
                case SchoolAssociation.LiveWhale:
                    return 6;
                case SchoolAssociation.LiveWhaleShark:
                    return 7;
                case SchoolAssociation.Other:
                    return 8;
                case SchoolAssociation.NoTuna:
                    return 9;
                default:
                    return null;
            }
        }
    }

    public class AssociationCodeResolver : ValueResolver<int?, SchoolAssociation?>
    {
        protected override SchoolAssociation? ResolveCore(int? source)
        {
            if (!source.HasValue || source.Value < 1 || source.Value > 9)
                return null;

            switch (source.Value)
            {
                case 1:
                    return SchoolAssociation.Unassociated;
                case 2:
                    return SchoolAssociation.FeedingOnBaitfish;
                case 3:
                    return SchoolAssociation.DriftingLog;
                case 4:
                    return SchoolAssociation.DriftingRaft;
                case 5:
                    return SchoolAssociation.AnchoredRaft;
                case 6:
                    return SchoolAssociation.LiveWhale;
                case 7:
                    return SchoolAssociation.LiveWhaleShark;
                case 8:
                    return SchoolAssociation.Other;
                case 9:
                    return SchoolAssociation.NoTuna;
                default:
                    return null;
            }
        }
    }
}
