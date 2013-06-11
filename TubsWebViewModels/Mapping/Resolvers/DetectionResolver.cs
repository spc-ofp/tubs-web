// -----------------------------------------------------------------------
// <copyright file="DetectionResolver.cs" company="Secretariat of the Pacific Community">
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

    public class DetectionMethodResolver : ValueResolver<DetectionMethod?, int?>
    {
        protected override int? ResolveCore(DetectionMethod? source)
        {
            if (!source.HasValue || source.Value == DetectionMethod.None)
                return null;

            switch (source.Value)
            {
                case DetectionMethod.SeenFromVessel:
                    return 1;
                case DetectionMethod.SeenFromHelicopter:
                    return 2;
                case DetectionMethod.MarkedWithBeacon:
                    return 3;
                case DetectionMethod.BirdRadar:
                    return 4;
                case DetectionMethod.Sonar:
                    return 5;
                case DetectionMethod.InfoFromOtherVessel:
                    return 6;
                case DetectionMethod.Anchored:
                    return 7;
                default:
                    return null;
            }
        }
    }

    public class DetectionCodeResolver : ValueResolver<int?, DetectionMethod?>
    {
        protected override DetectionMethod? ResolveCore(int? source)
        {
            if (!source.HasValue || source.Value < 1 || source.Value > 7)
                return null;

            switch (source.Value)
            {
                case 1:
                    return DetectionMethod.SeenFromVessel;
                case 2:
                    return DetectionMethod.SeenFromHelicopter;
                case 3:
                    return DetectionMethod.MarkedWithBeacon;
                case 4:
                    return DetectionMethod.BirdRadar;
                case 5:
                    return DetectionMethod.Sonar;
                case 6:
                    return DetectionMethod.InfoFromOtherVessel;
                case 7:
                    return DetectionMethod.Anchored;
                default:
                    return null;
            }
        }
    }
}
