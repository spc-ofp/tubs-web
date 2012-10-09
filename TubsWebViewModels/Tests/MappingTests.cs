// -----------------------------------------------------------------------
// <copyright file="MappingTests.cs" company="Secretariat of the Pacific Community">
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
    using System;
    using System.Linq;
    using AutoMapper;
    using NUnit.Framework;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;

    [TestFixture]
    public class MappingTests
    {
        [Test]
        public void TripEntityToPs1ViewModel([Values(70)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId) as PurseSeineTrip;
                Assert.NotNull(trip);
                var vm = Mapper.Map<PurseSeineTrip, Ps1ViewModel>(trip);
                Assert.NotNull(vm);
                Assert.AreEqual(tripId, vm.TripId);
                Assert.NotNull(vm.Characteristics);
                Assert.NotNull(vm.Gear);
                Assert.NotNull(vm.Inspection);
                Assert.NotNull(vm.Characteristics.CountryCode);
                StringAssert.AreEqualIgnoringCase("VU", vm.Characteristics.CountryCode.Trim());
            }
        }
        
        [Test]
        public void SetEntityToViewModel([Values(284)] int setId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<PurseSeineSet>(false))
            {
                var fset = repo.FindById(setId);
                Assert.NotNull(fset);
                var vm = Mapper.Map<PurseSeineSet, PurseSeineSetViewModel>(fset);
                Assert.NotNull(vm);
                Assert.AreNotEqual(0, vm.TripId);
                Assert.NotNull(vm.AllCatch);
                Assert.Greater(vm.AllCatch.Count, 0);
                Assert.NotNull(vm.TargetCatch);
                Assert.AreEqual(2, vm.TargetCatch.Count);
                Assert.NotNull(vm.ByCatch);
                Assert.AreEqual(3, vm.ByCatch.Count);
                Assert.AreEqual(vm.AllCatch.Count, (vm.TargetCatch.Count + vm.ByCatch.Count));

                StringAssert.AreEqualIgnoringCase("1726", vm.LogbookTime);
                Assert.True(vm.LogbookDate.HasValue);
                Assert.AreEqual(2, vm.SetNumber);
                Assert.AreEqual(setId, vm.SetId);

            }
        }

        [Test]
        public void FadEntityToViewModel([Values(1)] int fadId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Gen5Object>(false))
            {
                var fad = repo.FindById(fadId);
                Assert.NotNull(fad);
                var vm = Mapper.Map<Gen5Object, Gen5ViewModel>(fad);
                Assert.NotNull(vm);
            }
        }

        [Test]
        public void RoundTripTest([Values(284)] int setId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<PurseSeineSet>(false))
            {
                var fset = repo.FindById(setId);
                Assert.NotNull(fset);
                var vm = Mapper.Map<PurseSeineSet, PurseSeineSetViewModel>(fset);
                Assert.NotNull(vm);
                var entity = Mapper.Map<PurseSeineSetViewModel, PurseSeineSet>(vm);
                Assert.NotNull(entity);
                Assert.AreEqual(fset.Id, entity.Id);
                Assert.AreEqual(fset.RingsUp, entity.RingsUp);
                Assert.AreEqual(fset.CatchList.Count, entity.CatchList.Count);

            }
        }
    }
}