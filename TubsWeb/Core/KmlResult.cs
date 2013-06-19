// -----------------------------------------------------------------------
// <copyright file="KmlResult.cs" company="Secretariat of the Pacific Community">
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
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Google.Kml;

    /// <summary>
    /// MVC ActionResult for displaying Kml data.
    /// </summary>
    public class KmlResult : ActionResult
    {
        /// <summary>
        /// MIME type for standard KML file.
        /// </summary>
        public const string KML_MIME_TYPE = "application/vnd.google-earth.kml+xml";
        /// <summary>
        /// MIME type for compressed KML file.
        /// </summary>
        public const string KMZ_MIME_TYPE = "application/vnd.google-earth.kmz";
        
        /// <summary>
        /// KML document
        /// </summary>
        public Document Document { get; set; }

        /// <summary>
        /// Flag indicating if KML should be compressed.
        /// </summary>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Construct instance of KmlResult
        /// </summary>
        public KmlResult() { }

        /// <summary>
        /// Construct instance of KmlResult accepting an existing document
        /// </summary>
        /// <param name="doc">Existing KML document</param>
        public KmlResult(Document doc)
        {
            this.Document = doc;
        }

        /// <summary>
        /// Write KML document as binary data directly into response OutputStream.
        /// </summary>
        /// <param name="context">ControllerContext in which execute operates</param>
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.HttpContext.Response.ContentType =
                IsCompressed ?
                    KMZ_MIME_TYPE :
                    KML_MIME_TYPE;

            Kml kml = new Kml(Document);

            using (var stream =
                IsCompressed ?
                    new MemoryStream(kml.ToKmz()) :
                    new MemoryStream(Encoding.UTF8.GetBytes(kml.ToString())))
            {
                stream.WriteTo(context.HttpContext.Response.OutputStream);
            }
        }
    }
}