// -----------------------------------------------------------------------
// <copyright file="TransferProfile.cs" company="Secretariat of the Pacific Community">
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
    using Spc.Ofp.Tubs.DAL.Common;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    using DAL = Spc.Ofp.Tubs.DAL;

    /// <summary>
    /// AutoMapper profile for the conversion of GEN-1 transfer entity to/from
    /// MVC ViewModel..
    /// </summary>
    public class TransferProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            // ViewModel to Entity
            Mapper.CreateMap<TransferViewModel.Transfer, DAL.Entities.Transfer>()
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.Trip, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.EezCode, o => o.Ignore())
                .ForMember(d => d.Vessel, o => o.Ignore()) // TODO Figure out how this works
                .ForMember(d => d.TransferDateOnly, o => o.MapFrom(s => s.DateOnly))
                .ForMember(d => d.TransferTimeOnly, o => o.MapFrom(s => s.TimeOnly))
                .ForMember(d => d.TransferDate, o => o.MapFrom(s => s.DateOnly.Merge(s.TimeOnly)))
                .ForMember(d => d.VesselName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.VesselFlag, o => o.MapFrom(s => s.CountryCode))
                .ForMember(d => d.VesselType, o => o.ResolveUsing<VesselTypeCodeResolver>().FromMember(s => s.TypeCode))
                .ForMember(d => d.TonsOfMixed, o => o.MapFrom(s => s.Mixed))
                .ForMember(d => d.TonsOfSkipjack, o => o.MapFrom(s => s.Skipjack))
                .ForMember(d => d.TonsOfBigeye, o => o.MapFrom(s => s.Bigeye))
                .ForMember(d => d.TonsOfYellowfin, o => o.MapFrom(s => s.Yellowfin))
                .ForMember(d => d.ActionType, o => o.ResolveUsing<ActionCodeResolver>().FromMember(s => s.ActionCode))
                ;

            // Entity to ViewModel
            Mapper.CreateMap<DAL.Entities.Transfer, TransferViewModel.Transfer>()
                .ForMember(d => d._destroy, o => o.Ignore())
                .ForMember(d => d.NeedsFocus, o => o.Ignore())
                .ForMember(d => d.ActivityIconPath, o => o.Ignore())
                .ForMember(d => d.Name, o => o.MapFrom(s => s.VesselName))
                .ForMember(d => d.CountryCode, o => o.MapFrom(s => s.VesselFlag))
                .ForMember(d => d.DateOnly, o => o.MapFrom(s => s.TransferDateOnly))
                .ForMember(d => d.TimeOnly, o => o.MapFrom(s => s.TransferTimeOnly))
                .ForMember(d => d.TypeCode, o => o.ResolveUsing<TransferVesselResolver>().FromMember(s => s.VesselType))
                .ForMember(d => d.ActionCode, o => o.ResolveUsing<ActionTypeResolver>().FromMember(s => s.ActionType))
                .ForMember(d => d.TypeDescription, o => o.MapFrom(s => s.VesselType.HasValue ? s.VesselType.GetDescription() : "N/A"))
                .ForMember(d => d.ActionDescription, o => o.MapFrom(s => s.ActionType.HasValue ? s.ActionType.GetDescription() : "N/A"))
                .ForMember(d => d.Skipjack, o => o.MapFrom(s => s.TonsOfSkipjack))
                .ForMember(d => d.Yellowfin, o => o.MapFrom(s => s.TonsOfYellowfin))
                .ForMember(d => d.Mixed, o => o.MapFrom(s => s.TonsOfMixed))
                .ForMember(d => d.Bigeye, o => o.MapFrom(s => s.TonsOfBigeye))
                .ForMember(d => d.VesselId, o => o.Ignore())
                .AfterMap((s, d) =>
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

            Mapper.CreateMap<DAL.Entities.Trip, TransferViewModel>()
                .ForMember(d => d.TypeCodes, o => o.Ignore())
                .ForMember(d => d.ActionCodes, o => o.Ignore())
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                ;
        }
    }
}
