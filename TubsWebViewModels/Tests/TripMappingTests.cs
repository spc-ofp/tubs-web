﻿// -----------------------------------------------------------------------
// <copyright file="TripMappingTests.cs" company="Secretariat of the Pacific Community">
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

    /// <summary>
    /// TripMappingTests holds tests associated with mapping a
    /// DAL Trip entity to a ViewModel (and vice versa).
    /// </summary>
    [TestFixture]
    public class TripMappingTests
    {
        [Test]
        public void TripEntityToTripSamplingViewModel([Values(70)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId) as PurseSeineTrip;
                Assert.NotNull(trip);
                var vm = Mapper.Map<PurseSeineTrip, TripSamplingViewModel>(trip);
                Assert.NotNull(vm);
            }
        }

        [Test]
        public void TripEntityToTrackViewModel([Values(70)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId);
                Assert.NotNull(trip);
                var vm = Mapper.Map<Trip, TrackViewModel>(trip);
                Assert.NotNull(vm);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(vm.GeoJson);
                // Not really sure how to test this.  For now, assume it passes while dumping the
                // output to the console for external validation.
                System.Console.WriteLine(json);
                Assert.IsNotNullOrEmpty(json);
            }
        }

        [Test]
        public void TripEntityToPositionsViewModel([Values(70)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId);
                Assert.NotNull(trip);
                var vm = Mapper.Map<Trip, PositionsViewModel>(trip);
                Assert.NotNull(vm);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(vm.GeoJson);
                // Not really sure how to test this.  For now, assume it passes while dumping the
                // output to the console for external validation.
                System.Console.WriteLine(json);
                Assert.IsNotNullOrEmpty(json);
            }
        }
        
        
        [Test]
        public void PurseSeineTripEntityToTripSummaryViewModel([Values(70)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId) as PurseSeineTrip;
                Assert.NotNull(trip);
                var vm = Mapper.Map<PurseSeineTrip, TripSummaryViewModel>(trip);
                Assert.NotNull(vm);
                Assert.AreEqual(tripId, vm.Id);
                StringAssert.AreEqualIgnoringCase("S", vm.GearCode);
                // Expected assertion values determined by inspection
                Assert.AreEqual(20, vm.SeaDayCount);
                Assert.AreEqual(19, vm.ExpectedSeaDayCount);
                Assert.AreEqual(22, vm.SetCount);
                Assert.False(vm.HasCrew, "HasCrew mismatch");
                Assert.True(vm.HasPositions, "HasPositions mismatch");
                Assert.True(vm.HasGen3, "HasGen3 mismatch");
                Assert.Greater(vm.SightingCount, 10);
                Assert.AreEqual(3, vm.TransferCount);
            }
        }
        
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
        public void TripEntityToLongLineTripInfoViewModel([Values(4177)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId) as LongLineTrip;
                Assert.NotNull(trip);
                Assert.NotNull(trip.Gear, "Gear entity is null");
                Assert.NotNull(trip.Inspection, "Safety Inspection entity is null");
                var vm = Mapper.Map<LongLineTrip, LongLineTripInfoViewModel>(trip);
                Assert.NotNull(vm, "AutoMapper yielded a null ViewModel");
                Assert.AreEqual(tripId, vm.TripId);
                Assert.NotNull(vm.Characteristics);
                Assert.NotNull(vm.Gear);
                Assert.NotNull(vm.Inspection);
                Assert.NotNull(vm.Refrigeration);
                StringAssert.AreEqualIgnoringCase("NC", vm.Characteristics.CountryCode.Trim());
            }
        }

        [Test]
        public void MergeGearAndRefrigeration([Values(4177)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId) as LongLineTrip;
                Assert.NotNull(trip);
                Assert.NotNull(trip.Gear, "Gear entity is null");
                Assert.NotNull(trip.Inspection, "Safety Inspection entity is null");
                var tivm = Mapper.Map<LongLineTrip, LongLineTripInfoViewModel>(trip);
                Assert.NotNull(tivm, "AutoMapper yielded a null ViewModel");

                // Refrigeration first
                var gear = Mapper.Map<LongLineTripInfoViewModel.RefrigerationMethod, LongLineGear>(tivm.Refrigeration);
                Assert.NotNull(gear);
                // With refrigeration bits filled in, add in the rest of the gear
                Mapper.Map<LongLineTripInfoViewModel.FishingGear, LongLineGear>(tivm.Gear, gear);
                Assert.NotNull(gear);
                // Something from refrigeration
                Assert.True(gear.HasIce.HasValue && gear.HasIce.Value);
                // Something from terminal gear
                Assert.AreEqual(4.0M, gear.MainlineDiameter);
                Assert.AreEqual(Spc.Ofp.Tubs.DAL.Common.UsageCode.ALL, gear.LineShooterUsage);

            }           
            
        }

        [Test]
        public void MergeInfoAndComms([Values(69,70)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId);
                Assert.NotNull(trip);
                Assert.NotNull(trip.Electronics);
                Assert.Less(0, trip.Electronics.Count);
                Assert.NotNull(trip.CommunicationServices);

                var evm = Mapper.Map<Trip, ElectronicsViewModel>(trip);
                Assert.AreEqual(trip.CommunicationServices.Id, evm.ServiceId);

                var svc = Mapper.Map<ElectronicsViewModel.CommunicationServices, CommunicationServices>(evm.Communications);
                Assert.NotNull(svc);
                Mapper.Map<ElectronicsViewModel.InformationServices, CommunicationServices>(evm.Info, svc);
                Assert.NotNull(svc);

                // Something from communication services
                Assert.AreEqual(trip.CommunicationServices.HasSatellitePhone, svc.HasSatellitePhone);
                StringAssert.AreEqualIgnoringCase(trip.CommunicationServices.SatellitePhoneNumber, svc.SatellitePhoneNumber);

                // Something from information services
                Assert.AreEqual(trip.CommunicationServices.HasSeaHeightService, svc.HasSeaHeightService);
                Assert.AreEqual(trip.CommunicationServices.HasSatelliteMonitor, svc.HasSatelliteMonitor);

            }
        }

        // Trips picked at random from those with gear and safety inspection records
        [Test]
        public void TripInfoRoundTrip([Values(4177, 4231, 4242)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId) as LongLineTrip;
                Assert.NotNull(trip);
                Assert.NotNull(trip.Inspection);
                var tivm = Mapper.Map<LongLineTrip, LongLineTripInfoViewModel>(trip);
                Assert.NotNull(tivm);
                // Convert ViewModel back into entities
                // Refrigeration first
                var gear = Mapper.Map<LongLineTripInfoViewModel.RefrigerationMethod, LongLineGear>(tivm.Refrigeration);
                Assert.NotNull(gear);
                // With refrigeration bits filled in, add in the rest of the gear
                Mapper.Map<LongLineTripInfoViewModel.FishingGear, LongLineGear>(tivm.Gear, gear);
                Assert.NotNull(gear);
                Assert.AreEqual(trip.Gear.Id, gear.Id);

                var inspection = Mapper.Map<SafetyInspectionViewModel, SafetyInspection>(tivm.Inspection);
                Assert.NotNull(inspection);
                Assert.AreEqual(trip.Inspection.BuoyCount, inspection.BuoyCount);
                Assert.AreEqual(trip.Inspection.Id, inspection.Id);
                Assert.AreEqual(trip.Inspection.LifejacketAvailability, inspection.LifejacketAvailability);

                if (null != trip.VesselNotes)
                {
                    var notes = Mapper.Map<LongLineTripInfoViewModel.VesselCharacteristics, VesselNotes>(tivm.Characteristics);
                    Assert.NotNull(notes);
                    Assert.AreEqual(trip.VesselNotes.Captain, notes.Captain);
                }

            }
        }

        [Test]
        public void TripEntityToSightingViewModel([Values(70)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId);
                Assert.NotNull(trip);
                var vm = Mapper.Map<Trip, SightingViewModel>(trip);
                Assert.NotNull(vm);
                Assert.AreEqual(tripId, vm.TripId);
                Assert.NotNull(vm.Sightings);
                Assert.GreaterOrEqual(vm.Sightings.Count, 20);
            }
        }

        [Test]
        public void TripEntityToTransferViewModel([Values(111)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId);
                Assert.NotNull(trip);
                // var entity = Mapper.Map<TransferViewModel.Transfer, Transfer>(transfer);
                var vm = Mapper.Map<Trip, TransferViewModel>(trip);
                Assert.NotNull(vm);
                Assert.AreEqual(tripId, vm.TripId);
                Assert.NotNull(vm.Transfers);
                Assert.GreaterOrEqual(vm.Transfers.Count, 5);
            }
        }

        [Test]
        public void TripEntityToCrewViewModel([Values(103)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId) as PurseSeineTrip;
                Assert.NotNull(trip);
                var vm = Mapper.Map<PurseSeineTrip, CrewViewModel>(trip);
                Assert.NotNull(vm);
                Assert.NotNull(vm.Captain);
                StringAssert.AreEqualIgnoringCase("James T. Kirk", vm.Captain.Name);
                Assert.NotNull(vm.ChiefEngineer);
                Assert.NotNull(vm.Navigator);
                Assert.NotNull(vm.Cook);
                Assert.GreaterOrEqual(vm.Hands.Count, 2);
            }
        }

        [Test]
        public void TripEntityToPageCountViewModel([Values(70)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId) as PurseSeineTrip;
                Assert.NotNull(trip);
                var vm = Mapper.Map<PurseSeineTrip, PageCountViewModel>(trip);
                Assert.NotNull(vm);
                Assert.GreaterOrEqual(vm.PageCounts.Count, 6);
            }
        }

        [Test]
        public void TripEntityToElectronicsViewModel([Values(389)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId);
                Assert.NotNull(trip);
                var vm = Mapper.Map<Trip, ElectronicsViewModel>(trip);
                Assert.NotNull(vm);
                Assert.NotNull(vm.Gps);
                Assert.NotNull(vm.TrackPlotter);
                Assert.NotNull(vm.DepthSounder);
                Assert.AreEqual(1, vm.Buoys.Count, "Buoys");
                Assert.AreEqual(1, vm.Vms.Count, "VMS");
                Assert.Less(0, vm.OtherDevices.Count, "Other Devices");
                Assert.AreEqual(1, 
                    vm.OtherDevices.Where(d => "Doppler Current Meter".Equals(
                        d.DeviceType, StringComparison.InvariantCultureIgnoreCase)).Count());
                
            }
        }

        [Test]
        public void TripEntityToElectronicsViewModelWithComms([Values(70)] int tripId)
        {
            Mapper.AssertConfigurationIsValid();
            using (var repo = TubsDataService.GetRepository<Trip>(false))
            {
                var trip = repo.FindById(tripId);
                Assert.NotNull(trip);
                var vm = Mapper.Map<Trip, ElectronicsViewModel>(trip);
                Assert.NotNull(vm);
                Assert.NotNull(vm.Communications);
                StringAssert.AreEqualIgnoringCase("YES", vm.Communications.HasEmail);
                Assert.Null(vm.Communications.EmailAddress);
                StringAssert.AreEqualIgnoringCase("YES", vm.Communications.HasFax);
                StringAssert.AreEqualIgnoringCase("357687111", vm.Communications.FaxNumber);
                StringAssert.AreEqualIgnoringCase("YES", vm.Communications.HasSatellitePhone);
                StringAssert.AreEqualIgnoringCase("357687110", vm.Communications.SatellitePhoneNumber);

                Assert.NotNull(vm.Info);
                StringAssert.AreEqualIgnoringCase("NO", vm.Info.HasOther);
                StringAssert.AreEqualIgnoringCase("NO", vm.Info.HasSeaHeightService);
                Assert.Null(vm.Info.SeaHeightServiceUrl);
                StringAssert.AreEqualIgnoringCase("NO", vm.Info.HasPhytoplanktonService);
                Assert.Null(vm.Info.PhytoplanktonUrl);
                StringAssert.AreEqualIgnoringCase("YES", vm.Info.HasSatelliteMonitor);
            }
        }
    }
}
