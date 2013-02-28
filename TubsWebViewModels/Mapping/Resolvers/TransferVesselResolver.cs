// -----------------------------------------------------------------------
// <copyright file="TransferVesselResolver.cs" company="Secretariat of the Pacific Community">
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
    public class TransferVesselResolver : ValueResolver<VesselType?, int?>
    {
        protected override int? ResolveCore(VesselType? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case VesselType.SinglePurseSeine:
                    return 1;
                case VesselType.Longline:
                    return 2;
                case VesselType.PoleAndLine:
                    return 3;
                case VesselType.Mothership:
                    return 4;
                case VesselType.Troll:
                    return 5;
                case VesselType.NetBoat:
                    return 6;
                case VesselType.Bunker:
                    return 7;
                case VesselType.SearchAnchorOrLightBoat:
                    return 8;
                case VesselType.FishCarrier:
                    return 9;
                case VesselType.Trawler:
                    return 10;
                case VesselType.LightAircraft:
                    return 21;
                case VesselType.Helicopter:
                    return 22;
                case VesselType.Other:
                    return 31;
                default:
                    return null;
            }

        }
    }

    public class TransferVesselCodeResolver : ValueResolver<int?, VesselType?>
    {
        protected override VesselType? ResolveCore(int? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case 1:
                    return VesselType.SinglePurseSeine;
                case 2:
                    return VesselType.Longline;
                case 3:
                    return VesselType.PoleAndLine;
                case 4:
                    return VesselType.Mothership;
                case 5:
                    return VesselType.Troll;
                case 6:
                    return VesselType.NetBoat;
                case 7:
                    return VesselType.Bunker;
                case 8:
                    return VesselType.SearchAnchorOrLightBoat;
                case 9:
                    return VesselType.FishCarrier;
                case 10:
                    return VesselType.Trawler;
                case 21:
                    return VesselType.LightAircraft;
                case 22:
                    return VesselType.Helicopter;
                case 31:
                    return VesselType.Other;
                default:
                    return null;
            }
        }
    }
}
