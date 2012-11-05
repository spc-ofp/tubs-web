// -----------------------------------------------------------------------
// <copyright file="ActionCodeResolver.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ActionCodeResolver : ValueResolver<string, ActionType?>
    {
        protected override ActionType? ResolveCore(string source)
        {
            source = string.IsNullOrEmpty(source) ? source : source.Trim();
            if (string.IsNullOrEmpty(source))
                return null;

            return
                "FI".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.FI :
                "PF".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.PF :
                "NF".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.NF :
                "DF".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.DF :
                "TR".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.TR :
                "SR".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.SR :
                "BR".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.BR :
                "OR".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.OR :
                "TG".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.TG :
                "SG".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.SG :
                "BG".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.BG :
                "OG".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? ActionType.OG :
                (ActionType?)null;

        }
    }

    public class ActionTypeResolver : ValueResolver<ActionType?, string>
    {
        protected override string ResolveCore(ActionType? source)
        {
            if (!source.HasValue)
                return null;

            switch (source.Value)
            {
                case ActionType.FI:
                    return "FI";
                case ActionType.PF:
                    return "PF";
                case ActionType.NF:
                    return "NF";
                case ActionType.DF:
                    return "DF";
                case ActionType.TR:
                    return "TR";
                case ActionType.SR:
                    return "SR";
                case ActionType.BR:
                    return "BR";
                case ActionType.OR:
                    return "OR";
                case ActionType.TG:
                    return "TG";
                case ActionType.SG:
                    return "SG";
                case ActionType.BG:
                    return "BG";
                case ActionType.OG:
                    return "OG";
                default:
                    return null;
            }
        }
    }
}
