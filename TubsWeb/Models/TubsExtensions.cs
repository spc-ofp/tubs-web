// -----------------------------------------------------------------------
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
                string categoryText = (trip is PurseSeineTrip) ? "Purse Seine" : "Longline";
                entry.Categories.Add(new SyndicationCategory(categoryText));
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

        public static IList<PurseSeineActivity> AsActivities(this SeaDayViewModel sdvm)
        {
            if (null == sdvm || null == sdvm.Events || 0 == sdvm.Events.Count)
                return new List<PurseSeineActivity>();
            return (
                from evt in sdvm.Events
                where !string.IsNullOrEmpty(evt.Time) && !evt._destroy
                select new PurseSeineActivity()
                {
                    LocalTime = sdvm.ShipsDate.Merge(evt.Time),
                    LocalTimeDateOnly = sdvm.ShipsDate,
                    LocalTimeTimeOnly = evt.Time,
                    ActivityType = evt.ActivityCode.ActivityFromForm(),
                    Beacon = evt.BuoyNumber,
                    Comments = evt.Comments,
                    DetectionMethod = evt.DetectionCode.DetectionFromForm(),
                    EezCode = evt.EezCode,
                    Latitude = evt.Latitude,
                    Longitude = evt.Longitude,
                    Payao = evt.FadNumber,
                    SchoolAssociation = evt.AssociationCode.AssociationFromForm(),
                    SeaCode = evt.SeaCode.SeaCodeFromForm(),
                    WindDirection = evt.WindDirection,
                    WindSpeed = evt.WindSpeed,
                }
            ).ToList();
        }

        public static PurseSeineSeaDay AsEntity(this SeaDayViewModel sdvm)
        {
            PurseSeineSeaDay pssd = new PurseSeineSeaDay();
            if (null != sdvm)
            {
                pssd.Id = sdvm.DayId;
                pssd.StartOfDay = sdvm.ShipsDate.Merge(sdvm.ShipsTime);
                pssd.StartDateOnly = sdvm.ShipsDate;
                pssd.StartTimeOnly = sdvm.ShipsTime;
                pssd.UtcStartOfDay = sdvm.UtcDate.Merge(sdvm.UtcTime);
                pssd.UtcDateOnly = sdvm.UtcDate;
                pssd.UtcTimeOnly = sdvm.UtcTime;
                pssd.FadsNoSchool = sdvm.AnchoredWithNoSchool;
                pssd.FadsWithSchool = sdvm.AnchoredWithSchool;
                pssd.FloatingObjectsNoSchool = sdvm.FreeFloatingWithNoSchool;
                pssd.FloatingObjectsWithSchool = sdvm.FreeFloatingWithSchool;
                pssd.FreeSchools = sdvm.FreeSchool;
                pssd.Gen3Events =
                    string.IsNullOrEmpty(sdvm.HasGen3Event) ? (bool?)null :
                    "YES".Equals(sdvm.HasGen3Event, StringComparison.InvariantCultureIgnoreCase) ? true : false;
                pssd.DiaryPage = sdvm.DiaryPage;
            }
            return pssd;
        }      

        public static SeaDayViewModel AsViewModel(this PurseSeineSeaDay day)
        {
            SeaDayViewModel sdvm = new SeaDayViewModel();
            if (null != day)
            {
                sdvm.DayId = day.Id;
                
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
                sdvm.HasGen3Event =
                    !day.Gen3Events.HasValue ? string.Empty :
                    day.Gen3Events.Value ? "YES" : "NO";
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
            }
            return sdvm;
        }
    }
}