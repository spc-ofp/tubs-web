// -----------------------------------------------------------------------
// <copyright file="EditorAuthorizeAttribute.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    using System;
    using System.Web.Configuration;
    using System.Web.Mvc;

    /// <summary>
    /// Configurable editor group authorization attribute.
    /// Based on implementation here:
    /// http://weblogs.asp.net/srkirkland/archive/2010/01/04/authorizing-access-via-attributes-in-asp-net-mvc-without-magic-strings.aspx
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EditorAuthorizeAttribute : AuthorizeAttribute
    {
        public EditorAuthorizeAttribute()
        {
            string groups = WebConfigurationManager.AppSettings["EditorGroups"];
            if (string.IsNullOrEmpty(groups))
            {
                groups = @"NT AUTHORITY\Authenticated Users";
            }
            Roles = groups;
        }
    }
}