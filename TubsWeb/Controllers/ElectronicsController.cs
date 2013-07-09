// -----------------------------------------------------------------------
// <copyright file="ElectronicsController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
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
    /// MVC Controller for working with electronic equipment information
    /// as recorded on PS-1 and LL-1 forms.
    /// </summary>
    public class ElectronicsController : SuperController
    {
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }
            var vm = Mapper.Map<Trip, ElectronicsViewModel>(tripId) ?? new ElectronicsViewModel();

            if (IsApiRequest())
                return GettableJsonNetData(vm);

            return View(vm);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <returns></returns>
        public ActionResult Index(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <returns></returns>
        [EditorAuthorize]
        public ActionResult Edit(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripId"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        [EditorAuthorize]
        [HttpPost]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, ElectronicsViewModel vm)
        {
            if (null == tripId)
            {
                return InvalidTripResponse();
            }

            // TODO Are there any real validations on this model?
            // Maybe if someone provides a mobile phone # and then says there is
            // no mobile phone for the vessel...
            if (!ModelState.IsValid)
            {
                LogModelErrors();
                if (IsApiRequest())
                    return ModelErrorsResponse();
                return View(vm);
            }

            // The viewmodel entity with the 'ignore all' goes first
            var comms = Mapper.Map<ElectronicsViewModel.CommunicationServices, CommunicationServices>(vm.Communications);
            comms = Mapper.Map<ElectronicsViewModel.InformationServices, CommunicationServices>(vm.Info, comms);

            // There is no good way to set this in AutoMapper
            comms.Id = vm.ServiceId;
            comms.Trip = tripId;
            comms.SetAuditTrail(User.Identity.Name, DateTime.Now);

            var electronics = new List<ElectronicDevice>(16);
            electronics.AddRange(
                from d in vm.Vms.Union(vm.Buoys).Union(vm.OtherDevices)
                where d != null && !d._destroy
                select Mapper.Map<ElectronicsViewModel.DeviceModel, ElectronicDevice>(d)
            );

            //electronics.AddRange(
            //    from b in vm.Buoys
            //    where b != null && !b._destroy
            //    select Mapper.Map<ElectronicsViewModel.DeviceModel, ElectronicDevice>(b)
            //);

            //electronics.AddRange(
            //    from d in vm.OtherDevices
            //    where d != null && !d._destroy
            //    select Mapper.Map<ElectronicsViewModel.DeviceModel, ElectronicDevice>(d)
            //);

            electronics.AddRange(
                from c in vm.Categories
                select Mapper.Map<ElectronicsViewModel.DeviceCategory, ElectronicDevice>(c)
            );

            using (var xa = MvcApplication.CurrentSession.BeginTransaction())
            {
                IRepository<ElectronicDevice> erepo = TubsDataService.GetRepository<ElectronicDevice>(MvcApplication.CurrentSession);
                IRepository<CommunicationServices> crepo = TubsDataService.GetRepository<CommunicationServices>(MvcApplication.CurrentSession);

                // Deletes first
                vm.DeletedDevices.ToList().ForEach(dd => erepo.DeleteById(dd));

                foreach (var device in electronics)
                {
                    device.Trip = tripId;
                    // TODO Confirm entered by/updated by backfill is occurring
                    device.SetAuditTrail(User.Identity.Name, DateTime.Now);
                    erepo.Save(device);
                }

                crepo.Save(comms);

                xa.Commit();

            }

            if (IsApiRequest())
            {
                using (var repo = TubsDataService.GetRepository<Trip>(false))
                {
                    var trip = repo.FindById(tripId.Id);
                    vm = Mapper.Map<Trip, ElectronicsViewModel>(trip);
                }

                return GettableJsonNetData(vm);
            }

            return RedirectToAction("Edit", "Electronics", new { tripId = tripId.Id });
        }

    }
}
