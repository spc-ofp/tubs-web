﻿// -----------------------------------------------------------------------
// <copyright file="TripModelBinderProvider.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL.Entities;
    
    /// <summary>
    /// ModelBinderProvider for converting trip primary key value
    /// in URL into model passed in to actions.
    /// </summary>
    public class TripModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Get the appropriate binder
        /// </summary>
        /// <param name="modelType">Type the action is requesting</param>
        /// <returns>TripModelBinder if modelType is a Trip, null otherwise.</returns>
        public IModelBinder GetBinder(Type modelType)
        {
            return typeof(Trip).IsAssignableFrom(modelType) ?
                new TripModelBinder() :
                null;
        }
    }
}