// -----------------------------------------------------------------------
// <copyright file="RouteTests.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Tests
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
    using NUnit.Framework;
    using MvcRouteUnitTester;


    [TestFixture]
    public class RouteTests
    {
        [Test]
        [Ignore("This version of MvcRouteUnitTester doesn't support MVC4.  Sigh")]
        public void TestIncomingRoutes()
        {
            var tester = new RouteTester<MvcApplication>();
            tester.WithIncomingRequest("/").ShouldMatchRoute("Home", "Index");
        }
    }
}