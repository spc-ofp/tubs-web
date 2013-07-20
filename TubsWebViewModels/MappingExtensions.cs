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
    using Spc.Ofp.Tubs.DAL.Entities;

    /// <summary>
    /// 
    /// </summary>
    public static class MappingExtensions
    {
        // Purse Seine target species
        private static string[] TARGET_TUNA = 
        { 
            "BET",
            "YFT",
            "SKJ"
        };
        
        
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

        /// <summary>
        /// Target and bycatch records are saved in the same table.
        /// This extension method (and IncludeInBycatch) use the species
        /// code and the presence of extra information to determine which
        /// UI list this record should be displayed in.
        /// </summary>
        /// <param name="sc">PurseSeineSetCatch to be examined</param>
        /// <returns>True if target species and no extra information, false otherwise.</returns>
        public static bool IncludeInTargetCatch(this PurseSeineSetCatch sc)
        {
            if (null == sc)
                return false;

            return TARGET_TUNA.Contains(sc.SpeciesCode) &&
                   !(sc.CountObserved.HasValue ||
                     sc.CountFromLog.HasValue ||
                     !String.IsNullOrEmpty(sc.Comments));            
        }

        /// <summary>
        /// Target and bycatch records are saved in the same table.
        /// This extension method (and IncludeInBycatch) use the species
        /// code and the presence of extra information to determine which
        /// UI list this record should be displayed in.
        /// </summary>
        /// <param name="sc">PurseSeineSetCatch to be examined</param>
        /// <returns>True if bycatch species or extra information, false otherwise.</returns>
        public static bool IncludeInBycatch(this PurseSeineSetCatch sc)
        {
            if (null == sc)
                return false;

            return sc.CountObserved.HasValue ||
                   sc.CountFromLog.HasValue ||
                   !String.IsNullOrEmpty(sc.Comments) ||
                   !TARGET_TUNA.Contains(sc.SpeciesCode);
        }

        /// <summary>
        /// Electronic devices which are commonly found on all fishing vessels.
        /// There is little value in collecting make or model information of common
        /// devices.
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns>True if common equipment type, false otherwise.</returns>
        public static bool IsCommonDevice(this ElectronicDeviceType deviceType)
        {
            return
                deviceType == ElectronicDeviceType.Gps ||
                deviceType == ElectronicDeviceType.TrackPlotter ||
                deviceType == ElectronicDeviceType.DepthSounder ||
                deviceType == ElectronicDeviceType.SstGauge;
        }

        /// <summary>
        /// Electronic devices that are associated with buoys.
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns>True if associated with buoys, false otherwise.</returns>
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

        /// <summary>
        /// Convert string value into measuring instrument enumeration value.
        /// </summary>
        /// <remarks>
        /// One _HUGE_ annoyance with AutoMapper is that it's near on impossible to re-use resolvers outside
        /// a mapping context.  Hence, 
        /// </remarks>
        /// <param name="source">Verbose measuring instrument description</param>
        /// <returns>Enumeration value or null if string can't be converted.</returns>
        public static MeasuringInstrument? MeasuringInstrumentFromString(this string source)
        {
            source = String.IsNullOrEmpty(source) ? source : source.Trim();
            if (String.IsNullOrEmpty(source))
                return null;

            return
                "Board".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? MeasuringInstrument.B :
                "Aluminum Caliper".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? MeasuringInstrument.C :
                "Ruler".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? MeasuringInstrument.R :
                "Deck Tape".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? MeasuringInstrument.T :
                "Wooden Caliper".Equals(source, StringComparison.InvariantCultureIgnoreCase) ? MeasuringInstrument.W :
                (MeasuringInstrument?)null;
        }
    }
}
