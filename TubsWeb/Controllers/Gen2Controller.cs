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
    using Newtonsoft.Json;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;

    public class Gen2Controller : SuperController
    {

        internal IEnumerable<Interaction> SortedInteractions(Trip tripId)
        {
            return
                from i in tripId.Interactions
                orderby i.LandedDate, i.SpeciesCode
                select i;
        }

        internal ActionResult ViewActionImpl(Trip tripId, int pageNumber)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

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
            vm.HasPrevious = pageNumber > 0;
            vm.PreviousPage = pageNumber - 1;
            vm.HasNext = pageNumber < maxPages;
            vm.NextPage = pageNumber + 1;
            vm.ActionName = CurrentAction();

            if (IsApiRequest())
                return GettableJsonNetData(vm);

            AddMinMaxDates(tripId);            
            return View(CurrentAction(), vm);
        }

        public ActionResult List(Trip tripId)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

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
                    interaction is GearInteraction ? "Interacted with gear" :
                    "Unknown Interaction";

                currentPage++;
                summaries.Add(summary);
            }

            if (IsApiRequest())
                return GettableJsonNetData(summaries);

            return View(summaries);
        }

        public ActionResult Index(Trip tripId, int pageNumber)
        {
            return ViewActionImpl(tripId, pageNumber);
        }

        [EditorAuthorize]
        public ActionResult Edit(Trip tripId, int pageNumber)
        {
            return ViewActionImpl(tripId, pageNumber);
        }

        // Binder can't create an abstract class, but we can put something into the posted data to help it figure out the
        // correct concrete implementation.
        [HttpPost]
        [HandleTransactionManually]
        [EditorAuthorize]
        public ActionResult Edit(Trip tripId, [AbstractBind(ConcreteTypeParameter = "interactionType")] Gen2ViewModel vm)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            if (!ModelState.IsValid)
            {
                if (IsApiRequest())
                    return ModelErrorsResponse();
                return View(vm);
            }

            var entity = Mapper.Map<Gen2ViewModel, Interaction>(vm);
            entity.Trip = tripId;
            entity.SetAuditTrail(User.Identity.Name, DateTime.Now);
            // TODO:  What else needs setting here?  Audit trail and...

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<Interaction> repo = TubsDataService.GetRepository<Interaction>(MvcApplication.CurrentSession);
                repo.Save(entity);
                xa.Commit();
            }


            if (IsApiRequest())
            {
                using (var trepo = TubsDataService.GetRepository<Interaction>(false))
                {
                    var temp = trepo.FindById(entity.Id);
                    vm = Mapper.Map<Interaction, Gen2ViewModel>(temp);
                }
                return GettableJsonNetData(vm);

            }

            // Tricky part here is figuring out the page number
            return RedirectToAction("Edit", "Gen2", new { tripId = tripId.Id });
        }

        [EditorAuthorize]
        public ActionResult AddLanded(Trip tripId)
        {
            var vm = new Gen2LandedViewModel();
            vm.TripId = tripId.Id;
            vm.TripNumber = tripId.SpcTripNumber;
            AddMinMaxDates(tripId);
            return View(vm);
        }

        [EditorAuthorize]
        public ActionResult AddGear(Trip tripId)
        {
            var vm = new Gen2GearViewModel();
            vm.TripId = tripId.Id;
            vm.TripNumber = tripId.SpcTripNumber;
            AddMinMaxDates(tripId);
            return View(vm);
        }

        [EditorAuthorize]
        public ActionResult AddSighting(Trip tripId)
        {
            var vm = new Gen2SightingViewModel();
            vm.TripId = tripId.Id;
            vm.TripNumber = tripId.SpcTripNumber;
            AddMinMaxDates(tripId);
            return View(vm);
        }
    }
}