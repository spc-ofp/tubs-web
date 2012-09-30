// -----------------------------------------------------------------------
// <copyright file="DateUtilities.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    using System;
    
    public static class DateUtilities
    {
        public static Tuple<bool, DateTime> Parse(string dateVal)
        {
            if (string.IsNullOrEmpty(dateVal))
                return Tuple.Create(false, DateTime.MinValue);

            DateTime parsed;
            var result = DateTime.TryParseExact(
                dateVal, 
                "dd/MM/yy", 
                null, 
                System.Globalization.DateTimeStyles.None, 
                out parsed);

            return Tuple.Create(result, parsed);
        }
    }
}