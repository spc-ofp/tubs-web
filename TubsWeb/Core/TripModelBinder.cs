// -----------------------------------------------------------------------
// <copyright file="TripModelBinder.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
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
    
    /// <summary>
    /// TripModelBinder binds a Trip entity into controller methods.
    /// </summary>
    public class TripModelBinder : IModelBinder
    {
        /// <summary>
        /// BindModel binds the trip entity.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns>Trip model if Id represents existing trip, null otherwise.</returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (null == value)
            {
                return null;
            }

            if (String.IsNullOrEmpty(value.AttemptedValue))
            {
                return null;
            }

            int entityId;

            if (!Int32.TryParse(value.AttemptedValue, out entityId))
            {
                return null;
            }
            // Set TripId in ViewBag regardless of existence of Trip.
            controllerContext.Controller.ViewBag.TripId = entityId;
            // FIXME
            // This is a _BRUTAL HACK_
            // NHibernate session filters aren't used when looking up an entity directly by primary key.
            // However, if we change the API, we can force the use of a session filter
            // See the SimpleTripFilter, SimpleTripHeaderFilter, and SimpleTripFilterWithFindBy unit tests in the DAL project for more details.
            var trip = new TubsRepository<Trip>(MvcApplication.CurrentSession).FindBy(t => t.Id == entityId);
            // FIXME The following line is the original version and can be re-enabled when the DAL has a better security implementation
            //var trip = new TubsRepository<Trip>(MvcApplication.CurrentSession).FindBy(entityId);
            // ReadOnly is nonsense for a non-existant trip, but for the sake of completeness, we want
            // to ensure that IsReadOnly is present in the ViewBag
            controllerContext.Controller.ViewBag.IsReadOnly = (null == trip) ? true : trip.IsReadOnly;
            return trip;
        }
    }
}