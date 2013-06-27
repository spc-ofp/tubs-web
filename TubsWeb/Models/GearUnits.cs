// -----------------------------------------------------------------------
// <copyright file="GearUnits.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Models
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
    using System.Collections.Generic;
    using Spc.Ofp.Tubs.DAL.Common;
    
    /// <summary>
    /// GearUnits holds the units of measure for fishing gear.
    /// </summary>
    public static class GearUnits
    {
        /// <summary>
        /// Unit 1 (from MS Access implementation)
        /// </summary>
        public static IList<UnitOfMeasure> UnitOne = new List<UnitOfMeasure>()
        {
            UnitOfMeasure.Meters,
            UnitOfMeasure.Fathoms,
            UnitOfMeasure.Yards
        };

        /// <summary>
        /// Unit 2 (from MS Access implementation)
        /// </summary>
        public static IList<UnitOfMeasure> UnitTwo = new List<UnitOfMeasure>()
        {
            UnitOfMeasure.Centimeters,
            UnitOfMeasure.Inches
        };

        /// <summary>
        /// Unit 4 (from MS Access implementation)
        /// </summary>
        public static IList<UnitOfMeasure> UnitFour = new List<UnitOfMeasure>()
        {
            UnitOfMeasure.Meters,
            UnitOfMeasure.Kilometers,
            UnitOfMeasure.NauticalMiles
        };

        /// <summary>
        /// UnitFourLarge is a subset of all 'Unit 4' units of measure (still not sure what 'Unit 4'
        /// really is though).
        /// </summary>
        public static IList<UnitOfMeasure> UnitFourLarge = new List<UnitOfMeasure>()
        {
            UnitOfMeasure.Kilometers,
            UnitOfMeasure.NauticalMiles
        };
    }
}