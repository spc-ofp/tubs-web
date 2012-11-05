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

            // ViewModel to Entity
            CreateMap<Gen5ViewModel, DAL.Entities.Gen5Object>()
                .ForMember(d => d.Activity, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.EnteredBy, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.EnteredDate, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.UpdatedBy, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.UpdatedDate, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.DctNotes, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.DctScore, o => o.Ignore()) // Caller's responsibility
                .ForMember(d => d.IsSsiTrapped, o => o.ResolveUsing<YesNoResolver>().FromMember(s => s.SsiTrapped))
                .ForMember(d => d.Origin, o => o.ResolveUsing<FadOriginResolver>().FromMember(s => s.OriginCode))
                .ForMember(d => d.AsFound, o => o.ResolveUsing<FadTypeResolver>().FromMember(s => s.AsFoundCode))
                .ForMember(d => d.AsLeft, o => o.ResolveUsing<FadTypeResolver>().FromMember(s => s.AsLeftCode))
                .ForMember(d => d.Materials, o => o.Ignore()) // AfterMap
                .AfterMap((s,d) => 
                {
                    if (s.MainMaterials != null)
                    {
                        s.MainMaterials.Where(x => x != null && !x._destroy).ToList().ForEach(x =>
                        {
                            var material = FadMaterialResolver.MaterialLookup(x.MaterialCode);
                            if (material.HasValue)
                            {
                                d.Materials.Add(new DAL.Entities.Gen5Material
                                {
                                    Fad = d,
                                    IsAttachment = false,
                                    Id = x.Id,
                                    Material = material.Value
                                });
                            }
                        });
                    }
                    if (s.Attachments != null)
                    {
                        s.Attachments.Where(x => x != null && !x._destroy).ToList().ForEach(x =>
                        {
                            var material = FadMaterialResolver.MaterialLookup(x.MaterialCode);
                            if (material.HasValue)
                            {
                                d.Materials.Add(new DAL.Entities.Gen5Material
                                {
                                    Fad = d,
                                    IsAttachment = true,
                                    Id = x.Id,
                                    Material = material.Value
                                });
                            }
                        });
                    }
                })
                ;

            // Entity to ViewModel
            CreateMap<DAL.Entities.Gen5Material, Gen5ViewModel.FadMaterial>()
                .ForMember(d => d._destroy, o => o.Ignore()) // UX, not database
                .ForMember(d => d.NeedsFocus, o => o.Ignore()) // UX, not database
                .ForMember(d => d.MaterialCode, o => o.ResolveUsing<MaterialCodeResolver>().FromMember(s => s.Material))
                ;
            
            
            CreateMap<DAL.Entities.Gen5Object, Gen5ViewModel>()
                .ForMember(d => d.OriginCodes, o => o.Ignore()) // UX, not database
                .ForMember(d => d.DescriptionCodes, o => o.Ignore()) // UX, not database
                .ForMember(d => d.MaterialCodes, o => o.Ignore()) // UX, not database
                .ForMember(d => d.BooleanValues, o => o.Ignore()) // UX, not database
                .ForMember(d => d.ActivityId, o => o.MapFrom(s => s.Activity.Id))
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Activity.Day.Trip.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.Activity.Day.Trip.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Activity.Day.Trip.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.SsiTrapped, o => o.ResolveUsing<BooleanResolver>().FromMember(s => s.IsSsiTrapped))
                .ForMember(d => d.OriginCode, o => o.ResolveUsing<OriginCodeResolver>().FromMember(s => s.Origin))
                .ForMember(d => d.Origin, o => o.ResolveUsing<EnumDescriptionResolver>().FromMember(s => s.Origin))
                .ForMember(d => d.AsFoundCode, o => o.ResolveUsing<ObjectCodeResolver>().FromMember(s => s.AsFound))
                .ForMember(d => d.AsFound, o => o.ResolveUsing<EnumDescriptionResolver>().FromMember(s => s.AsFound))
                .ForMember(d => d.AsLeftCode, o => o.ResolveUsing<ObjectCodeResolver>().FromMember(s => s.AsLeft))
                .ForMember(d => d.AsLeft, o => o.ResolveUsing<EnumDescriptionResolver>().FromMember(s => s.AsLeft))
                .ForMember(d => d.MainMaterials, o => o.MapFrom(s => s.Materials.Where(m => m != null && !m.IsAttachment)))
                .ForMember(d => d.Attachments, o => o.MapFrom(s => s.Materials.Where(m => m != null && m.IsAttachment)))
                .ForMember(d => d.MainMaterialDescriptions, o => o.ResolveUsing<MaterialDescriptionListResolver>().FromMember(s => s.MainMaterials))
                .ForMember(d => d.AttachmentDescriptions, o => o.ResolveUsing<MaterialDescriptionListResolver>().FromMember(s => s.Attachments))
                ;
        }
    }
}
