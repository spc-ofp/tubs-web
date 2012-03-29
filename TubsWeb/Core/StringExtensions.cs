// -----------------------------------------------------------------------
// <copyright file="TubsExtensions.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    using System;

    public static class StringExtensions
    {
        public static string WithoutDomain(this string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                return username;
            }

            if (!username.Contains(@"\"))
            {
                return username;
            }
            int slash = username.IndexOf(@"\");
            return username.Substring(slash + 1);
        }
    }
}