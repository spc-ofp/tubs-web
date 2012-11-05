// -----------------------------------------------------------------------
// <copyright file="MappingExtensions.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AutoMapper;

    /// <summary>
    /// 
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// Extension method for marking all properties as ignored.  This is dangerous as it could
        /// mean that using AutoMapper's built-in facilities for confirming that a map is complete
        /// are no longer present.
        /// http://stackoverflow.com/questions/4367591/automapper-how-to-ignore-all-destination-members-except-the-ones-that-are-mapp
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        [Obsolete("Danger, Will Robinson!")]
        public static IMappingExpression<TSource, TDest> IgnoreAllUnmapped<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Ignore());
            return expression;
        }
    }
}
