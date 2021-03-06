﻿// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    using System;

    /// <summary>
    /// Extension methods for working with String objects.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Trip the Active Directory domain from a username.
        /// It only works with the classic format of
        /// DOMAIN\username and not
        /// username@DOMAIN
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static string NullSafeToUpper(this string stringValue)
        {
            return null == stringValue ? null : stringValue.ToUpper();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static string AsSpcLatitude(this string stringValue)
        {
            if (String.IsNullOrEmpty(stringValue))
            {
                return stringValue;
            }

            // Assume that if a period (full stop) is present,
            // value has already been correctly formatted
            // Might want to change this assumption in the future
            if (stringValue.IndexOf('.') > -1)
                return stringValue;

            return stringValue.Substring(0, 4) + "." + stringValue.Substring(4);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static string AsSpcLongitude(this string stringValue)
        {
            if (String.IsNullOrEmpty(stringValue))
            {
                return stringValue;
            }

            // Assume that if a period (full stop) is present,
            // value has already been correctly formatted
            // Might want to change this assumption in the future
            if (stringValue.IndexOf('.') > -1)
                return stringValue;

            return stringValue.Substring(0, 5) + "." + stringValue.Substring(5);

        }
    }
}