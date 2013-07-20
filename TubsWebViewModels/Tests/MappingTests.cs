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
    using System.Collections.Generic;
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
        public void SeaDayEntityToViewModel([Values(431)] int dayId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<PurseSeineSeaDay>(false))
            {
                var day = repo.FindById(dayId);
                Assert.NotNull(day);
                var vm = Mapper.Map<PurseSeineSeaDay, SeaDayViewModel>(day);
                Assert.NotNull(vm, "AutoMapper yielded a null ViewModel (SeaDayViewModel)");
                Assert.AreEqual(dayId, vm.DayId);
                Assert.AreEqual(6, vm.Events.Count);
            }
        }

        [Test]
        public void SeaDayViewModelRoundTrip([Values(431)] int dayId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<PurseSeineSeaDay>(false))
            {
                var day = repo.FindById(dayId);
                Assert.NotNull(day);
                var vm = Mapper.Map<PurseSeineSeaDay, SeaDayViewModel>(day);
                Assert.NotNull(vm, "AutoMapper yielded a null ViewModel (SeaDayViewModel)");
                var rtDay = Mapper.Map<SeaDayViewModel, PurseSeineSeaDay>(vm);
                Assert.NotNull(rtDay);
                Assert.AreEqual(day.Activities.Count, rtDay.Activities.Count);
                Assert.AreEqual(day.Activities[0].LocalTime, rtDay.Activities[0].LocalTime);
            }
        }

        // Not a real test -- Used to find an otherwise difficult to find NPE
        // in AutoMapper
        [Test]
        public void ChildEntityToViewModel([Values(4177)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<LongLineGear>(false))
            {
                var gear = repo.FindBy(g => g.Trip.Id == tripId);
                Assert.NotNull(gear);
                var vm = Mapper.Map<LongLineGear, LongLineTripInfoViewModel.FishingGear>(gear);
                Assert.NotNull(vm, "AutoMapper yielded a null ViewModel (LongLineGear)");
            }

            using (var repo = TubsDataService.GetRepository<SafetyInspection>(false))
            {
                var inspection = repo.FindBy(i => i.Trip.Id == tripId);
                Assert.NotNull(inspection);
                var vm = Mapper.Map<SafetyInspection, SafetyInspectionViewModel>(inspection);
                Assert.NotNull(vm, "AutoMapper yielded a null ViewModel (SafetyInspection)");
            }
        }

        [Test]
        public void LongLineSetEntityToViewModel([Values(26)] int setId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<LongLineSet>(false))
            {
                var fset = repo.FindById(setId);
                Assert.NotNull(fset);
                var vm = Mapper.Map<LongLineSet, LongLineSetViewModel>(fset);
                Assert.NotNull(vm);
                Assert.AreEqual(setId, vm.SetId);
                Assert.AreNotEqual(0, vm.TripId);
                Assert.NotNull(vm.StartOfSet);
                StringAssert.AreEqualIgnoringCase("0535", vm.StartOfSet.LocalTime);
                Assert.NotNull(vm.EndOfSet);
                StringAssert.AreEqualIgnoringCase("0930", vm.EndOfSet.LocalTime);
                Assert.NotNull(vm.StartOfHaul);
                Assert.NotNull(vm.EndOfHaul);
                Assert.NotNull(vm.IntermediateHaulPositions);
                Assert.GreaterOrEqual(10, vm.IntermediateHaulPositions.Count);
            }
        }

        [Test]
        public void LongLineSetRoundTrip([Values(26)] int setId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<LongLineSet>(false))
            {
                var fset = repo.FindById(setId);
                Assert.NotNull(fset);
                var vm = Mapper.Map<LongLineSet, LongLineSetViewModel>(fset);
                Assert.NotNull(vm);
                var entity = Mapper.Map<LongLineSetViewModel, LongLineSet>(vm);
                Assert.NotNull(entity);
                Assert.AreEqual(fset.LineSettingSpeedUnit, entity.LineSettingSpeedUnit);
                Assert.AreEqual(fset.EventList.Count, entity.EventList.Count);
                Assert.AreEqual(fset.NotesList.Count, entity.NotesList.Count);
                // Event portion
                var startEvent = entity.EventList[0];
                Assert.NotNull(startEvent);
                Assert.NotNull(startEvent.FishingSet);
                Assert.AreEqual(fset.Id, startEvent.FishingSet.Id);
                Assert.AreEqual(fset.EventList[0].ActivityType, startEvent.ActivityType);
                Assert.AreEqual(fset.EventList[0].Id, startEvent.Id);
                Assert.AreEqual(fset.EventList[0].LogDate, startEvent.LogDate);
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
                Assert.True(vm.AllCatch.Any());
                //Assert.Greater(vm.AllCatch.Count, 0);
                Assert.NotNull(vm.TargetCatch);
                Assert.AreEqual(2, vm.TargetCatch.Count);
                Assert.NotNull(vm.ByCatch);
                Assert.AreEqual(3, vm.ByCatch.Count);
                //Assert.AreEqual(vm.AllCatch.Count, (vm.TargetCatch.Count + vm.ByCatch.Count));

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
        public void RoundTripSetTest([Values(284)] int setId)
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

        // Values determined by inspection
        [Test]
        public void LengthSampleHeaderToViewModel([Values(234)] int lengthFrequencyId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<LengthSamplingHeader>(false))
            {
                var ps4 = repo.FindById(lengthFrequencyId);
                Assert.NotNull(ps4);
                var vm = Mapper.Map<LengthSamplingHeader, LengthFrequencyViewModel>(ps4);
                Assert.NotNull(vm);
                StringAssert.AreEqualIgnoringCase("1729", vm.StartBrailingTime);
                StringAssert.AreEqualIgnoringCase("1859", vm.EndBrailingTime);
                Assert.True(vm.IsBrail1, "IsBrail1?");
                Assert.AreEqual(1, vm.SamplePageNumber);
                Assert.AreEqual(3, vm.SamplePageTotal);
                Assert.AreEqual(5, vm.GrabTarget);
                Assert.AreEqual(10, vm.SevenEighthsBrailCount);
                Assert.AreEqual(10, vm.ThreeQuartersBrailCount);
                Assert.AreEqual(30, vm.TotalBrails);
                Assert.AreEqual(21.5, vm.SumOfAllBrails);
            }
        }

        [Test]
        public void Gen3ToViewModel([Values(4167)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId);
                Assert.NotNull(trip);
                var vm = Mapper.Map<Trip, Gen3ViewModel>(trip);
                Assert.NotNull(vm);
                Assert.NotNull(vm.Incidents);
                Assert.AreEqual(2, vm.Incidents.Count);
                Assert.True(vm.IncidentByCode.ContainsKey("RS-A"));
                Assert.NotNull(vm.Notes);
                Assert.AreEqual(1, vm.Notes.Count);
            }
        }

        [Test]
        public void LegacyGen3ToViewModel([Values(70)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId);
                Assert.NotNull(trip);
                var vm = Mapper.Map<TripMonitor, Gen3ViewModel>(trip.TripMonitor);
                Assert.NotNull(vm);
                Assert.NotNull(vm.Incidents);
                Assert.AreEqual(20, vm.Incidents.Count);
                StringAssert.AreEqualIgnoringCase("NO", vm.Incidents[7].Answer);
                StringAssert.AreEqualIgnoringCase("YES", vm.Incidents[8].Answer);
                Assert.NotNull(vm.Notes);
                Assert.AreEqual(3, vm.Notes.Count);
            }
        }

        [Test]
        public void LegacyGen3ViewModelToEntity()
        {
            Mapper.AssertConfigurationIsValid();
            var vm = new Gen3ViewModel();
            vm.Incidents = new List<Gen3ViewModel.Incident>(20);
            for (int i = 0; i < 20; i++)
            {
                vm.Incidents.Add(new Gen3ViewModel.Incident() { Answer = "NO", QuestionCode = Char.ToString((char)(97 + i)) });
            }
            vm.Incidents[6].Answer = "YES";
            var entity = Mapper.Map<Gen3ViewModel, TripMonitor>(vm);
            Assert.NotNull(entity);
            Assert.True(entity.Question7.HasValue);
            Assert.True(entity.Question7.Value);
        }

        [Test]
        public void Gen3ViewModelToEntity()
        {
            // Arrange
            Mapper.AssertConfigurationIsValid();
            var vm = new Gen3ViewModel();
            vm.Incidents = new List<Gen3ViewModel.Incident>(3);
            vm.Incidents.Add(new Gen3ViewModel.Incident() { Answer = "YES", QuestionCode = "RS-A", JournalPage = 1 });
            vm.Incidents.Add(new Gen3ViewModel.Incident() { Answer = "YES", QuestionCode = "RS-C", JournalPage = 5 });
            vm.Incidents.Add(new Gen3ViewModel.Incident() { Answer = "YES", QuestionCode = "NR-A", JournalPage = 3 });
            vm.Notes = new List<Gen3ViewModel.Note>(2);
            vm.Notes.Add(new Gen3ViewModel.Note() { Date = DateTime.Now, Comments = "Xyzzy" });
            vm.Notes.Add(new Gen3ViewModel.Note() { Date = DateTime.Now, Comments = "Plover" });
            // Act
            var entity = Mapper.Map<Gen3ViewModel, MockTripMonitor>(vm);
            // Assert
            Assert.NotNull(entity);
            Assert.NotNull(entity.Answers);
            Assert.AreEqual(3, entity.Answers.Count);
            Assert.NotNull(entity.Details);
            Assert.AreEqual(2, entity.Details.Count);
        }

        [Test]
        public void Gen2SightingEntityToViewModel([Values(7)] int sspId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Interaction>(false))
            {
                var ssi = repo.FindById(sspId);
                Assert.NotNull(ssi, "Entity lookup returned null");
                var vm = Mapper.Map<Interaction, Gen2ViewModel>(ssi) as Gen2SightingViewModel;              
                Assert.NotNull(vm, "AutoMapper view model is null");
                StringAssert.AreEqualIgnoringCase("FAW", vm.SpeciesCode);
                Assert.True(vm.NumberSighted.HasValue);
                Assert.AreEqual(8, vm.NumberSighted);

            }
        }

        [Test]
        public void Gen2LandedEntityToViewModel([Values(10)] int sspId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Interaction>(false))
            {
                var ssi = repo.FindById(sspId);
                Assert.NotNull(ssi, "Entity lookup returned null");
                var vm = Mapper.Map<Interaction, Gen2ViewModel>(ssi) as Gen2LandedViewModel;
                Assert.NotNull(vm, "AutoMapper view model is null");
                StringAssert.AreEqualIgnoringCase("RHN", vm.SpeciesCode);
                Assert.AreEqual("A0", vm.LandedConditionCode);
                

            }
        }
    }
}