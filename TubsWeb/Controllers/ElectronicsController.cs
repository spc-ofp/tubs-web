// -----------------------------------------------------------------------
// <copyright file="ElectronicsController.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Controllers
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
    using System.Web.Mvc;
    using Spc.Ofp.Tubs.DAL.Entities;
    using TubsWeb.Core;
    using TubsWeb.Models;
    
    public class ElectronicsController : SuperController
    {
        private const string PathToDeviceModelEditorPartial = @"~/Views/Shared/EditorTemplates/DeviceModel.cshtml";


        private static List<string> NAMED_DEVICES = new List<string>()
        {
            "GPS",
            "TRACK PLOTTER",
            "DEPTH SOUNDER",
            "SST GAUGE",
            "BIRD RADAR",
            "SONAR",
            "GPS BUOYS",
            "ECHO SOUNDING BUOY",
            "NET DEPTH INSTRUMENTATION",
            "DOPPLER CURRENT METER",
            "VMS"
        };

        private static ElectronicsViewModel GetViewModel(Trip trip)
        {
            ElectronicsViewModel evm = new ElectronicsViewModel();
            if (null == trip)
                return evm;

            evm.TripId = trip.Id;
            // Materialize this to convert it to a standard list instead of the collection used by NHibernate
            var devices = trip.Electronics.ToList<ElectronicDevice>();
            devices.Sort(delegate(ElectronicDevice d1, ElectronicDevice d2)
            {
                return Comparer<string>.Default.Compare(d1.DeviceType.Description, d2.DeviceType.Description);
            });            
            
            evm.Gps = GetDeviceModel(devices, "GPS");
            evm.TrackPlotter = GetDeviceModel(devices, "TRACK PLOTTER");
            evm.DepthSounder = GetDeviceModel(devices, "DEPTH SOUNDER"); // FIXME for device name "DEPTH SOUNDER #1"
            evm.SstGauge = GetDeviceModel(devices, "SEA SURFACE TEMPERATURE GAUGE"); // FIXME for device named "SEA SURFACE TEMP. GAUGE"

            evm.BirdRadar = GetDeviceModel(devices, "BIRD RADAR");
            evm.Sonar = GetDeviceModel(devices, "SONAR");
            evm.GpsBuoys = GetDeviceModel(devices, "GPS BUOYS");
            evm.EchoSoundingBuoy = GetDeviceModel(devices, "ECHO SOUNDING BUOY");
            evm.NetDepthInstrumentation = GetDeviceModel(devices, "NET DEPTH INSTRUMENTATION");
            evm.DopplerCurrentMeter = GetDeviceModel(devices, "DOPPLER CURRENT METER");
            evm.Vms = GetDeviceModel(devices, "VMS");

            evm.OtherDevices.AddRange(
                from d in devices
                where !NAMED_DEVICES.Contains(d.DeviceType.Description)
                select new ElectronicsViewModel.DeviceModel(d.DeviceType.Description)
                {
                    Id = d.Id,
                    Installed = !d.IsInstalled.HasValue ? "N/A" : d.IsInstalled.Value ? "True" : "False",
                    Usage = d.Usage.HasValue ? d.Usage.ToString() : "N/A",
                    Make = d.Make,
                    Model = d.Model,
                    Comments = d.Comments
                }
            );

            return evm;
        }

        private static ElectronicsViewModel.DeviceModel GetDeviceModel(List<ElectronicDevice> devices, string deviceName)
        {
            return (
                from d in devices
                where deviceName.Equals(d.DeviceType.Category, StringComparison.InvariantCultureIgnoreCase)
                select new ElectronicsViewModel.DeviceModel(deviceName)
                {
                    Id = d.Id,
                    Installed = !d.IsInstalled.HasValue ? "N/A" : d.IsInstalled.Value ? "True" : "False",
                    Usage = d.Usage.HasValue ? d.Usage.ToString() : "N/A",
                    Make = d.Make,
                    Model = d.Model,
                    Comments = d.Comments
                }
            ).FirstOrDefault<ElectronicsViewModel.DeviceModel>() ?? new ElectronicsViewModel.DeviceModel(deviceName);
        }
        
        //
        // GET: /Electronics/
        public ActionResult List(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ElectronicsViewModel evm = GetViewModel(tripId);
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            ViewBag.Title = String.Format("Electronics for {0}", tripId.ToString());

            return View(evm);
        }

        public ActionResult Edit(Trip tripId)
        {
            if (null == tripId)
            {
                return new NoSuchTripResult();
            }

            ElectronicsViewModel evm = GetViewModel(tripId);
            ViewBag.TripNumber = tripId.SpcTripNumber ?? "This Trip";
            ViewBag.Title = String.Format("Edit Electronics for {0}", tripId.ToString());

            return View(evm);
        }

        [HttpPost]
        [Authorize(Roles = Security.EditRoles)]
        [OutputCache(NoStore = true, VaryByParam = "None", Duration = 0)]
        public PartialViewResult EditSingle(Trip tripId, ElectronicsViewModel.DeviceModel device)
        {
            if (null == tripId)
            {
                return PartialView(PathToDeviceModelEditorPartial, device);
            }

            // Convert the device to the DAL equivalent, save, and then reload ViewModel from DAL
            return PartialView(PathToDeviceModelEditorPartial, device);
        }

        

    }
}
