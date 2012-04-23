// -----------------------------------------------------------------------
// <copyright file="PositionAuditViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Models
{

    using System;
    using System.ComponentModel.DataAnnotations;
    using Spc.Ofp.Tubs.DAL.Entities;

    public class PositionAuditViewModel
    {
        // Radius of the Earth in Km, naturally.
        public const double RadiusOfTheEarth = 6371;
        public const double DegreesToRadians = Math.PI / 180;

        [Display(Name = "Timestamp")]
        public DateTime? Timestamp { get; set; }

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [Display(Name = "Time Difference From Last Position")]
        public TimeSpan? DeltaTime { get; set; }

        [Display(Name = "Distance From Last Position")]
        public double? DeltaPosition { get; set; }

        [Display(Name = "Velocity From Last Position")]
        public double? DeltaVelocity { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public bool HasPosition
        {
            get
            {
                return this.Latitude.HasValue && this.Longitude.HasValue;
            }
        }

        public static PositionAuditViewModel FromPushpin(Pushpin pushpin)
        {
            if (null == pushpin)
                return null;

            return new PositionAuditViewModel()
            {
                Timestamp = pushpin.Timestamp,
                Latitude = (double)pushpin.Latitude,
                Longitude = (double)pushpin.Longitude,
                Description = pushpin.Description
            };
        }

        public static PositionAuditViewModel Diff(PositionAuditViewModel previous, PositionAuditViewModel current)
        {
            PositionAuditViewModel pavm = new PositionAuditViewModel()
            {
                Timestamp = current.Timestamp,
                Latitude = current.Latitude,
                Longitude = current.Longitude,
                Description = current.Description
            };

            if (previous.Timestamp.HasValue && current.Timestamp.HasValue)
            {
                pavm.DeltaTime = current.Timestamp.Value.Subtract(previous.Timestamp.Value);
            }

            if (current.HasPosition && previous.HasPosition)
            {
                pavm.DeltaPosition = GreatCircleDistance(
                        current.Latitude.Value * DegreesToRadians,
                        current.Longitude.Value * DegreesToRadians,
                        previous.Latitude.Value * DegreesToRadians,
                        previous.Longitude.Value * DegreesToRadians);
            }

            if (pavm.DeltaPosition.HasValue && pavm.DeltaTime.HasValue && pavm.DeltaTime.Value.TotalHours > 0)
            {
                pavm.DeltaVelocity = pavm.DeltaPosition.Value / Math.Abs(pavm.DeltaTime.Value.TotalHours);
            }

            return pavm;
        }

        public static double GreatCircleDistance(double lat1, double lon1, double lat2, double lon2)
        {
            return RadiusOfTheEarth * Math.Acos(
                Math.Sin(lat1) * Math.Sin(lat2)
                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1));
        }

         
    }
}