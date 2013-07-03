// -----------------------------------------------------------------------
// <copyright file="LongLineTests.cs" company="Secretariat of the Pacific Community">
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
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using NUnit.Framework;
    using Spc.Ofp.Tubs.DAL;
    using Spc.Ofp.Tubs.DAL.Common;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.ViewModels;

    /// <summary>
    /// Long Line ViewModel tests.
    /// TODO: Move Long Line entities here from elsewhere.
    /// </summary>
    [TestFixture]
    public class LongLineTests
    {
        
        /// <summary>
        /// This test was used to diagnose an issue with NHibernate.
        /// https://nhibernate.jira.com/browse/NH-3043
        /// </summary>
        /// <param name="tripId"></param>
        [Test]
        public void LoadLongLineSetsForTrip([Values(4314,5150)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<LongLineSet>(false))
            {
                var sets = repo.FilterBy(s => s.Trip.Id == tripId).ToList();
                Assert.NotNull(sets);
                Assert.Greater(sets.Count, 0);

                var summaries =
                    from s in sets
                    select new LongLineCatchSummaryViewModel
                    {
                        TripId = tripId,
                        SetNumber = s.SetNumber.Value,
                        SetDate = s.SetDate,
                        SampleCount = null == s.CatchList ? 0 : s.CatchList.Count
                    };

                Assert.NotNull(summaries);
                Assert.AreEqual(sets.Count, summaries.Count());

            }
        }
        
        [Test]
        public void LongLineSampleToViewModel([Values(25)] int setId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<LongLineSet>(false))
            {
                var fset = repo.FindById(setId);
                Assert.IsNotNull(fset);
                // Convert to intermediate object
                var header = new LongLineCatchHeader();
                header.FishingSet = fset;
                header.Samples = fset.CatchList;

                var vm = Mapper.Map<LongLineCatchHeader, LongLineSampleViewModel>(header);
                Assert.NotNull(vm);

                Assert.AreEqual(fset.CatchList.Count, vm.Details.Count);
                Assert.True(fset.MeasuringInstrument.HasValue);
                Assert.AreEqual(MeasuringInstrument.C, fset.MeasuringInstrument);
                Assert.AreEqual("Aluminum Caliper", vm.MeasuringInstrument);

            }
        }

        [Test]
        public void LongLineSampleRoundTrip([Values(26)] int setId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<LongLineSet>(false))
            {
                var fset = repo.FindById(setId);
                Assert.IsNotNull(fset);
                // Convert to intermediate object
                var header = new LongLineCatchHeader();
                header.FishingSet = fset;
                header.Samples = fset.CatchList;

                var vm = Mapper.Map<LongLineCatchHeader, LongLineSampleViewModel>(header);
                Assert.NotNull(vm);

                var entity = Mapper.Map<LongLineSampleViewModel, LongLineCatchHeader>(vm);
                Assert.IsNotNull(entity);
                Assert.AreEqual(fset.CatchList.Count, entity.Samples.Count);
                for (int i = 0; i < fset.CatchList.Count; i++)
                {
                    var catch_ori = fset.CatchList[i];
                    Assert.IsNotNull(catch_ori);
                    var catch_rt = entity.Samples[i];
                    Assert.IsNotNull(catch_rt);

                    Assert.AreEqual(catch_ori.Id, catch_rt.Id);
                    Assert.AreEqual(catch_ori.SpeciesCode, catch_rt.SpeciesCode);
                    Assert.AreEqual(catch_ori.FateCode, catch_rt.FateCode);
                    Assert.AreEqual(catch_ori.Date, catch_rt.Date);
                }

            }
        }
    }
}
