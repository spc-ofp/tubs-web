// -----------------------------------------------------------------------
// <copyright file="RssResult.cs" company="Secretariat of the Pacific Community">
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
    using System.ServiceModel.Syndication;
    using System.Web.Mvc;
    using System.Xml;
    
    /// <summary>
    /// MVC ActionResult for displaying RSS data.
    /// Implementation from here:
    /// http://blogs.msdn.com/b/jowardel/archive/2009/03/11/asp-net-rss-actionresult.aspx
    /// </summary>
    public class RssResult : ActionResult
    {
        /// <summary>
        /// RSS data as SyndicationFeed
        /// </summary>
        public SyndicationFeed Feed { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RssResult() { }

        /// <summary>
        /// Constructor accepting existing SyndicationFeed
        /// </summary>
        /// <param name="feed">SyndicationFeed to return</param>
        public RssResult(SyndicationFeed feed)
        {
            this.Feed = feed;
        }

        /// <summary>
        /// Write SyndicationFeed to response OutputStream.
        /// </summary>
        /// <param name="context">Context in which action executes</param>
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";

            Rss20FeedFormatter formatter = new Rss20FeedFormatter(this.Feed);

            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                formatter.WriteTo(writer);
            }
        }
    }
}