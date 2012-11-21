// -----------------------------------------------------------------------
// <copyright file="PushpinExtensions.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Google.Kml;
    using Spc.Ofp.Tubs.DAL.Entities;
    
    /// <summary>
    /// Extension methods for working with the Pushpin entity.
    /// </summary>
    public static class PushpinExtensions
    {
        public static Track ToTrack(this IList<Pushpin> pins, string formatString = "yyyy-MM-ddTHH:mm:ssK")
        {
            var track = new Track();
            if (null != pins && pins.Count > 0)
            {
                // They should already be sorted, but we're lenient in what we accept
                var sorted = 
                    pins.Where(p => p.Timestamp.HasValue && p.Latitude.HasValue && p.Longitude.HasValue)
                        .OrderBy(p => p.Timestamp);

                track.Timestamps = (
                    from p in sorted
                    let ts = new FormattedDateTime(p.Timestamp.Value, formatString)
                    select new TimeStamp(ts)
                ).ToList();

                track.Coordinates = (
                    from p in sorted
                    select new coord((double)p.Longitude.Value, (double)p.Latitude.Value)
                ).ToList();
                
            }
            return track;
        }
    }
}