// -----------------------------------------------------------------------
// <copyright file="TubsExtensions.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Models
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
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using NHibernate;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    
    /// <summary>
    /// FormExtensions holds the extension methods that convert from
    /// database reference IDs to actual form values (and vice versa).
    /// </summary>
    public static class FormExtensions
    {
        #region Detection Method
        public static DetectionMethod? DetectionFromForm(this int? formValue)
        {
            if (!formValue.HasValue || formValue.Value < 1 || formValue.Value > 7)
                return null;

            switch (formValue.Value)
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
        
        public static int? ToFormValue(this DetectionMethod? detectionMethod)
        {
            if (!detectionMethod.HasValue || detectionMethod.Value == DetectionMethod.None)
                return null;

            switch (detectionMethod.Value)
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
        #endregion

        #region School Association
        public static SchoolAssociation? AssociationFromForm(this int? formValue)
        {
            if (!formValue.HasValue || formValue.Value < 1 || formValue.Value > 9)
                return null;

            switch (formValue.Value)
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

        public static int? ToFormValue(this SchoolAssociation? association)
        {
            if (!association.HasValue || association.Value == SchoolAssociation.None)
                return null;

            switch (association.Value)
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
        #endregion

        #region Activity
        public static ActivityType? ActivityFromForm(this string formValue)
        {
            formValue = formValue.NullSafeTrim().NullSafeToUpper();
            if (string.IsNullOrEmpty(formValue))
                return null;

            switch (formValue)
            {
                case "1":
                    return ActivityType.Fishing;
                case "2":
                    return ActivityType.Searching;
                case "3":
                    return ActivityType.Transit;
                case "4":
                    return ActivityType.NoFishingBreakdown;
                case "5":
                    return ActivityType.NoFishingBadWeather;
                case "6":
                    return ActivityType.InPort;
                case "7":
                    return ActivityType.NetCleaningSet;
                case "8":
                    return ActivityType.InvestigateFreeSchool;
                case "9":
                    return ActivityType.InvestigateFloatingObject;
                case "10D":
                    return ActivityType.DeployFad;
                case "10R":
                    return ActivityType.RetrieveFad;
                case "11":
                    return ActivityType.NoFishingDriftingAtDaysEnd;
                case "12":
                    return ActivityType.NoFishingDriftingWithFloatingObject;
                case "13":
                    return ActivityType.NoFishingOther;
                case "14":
                    return ActivityType.DriftingWithLights;
                case "15D":
                    return ActivityType.DeployRadioBuoy;
                case "15R":
                    return ActivityType.RetrieveRadioBuoy;
                case "16":
                    return ActivityType.TransshippingOrBunkering;
                case "17":
                    return ActivityType.ServicingFad;
                case "H1":
                    return ActivityType.HelicopterTakesOffToSearch;
                case "H2":
                    return ActivityType.HelicopterReturnsFromSearch;
                default:
                    return null;
            }
        }

        public static string ToFormValue(this ActivityType? activity)
        {
            if (!activity.HasValue || activity.Value == ActivityType.None)
                return null;

            switch (activity.Value)
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
        #endregion

        public static SeaCode? SeaCodeFromForm(this string formValue)
        {
            formValue = formValue.NullSafeTrim().NullSafeToUpper();
            if (string.IsNullOrEmpty(formValue))
                return null;

            switch (formValue)
            {
                case "C":
                    return SeaCode.C;
                case "S":
                    return SeaCode.S;
                case "M":
                    return SeaCode.M;
                case "R":
                    return SeaCode.R;
                case "V":
                    return SeaCode.V;
                default:
                    return null;
            }
        }

        public static string ToYesNoFormValue(this bool? dbValue)
        {
            if (!dbValue.HasValue)
                return String.Empty;

            return dbValue.Value ? "YES" : "NO";
        }

        public static bool? FromYesNoFormValue(this string formValue)
        {
            formValue = formValue.NullSafeTrim();
            if (String.IsNullOrEmpty(formValue))
                return null;

            return "YES".Equals(formValue, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}