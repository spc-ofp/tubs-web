// -----------------------------------------------------------------------
// <copyright file="ValidationController.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Models;
    using TubsWeb.Core;
    using System.Collections;
    using System.Text.RegularExpressions;
    
    /// <summary>
    /// ValidationController holds methods used for unobtrusive remote validation.
    /// The attributes of interest for this are:
    /// 
    /// data-val-remote-url
    /// data-val-remote-type
    /// data-val-remote-additionalfield
    /// 
    /// In addition, client needs to include jquery.validate.js and jquery.validate.unobtrusive.js
    /// 
    /// See this site:
    /// http://xhalent.wordpress.com/2011/01/21/arbitrary-client-side-validation-in-asp-net-mvc-3/
    /// </summary>
    public class ValidationController : SuperController
    {
        private const string SpeciesValidationQuery = 
            @"select 1 as valid from ref.species where UPPER(sp_code) = UPPER(?)";

        // Find any parameter that looks like species code
        private static Regex SpeciesCodeRegex = new Regex(@"speciescode=(\w{3})", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant); 

        // GET /Validation/IsSpeciesCodeValid?speciesCode=skj
        public JsonResult IsSpeciesCodeValid(string speciesCode)
        {
            if (String.IsNullOrEmpty(speciesCode))
            {
                Logger.Debug("Checking Url via regex...");
                // Rummage through the raw Url
                var match = SpeciesCodeRegex.Match(this.HttpContext.Request.RawUrl);
                if (match.Success && match.Groups.Count > 1)
                {
                    Logger.Debug("Got a match...");
                    speciesCode = match.Groups[1].Value;
                    Logger.DebugFormat("New value of speciesCode: {0}", speciesCode);
                }
            }

            if (null != speciesCode && 3 == speciesCode.Trim().Length)
            {
                IList results = TubsDataService.Execute(SpeciesValidationQuery, speciesCode);
                if (1 == results.Count)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(
                String.Format("{0} is not a valid species code", speciesCode),
                JsonRequestBehavior.AllowGet);
        }

    }
}
