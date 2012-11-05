// -----------------------------------------------------------------------
// <copyright file="FadOriginResolver.cs" company="Secretariat of the Pacific Community">
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
    using Tubs = Spc.Ofp.Tubs.DAL.Common;

    /// <summary>
    /// FadOriginResolver converts numeric form code to
    /// Enum value for data access layer.
    /// </summary>
    public class FadOriginResolver : ValueResolver<int?, Tubs.FadOrigin?>
    {
        protected override Tubs.FadOrigin? ResolveCore(int? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case 1:
                    return Tubs.FadOrigin.DeployedThisTrip;
                case 2:
                    return Tubs.FadOrigin.DeployedPreviousTrip;
                case 3:
                    return Tubs.FadOrigin.OtherVesselOwnerConsent;
                case 4:
                    return Tubs.FadOrigin.OtherVesselNoOwnerConsent;
                case 5:
                    return Tubs.FadOrigin.OtherVesselConsentUnknown;
                case 6:
                    return Tubs.FadOrigin.FoundDrifting;
                case 7:
                    return Tubs.FadOrigin.DeployedByAuxillary;
                case 8:
                    return Tubs.FadOrigin.Unknown;
                case 9:
                    return Tubs.FadOrigin.Other;
                default:
                    return null;
            }
        }
    }

    public class OriginDescriptionResolver : ValueResolver<Tubs.FadOrigin?, string>
    {
        protected override string ResolveCore(Tubs.FadOrigin? source)
        {
            if (!source.HasValue)
                return string.Empty;

            return source.Value.GetDescription();
        }
    }

    /// <summary>
    /// OriginCodeResolver converts data access layer enum value to
    /// numeric form code.
    /// </summary>
    public class OriginCodeResolver : ValueResolver<Tubs.FadOrigin?, int?>
    {
        protected override int? ResolveCore(Tubs.FadOrigin? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case Tubs.FadOrigin.DeployedThisTrip:
                    return 1;
                case Tubs.FadOrigin.DeployedPreviousTrip:
                    return 2;
                case Tubs.FadOrigin.OtherVesselOwnerConsent:
                    return 3;
                case Tubs.FadOrigin.OtherVesselNoOwnerConsent:
                    return 4;
                case Tubs.FadOrigin.OtherVesselConsentUnknown:
                    return 5;
                case Tubs.FadOrigin.FoundDrifting:
                    return 6;
                case Tubs.FadOrigin.DeployedByAuxillary:
                    return 7;
                case Tubs.FadOrigin.Unknown:
                    return 8;
                case Tubs.FadOrigin.Other:
                    return 9;
                default:
                    return null;
            }
        }
    }
}
