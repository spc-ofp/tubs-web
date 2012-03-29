// -----------------------------------------------------------------------
// <copyright file="CxmlResult.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    using System;
    using System.Web.Mvc;
    using System.Xml;
    
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