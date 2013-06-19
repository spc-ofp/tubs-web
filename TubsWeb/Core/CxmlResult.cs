// -----------------------------------------------------------------------
// <copyright file="CxmlResult.cs" company="Secretariat of the Pacific Community">
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
    using System.Web.Mvc;
    using System.Xml;
    
    /// <summary>
    /// ActionResult child class for returning CXML for use in
    /// Silverlight PivotViewer control
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CxmlResult<T> : ActionResult
    {
        // http://www.dotnetcurry.com/ShowArticle.aspx?ID=682
        // This is more appropriate for general XML serialization
        // Need to think harder about how to create CXML
        // Other resources:
        // https://github.com/smarx/NetflixPivot
        // http://blog.smarx.com/posts/pivot-odata-and-windows-azure-visual-netflix-browsing
        // http://www.silverlight.net/learn/data-networking/pivot-viewer/pivotviewer-control
        public T Data { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Buffer = true;
            response.Clear();
            response.ContentType = "application/xml";

            // 
        }
    }
}