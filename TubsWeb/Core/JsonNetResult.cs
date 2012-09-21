// -----------------------------------------------------------------------
// <copyright file="JsonNetResult.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    
    /// <summary>
    /// Replacement JsonResult using the Newtonsoft JSON library.
    /// This solves problems with the default serializer and it's use
    /// of WCF dates.  Unfortunately, the MVC model binder doesn't know
    /// how to handle these dates coming back...
    /// 
    /// Implementation from here:
    /// http://stackoverflow.com/questions/6883204/change-default-json-serializer-used-in-asp-mvc3
    /// </summary>
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            // Might flip this to AllowGet, since I'm using it for API type behavior
            JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("JSON Get Request not allowed");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                response.Write(JsonConvert.SerializeObject(Data));
            }
        }
    }
}