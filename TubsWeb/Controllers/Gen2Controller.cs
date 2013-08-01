// -----------------------------------------------------------------------
// <copyright file="Gen2Controller.cs" company="Secretariat of the Pacific Community">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    /// <summary>
    /// Controller for GEN-2 data.
    /// </summary>
    public class Gen2Controller : SuperController
    {

        /// <summary>
        /// SortedInteractions ensures that all the methods in this controller use the same
        /// sort algorithm so that page numbers are consistent throughout the app.
        /// </summary>
        /// <param name="tripId">Trip with interactions</param>
        /// <returns>IEnumerable of Interaction entities sorted by date and then species code</returns>
        internal IEnumerable<Interaction> SortedInteractions(Trip tripId)
        {
            return
                from i in tripId.Interactions
                orderby i.LandedDate, i.SpeciesCode
                select i;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        internal ActionResult ViewActionImpl(Trip tripId, int pageNumber)
        {
            int maxPages = tripId.Interactions.Count;
            if (pageNumber > maxPages)
            {
                pageNumber = maxPages;
            }

            ViewBag.MaxPages = maxPages;
            ViewBag.CurrentPage = pageNumber;

            var interaction = SortedInteractions(tripId).Skip(pageNumber - 1).Take(1).FirstOrDefault();
            var vm = Mapper.Map<Interaction, Gen2ViewModel>(interaction);

            // Set some nav properties
            vm.PageNumber = pageNumber;
            vm.HasPrevious = pageNumber > 0;
            vm.PreviousPage = pageNumber - 1;
            vm.HasNext = pageNumber < maxPages;
            vm.NextPage = pageNumber + 1;
            vm.ActionName = CurrentAction;

            if (IsApiRequest)
                return GettableJsonNetData(vm);

            AddMinMaxDates(tripId);            
            return View(CurrentAction, vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="vm"></param>
        /// <returns></returns>
        internal ActionResult AddImpl(Trip tripId, Gen2ViewModel vm)
        {
            vm.TripId = tripId.Id;
            vm.TripNumber = tripId.SpcTripNumber;
            vm.ActionName = CurrentAction;
            AddMinMaxDates(tripId);
            return View(CurrentAction, vm);
        }

        /// <summary>
        /// MVC Action for displaying all SSIs for a given trip.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [ValidTripFilter]
        public ActionResult List(Trip tripId)
        {
            ViewBag.SpcTripNumber = tripId.SpcTripNumber;

            IList<Gen2SummaryViewModel> summaries = new List<Gen2SummaryViewModel>();
            
            int currentPage = 1;
            foreach (var interaction in SortedInteractions(tripId))
            {
                // Gen2SummaryViewModel is small enough that we'll fill the object by hand
                var summary = new Gen2SummaryViewModel()
                {
                    ShipsDate = interaction.LandedDate.Value,
                    SpeciesCode = interaction.SpeciesCode,
                    PageNumber = currentPage
                };

                summary.InteractionType =
                    interaction is SightingInteraction ? "Sighted Only" :
                    interaction is LandedInteraction ? "Landed On Deck" :
                    interaction is GearInteraction ? "Interacted With Gear" :
                    "Unknown Interaction";

                currentPage++;
                summaries.Add(summary);
            }

            if (IsApiRequest)
                return GettableJsonNetData(summaries);

            return View(summaries);
        }

        /// <summary>
        /// MVC Action for displaying a singe special species interaction.
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [ValidTripFilter]
        public ActionResult Index(Trip tripId, int pageNumber)
        {
            return ViewActionImpl(tripId, pageNumber);
        }

        /// <summary>
        /// MVC Action 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [EditorAuthorize]
        [ValidTripFilter]
        public ActionResult AddLanded(Trip tripId)
        {
            return AddImpl(tripId, new Gen2LandedViewModel());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [EditorAuthorize]
        [ValidTripFilter]
        public ActionResult AddGear(Trip tripId)
        {
            return AddImpl(tripId, new Gen2GearViewModel());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <returns></returns>
        [EditorAuthorize]
        [ValidTripFilter]
        public ActionResult AddSighting(Trip tripId)
        {
            return AddImpl(tripId, new Gen2SightingViewModel());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId">Current trip</param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [EditorAuthorize]
        [ValidTripFilter]
        public ActionResult Edit(Trip tripId, int pageNumber)
        {
            return ViewActionImpl(tripId, pageNumber);
        }

        /// <summary>
        /// MVC Action to modify interaction data.
        /// </summary>
        /// <remarks>
        /// Binder can't create an abstract class, but we can put something into the posted data to help it figure out the
        /// correct concrete implementation.
        /// </remarks>
        /// <param name="tripId">Current trip</param>
        /// <param name="vm">Interaction view model</param>
        /// <returns></returns>
        [HttpPost]       
        [EditorAuthorize]
        [ValidTripFilter]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, [AbstractBind(ConcreteTypeParameter = "InteractionType")] Gen2ViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                if (IsApiRequest)
                    return ModelErrorsResponse();
                return View(vm);
            }

            var entity = Mapper.Map<Gen2ViewModel, Interaction>(vm);
            entity.Trip = tripId;
            // TODO:  Parent/child entities like this, where it is unlikely that operations will be
            // done on the child alone should manage the full audit trail here
            entity.SetAuditTrail(User.Identity.Name, DateTime.Now);

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<Interaction> repo = TubsDataService.GetRepository<Interaction>(MvcApplication.CurrentSession);
                IRepository<GearInteractionDetail> drepo = TubsDataService.GetRepository<GearInteractionDetail>(MvcApplication.CurrentSession);

                // Delete detail objects as appropriate
                if (vm is Gen2GearViewModel)
                {                    
                    // Deletes first
                    var gvm = vm as Gen2GearViewModel;                    
                    gvm.Deleted.ToList().ForEach(i => drepo.DeleteById(i.Id));
                }

                repo.Save(entity);

                // Save details after the entity
                if (vm is Gen2GearViewModel)
                {
                    var gentity = entity as GearInteraction;
                    gentity.Details.ToList().ForEach(d =>
                    {
                        d.SetAuditTrail(User.Identity.Name, DateTime.Now);
                        //drepo.Update(d, d.Id != default(int));
                        drepo.Save(d);
                    }); 
                }

                xa.Commit();
            }


            if (IsApiRequest)
            {
                using (var trepo = TubsDataService.GetRepository<Interaction>(false))
                {
                    var temp = trepo.FindById(entity.Id);
                    vm = Mapper.Map<Interaction, Gen2ViewModel>(temp);
                }
                return GettableJsonNetData(vm);

            }

            // Load trip on a new session and look for the interaction that matches the one
            // that was just saved.  That will give us the correct "page number" until
            // the inclusion of page number in the model gets worked out.
            // This is subject to a race condition, but I'm not going to worry about it
            // for what I feel is a very unlikely occurrence.
            int pageNumber = 1;
            using (var tripRepo = TubsDataService.GetRepository<Trip>(false))
            {
                var temp = tripRepo.FindById(tripId.Id);
                var interactions = SortedInteractions(temp);

                foreach (var item in interactions)
                {
                    if (item.Id == entity.Id)
                    {
                        break;
                    }

                    pageNumber++;
                }
            }

            return RedirectToAction("Edit", "Gen2", new { tripId = tripId.Id, pageNumber = pageNumber });
        }
        
    }
}