// -----------------------------------------------------------------------
// <copyright file="DeviceResolver.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.ViewModels.Resolvers
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
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL.Common;
    using Tubs = Spc.Ofp.Tubs.DAL.Common;

    /// <summary>
    /// AutoMapper resolver for converting a string into an enumeration using the
    /// enumeration's description attribute.
    /// </summary>
    public sealed class DeviceTypeResolver : ValueResolver<string, Tubs.ElectronicDeviceType>
    {
        protected override ElectronicDeviceType ResolveCore(string source)
        {
            var deviceType = Tubs.ElectronicDeviceType.Other;
            try
            {
                deviceType = MappingExtensions.FromDescription<Tubs.ElectronicDeviceType>(source);
            }
            catch (ArgumentException ae)
            { /* NOPMD: Swallow this type of exception */}
            return deviceType;
        }
    }

    /// <summary>
    /// AutoMapper resolver for converting an enumeration value into a string using
    /// the enumeration's description attribute.
    /// </summary>
    public sealed class DeviceNameResolver : ValueResolver<Tubs.ElectronicDeviceType, string>
    {
        protected override string ResolveCore(ElectronicDeviceType source)
        {
            return source.Description();
        }
    }
}
