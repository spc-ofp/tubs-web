// -----------------------------------------------------------------------
// <copyright file="MappingExtensions.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using AutoMapper;
    using Spc.Ofp.Tubs.DAL.Common;

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

        public static bool IsDeviceCategory(this ElectronicDeviceType deviceType)
        {
            return
                deviceType == ElectronicDeviceType.Gps ||
                deviceType == ElectronicDeviceType.TrackPlotter ||
                deviceType == ElectronicDeviceType.DepthSounder ||
                deviceType == ElectronicDeviceType.SstGauge;
        }

        public static bool IsBuoy(this ElectronicDeviceType deviceType)
        {
            return
                deviceType == ElectronicDeviceType.GpsBuoys ||
                deviceType == ElectronicDeviceType.EchoSoundingBuoy;
        }

        // To/From description implementations direct from StackOverflow
        // Question 4367723

        /// <summary>
        /// Read description attribute from an enumeration value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Description(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            var attribute =
                Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return null == attribute ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// Convert string into an enumeration using the description attribute rather than a strict
        /// string comparison of Enum's ToString().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T FromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) 
                throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = 
                    Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (null != attribute)
                {
                    if (description.Equals(attribute.Description, StringComparison.InvariantCultureIgnoreCase))
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (description.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase))
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Enumeration value not found", "description");

        }
    }
}
