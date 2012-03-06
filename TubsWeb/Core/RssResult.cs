// -----------------------------------------------------------------------
// <copyright file="RssResult.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
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
        public SyndicationFeed Feed { get; set; }

        public RssResult() { }

        public RssResult(SyndicationFeed feed)
        {
            this.Feed = feed;
        }

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