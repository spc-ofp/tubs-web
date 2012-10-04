// -----------------------------------------------------------------------
// <copyright file="NavPill.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------


using System.Collections.Generic;
namespace TubsWeb.Core
{
    public class NavPill
    {
        public string Title { get; set; }
        public string Href { get; set; }
        // TODO Come up with a good collection for this
        public IDictionary<string, string> Attributes { get; set; }
    }
}