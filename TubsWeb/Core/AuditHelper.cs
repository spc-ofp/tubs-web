// -----------------------------------------------------------------------
// <copyright file="AuditHelper.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Infrastructure;
    
    /// <summary>
    /// AuditHelper is a throw-away class used to backfill audit trails while
    /// a long term solution for the problem of "disappearing" audit trails is
    /// worked out.
    /// </summary>
    public sealed class AuditHelper
    {
        public static void BackfillTrail<T>(object pkid, T entity, IRepository<T> repo) where T : class, IAuditable
        {
            if (null != pkid && null != entity && null != repo)
            {
                var previous = repo.FindById(pkid);
                entity.EnteredBy = previous.EnteredBy;
                entity.EnteredDate = previous.EnteredDate;
            }
        }
    }
}