// -----------------------------------------------------------------------
// <copyright file="PortController.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    
    /// <summary>
    /// Lookup-only controller for adding Ajax autocomplete capabilities.
    /// </summary>
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class PortController : SuperController
    {

        private static string Format(Port port)
        {
            return null == port ? String.Empty : String.Format("{0} ({1})", port.Name.Trim(), port.CountryCode);
        }

        [UseStatelessSessions]
        public JsonResult Find(string term)
        {
            var repo = TubsDataService.GetRepository<Port>(MvcApplication.CurrentStatelessSession);
            var ports = (
                from port in repo.FilterBy(p => p.Name.Contains(term))
                where port.PortCode != null
                select new
                {
                    id = port.PortCode,
                    label = Format(port),
                    value = Format(port)
                }
            );
            return Json(ports.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
