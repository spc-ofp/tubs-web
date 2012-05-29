// -----------------------------------------------------------------------
// <copyright file="DateTimeModelBinder.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Globalization;
    
    /// <summary>
    /// Work-around for the "date doesn't work if POSTed" issue
    /// From here:
    /// http://weblogs.asp.net/melvynharbour/archive/2008/11/21/mvc-modelbinder-and-localization.aspx
    /// http://stackoverflow.com/questions/8474768/asp-net-mvc3-force-controller-to-use-date-format-dd-mm-yyyy
    /// </summary>
    public class DateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var date = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).AttemptedValue;

            if (String.IsNullOrEmpty(date))
                return null;

            bindingContext.ModelState.SetModelValue(
                bindingContext.ModelName, 
                bindingContext.ValueProvider.GetValue(bindingContext.ModelName));

            try
            {
                return DateTime.ParseExact(date, "dd/mm/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, String.Format("\"{0}\" is invalid.", bindingContext.ModelName));
                return null;
            }
        }
    }
}