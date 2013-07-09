// -----------------------------------------------------------------------
// <copyright file="Ps1Profile.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// AutoMapper profile for conversion of data access layer
    /// entities to ViewModel.  Reverse of these mappings found
    /// in Ps1ViewModelProfile
    /// </summary>
    public class Ps1Profile : Profile
    {
        protected override void Configure()
        {
            base.Configure();
            // ViewModel to Entity handled in Ps1ViewModelProfile
            
            // Entity to ViewModel
            CreateMap<DAL.Entities.PurseSeineGear, Ps1ViewModel.FishingGear>()
                .ForMember(d => d.NetStripCount, o => o.MapFrom(s => s.NetStrips))
                .ForMember(d => d.Brail1Capacity, o => o.MapFrom(s => s.Brail1Size))
                .ForMember(d => d.Brail2Capacity, o => o.MapFrom(s => s.Brail2Size))
                .ForMember(d => d.NetMeshSize, o => o.MapFrom(s => s.MeshSize))
                .ForMember(d => d.NetDepthUnits, o => o.ResolveUsing<UnitOfMeasureResolver>().FromMember(s => s.NetDepthUnit))
                .ForMember(d => d.NetMeshUnits, o => o.ResolveUsing<UnitOfMeasureResolver>().FromMember(s => s.MeshSizeUnits))
                ;

            CreateMap<DAL.Entities.PurseSeineVesselAttributes, Ps1ViewModel.VesselCharacteristics>()
                .ForMember(d => d.Owner, o => o.Ignore()) // TODO:  Add to DAL?
                .ForMember(d => d.RegistrationNumber, o => o.Ignore()) // AfterMap
                .ForMember(d => d.CountryCode, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Ircs, o => o.Ignore()) // AfterMap
                .ForMember(d => d.Length, o => o.Ignore()) // AfterMap
                .ForMember(d => d.LengthUnits, o => o.Ignore()) // AfterMap
                .ForMember(d => d.GrossTonnage, o => o.Ignore()) // AfterMap
                .ForMember(d => d.TenderBoatAnswer, o => o.MapFrom(s => s.HasTenderBoats))
                .ForMember(d => d.HelicopterRegistration, o => o.MapFrom(s => s.HelicopterRegistrationNumber))
                .ForMember(d => d.HelicopterRangeUnits, o => o.ResolveUsing<UnitOfMeasureResolver>().FromMember(s => s.HelicopterRangeUnit))
                .AfterMap((s,d) => {
                    if (null != s && null != s.Trip && null != s.Trip.Vessel)
                    {
                        var vessel = s.Trip.Vessel;
                        d.RegistrationNumber = vessel.RegistrationNumber;
                        d.CountryCode = vessel.RegisteredCountryCode;
                        d.Ircs = vessel.Ircs;
                        d.Length = vessel.Length;
                        d.GrossTonnage = vessel.GrossTonnage;
                    }                    
                })
                ;

            CreateMap<DAL.Entities.PurseSeineTrip, Ps1ViewModel>()
                .ForMember(d => d.AvailabilityValues, o => o.Ignore())
                .ForMember(d => d.BooleanValues, o => o.Ignore())
                .ForMember(d => d.NetUnits, o => o.Ignore())
                .ForMember(d => d.MeshUnits, o => o.Ignore())
                .ForMember(d => d.RangeUnits, o => o.Ignore())
                .ForMember(d => d.PermitNumbers, o => o.MapFrom(s => s.VesselNotes != null ? s.VesselNotes.Licenses : null))
                .ForMember(d => d.Page2Comments, o => o.Ignore()) // TODO:  Where?
                .ForMember(d => d.TripId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TripNumber, o => o.MapFrom(s => (s.SpcTripNumber ?? "This Trip").Trim()))
                .ForMember(d => d.VersionNumber, o => o.MapFrom(s => s.Version == DAL.Common.WorkbookVersion.v2009 ? 2009 : 2007))
                .ForMember(d => d.Page1Comments, o => o.MapFrom(s => s.VesselNotes != null ? s.VesselNotes.Comments : null))
                .ForMember(d => d.Characteristics, o => o.MapFrom(s => s.VesselAttributes))
                .AfterMap((s,d) => 
                {
                    if (null == d.Inspection)
                        d.Inspection = new SafetyInspectionViewModel();
                    if (null == d.Gear)
                        d.Gear = new Ps1ViewModel.FishingGear();
                    if (null == d.Characteristics)
                        d.Characteristics = new Ps1ViewModel.VesselCharacteristics();
                })
                ;
        }
    }
}
