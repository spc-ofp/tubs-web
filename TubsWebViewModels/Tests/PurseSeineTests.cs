// -----------------------------------------------------------------------
// <copyright file="PurseSeineTests.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
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
    using AutoMapper;
    using NUnit.Framework;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;

    /// <summary>
    /// Purse Seine ViewModel tests
    /// TODO: Move Purse Seine entities here from elsewhere
    /// </summary>
    [TestFixture]
    public class PurseSeineTests
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// The three headers in question are all from the same set.
        /// </remarks>
        /// <param name="headerId">PS-4 header primary key</param>
        [Test]
        public void SinglePs4PageEntityToViewModel([Values(238, 239, 240)] int headerId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<LengthSamplingHeader>(false))
            {
                var header = repo.FindById(headerId);
                Assert.NotNull(header);
                var vm = Mapper.Map<LengthSamplingHeader, LengthFrequencyViewModel>(header);
                Assert.NotNull(vm);
                Assert.AreEqual(header.Id, vm.Id);
                Assert.NotNull(vm.Brails);
                Assert.Less(10, vm.Brails.Count);
                Assert.NotNull(vm.Samples);
                Assert.Less(5, vm.Samples.Count);
                StringAssert.AreEqualIgnoringCase("Grab", vm.SampleType);
                Assert.AreEqual(3, vm.PageCount);
            }
        }
    }
}
