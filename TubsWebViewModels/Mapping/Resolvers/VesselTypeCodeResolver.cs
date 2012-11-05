// -----------------------------------------------------------------------
// <copyright file="VesselTypeCodeResolver.cs" company="Secretariat of the Pacific Community">
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

    public class VesselTypeCodeResolver : ValueResolver<int?, SightedVesselType?>
    {
        protected override SightedVesselType? ResolveCore(int? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case 1:
                    return SightedVesselType.SinglePurseSeine;
                case 2:
                    return SightedVesselType.Longline;
                case 3:
                    return SightedVesselType.PoleAndLine;
                case 4:
                    return SightedVesselType.Mothership;
                case 5:
                    return SightedVesselType.Troll;
                case 6:
                    return SightedVesselType.NetBoat;
                case 7:
                    return SightedVesselType.Bunker;
                case 8:
                    return SightedVesselType.SearchAnchorOrLightBoat;
                case 9:
                    return SightedVesselType.FishCarrier;
                case 10:
                    return SightedVesselType.Trawler;
                case 21:
                    return SightedVesselType.LightAircraft;
                case 22:
                    return SightedVesselType.Helicopter;
                case 31:
                    return SightedVesselType.Other;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VesselTypeResolver : ValueResolver<SightedVesselType?, int?>
    {
        protected override int? ResolveCore(SightedVesselType? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case SightedVesselType.SinglePurseSeine:
                    return 1;
                case SightedVesselType.Longline:
                    return 2;
                case SightedVesselType.PoleAndLine:
                    return 3;
                case SightedVesselType.Mothership:
                    return 4;
                case SightedVesselType.Troll:
                    return 5;
                case SightedVesselType.NetBoat:
                    return 6;
                case SightedVesselType.Bunker:
                    return 7;
                case SightedVesselType.SearchAnchorOrLightBoat:
                    return 8;
                case SightedVesselType.FishCarrier:
                    return 9;
                case SightedVesselType.Trawler:
                    return 10;
                case SightedVesselType.LightAircraft:
                    return 21;
                case SightedVesselType.Helicopter:
                    return 22;
                case SightedVesselType.Other:
                    return 31;
                default:
                    return null;
            }
        }
    }
}
