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
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    /// <summary>
    /// MVC controller for GEN-5 (FAD) forms.
    /// </summary>
    public class Gen5Controller : SuperController
    {
        /// <summary>
        /// Action for displaying a list of GEN-5 (FAD) interactions for a trip.
        /// </summary>
        /// <example>
        /// GET: /Trip/12345/GEN-5/Index
        /// </example>
        /// <param name="tripId"></param>
        /// <returns></returns>
        [ValidTripFilter(TripType=typeof(PurseSeineTrip))]
        public ActionResult Index(Trip tripId)
        {
            var trip = tripId as PurseSeineTrip;
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

        /// <summary>
        /// Used by the display and edit actions.  An ID must be specified for
        /// either the FAD or the activity.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="fadId">FAD entity primary key (optional)</param>
        /// <param name="activityId">Activity entity primary key (optional)</param>
        /// <returns>ViewModel representing a single FAD interaction</returns>
        internal ActionResult ViewActionImpl(Trip tripId, int? fadId, int? activityId)
        {
            var trip = tripId as PurseSeineTrip;
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

            if (!IsAdd && (null == fad || fad.Activity.Day.Trip.Id != trip.Id))
            {
                if (IsApiRequest)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return GettableJsonNetData("Missing or invalid GEN-5 record");
                }
                // TODO Need a better strategy...
                return RedirectToAction("Index", new { tripId = trip.Id });
            }

            var fvm = Mapper.Map<Gen5Object, Gen5ViewModel>(fad);
            if (IsAdd && null == fvm)
            {
                fvm = new Gen5ViewModel
                {
                    TripId = trip.Id,
                    TripNumber = trip.SpcTripNumber,
                    ActivityId = activityId.HasValue ? activityId.Value : 0,
                    VersionNumber = trip.Version == WorkbookVersion.v2009 ? 2009 : 2007
                };
            }
            if (IsApiRequest)
                return GettableJsonNetData(fvm);

            return View(CurrentAction, fvm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="fvm"></param>
        /// <returns></returns>
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        internal ActionResult SaveActionImpl(Trip tripId, Gen5ViewModel fvm)
        {
            var trip = tripId as PurseSeineTrip;
            // TODO:  What kind of validation needs to occur?
            // Should we pull this snippet out into a new function?
            if (!ModelState.IsValid)
            {
                if (IsApiRequest)
                    return ModelErrorsResponse();
                
                return View(CurrentAction, fvm);
            }

            var fad = Mapper.Map<Gen5ViewModel, Gen5Object>(fvm);
            if (null == fad)
            {
                if (IsApiRequest)
                    return ModelErrorsResponse();

                return View(CurrentAction, fvm);
            }

            fad.SetAuditTrail(User.Identity.Name, DateTime.Now);

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<Gen5Object> frepo = TubsDataService.GetRepository<Gen5Object>(MvcApplication.CurrentSession);     
                IRepository<Gen5Material> mrepo = TubsDataService.GetRepository<Gen5Material>(MvcApplication.CurrentSession);                
                IRepository<Activity> erepo = TubsDataService.GetRepository<Activity>(MvcApplication.CurrentSession);

                fad.Activity = erepo.FindById(fvm.ActivityId) as PurseSeineActivity;
                if (null == fad.Activity)
                {
                    throw new Exception("Yo dawg, this shouldn't happen");
                }

                // This is the tricky part.  If the item is new, it needs to be
                // saved so that the child entities have a non-null parent key.
                // _HOWEVER_, if the item is not new, NHibernate freaks out
                // when you try to save the parent first, saying there are
                // transient objects that need saving.
                // I'm not super happy about this approach, but it works:
                // 1)  If the item is new, save it before processing children
                // 2)  If the item is not new, defer the save until after the children have been processed
                // TODO:  Migrate this strategy through the app
                bool isNew = fad.IsNew();
                if (isNew)
                {
                    frepo.Save(fad);
                }

                // Manually handle deletes
                if (null != fvm.MainMaterials && fvm.MainMaterials.Count > 0)
                {
                    fvm.MainMaterials.Where(x => x != null && x._destroy).ToList().ForEach(x =>
                    {
                        mrepo.DeleteById(x.Id);
                    });
                }

                if (null != fvm.Attachments && fvm.Attachments.Count > 0)
                {
                    fvm.Attachments.Where(x => x != null && x._destroy).ToList().ForEach(x =>
                    {
                        mrepo.DeleteById(x.Id);
                    });
                }

                fad.Materials.ToList().ForEach(x =>
                {
                    mrepo.Update(x, x.Id != default(int));
                });

                if (!isNew)
                {
                    frepo.Save(fad);
                }
                
                xa.Commit();

            }

            if (IsApiRequest)
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <example>GET: /Trip/12345/GEN-5/Details</example>
        /// <param name="tripId"></param>
        /// <param name="fadId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Details(Trip tripId, int? fadId, int? activityId)
        {
            return ViewActionImpl(tripId, fadId, activityId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="fadId"></param>
        /// <returns></returns>
        [HttpGet]
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Edit(Trip tripId, int fadId)
        {
            return ViewActionImpl(tripId, fadId, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="fvm"></param>
        /// <returns></returns>
        [HttpPost]
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, Gen5ViewModel fvm)
        {
            return SaveActionImpl(tripId, fvm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [HttpGet]
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        public ActionResult Add(Trip tripId, int activityId)
        {
            return ViewActionImpl(tripId, null, activityId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="fvm"></param>
        /// <returns></returns>
        [HttpPost]       
        [EditorAuthorize]
        [ValidTripFilter(TripType = typeof(PurseSeineTrip))]
        [HandleTransactionManually]
        public ActionResult Add(Trip tripId, Gen5ViewModel fvm)
        {
            return SaveActionImpl(tripId, fvm);
        } 

        

    }
}
