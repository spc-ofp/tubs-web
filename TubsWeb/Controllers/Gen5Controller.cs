// -----------------------------------------------------------------------
// <copyright file="Gen5Controller.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
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
    using System.Net;
    using System.Web.Mvc;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;
    using TubsWeb.Core;
    using Spc.Ofp.Tubs.DAL.Common;
    using System.Collections.Generic;

    public class Gen5Controller : SuperController
    {
        //
        // GET: /GEN-5/Details
        public ActionResult Index(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            ViewBag.TripNumber = trip.SpcTripNumber;
            // I'd like this to be stateless, since we could give a fig about the
            // materials, but the AutoMapper portion would choke...
            var repo = TubsDataService.GetRepository<Gen5Object>(MvcApplication.CurrentSession);
            var fads = repo.FilterBy(f => f.Activity.Day.Trip.Id == tripId.Id);
            var viewModels =
                from f in fads
                select Mapper.Map<Gen5Object, Gen5ViewModel>(f);

            return View(viewModels.AsEnumerable());
        }

        internal ActionResult ViewActionImpl(Trip tripId, int? fadId, int? activityId)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            var repo = TubsDataService.GetRepository<Gen5Object>(MvcApplication.CurrentSession);
            Gen5Object fad = null;

            if (fadId.HasValue)
            {
                fad = repo.FindById(fadId.Value);
            }
            else if (activityId.HasValue)
            {
                fad = repo.FilterBy(f => f.Activity.Id == activityId.Value).FirstOrDefault();
            }

            if (!IsAdd() && (null == fad || fad.Activity.Day.Trip.Id != trip.Id))
            {
                if (IsApiRequest())
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return GettableJsonNetData("Missing or invalid GEN-5 record");
                }
                // TODO Need a better strategy...
                return RedirectToAction("Index", new { tripId = trip.Id });
            }

            var fvm = Mapper.Map<Gen5Object, Gen5ViewModel>(fad);
            if (IsAdd() && null == fvm)
            {
                fvm = new Gen5ViewModel
                {
                    TripId = trip.Id,
                    TripNumber = trip.SpcTripNumber,
                    ActivityId = activityId.HasValue ? activityId.Value : 0,
                    VersionNumber = trip.Version == WorkbookVersion.v2009 ? 2009 : 2007,
                    MainMaterials = new List<Gen5ViewModel.FadMaterial>(),
                    Attachments = new List<Gen5ViewModel.FadMaterial>()
                };
            }
            if (IsApiRequest())
                return GettableJsonNetData(fvm);

            return View(CurrentAction(), fvm);
        }

        internal ActionResult SaveActionImpl(Trip tripId, Gen5ViewModel fvm)
        {
            var trip = tripId as PurseSeineTrip;
            if (null == trip)
            {
                return InvalidTripResponse();
            }

            // TODO:  What kind of validation needs to occur?
            // Should we pull this snippet out into a new function?
            if (!ModelState.IsValid)
            {
                if (IsApiRequest())
                    return ModelErrorsResponse();
                
                return View(CurrentAction(), fvm);
            }

            var fad = Mapper.Map<Gen5ViewModel, Gen5Object>(fvm);
            if (null == fad)
            {
                if (IsApiRequest())
                    return ModelErrorsResponse();

                return View(CurrentAction(), fvm);
            }

            fad.SetAuditTrail(User.Identity.Name, DateTime.Now);

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<Gen5Material> mrepo = TubsDataService.GetRepository<Gen5Material>(MvcApplication.CurrentSession);
                IRepository<Gen5Object> frepo = TubsDataService.GetRepository<Gen5Object>(MvcApplication.CurrentSession);
                IRepository<Activity> erepo = TubsDataService.GetRepository<Activity>(MvcApplication.CurrentSession);

                fad.Activity = erepo.FindById(fvm.ActivityId) as PurseSeineActivity;
                if (null == fad.Activity)
                {
                    throw new Exception("Yo dawg, this shouldn't happen");
                }

                // Manually handle deletes
                fvm.MainMaterials.Where(x => x != null && x._destroy).ToList().ForEach(x =>
                {
                    mrepo.DeleteById(x.Id);
                });

                fvm.Attachments.Where(x => x != null && x._destroy).ToList().ForEach(x =>
                {
                    mrepo.DeleteById(x.Id);
                });

                fad.Materials.ToList().ForEach(x =>
                {
                    mrepo.Update(x, x.Id != default(int));
                });

                bool merge = fad.Id != default(int);
                frepo.Update(fad, merge);
                xa.Commit();
                MvcApplication.CurrentSession.Evict(fad);
                // Reload doesn't appear to be all that valuable
                // for entities with a parent-child relationship
                // Still, we'll keep the reload so that we can guarantee
                // we've got the correct entity primary key
                frepo.Reload(fad);
            }

            if (IsApiRequest())
            {
                using (var repo = TubsDataService.GetRepository<Gen5Object>(false))
                {
                    fad = repo.FindById(fad.Id);
                    fvm = Mapper.Map<Gen5Object, Gen5ViewModel>(fad);
                }
                
                return GettableJsonNetData(fvm);
            }

            // If this isn't an API request (which shouldn't really happen)
            // always push to the Edit page.  It's been saved, so an Add is counter-productive
            // (besides, redirecting to Add with the current dayNumber will redirect to Edit anyways...)
            return RedirectToAction("Edit", "Gen5", new { tripId = tripId.Id, fadId = fad.Id });
        }
        
        //
        // GET: /GEN-5/Details
        public ActionResult Details(Trip tripId, int? fadId, int? activityId)
        {
            return ViewActionImpl(tripId, fadId, activityId);
        }

        public ActionResult Edit(Trip tripId, int fadId)
        {
            return ViewActionImpl(tripId, fadId, null);
        }

        [HttpPost]
        [HandleTransactionManually]
        [EditorAuthorize]
        public ActionResult Edit(Trip tripId, Gen5ViewModel fvm)
        {
            return SaveActionImpl(tripId, fvm);
        }

        public ActionResult Add(Trip tripId, int activityId)
        {
            return ViewActionImpl(tripId, null, activityId);
        }

        // TODO:  Do we need this?  Not that there's much to it, but
        // it could be confusing...
        [HttpPost]
        [HandleTransactionManually]
        [EditorAuthorize]
        public ActionResult Add(Trip tripId, Gen5ViewModel fvm)
        {
            return SaveActionImpl(tripId, fvm);
        } 

        

    }
}
