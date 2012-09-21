﻿// -----------------------------------------------------------------------
// <copyright file="TubsExtensions.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Models.ExtensionMethods
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

    public static class TubsExtensions
    {
        public static string GearCodeFromVesselType(VesselTypeCode vtype)
        {
            string gearCode = String.Empty;
            switch (vtype)
            {
                case VesselTypeCode.PS:
                    gearCode = "S";
                    break;
                case VesselTypeCode.LL:
                    gearCode = "L";
                    break;
                case VesselTypeCode.PL:
                    gearCode = "P";
                    break;
                default:
                    break;
            }
            return gearCode;
        }

        public static void FillDependentObjects(this Trip trip, TripHeaderViewModel thvm, ISession session)
        {
            if (null != trip && null != thvm && null != session)
            {
                if (thvm.VesselId != default(int))
                {
                    var vesselRepo = new TubsRepository<Vessel>(session);
                    trip.Vessel = vesselRepo.FindBy(thvm.VesselId);
                }

                var staffRepo = new TubsRepository<Observer>(session);
                trip.Observer = staffRepo.FindBy(thvm.ObserverCode);

                var portRepo = new TubsRepository<Port>(session);
                trip.DeparturePort = portRepo.FindBy(thvm.DeparturePortCode);
                trip.ReturnPort = portRepo.FindBy(thvm.ReturnPortCode);
            }
        }

        public static SyndicationItem ToFeedEntry(this Trip trip)
        {
            SyndicationItem entry = new SyndicationItem();
            if (null != trip)
            {
                entry.Categories.Add(new SyndicationCategory("Purse Seine"));
                entry.Title = new TextSyndicationContent(trip.ToString());
            }
            return entry;
        }

        public static Trip ToTrip(this TripHeaderViewModel thvm)
        {
            if (null == thvm)
            {
                return null;
            }

            Trip trip = null;
            switch (thvm.GearCode)
            {
                case "S":
                    trip = new PurseSeineTrip();
                    break;
                case "L":
                    trip = new LongLineTrip();
                    break;
                case "P":
                    trip = new PoleAndLineTrip();
                    break;
                default:
                    // Don't know what kind of trip this is...
                    break;
            }

            // It's not correct to assume that DepartureDate and ReturnDate have values.
            // Yes, they are required, but the caller may pass us an invalid structure
            // here.  Bleah!
            if (null != trip)
            {
                trip.DepartureDate = thvm.DepartureDate;
                if (trip.DepartureDate.HasValue)
                {
                    trip.DepartureTimeOnly = trip.DepartureDate.Value.ToString("HHmm");
                    trip.DepartureDateOnly = thvm.DepartureDate.Value.Subtract(thvm.DepartureDate.Value.TimeOfDay);
                }
                trip.ReturnDate = thvm.ReturnDate;
                if (trip.ReturnDate.HasValue)
                {
                    trip.ReturnTimeOnly = trip.ReturnDate.Value.ToString("HHmm");
                    trip.ReturnDateOnly = thvm.ReturnDate.Value.Subtract(thvm.ReturnDate.Value.TimeOfDay);
                }
                trip.TripNumber = thvm.TripNumber;
                if (Enum.IsDefined(typeof(ObserverProgram), thvm.ProgramCode))
                {
                    trip.ProgramCode = (ObserverProgram)Enum.Parse(typeof(ObserverProgram), thvm.ProgramCode);
                }
                trip.CountryCode = thvm.CountryCode;

                if (Enum.IsDefined(typeof(WorkbookVersion), thvm.Version))
                {
                    trip.Version = (WorkbookVersion)Enum.Parse(typeof(WorkbookVersion), thvm.Version);
                }
            }
            return trip;
        }

        public static void CopyTo(this CrewViewModel.CrewMemberModel cmm, Crew crew)
        {
            if (null != cmm)
            {
                crew.Id = cmm.Id;
                crew.Job = cmm.Job;
                crew.Name = cmm.Name;
                crew.YearsExperience = cmm.Years;
                crew.CountryCode = cmm.Nationality;
                crew.Comments = cmm.Comments;
            }
        }

        // TODO:  This is hard-coded for Purse Seine crew, something that needs fixing...
        public static Crew AsCrew(this CrewViewModel.CrewMemberModel cmm, JobType job)
        {
            Crew crew = null;
            if (null != cmm && cmm.IsFilled)
            {
                crew = new PurseSeineCrew();
                cmm.CopyTo(crew);
                if (!crew.Job.HasValue)
                    crew.Job = job;
            }
            return crew;
        }

        public static IList<Crew> AsCrewList(this CrewViewModel cvm)
        {
            var crewlist = new List<Crew>(16);
            crewlist.Add(cvm.Captain.AsCrew(JobType.Captain));
            crewlist.Add(cvm.Navigator.AsCrew(JobType.NavigatorOrMaster));
            crewlist.Add(cvm.Mate.AsCrew(JobType.Mate));
            crewlist.Add(cvm.ChiefEngineer.AsCrew(JobType.ChiefEngineer));
            crewlist.Add(cvm.AssistantEngineer.AsCrew(JobType.AssistantEngineer));
            crewlist.Add(cvm.DeckBoss.AsCrew(JobType.DeckBoss));
            crewlist.Add(cvm.Cook.AsCrew(JobType.Cook));
            crewlist.Add(cvm.HelicopterPilot.AsCrew(JobType.HelicopterPilot));
            crewlist.Add(cvm.SkiffMan.AsCrew(JobType.SkiffMan));
            crewlist.Add(cvm.WinchMan.AsCrew(JobType.WinchMan));           
            crewlist.AddRange(cvm.Hands.Select(c => c.AsCrew(JobType.Crew)));
            return crewlist.Where(c => c != null).ToList();
        }

        public static Crew CreateCrew(this Trip trip)
        {
            if (null == trip)
            {
                return null;
            }
            var tripType = trip.GetType();
            // As more crew types come online, add them here
            return typeof(PurseSeineTrip) == tripType ? new PurseSeineCrew() : null;
        }

        public static SeaDay CreateSeaDay(this Trip trip, DateTime date)
        {
            if (null == trip)
            {
                return null;
            }

            var tripType = trip.GetType();

            DateTime midnightOf = new DateTime(date.Ticks);
            midnightOf.Subtract(date.TimeOfDay);
            // As more trip types come online, add them here
            SeaDay day =
                typeof(PurseSeineTrip) == tripType ?
                    new PurseSeineSeaDay() :
                    null;

            if (null != day)
            {
                day.StartDateOnly = midnightOf;
                day.StartOfDay = midnightOf;
                day.StartTimeOnly = "0000";
            }

            return day;
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

        public static SeaDayViewModel AsViewModel(this PurseSeineSeaDay day)
        {
            SeaDayViewModel sdvm = new SeaDayViewModel();
            if (null != day)
            {
                sdvm.TripId = day.Trip.Id;
                sdvm.DayId = day.Id;
                sdvm.TripNumber = day.Trip.SpcTripNumber ?? "This Trip";
                sdvm.VersionNumber = day.Trip.Version == WorkbookVersion.v2009 ? 2009 : 2007;
                sdvm.ActivityCodes = 
                    sdvm.VersionNumber == 2009 ? 
                        SeaDayViewModel.v2009ActivityCodes : 
                        SeaDayViewModel.v2007ActivityCodes;
                
                // Start of Day
                sdvm.ShipsDate = day.StartDateOnly;
                sdvm.ShipsTime = day.StartTimeOnly;
                sdvm.UtcDate = day.UtcDateOnly;
                sdvm.UtcTime = day.UtcTimeOnly;

                // Floating object and school sightings
                sdvm.AnchoredWithNoSchool = day.FadsNoSchool;
                sdvm.AnchoredWithSchool = day.FadsWithSchool;
                sdvm.FreeFloatingWithNoSchool = day.FloatingObjectsNoSchool;
                sdvm.FreeFloatingWithSchool = day.FloatingObjectsWithSchool;
                sdvm.FreeSchool = day.FreeSchools;
                sdvm.HasGen3Event = day.Gen3Events;
                sdvm.DiaryPage = day.DiaryPage;

                // Line items
                if (null != day.Activities && day.Activities.Count > 0)
                {
                    var lineItems =
                        from a in day.Activities
                        select new SeaDayViewModel.SeaDayEvent
                        {
                            EventId = a.Id,
                            Time = a.LocalTime.HasValue ? a.LocalTime.Value.ToString("HHmm") : string.Empty,
                            Latitude = a.Latitude.NullSafeTrim(),
                            Longitude = a.Longitude.NullSafeTrim(),
                            EezCode = a.EezCode.NullSafeTrim(),
                            ActivityCode = a.ActivityType.ToFormValue(),
                            WindSpeed = a.WindSpeed,
                            WindDirection = a.WindDirection,
                            SeaCode = a.SeaCode.HasValue ? a.SeaCode.ToString() : null,
                            DetectionCode = a.DetectionMethod.ToFormValue(),
                            AssociationCode = a.SchoolAssociation.ToFormValue(),
                            FadNumber = a.Payao.NullSafeTrim(),
                            BuoyNumber = a.Beacon.NullSafeTrim(),
                            Comments = a.Comments.NullSafeTrim(),
                            HasSet = (Spc.Ofp.Tubs.DAL.Common.ActivityType.Fishing == a.ActivityType && a.FishingSet != null)
                        };
                    foreach (var e in lineItems) { sdvm.Events.Add(e); }
                }
                else
                {
                    // Ensure there's at least one event
                    sdvm.Events.Add(new SeaDayViewModel.SeaDayEvent());
                }
            }
            return sdvm;
        }
    }
}