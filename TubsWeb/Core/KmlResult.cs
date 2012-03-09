// -----------------------------------------------------------------------
// <copyright file="KmlResult.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
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
        public const string KML_MIME_TYPE = "application/vnd.google-earth.kml+xml";
        public const string KMZ_MIME_TYPE = "application/vnd.google-earth.kmz";
        
        public Document Document { get; set; }
        public bool IsCompressed { get; set; }

        public KmlResult() { }

        public KmlResult(Document doc)
        {
            this.Document = doc;
        }

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