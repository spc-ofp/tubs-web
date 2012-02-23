// -----------------------------------------------------------------------
// <copyright file="ObserverController.cs" company="Secretariat of the Pacific Community">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;


    /// <summary>
    /// Lookup-only controller for adding Ajax autocomplete capabilities.
    /// </summary>
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class ObserverController : SuperController
    {
 
        private static string Format(Observer observer)
        {
            return null == observer ? String.Empty : observer.ToString();
        }

        public JsonResult Find(string term)
        {
            var repo = new TubsRepository<Observer>(MvcApplication.CurrentSession);
            var observers = (
                from observer in repo.FilterBy(o => o.FirstName.Contains(term) || o.LastName.Contains(term))
                select new
                {
                    id = observer.StaffCode,
                    label = Format(observer),
                    value = Format(observer)
                }
            );
            return Json(observers.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
