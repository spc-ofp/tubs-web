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

    public class MvcApplication : System.Web.HttpApplication
    {
        // Use the following string keys for stashing objects in the current request
        public const string ISessionKey = "current.session";
        public const string IStatelessSessionKey = "current.stateless-session";
        public const string ITransactionKey = "current.transaction";
        public const string IsEnrolledInTransactionKey = "current.transaction-enrolled";

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
                if (!HttpContext.Current.Items.Contains(ISessionKey))
                {
                    CurrentSession = TubsDataService.GetSession();
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

        public static ISession CurrentSession
        {
            get { return (ISession)HttpContext.Current.Items[ISessionKey]; }
            set { HttpContext.Current.Items[ISessionKey] = value; }
        }

        public static IStatelessSession CurrentStatelessSession
        {
            get { return (IStatelessSession)HttpContext.Current.Items[IStatelessSessionKey]; }
            set { HttpContext.Current.Items[IStatelessSessionKey] = value; }
        }

        public static ITransaction CurrentTransaction
        {
            get { return (ITransaction)HttpContext.Current.Items[ITransactionKey]; }
            set { HttpContext.Current.Items[ITransactionKey] = value; }
        }

        public static bool IsEnrolledInTransaction
        {
            get { return (bool)HttpContext.Current.Items[IsEnrolledInTransactionKey];  }
            set { HttpContext.Current.Items[IsEnrolledInTransactionKey] = value; }
        }
        
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //An authorize attribute filter takes the place of allow roles in web.config
            //filters.Add(new AuthorizeAttribute { Roles = @"NOUMEA\OFP Users" });
            // The following filter is only for non-AD authentication
            filters.Add(new LogonAuthorize());
            filters.Add(new ElmahHandledErrorLoggerFilter());
            filters.Add(new HandleErrorAttribute());
        }

        protected void Application_Start()
        {
            // Hard to get Log4net working if you don't call this -- D'Oh!
            log4net.Config.XmlConfigurator.Configure();

            AreaRegistration.RegisterAllAreas();
            // Trip is the only model worth binding.
            ModelBinderProviders.BinderProviders.Add(new TripModelBinderProvider());
            // Requires WebApi package(s)
            // Install-Package Microsoft.AspNet.WebApi.WebHost
            var f = GlobalConfiguration.Configuration.Formatters.OfType<JsonMediaTypeFormatter>().First();
            f.SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.UseDataContractJsonSerializer = false;
            json.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;

            RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // TODO
            //WebApiConfig.Register(GlobalConfiguration.Configuration);

            // AutoMapper is used to convert entities to viewmodels (and vice versa)
            MappingConfig.Configure();
            
        }
    }
}