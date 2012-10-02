// -----------------------------------------------------------------------
// <copyright file="SeaDayController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers
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
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models;
    using TubsWeb.Models.ExtensionMethods;

    /// <summary>
    /// SeaDayController manages PS-2 Daily Log data.
    /// As it's focused on PS-2 data, it only works for
    /// Purse Seine trips.
    /// </summary>
    public class SeaDayController : SuperController
    {

        /// <summary>
        /// NeedsRedirect validates the dayNumber param for a trip based on the
        /// action and the number of days already existing for this trip.
        /// 
        /// The logic is as follows:
        /// - Index:  Don't care -- Index will cap dayNumber @ maxDays
        /// - Add:  If dayNumber is a day that already exists, push to Edit.
        ///         If dayNumber isn't the appropriate value based on maxDays,
        ///         redirect to Add with the appropriate value
        /// - Edit: If dayNumber is past the end of the trip, redirect to edit
        ///         with the final dayNumber
        /// </summary>
        /// <param name="tripId">obstrip_id</param>
        /// <param name="dayNumber">Index (one based) of day number for this trip</param>
        /// <param name="maxDays">Number of days already added to this trip</param>
        /// <returns></returns>
        internal Tuple<bool, RouteValueDictionary> NeedsRedirect(int tripId, int dayNumber, int maxDays)
        {
            var needsRedirect = false;
            // Fill values that don't change
            var rvd = new RouteValueDictionary(
                new { controller = "SeaDay", tripId = tripId }
            );

            if (IsAdd())
            {
                if (dayNumber <= maxDays)
                {
                    // Redirect to edit
                    rvd["dayNumber"] = dayNumber;
                    rvd["action"] = "Edit";
                    needsRedirect = true;
                }
                else if (dayNumber > (maxDays + 1))
                {
                    // Redirect to add, but with correct dayNumber
                    rvd["dayNumber"] = (maxDays + 1);
                    rvd["action"] = "Add";
                    needsRedirect = true;
                }
            }
            else if (IsEdit())
            {
                if (dayNumber > maxDays)
                {
                    rvd["dayNumber"] = maxDays;
                    rvd["action"] = "Edit";
                    needsRedirect = true;
                }
            }

            return Tuple.Create(needsRedirect, rvd);
        }

        /// <summary>
        /// The HttpGet portion of the Add/Edit screens is very similar.
        /// By extracting it all into a single function, there's less copy/paste
        /// inheritance going on.
        /// </summary>
        /// <param name="tripId">Trip parameter (passed as int by the front end, turned into a trip by Model binder)</param>
        /// <param name="dayNumber">Day Number parameter</param>
        /// <returns></returns>
        internal ActionResult ViewActionImpl(Trip tripId, int dayNumber)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var days = TubsDataService.GetRepository<SeaDay>(
                MvcApplication.CurrentSession).FilterBy(
                    d => d.Trip.Id == tripId.Id);

            // This LINQ method results in a trip to the database
            // (select count(1) from ... where obstrip_id = ?)
            // It's also worth mentioning that if an Add/Edit has
            // a 'bad' dayNumber param, we'll be running this query twice
            // I expect SQL Server to be able to handle that...
            int maxDays = days.Count();
            var checkpoint = NeedsRedirect(tripId.Id, dayNumber, maxDays);
            if (checkpoint.Item1)
                return new RedirectToRouteResult(checkpoint.Item2);

            // One minor point.  If the user passes in a completely crazy dayNumber for Index
            // we'll re-interpret based on intent.
            if (IsIndex())
            {
                if (dayNumber < 1) { dayNumber = 1; }
                if (dayNumber > maxDays) { dayNumber = maxDays; } 
            }

            // Based on NeedsRedirect, we should be okay -- the
            // dayNumber should be perfect for the action
            var day = days.Skip(dayNumber - 1).Take(1).FirstOrDefault() as PurseSeineSeaDay;
            var sdvm = day.AsViewModel(trip, dayNumber, maxDays);
            sdvm.ActionName = CurrentAction();

            if (sdvm.NeedsDates() && (IsAdd() || IsEdit()))
            {
                // First day is easy... (Maybe we should do this in 'CreateTrip'?)
                if (dayNumber == 1)
                {
                    if (!sdvm.ShipsDate.HasValue)
                    {
                        sdvm.ShipsDate = trip.DepartureDateOnly;
                    }
                    if (string.IsNullOrEmpty(sdvm.ShipsTime))
                    {
                        sdvm.ShipsTime = trip.DepartureTimeOnly;
                    }
                }
                else
                {
                    var yesterday = trip.SeaDays.Skip(dayNumber - 2).Take(1).FirstOrDefault();
                    if (null != yesterday)
                    {
                        if (!sdvm.UtcDate.HasValue && yesterday.UtcStartOfDay.HasValue)
                            sdvm.UtcDate = yesterday.UtcDateOnly.Value.AddDays(1);
                        if (!sdvm.ShipsDate.HasValue && yesterday.StartOfDay.HasValue)
                            sdvm.ShipsDate = yesterday.StartDateOnly.Value.AddDays(1);
                    }
                }
            }

            if (IsApiRequest())
                return GettableJsonNetData(sdvm);

            return View(CurrentAction(), sdvm);

        }

        /// <summary>
        /// Validate checks the validity of the view model.
        /// Errors are saved via ModelState
        /// </summary>
        /// <param name="tripId">Trip model, used for some validation.</param>
        /// <param name="sdvm">View model to be validated.</param>
        internal void Validate(Trip tripId, SeaDayViewModel sdvm)
        {
            // The delta between Ship's date and UTC date shouldn't be more than +/- 1 day 
            DateTime? shipStartOfDay = sdvm.ShipsDate.Merge(sdvm.ShipsTime);
            DateTime? utcStartOfDay = sdvm.UtcDate.Merge(sdvm.UtcTime);
            if (shipStartOfDay.HasValue && utcStartOfDay.HasValue)
            {
                var deltaT = shipStartOfDay.Value.Subtract(utcStartOfDay.Value);
                if (Math.Abs(deltaT.TotalDays) > 1.0d)
                {
                    var msg = string.Format("Difference of {0} days between ship's and local time too large", Math.Abs(deltaT.TotalDays));
                    ModelState["UtcDate"].Errors.Add(msg);
                    // TODO Work out whether we should set the error state on all 4 fields, or just one at random
                }
            }

            // TODO Check the dayNumber?

            // No start of day should be before the trip start date
            Logger.Info("ShipsDate: " + sdvm.ShipsDate);
            Logger.Info("ShipsTime: " + sdvm.ShipsTime);
            Logger.Info("shipStartOfDay: " + shipStartOfDay);
            if (shipStartOfDay.HasValue && shipStartOfDay.Value.CompareTo(tripId.DepartureDate) < 0)
            {
                ModelState["ShipsDate"].Errors.Add("Start of day can't be before the trip departure date");
            }

            // We don't particularly care which control has these, since the drop downs should prevent the user
            // from actually changing these.  This is here to guard against API bugs
            var seaCodes = sdvm.Keepers.Select(e => e.SeaCode).DefaultIfEmpty();
            var invalidSeaCodes = seaCodes.Where(c => c != null && !sdvm.SeaCodes.Contains(c)).Distinct();
            if (invalidSeaCodes.Count() > 0)
                ModelState["SeaCode"].Errors.Add(String.Format("Invalid Sea Code(s): [{0}]", string.Join(",", invalidSeaCodes)));

            var detectionCodes = sdvm.Keepers.Select(e => e.DetectionCode).DefaultIfEmpty();
            var invalidDetectionCodes = detectionCodes.Where(c => c != null && !sdvm.DetectionCodes.Contains(c)).Distinct();
            if (invalidDetectionCodes.Count() > 0)
                ModelState["DetectionCode"].Errors.Add(String.Format("Invalid Detection Code(s): [{0}]", string.Join(",", invalidDetectionCodes)));

            var associationCodes = sdvm.Keepers.Select(e => e.AssociationCode).DefaultIfEmpty();
            var invalidAssociationCodes = associationCodes.Where(c => c!= null && !sdvm.AssociationCodes.Contains(c)).Distinct();
            if (invalidAssociationCodes.Count() > 0)
                ModelState["AssociationCode"].Errors.Add(String.Format("Invalid Association Code(s): [{0}]", string.Join(",", invalidAssociationCodes)));

            var invalidDeletes = sdvm.Deleted.Where(e => e.HasSet && e.IsLocked);
            if (invalidDeletes.Count() > 0)
                ModelState["ActivityCode"].Errors.Add("Can't delete set activity without confirmation");

            // In this case, we're trusting the UI to send us good information.  If that turns into a problem, we can
            // bounce the IDs off the database instead
            var invalidActivities = sdvm.Keepers.Where(e => e.HasSet && !"1".Equals(e.ActivityCode));
            if (invalidActivities.Count() > 0)
                ModelState["ActivityCode"].Errors.Add("Can't change Activity Code with associated set");

        }

        // 
        internal ActionResult SaveActionImpl(Trip tripId, int dayNumber, SeaDayViewModel sdvm)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                if (IsApiRequest())
                    return GettableJsonNetData("No such trip");
                return new NoSuchTripResult();
            }

            Validate(tripId, sdvm);           

            if (!ModelState.IsValid)
            {
                if (IsApiRequest())
                    return ModelErrorsResponse();
                return View(sdvm);
            }

            // Try this for now, refactor later (if at all)
            // drepo for (d)ay, erepo for (e)vent (aka Activity).
            var seaDay = sdvm.AsEntity();

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<SeaDay> drepo = TubsDataService.GetRepository<SeaDay>(MvcApplication.CurrentSession);
                IRepository<Activity> erepo = TubsDataService.GetRepository<Activity>(MvcApplication.CurrentSession);
                IRepository<PurseSeineSet> srepo = TubsDataService.GetRepository<PurseSeineSet>(MvcApplication.CurrentSession);

                seaDay.SetAuditTrail(User.Identity.Name, DateTime.Now);
                seaDay.Trip = trip;

                drepo.Add(seaDay);
                // If we get here, the database didn't freak over the header, so now we can add the details  
                // Deletes first
                sdvm.Deleted.ToList().ForEach(e => erepo.DeleteById(e.EventId));

                var activities = sdvm.AsActivities();
                foreach (var activity in activities)
                {
                    // If an activity is _new_ and has an activity code of fishing, create a new empty
                    // set.  This will enable easier entry on the PS-3 side.  Set# and Page# should match, so remind users
                    // to check?
                    bool addSet = (default(int) == activity.Id && ActivityType.Fishing == activity.ActivityType);
                    activity.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    activity.Day = seaDay;
                    erepo.Add(activity);
                    if (addSet)
                    {
                        // Application expects SetNumber to be set at creation.
                        // There's an easy path and a hard path...
                        var sets = srepo.FilterBy(fs => fs.Activity.Day.Trip.Id == trip.Id);

                        var maxSetDate = sets.Select(fs => fs.SkiffOff).Max();

                        // Whether the set order is okay or not, we use the next available set number
                        // for .SetNumber
                        int nextSetNumber = (sets.Select(fs => fs.SetNumber).Max() ?? 0) + 1;

                        // This is not entirely true, but it's good enough for a first pass approximation
                        bool setOrderOkay = 
                            !maxSetDate.HasValue ? 
                                true :
                                -1 == maxSetDate.Value.CompareTo(activity.LocalTime.Value);

                        var fset = new PurseSeineSet();
                        fset.SetNumber = nextSetNumber;
                        fset.SkiffOff = activity.LocalTime;
                        fset.SkiffOffTimeOnly = activity.LocalTimeTimeOnly;
                        fset.Activity = activity;
                        fset.SetAuditTrail(User.Identity.Name, DateTime.Now);
                        srepo.Add(fset);

                        // This is where stuff goes sideways...
                        if (!setOrderOkay)
                        {
                            // TODO: Re-order the sets according to date
                            // This could get crazy, so perhaps we need to notify the
                            // user somehow...
                        }
                    }
                }

                xa.Commit();
            }

            // If the save worked, the entity should have been updated with the
            // correct primary key.  Use the primary key to reload the entity
            // NOTE:  There's a second database call to get the current number of days
            // We should be able to calculate that, but we might as well treat the
            // database as definitive.

            // This is the happy path -- if we get here, everything should have worked...
            if (IsApiRequest())
            {
                // For some reason (could be a bug, could be something I'm forgetting to do)
                // the ISession that was used for the updates doesn't reflect said updates
                // after the commit.
                // This didn't appear in CrewController because the reload used the
                // stateless session (no dependent objects on crew).
                // To be clear, if I use MvcApplication.CurrentSession here, the parent object
                // (PurseSeineSeaDay) is loaded, but the child objects (Activities) are not.
                // This isn't a great workaround, but it's a workaround nonetheless.
                using (var repo = TubsDataService.GetRepository<SeaDay>(false))
                {
                    // Force a reload after a save to ensure that PKs are set for all
                    // appropriate entities.
                    int maxDays = repo.FilterBy(d => d.Trip.Id == tripId.Id).Count();

                    var day = repo.FindById(seaDay.Id) as PurseSeineSeaDay;

                    sdvm = day.AsViewModel(tripId, dayNumber, maxDays);
                    sdvm.ActionName = CurrentAction();
                }

                return GettableJsonNetData(sdvm);
            }

            // If this isn't an API request (which shouldn't really happen)
            // always push to the Edit page.  It's been saved, so an Add is counter-productive
            // (besides, redirecting to Add with the current dayNumber will redirect to Edit anyways...)
            return RedirectToAction("Edit", "SeaDay", new { tripId = tripId.Id, dayNumber = dayNumber });
        }
        
        //
        // GET: /SeaDay/
        public ActionResult List(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            var repo = new TubsRepository<SeaDay>(MvcApplication.CurrentSession);            
            // Push the projection into a List so that it's not the NHibernate collection implementation
            var days = repo.FilterBy(d => d.Trip.Id == tripId.Id).ToList<SeaDay>();
            ViewBag.Title = String.Format("Sea days for {0}", tripId.ToString());
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            return View(days);
        }

        public ActionResult Index(Trip tripId, int dayNumber)
        {
            return ViewActionImpl(tripId, dayNumber);
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Add(Trip tripId, int dayNumber)
        {
            return ViewActionImpl(tripId, dayNumber);
        }

        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId, int dayNumber)
        {
            return ViewActionImpl(tripId, dayNumber);
        }

        [HttpPost]
        [HandleTransactionManually]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Add(Trip tripId, int dayNumber, SeaDayViewModel sdvm)
        {
            return SaveActionImpl(tripId, dayNumber, sdvm);
        }

        // This should only ever be hit by the Knockout function, but we'll still probably want
        // to cater for straight up HTML Forms
        [HttpPost]
        [HandleTransactionManually]
        [Authorize(Roles = Security.EditRoles)]
        public ActionResult Edit(Trip tripId, int dayNumber, SeaDayViewModel sdvm)
        {
            return SaveActionImpl(tripId, dayNumber, sdvm);
        }

        // TODO Remove this
        public ActionResult AutoFill(Trip tripId)
        {
            if (null == tripId)
            {
                // TODO Figure out if this is really how we want to handle this...
                return new NoSuchTripResult();
            }

            var repo = new TubsRepository<SeaDay>(MvcApplication.CurrentSession);
            if (repo.FilterBy(d => d.Trip.Id == tripId.Id).Count() > 0)
            {
                // Already has at least one day -- don't mess with it
                return JavaScript("alert('One or more days already present');");
            }

            // Figure out how many days are between departure and end date
            TimeSpan span = tripId.ReturnDate.Value.Subtract(tripId.DepartureDate.Value);
            int daysAdded = 0;
            for (int i = 0; i <= span.Days; i++)
            {
                DateTime startDate = tripId.DepartureDate.Value.AddDays(i);
                SeaDay seaDay = tripId.CreateSeaDay(startDate);
                var enteredDate = DateTime.Now;
                if (null != seaDay)
                {
                    seaDay.Trip = tripId;
                    seaDay.EnteredBy = User.Identity.Name;
                    seaDay.EnteredDate = enteredDate;
                    repo.Add(seaDay);
                    daysAdded++;
                }
            }

            return JavaScript(String.Format("alert('Added {0} day(s)');", daysAdded));
        }

    }
}
