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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.ViewModels;
    using AutoMapper;
    
    /// <summary>
    /// MVC Controller for working with electronic equipment information
    /// as recorded on PS-1 and LL-1 forms.
    /// </summary>
    public class ElectronicsController : SuperController
    {
        internal ActionResult ViewActionImpl(Trip tripId)
        {
            var vm = Mapper.Map<Trip, ElectronicsViewModel>(tripId) ?? new ElectronicsViewModel();
            return View(vm);
        }
        
        public ActionResult Index(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        [EditorAuthorize]
        public ActionResult Edit(Trip tripId)
        {
            return ViewActionImpl(tripId);
        }

        [EditorAuthorize]
        [HttpPost]
        [HandleTransactionManually]
        public ActionResult Edit(Trip tripId, ElectronicsViewModel vm)
        {
            return View();
        }

    }
}
