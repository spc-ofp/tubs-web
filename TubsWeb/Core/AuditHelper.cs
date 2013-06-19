// -----------------------------------------------------------------------
// <copyright file="AuditHelper.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
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
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Infrastructure;
    
    /// <summary>
    /// AuditHelper is a throw-away class used to backfill audit trails while
    /// a long term solution for the problem of "disappearing" audit trails is
    /// worked out.
    /// </summary>
    public sealed class AuditHelper
    {
        /// <summary>
        /// BackfillTrail will fill the EnteredBy/EnteredDate properties for an IAuditable
        /// entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pkid">Entity primary key</param>
        /// <param name="entity">Entity</param>
        /// <param name="repo">Repository for accessing entities of type T</param>
        public static void BackfillTrail<T>(object pkid, T entity, IRepository<T> repo) where T : class, IAuditable
        {
            if (null != pkid && null != entity && null != repo)
            {
                var previous = repo.FindById(pkid);
                if (null != previous)
                {
                    entity.EnteredBy = previous.EnteredBy;
                    entity.EnteredDate = previous.EnteredDate;
                }
            }
        }
    }
}