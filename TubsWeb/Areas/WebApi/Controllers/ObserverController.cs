// -----------------------------------------------------------------------
// <copyright file="ObserverController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Areas.WebApi.Controllers
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
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.OData;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;

    // This appears to be broken in the current version
    // http://aspnetwebstack.codeplex.com/workitem/416
    // Wait for an update before implementing
    public class ObserverController : EntitySetController<Observer, string>
    {
        [Queryable]
        public override IQueryable<Observer> Get()
        {
            var repo = TubsDataService.GetRepository<Observer>(MvcApplication.CurrentStatelessSession);
            return repo.All();
        }

        protected override Observer GetEntityByKey(string key)
        {
            var repo = TubsDataService.GetRepository<Observer>(MvcApplication.CurrentStatelessSession);
            return repo.FindById(key);
        }
    }
}
