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
                cfg.AddProfile<SeaDayProfile>();
                cfg.AddProfile<SetProfile>();
                cfg.AddProfile<LongLineSetProfile>();
                cfg.AddProfile<SafetyInspectionProfile>();
                cfg.AddProfile<TripInfoProfile>();
                cfg.AddProfile<Ps1Profile>();
                cfg.AddProfile<Ps1ViewModelProfile>();
                cfg.AddProfile<SightingProfile>();
                cfg.AddProfile<TransferProfile>();
                cfg.AddProfile<CrewProfile>();
                cfg.AddProfile<ElectronicsProfile>();
                cfg.AddProfile<LengthFrequencyProfile>();
                cfg.AddProfile<Gen2Profile>();
                cfg.AddProfile<Gen3Profile>();
                cfg.AddProfile<Gen5Profile>();
                cfg.AddProfile<PageCountProfile>();
                cfg.AddProfile<TripSummaryProfile>();
                cfg.AddProfile<TrackProfile>();
                cfg.AddProfile<WellContentProfile>();
                cfg.AddProfile<PositionsProfile>();
                cfg.AddProfile<LongLineSampleProfile>();
                cfg.AddProfile<TripSamplingProfile>(); // Purse Seine sampling
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