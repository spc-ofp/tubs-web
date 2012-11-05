// -----------------------------------------------------------------------
// <copyright file="SightingProfile.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Mapping.Profiles
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
    using AutoMapper;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    using DAL = Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SightingProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            Mapper.CreateMap<DAL.Entities.Sighting, SightingViewModel.Sighting>()
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())
                .ForMember(d => d.ActivityIconPath, o => o.Ignore())
                .ForMember(d => d.Name, o => o.MapFrom(s => s.VesselName))
                .ForMember(d => d.CountryCode, o => o.MapFrom(s => s.VesselFlag))
                .ForMember(d => d.DateOnly, o => o.MapFrom(s => s.EventDateOnly))
                .ForMember(d => d.TimeOnly, o => o.MapFrom(s => s.EventTimeOnly))
                .ForMember(d => d.PhotoFrame, o => o.MapFrom(s => s.PhotoNumber))
                .ForMember(d => d.TypeCode, o => o.ResolveUsing<VesselTypeResolver>().FromMember(s => s.VesselType))
                .ForMember(d => d.ActionCode, o => o.ResolveUsing<ActionTypeResolver>().FromMember(s => s.ActionType))
                .ForMember(d => d.TypeDescription, o => o.MapFrom(s => s.VesselType.HasValue ? s.VesselType.GetDescription() : "N/A"))
                .ForMember(d => d.ActionDescription, o => o.MapFrom(s => s.ActionType.HasValue ? s.ActionType.GetDescription() : "N/A"))
                .ForMember(d => d.VesselId, o => o.Ignore())
                .AfterMap((s,d) => 
                {
                    // If there's an associated Vessel entity, use that for name, IRCS, etc.
                    if (null != s.Vessel)
                    {
                        d.VesselId = s.Vessel.Id;
                        d.Name = s.Vessel.Name ?? d.Name;
                        d.Ircs = s.Vessel.Ircs ?? d.Ircs;
                        d.CountryCode = s.Vessel.RegisteredCountryCode; // Required                        
                    }
                })
                ;

            Mapper.CreateMap<DAL.Entities.Trip, SightingViewModel>()
                .ForMember(d => d.TypeCodes, o => o.Ignore())
                .ForMember(d => d.ActionCodes, o => o.Ignore())
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                ;

        }
    }
}
