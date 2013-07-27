// -----------------------------------------------------------------------
// <copyright file="ValidTripFilterAttribute.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL.Entities;

    /// <summary>
    /// MVC ActionFilter for determining the presence of a valid trip.
    /// </summary>
    /// <remarks>
    /// Can be used to check for a non-null Trip, or can verify that the bound
    /// trip is of a specific concrete type.
    /// </remarks>
    public class ValidTripFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// MIME type for JSON content.
        /// </summary>
        public const string JSON_MIME_TYPE = "application/json";

        /// <summary>
        /// Message when trip doesn't exist in the database.
        /// </summary>
        public const string MISSING_TRIP_MESSAGE = "Trip doesn't exist in system";

        /// <summary>
        /// The caller is expecting a different concrete Trip class.
        /// (e.g. Expecting a PurseSeine trip and received a LongLine trip)
        /// </summary>
        public const string INVALID_TRIP_MESSAGE = "Wrong trip type";

        /// <summary>
        /// Parameter name of model-bound trip parameter.
        /// </summary>
        public string Parameter = "tripId";

        /// <summary>
        /// Desired trip type.  Default is base Trip clase, but can be specific class
        /// as well.
        /// </summary>
        public Type TripType = typeof(Trip);
        
        /// <summary>
        /// IsApiRequest determines if the current action request is a Javascript API call
        /// or destined for Razor output.
        /// </summary>
        /// <param name="filterContext">Current action context</param>
        /// <returns>True if request is an API call, false otherwise.</returns>
        internal bool IsApiRequest(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            return null != request.AcceptTypes && request.AcceptTypes.Contains("application/json");
        }

        /// <summary>
        /// Check for presence of valid trip.
        /// </summary>
        /// <remarks>
        /// NOTE:  Assumes that model binder has already worked it's magic on the
        /// request pipeline.
        /// </remarks>
        /// <param name="filterContext">Current action context</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // If the parameter doesn't exist in the collection of params, assume that the
            // filter was incorrectly applied and drive on.
            if (!filterContext.ActionParameters.ContainsKey(Parameter))
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            
            var trip = filterContext.ActionParameters[Parameter];
            bool isPresent = null != trip;
            bool isCorrectType = isPresent && this.TripType.IsAssignableFrom(trip.GetType());

            // Trip exists and is of the correct type.
            if (isPresent && isCorrectType)
            {                    
                base.OnActionExecuting(filterContext);
                return;
            }

            // Push to our custom 404 page called 'NoSuchTripResult'
            if (!IsApiRequest(filterContext))
            {
                filterContext.Result = new NoSuchTripResult();
                return;
            }

            // At this point, we know that the trip is missing (or wrong type)
            // and need to send an appropriate result.
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var result = new JsonNetResult()
            {
                Data = isPresent ? INVALID_TRIP_MESSAGE : MISSING_TRIP_MESSAGE,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            filterContext.Result = result;
        }
    }
}