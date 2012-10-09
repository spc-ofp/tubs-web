// -----------------------------------------------------------------------
// <copyright file="Gen5Profile.cs" company="Secretariat of the Pacific Community">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using DAL = Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Gen5Profile : Profile
    {
        
        protected override void Configure()
        {
            base.Configure();
            // TODO: ViewModel to Entity

            // .ForMember(dest => dest.Tags, opt => opt.MapFrom(so => so.Tags.Select(t=>t.Name).ToList()));
            
            // Entity to ViewModel
            CreateMap<DAL.Entities.Gen5Object, Gen5ViewModel>()
                .ForMember(d => d.OriginCodes, o => o.Ignore()) // UX, not database
                .ForMember(d => d.DescriptionCodes, o => o.Ignore()) // UX, not database
                .ForMember(d => d.MaterialCodes, o => o.Ignore()) // UX, not database
                .ForMember(d => d.AttachmentCodes, o => o.Ignore()) // UX, not database
                .ForMember(d => d.ActivityId, o => o.MapFrom(s => s.Activity.Id))
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Activity.Day.Trip.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.Activity.Day.Trip.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Activity.Day.Trip.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.SsiTrapped, o => o.MapFrom(s => s.IsSsiTrapped)) // TODO Don't use bool? in ViewModel
                .ForMember(d => d.OriginCode, o => o.ResolveUsing<OriginCodeResolver>().FromMember(s => s.Origin))
                .ForMember(d => d.AsFoundCode, o => o.ResolveUsing<ObjectCodeResolver>().FromMember(s => s.AsFound))
                .ForMember(d => d.AsLeftCode, o => o.ResolveUsing<ObjectCodeResolver>().FromMember(s => s.AsLeft))
                .ForMember(d => d.MainMaterials, o => o.ResolveUsing<MaterialListResolver>().FromMember(s => s.MainMaterials))
                .ForMember(d => d.Attachments, o => o.ResolveUsing<MaterialListResolver>().FromMember(s => s.Attachments))
                ;
        }
    }
}
