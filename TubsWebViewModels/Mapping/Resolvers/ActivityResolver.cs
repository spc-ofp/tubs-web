// -----------------------------------------------------------------------
// <copyright file="ActivityResolver.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Although it violates best practices, I find it's handy to keep both directions
    /// of a value resolver in the same .cs file
    /// </remarks>
    public class ActivityTypeResolver : ValueResolver<ActivityType?, string>
    {
        protected override string ResolveCore(ActivityType? source)
        {
            if (!source.HasValue)
                return null;

            switch(source.Value)
            {
                case ActivityType.Fishing:
                    return "1";
                case ActivityType.Searching:
                    return "2";
                case ActivityType.Transit:
                    return "3";
                case ActivityType.NoFishingBreakdown:
                    return "4";
                case ActivityType.NoFishingBadWeather:
                    return "5";
                case ActivityType.InPort:
                    return "6";
                case ActivityType.NetCleaningSet:
                    return "7";
                case ActivityType.InvestigateFreeSchool:
                    return "8";
                case ActivityType.InvestigateFloatingObject:
                    return "9";
                case ActivityType.DeployFad:
                    return "10D";
                case ActivityType.RetrieveFad:
                    return "10R";
                case ActivityType.NoFishingDriftingAtDaysEnd:
                    return "11";
                case ActivityType.NoFishingDriftingWithFloatingObject:
                    return "12";
                case ActivityType.NoFishingOther:
                    return "13";
                case ActivityType.DriftingWithLights:
                    return "14";
                case ActivityType.RetrieveRadioBuoy:
                    return "15R";
                case ActivityType.DeployRadioBuoy:
                    return "15D";
                case ActivityType.TransshippingOrBunkering:
                    return "16";
                case ActivityType.ServicingFad:
                    return "17";
                case ActivityType.HelicopterTakesOffToSearch:
                    return "H1";
                case ActivityType.HelicopterReturnsFromSearch:
                    return "H2";
                default:
                    return null;
            }
        }
    }

    public class ActivityCodeResolver : ValueResolver<string, ActivityType?>
    {
        protected override ActivityType? ResolveCore(string source)
        {
            source = string.IsNullOrEmpty(source) ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            return
                "1".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.Fishing :
                "2".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.Searching :
                "3".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.Transit :
                "4".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.NoFishingBreakdown :
                "5".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.NoFishingBadWeather :
                "6".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.InPort :
                "7".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.NetCleaningSet :
                "8".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.InvestigateFreeSchool :
                "9".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.InvestigateFloatingObject :
                "10D".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.DeployFad :
                "10R".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.RetrieveFad :
                "11".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.NoFishingDriftingAtDaysEnd :
                "12".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.NoFishingDriftingWithFloatingObject :
                "13".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.NoFishingOther :
                "14".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.DriftingWithLights :
                "15D".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.DeployRadioBuoy :
                "15R".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.RetrieveRadioBuoy :
                "16".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.TransshippingOrBunkering :
                "17".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.ServicingFad :
                "H1".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.HelicopterTakesOffToSearch :
                "H2".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActivityType.HelicopterReturnsFromSearch :
                (ActivityType?)null;
        }
    }
}
