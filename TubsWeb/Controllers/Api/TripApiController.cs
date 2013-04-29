// -----------------------------------------------------------------------
// <copyright file="TripApiController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Spc.Ofp.Tubs.DAL;
    using TubsWeb.Models;
    
    public class TripApiController : ApiController
    {
        public TripHeaderViewModel Get(int id)
        {
            var repo = new TubsRepository<TripHeaderViewModel>(MvcApplication.CurrentSession);
            var trip = repo.FindBy(id);
            if (null == trip)
            {
                // Do something here
            }
            return trip;
        }
    }
}
