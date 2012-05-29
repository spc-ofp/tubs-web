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

            if (null != trip)
            {
                trip.DepartureDate = thvm.DepartureDate;
                trip.DepartureTimeOnly = trip.DepartureDate.Value.ToString("HHmm");
                trip.DepartureDateOnly = thvm.DepartureDate.Value.Subtract(thvm.DepartureDate.Value.TimeOfDay);
                trip.ReturnDate = thvm.ReturnDate;
                trip.ReturnTimeOnly = trip.ReturnDate.Value.ToString("HHmm");
                trip.ReturnDateOnly = thvm.ReturnDate.Value.Subtract(thvm.ReturnDate.Value.TimeOfDay);
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
                crew.MonthsExperience = cmm.Months;
                crew.CountryCode = cmm.Nationality;
                crew.Comments = cmm.Comments;
            }
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
    }
}