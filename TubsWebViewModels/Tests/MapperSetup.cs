// -----------------------------------------------------------------------
// <copyright file="MapperSetup.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Tests
{
    using NUnit.Framework;
    using TubsWeb;

    [SetUpFixture]
    public class MapperSetup
    {
        [SetUp]
        public void LoadProfiles()
        {
            System.Console.WriteLine("Loading profiles...");
            MappingConfig.Configure();
        }
    }
}