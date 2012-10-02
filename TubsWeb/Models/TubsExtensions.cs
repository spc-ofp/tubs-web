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

    /// <summary>
    /// TubsExtensions currently holds a grab-bag of Extension methods.
    /// This could no doubt be improved by moving extensions into smaller
    /// classes and re-factoring code to remove the need for
    /// these extensions.
    /// 
    /// Also, the to/from ViewModel stuff should be moved to
    /// AutoMapper which will improve the readability of the code.
    /// </summary>
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
        // Probably the best fix would be to centralize crew into a common table
        // and enforce jobtype constraints at the application level.
        public static Crew AsCrew(this CrewViewModel.CrewMemberModel cmm, JobType job)
        {
            Crew crew = null;
            if (null != cmm && !cmm._destroy && cmm.IsFilled)
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
                    Id = evt.EventId,
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

        public static string GetTime(this Activity activity)
        {
            if (null == activity)
                return string.Empty;

            // Prefer using the value in the _dtime column
            if (activity.LocalTime.HasValue)
                return activity.LocalTime.Value.ToString("HHmm");

            // I would hope this isn't the case, but legacy data is screwy...
            if (!string.IsNullOrEmpty(activity.LocalTimeTimeOnly.NullSafeTrim()))
                return activity.LocalTimeTimeOnly;

            // No idea!
            return string.Empty;
        }

        public static PurseSeineSetViewModel AsViewModel(this PurseSeineSet fset)
        {
            var fsvm = new PurseSeineSetViewModel();
            if (null != fset)
            {
                fsvm.SkiffOff = fset.SkiffOff;
                fsvm.SkiffOffTimeOnly = fset.SkiffOffTimeOnly;
                fsvm.WinchOnTimeOnly = fset.WinchOnTimeOnly;
                fsvm.RingsUpTimeOnly = fset.RingsUpTimeOnly;
                fsvm.BeginBrailingTimeOnly = fset.BeginBrailingTimeOnly;
                fsvm.EndBrailingTimeOnly = fset.EndBrailingTimeOnly;
                fsvm.EndOfSetTimeOnly = fset.EndOfSetTimeOnly;
                fsvm.LogbookDate = fset.StartOfSetFromLog.HasValue ? fset.StartOfSetFromLog.Value.Date : (DateTime?)null;
                fsvm.LogbookTime = fset.StartOfSetFromLog.HasValue ? fset.StartOfSetFromLog.Value.ToString("HHmm") : string.Empty;
                fsvm.SetNumber = fset.SetNumber.HasValue ? fset.SetNumber.Value : default(Int32);

                // Weights
                fsvm.WeightOnboardFromLog = fset.WeightOnboardFromLog;
                fsvm.WeightOnboardObserved = fset.WeightOnboardObserved;
                fsvm.RetainedTonnageObserved = fset.RetainedTonnageObserved;

                fsvm.NewOnboardTotalObserved = fset.NewOnboardTotalObserved;
                fsvm.RetainedTonnageFromLog = fset.RetainedTonnageFromLog;
                fsvm.NewOnboardTotalFromLog = fset.NewOnboardTotalFromLog;
                fsvm.SumOfBrail1 = fset.SumOfBrail1;
                fsvm.SumOfBrail2 = fset.SumOfBrail2;
                fsvm.TonsOfTunaObserved = fset.TonsOfTunaObserved;
                fsvm.TotalCatch = fset.TotalCatch;
                fsvm.RecoveredTagCount = fset.RecoveredTagCount;

                // SKJ
                fsvm.ContainsSkipjack = fset.ContainsSkipjack.ToYesNoFormValue();
                fsvm.SkipjackPercentage = fset.SkipjackPercentage;
                fsvm.TonsOfSkipjackObserved = fset.TonsOfSkipjackObserved;

                // YFT
                fsvm.ContainsYellowfin = fset.ContainsYellowfin.ToYesNoFormValue();
                fsvm.ContainsLargeYellowfin = fset.ContainsLargeYellowfin.ToYesNoFormValue();
                fsvm.YellowfinPercentage = fset.YellowfinPercentage;
                fsvm.TonsOfYellowfinObserved = fset.TonsOfYellowfinObserved;

                // BET
                fsvm.ContainsBigeye = fset.ContainsBigeye.ToYesNoFormValue();
                fsvm.ContainsLargeBigeye = fset.ContainsLargeBigeye.ToYesNoFormValue();
                fsvm.BigeyePercentage = fset.BigeyePercentage;
                fsvm.TonsOfBigeyeObserved = fset.TonsOfBigeyeObserved;

                var all =
                    from sc in fset.CatchList
                    where sc != null
                    select sc;
                fsvm.TargetCatch.AddRange(all.AsViewModelSetCatch());

                var target =
                    from sc in fset.CatchList
                    where
                        sc != null && (
                        sc.SpeciesCode == "BET" ||
                        sc.SpeciesCode == "YFT" ||
                        sc.SpeciesCode == "SKJ")
                    orderby sc.SpeciesCode, sc.FateCode descending
                    select sc;
                fsvm.TargetCatch.AddRange(target.AsViewModelSetCatch());

                var bycatch =
                    from sc in fset.CatchList
                    where
                        sc != null &&
                        sc.SpeciesCode != "BET" &&
                        sc.SpeciesCode != "YFT" &&
                        sc.SpeciesCode != "SKJ"
                    select sc;
                fsvm.ByCatch.AddRange(bycatch.AsViewModelSetCatch());

                // Start backfilling now
                if (fsvm.VersionNumber != 2009 && 
                    !String.IsNullOrEmpty(fset.LargeSpecies) && 
                    (fset.LargeSpeciesPercentage.HasValue || fset.LargeSpeciesCount.HasValue))
                {
                    var sp_code = fset.LargeSpecies.NullSafeTrim().NullSafeToUpper();
                    if (3 == sp_code.Length)
                    {
                        switch (sp_code)
                        {
                            case "YFT":
                                fsvm.ContainsLargeYellowfin = "YES";
                                fsvm.LargeYellowfinPercentage = fset.LargeSpeciesPercentage;
                                fsvm.LargeYellowfinCount = fset.LargeSpeciesCount;
                                break;
                            case "BET":
                                fsvm.ContainsLargeBigeye = "YES";
                                fsvm.LargeBigeyePercentage = fset.LargeSpeciesPercentage;
                                fsvm.LargeBigeyeCount = fset.LargeSpeciesCount;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        // This is a matter of guesstimation.  If we get here, then the
                        // observer recorded that the large species were both BET and YFT
                        // (well, we hope so...)
                        bool yftRecorded = sp_code.Contains("YFT");
                        bool betRecorded = sp_code.Contains("BET");

                    }
                    


                }
                fsvm.LargeSpecies = fset.LargeSpecies;
                fsvm.LargeSpeciesPercentage = fset.LargeSpeciesPercentage;
                fsvm.LargeSpeciesCount = fset.LargeSpeciesCount;
                fsvm.Comments = fset.Comments;
            }
            return fsvm;
        }

        public static IEnumerable<PurseSeineSetViewModel.SetCatch> AsViewModelSetCatch(
            this IEnumerable<PurseSeineSetCatch> catchlist)
        {
            if (null == catchlist)
                return Enumerable.Empty<PurseSeineSetViewModel.SetCatch>();
            return
                from sc in catchlist
                select new PurseSeineSetViewModel.SetCatch
                {
                    Id = sc.Id,
                    SpeciesCode = sc.SpeciesCode,
                    FateCode = sc.FateCode,
                    ObservedWeight = sc.MetricTonsObserved,
                    ObservedCount = sc.CountObserved,
                    LogbookWeight = sc.MetricTonsFromLog,
                    LogbookCount = sc.CountFromLog,
                    Comments = sc.Comments
                };
            
        }

        public static SeaDayViewModel AsViewModel(this PurseSeineSeaDay day, Trip trip = null, int dayNumber = -1, int maxDays = -1)
        {
            SeaDayViewModel sdvm = new SeaDayViewModel();

            if (dayNumber > -1 && maxDays > -1)
            {
                sdvm.DayNumber = dayNumber;
                sdvm.MaxDays = maxDays;
                sdvm.NextDay = dayNumber + 1;
                sdvm.PreviousDay = dayNumber - 1;
                sdvm.HasNext = dayNumber < maxDays;
                sdvm.HasPrevious = dayNumber > 1;
            }

            if (null != trip)
            {
                sdvm.TripId = trip.Id;
                sdvm.TripNumber = trip.SpcTripNumber ?? "This Trip";
                sdvm.TripNumber = sdvm.TripNumber.Trim();
                sdvm.VersionNumber = trip.Version == WorkbookVersion.v2009 ? 2009 : 2007;
                sdvm.ActivityCodes =
                    sdvm.VersionNumber == 2009 ?
                        SeaDayViewModel.v2009ActivityCodes :
                        SeaDayViewModel.v2007ActivityCodes;
            }

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
                            Time = a.GetTime(),
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
                            HasSet = (Spc.Ofp.Tubs.DAL.Common.ActivityType.Fishing == a.ActivityType && a.FishingSet != null),
                            /* TODO: At some point we'll have to add an extension method to check this */
                            IsLocked = (Spc.Ofp.Tubs.DAL.Common.ActivityType.Fishing == a.ActivityType && a.FishingSet != null)
                        };
                    foreach (var e in lineItems) { sdvm.Events.Add(e); }
                }
            }
            return sdvm;
        }
    }
}