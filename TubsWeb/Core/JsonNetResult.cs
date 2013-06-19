// -----------------------------------------------------------------------
// <copyright file="JsonNetResult.cs" company="Secretariat of the Pacific Community">
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
        /// <summary>
        /// log4net logger.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(JsonNetResult));
        
        /// <summary>
        /// Constructor.
        /// </summary>
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

            /*
            // This appears to operate differently between IIS and Cassini, so
            // I'm hard-coding to an appropriate ContentType
            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            */
            response.ContentType = "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            // This might be better than messing around with web.config
            // http://www.west-wind.com/weblog/posts/2009/Apr/29/IIS-7-Error-Pages-taking-over-500-Errors
            // http://msdn.microsoft.com/en-us/library/system.web.httpresponse.tryskipiiscustomerrors(v=vs.100).aspx
            response.TrySkipIisCustomErrors = true;

            Logger.WarnFormat("Data is null? {0}", null == Data);
            if (Data != null)
            {
                Logger.WarnFormat("Data.GetType(): {0}", Data.GetType());
                var serialized = JsonConvert.SerializeObject(Data);
                Logger.WarnFormat("JSON error text:\n{0}", serialized);
                response.Write(serialized);
                response.Flush();
            }
        }
    }
}