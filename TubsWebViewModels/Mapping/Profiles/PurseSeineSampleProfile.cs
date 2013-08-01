// -----------------------------------------------------------------------
// <copyright file="PurseSeineSampleProfile.cs"  company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
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
    using System.Linq;
    using AutoMapper;
    using TubsWeb.ViewModels;
    using TubsWeb.ViewModels.Resolvers;
    using DAL = Spc.Ofp.Tubs.DAL;

    public sealed class PurseSeineSampleProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();
            // ViewModel to Entity
            CreateMap<PurseSeineSampleViewModel, DAL.Entities.LengthSample>()
                // Ignore entity relationships
                .ForMember(d => d.Header, o => o.Ignore())
                // Standard ignores
                .ForMember(d => d.DctNotes, o => o.Ignore())
                .ForMember(d => d.DctScore, o => o.Ignore())
                .ForMember(d => d.EnteredBy, o => o.Ignore())
                .ForMember(d => d.EnteredDate, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedDate, o => o.Ignore())
                .ForMember(d => d.SequenceNumber, o => o.MapFrom(s => s.Number))
                ;

            // Entity to ViewModel
            CreateMap<DAL.Entities.LengthSample, PurseSeineSampleViewModel>()
                .ForMember(d => d.Number, o => o.MapFrom(s => s.SequenceNumber))
                ;
        }
    }
}
