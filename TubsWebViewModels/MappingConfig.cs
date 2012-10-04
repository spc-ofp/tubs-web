// -----------------------------------------------------------------------
// <copyright file="MappingConfig.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb
{
    using AutoMapper;
    using TubsWeb.Mapping.Profiles;

    /// <summary>
    /// MappingConfig performs the one-time AutoMapper configuration
    /// at application start.
    /// </summary>
    public class MappingConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<SetProfile>();
                // Trim all strings.  If the trimmed string is empty, return a null
                Mapper.CreateMap<string, string>().ConvertUsing(s =>
                {
                    if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
                        return null;
                    return s.Trim();
                });
            });
        }
    }
}