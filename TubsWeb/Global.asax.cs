// -----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb
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
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Newtonsoft.Json.Converters;
    using NHibernate;
    using Spc.Ofp.Tubs.DAL;
    using TubsWeb.Core;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    /// <summary>
    /// Application object.
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {

        /// <summary>
        /// ISessionKey is the string key for finding the current stateful NHibernate session within the HTTP request context.
        /// </summary>
        public const string ISessionKey = "current.session";
        /// <summary>
        /// IStatelessSessionKey is the string key for finding the current stateless NHibernate session within the HTTP request context.
        /// </summary>
        public const string IStatelessSessionKey = "current.stateless-session";
        /// <summary>
        /// ITransactionKey is the string key for finding the current NHibernate transaction within the HTTP request context.
        /// </summary>
        public const string ITransactionKey = "current.transaction";
        /// <summary>
        /// IsEnrolledInTransactionKey is the string key for determining if the current HTTP request is participating in a transaction.
        /// </summary>
        public const string IsEnrolledInTransactionKey = "current.transaction-enrolled";

        /// <summary>
        /// Constructor
        /// </summary>
        public MvcApplication()
        {
            /*
             * This implementation is per Ayende:
             * http://ayende.com/blog/4101/do-you-need-a-framework
             * Essentially, what happens is that each Request gets a new Session on creation,
             * and when the request is finished, the Session is disposed of.  NHibernate Sessions
             * are very lightweight, so there's no significant overhead to this.
             * Updated 03/09/2012:
             * Add a second, stateless session to each request.  There's still only one transaction
             * but we have the option of using stateless or stateful sessions depending on
             * use case.
             */
            BeginRequest += delegate
            {
                // NOTE:  While researching another issue, it was noted that
                // this pattern may result in transactions on the request of static content
                // like CSS and graphics.  If it can be solved in a few minutes, it might be
                // worth looking into preventing a transaction for static content
                if (!HttpContext.Current.Items.Contains(ISessionKey))
                {
                    // If we're running on the Intranet, return a vanilla session
                    CurrentSession = TubsDataService.GetSession();

                    // If we're running on the public Internet, add a session filter
                    // for the parent entity of the current logged in user
                }
                if (!HttpContext.Current.Items.Contains(IStatelessSessionKey))
                {
                    CurrentStatelessSession = TubsDataService.GetStatelessSession();
                }
            };
            EndRequest += delegate
            {
                if (null != CurrentSession)
                {
                    CurrentSession.Dispose();
                    //HttpContext.Current.Items.Remove(ITransactionKey);
                    HttpContext.Current.Items.Remove(ISessionKey);
                }
                if (null != CurrentStatelessSession)
                {
                    CurrentStatelessSession.Dispose();
                    HttpContext.Current.Items.Remove(IStatelessSessionKey);
                }
                HttpContext.Current.Items.Remove(ITransactionKey);
            };
        }

        /// <summary>
        /// Current NHibernate session property
        /// </summary>
        public static ISession CurrentSession
        {
            get { return (ISession)HttpContext.Current.Items[ISessionKey]; }
            set { HttpContext.Current.Items[ISessionKey] = value; }
        }

        /// <summary>
        /// Current NHibernate stateless session property
        /// </summary>
        public static IStatelessSession CurrentStatelessSession
        {
            get { return (IStatelessSession)HttpContext.Current.Items[IStatelessSessionKey]; }
            set { HttpContext.Current.Items[IStatelessSessionKey] = value; }
        }

        /// <summary>
        /// Current NHibernate transaction property
        /// </summary>
        public static ITransaction CurrentTransaction
        {
            get { return (ITransaction)HttpContext.Current.Items[ITransactionKey]; }
            set { HttpContext.Current.Items[ITransactionKey] = value; }
        }

        /// <summary>
        /// Property indicating the current HttpContext is participating in a transaction
        /// </summary>
        public static bool IsEnrolledInTransaction
        {
            get { return (bool)HttpContext.Current.Items[IsEnrolledInTransactionKey];  }
            set { HttpContext.Current.Items[IsEnrolledInTransactionKey] = value; }
        }
        
        /// <summary>
        /// Register global filters
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //An authorize attribute filter takes the place of allow roles in web.config
            //filters.Add(new AuthorizeAttribute { Roles = @"NOUMEA\OFP Users" });
            // The following filter is only for non-AD authentication
            filters.Add(new LogonAuthorize());
            filters.Add(new ElmahHandledErrorLoggerFilter());
            filters.Add(new HandleErrorAttribute());
        }

        /// <summary>
        /// Startup tasks
        /// </summary>
        protected void Application_Start()
        {
            // Turn off classic ASPX
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            
            // Hard to get Log4net working if you don't call this -- D'Oh!
            log4net.Config.XmlConfigurator.Configure();

            AreaRegistration.RegisterAllAreas();
            // Trip is the only model worth binding.
            ModelBinderProviders.BinderProviders.Add(new TripModelBinderProvider());

            var f = GlobalConfiguration.Configuration.Formatters.OfType<JsonMediaTypeFormatter>().First();
            f.SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.UseDataContractJsonSerializer = false;
            json.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;

            RegisterGlobalFilters(GlobalFilters.Filters);
            // WebApiConfig needs to happen before RouteConfig
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);            

            // AutoMapper is used to convert entities to viewmodels (and vice versa)
            MappingConfig.Configure();
            
        }
    }
}