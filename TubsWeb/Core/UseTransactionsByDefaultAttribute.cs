// -----------------------------------------------------------------------
// <copyright file="UseTransactionsByDefaultAttribute.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    using System;
    using System.Web.Mvc;
    using NHibernate;

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

    /// <summary>
    /// Much of the ideas behind this implementation come from Scott Kirkland
    /// However, his implementation is MVC2 only due to a breaking change in MVC3
    /// http://weblogs.asp.net/srkirkland/archive/2009/11/16/making-asp-net-mvc-actions-be-transactional-by-default.aspx
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UseTransactionsByDefaultAttribute : ActionFilterAttribute
    {
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(UseTransactionsByDefaultAttribute));
        
        private static bool ShouldDelegateTransactionSupport(ActionExecutingContext filterContext)
        {
            var attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(TransactionalActionBaseAttribute), false);

            return attrs.Length > 0;
        }

        // TODO It should be possible to accomplish this by implementing IActionFilter and IResultFilter
        // Method implementation is the same, but filters could be globally hooked so that users don't need
        // to extend SuperController

        // This is part of IActionFilter
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool shouldDelegate = ShouldDelegateTransactionSupport(filterContext);
            Logger.DebugFormat("shouldDelegate? {0}", shouldDelegate);
            MvcApplication.IsEnrolledInTransaction = !shouldDelegate;
            // This also needs to get stuffed into HttpContext.Current

            if (!shouldDelegate)
            {
                Logger.Debug("Creating transaction...");
                ISession session = MvcApplication.CurrentSession;
                MvcApplication.CurrentTransaction = session.BeginTransaction();
            }

            Logger.Debug("Passing execution to base");
            base.OnActionExecuting(filterContext);
        }

        // This is part of IResultFilter
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            Logger.DebugFormat("IsEnrolledInTransaction? {0}", MvcApplication.IsEnrolledInTransaction);

            if (MvcApplication.IsEnrolledInTransaction)
            {
                // Adding new trip headers is sketchy in prod -- try this to find root cause               
                ITransaction transaction = null;
                try
                {
                    transaction = MvcApplication.CurrentTransaction;
                    Logger.DebugFormat("CurrentTransaction is null? {0}", null == MvcApplication.CurrentTransaction);
                    Logger.DebugFormat("filterContext.Exception is not null? {0}", null != filterContext.Exception);
                    Logger.DebugFormat("filterContext.ExceptionHandled? {0}", filterContext.ExceptionHandled);
                    if ((filterContext.Exception != null) && (!filterContext.ExceptionHandled))
                    {
                        Logger.Debug("Exception causing rollback", filterContext.Exception);
                        transaction.Rollback();
                    }
                    else
                    {
                        Logger.Debug("Committing transaction...");
                        transaction.Commit();
                        Logger.Debug("Transaction committed");
                    }

                }
                catch (Exception ex)
                {
                    ex.Data.Add("IsEnrolledInTransaction", MvcApplication.IsEnrolledInTransaction);
                    if (null != filterContext.Exception)
                    {
                        ex.Data.Add("FilterException", filterContext.Exception);
                        ex.Data.Add("FilterExceptionHandled", filterContext.ExceptionHandled);
                    }
                    Logger.Error("Error finalizing transaction", ex);
                    throw;
                }
                finally
                {
                    if (null != transaction)
                    {
                        transaction.Dispose();
                    }

                }
            }
          
        }
    }
}