// -----------------------------------------------------------------------
// <copyright file="FadMaterialResolver.cs" company="Secretariat of the Pacific Community">
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
    using System.Collections.Generic;
    using AutoMapper;
    using Tubs = Spc.Ofp.Tubs.DAL.Common;
    using System.Linq;

    /// <summary>
    /// FadMaterialResolver converts numeric form code to
    /// Enum value for data access layer.
    /// </summary>
    public class FadMaterialResolver : ValueResolver<int?, Tubs.FadMaterials?>
    {
        protected override Tubs.FadMaterials? ResolveCore(int? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case 1:
                    return Tubs.FadMaterials.LogsOrTrees;
                case 2:
                    return Tubs.FadMaterials.Timber;
                case 3:
                    return Tubs.FadMaterials.PvcOrPlasticTubing;
                case 4:
                    return Tubs.FadMaterials.PlasticDrums;
                case 5:
                    return Tubs.FadMaterials.PlasticSheeting;
                case 6:
                    return Tubs.FadMaterials.MetalDrums;
                case 7:
                    return Tubs.FadMaterials.Philippines;
                case 8:
                    return Tubs.FadMaterials.BambooOrCane;
                case 9:
                    return Tubs.FadMaterials.FloatsOrCorks;
                case 10:
                    return Tubs.FadMaterials.Unknown;
                case 11:
                    return Tubs.FadMaterials.ChainCableRingsOrWeights;
                case 12:
                    return Tubs.FadMaterials.CordOrRope;
                case 13:
                    return Tubs.FadMaterials.Netting;
                case 14:
                    return Tubs.FadMaterials.BaitContainers;
                case 15:
                    return Tubs.FadMaterials.SackingOrBagging;
                case 16:
                    return Tubs.FadMaterials.CoconutFronds;
                case 17:
                    return Tubs.FadMaterials.Other;
                default:
                    return null;
            }
        }
    }

    public class MaterialListResolver : ValueResolver<IList<Tubs.FadMaterials>, IList<int>>
    {
        public static int? Convert(Tubs.FadMaterials source)
        {
            switch (source)
            {
                case Tubs.FadMaterials.LogsOrTrees:
                    return 1;
                case Tubs.FadMaterials.Timber:
                    return 2;
                case Tubs.FadMaterials.PvcOrPlasticTubing:
                    return 3;
                case Tubs.FadMaterials.PlasticDrums:
                    return 4;
                case Tubs.FadMaterials.PlasticSheeting:
                    return 5;
                case Tubs.FadMaterials.MetalDrums:
                    return 6;
                case Tubs.FadMaterials.Philippines:
                    return 7;
                case Tubs.FadMaterials.BambooOrCane:
                    return 8;
                case Tubs.FadMaterials.FloatsOrCorks:
                    return 9;
                case Tubs.FadMaterials.Unknown:
                    return 10;
                case Tubs.FadMaterials.ChainCableRingsOrWeights:
                    return 11;
                case Tubs.FadMaterials.CordOrRope:
                    return 12;
                case Tubs.FadMaterials.Netting:
                    return 13;
                case Tubs.FadMaterials.BaitContainers:
                    return 14;
                case Tubs.FadMaterials.SackingOrBagging:
                    return 15;
                case Tubs.FadMaterials.CoconutFronds:
                    return 16;
                case Tubs.FadMaterials.Other:
                    return 17;
                default:
                    return null;
            }
        }
        
        protected override IList<int> ResolveCore(IList<Tubs.FadMaterials> source)
        {
            if (null == source || 0 == source.Count)
                return new List<int>();

            var materials = new List<int>(source.Count);
            foreach (var src in source)
            {
                var cvt = Convert(src);
                if (cvt.HasValue)
                    materials.Add(cvt.Value);
            }

            return materials;
        }
    }

    /// <summary>
    /// MaterialCodeResolver converts data access layer enum value to
    /// numeric form code.
    /// </summary>
    public class MaterialCodeResolver : ValueResolver<Tubs.FadMaterials?, int?>
    {
        protected override int? ResolveCore(Tubs.FadMaterials? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case Tubs.FadMaterials.LogsOrTrees:
                    return 1;
                case Tubs.FadMaterials.Timber:
                    return 2;
                case Tubs.FadMaterials.PvcOrPlasticTubing:
                    return 3;
                case Tubs.FadMaterials.PlasticDrums:
                    return 4;
                case Tubs.FadMaterials.PlasticSheeting:
                    return 5;
                case Tubs.FadMaterials.MetalDrums:
                    return 6;
                case Tubs.FadMaterials.Philippines:
                    return 7;
                case Tubs.FadMaterials.BambooOrCane:
                    return 8;
                case Tubs.FadMaterials.FloatsOrCorks:
                    return 9;
                case Tubs.FadMaterials.Unknown:
                    return 10;
                case Tubs.FadMaterials.ChainCableRingsOrWeights:
                    return 11;
                case Tubs.FadMaterials.CordOrRope:
                    return 12;
                case Tubs.FadMaterials.Netting:
                    return 13;
                case Tubs.FadMaterials.BaitContainers:
                    return 14;
                case Tubs.FadMaterials.SackingOrBagging:
                    return 15;
                case Tubs.FadMaterials.CoconutFronds:
                    return 16;
                case Tubs.FadMaterials.Other:
                    return 17;
                default:
                    return null;
            }
        }
    }
}
