// -----------------------------------------------------------------------
// <copyright file="UseTransactionsByDefaultAttribute.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
            MvcApplication.IsEnrolledInTransaction = !shouldDelegate;
            // This also needs to get stuffed into HttpContext.Current

            if (!shouldDelegate)
            {
                ISession session = MvcApplication.CurrentSession;
                MvcApplication.CurrentTransaction = session.BeginTransaction();
            }

            base.OnActionExecuting(filterContext);
        }

        // This is part of IResultFilter
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            if (MvcApplication.IsEnrolledInTransaction)
            {
                // TODO This isn't working in prod...
                ITransaction transaction = null;
                try
                {
                    transaction = MvcApplication.CurrentTransaction;
                    if ((filterContext.Exception != null) && (!filterContext.ExceptionHandled))
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                    }

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