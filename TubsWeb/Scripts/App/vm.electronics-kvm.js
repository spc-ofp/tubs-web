/** 
 * @file Knockout ViewModel for editing an PS-1/LL-1 electronics
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * knockout.js
 * underscore.js
 * knockout.viewmodel plugin
 */

/// <reference name="../underscore.js" />
/// <reference name="../knockout-2.1.0.debug.js" />
/// <reference name="../knockout.viewmodel.2.0.2.js" />
/// <reference name="datacontext.js" />

// All the view models are in the tubs namespace
var tubs = tubs || {};

/**
 * Knockout ViewModel options for mapping JavaScript object
 * to full view model.
 * The mapping is intended to:
 * Add a 'Remove' method to the FIXME observable array.
 * Add a KoLite dirty flag to all members of the FIXME observable array.
 * Track members of the FIXME observable array using their intrinsic 'Id' property.
 */
tubs.electronicsOptions = {
    extend: {
        "{root}.Buoys": "ExtendWithDestroy",
        "{root}.Buoys[i]": function (buoy) {
            buoy.dirtyFlag = new ko.DirtyFlag([
                buoy.DeviceType,
                buoy.IsInstalled,
                buoy.Usage,
                buoy.Make,
                buoy.Model,
                buoy.Comments,
                buoy.BuoyCount
            ], false);

            buoy.isDirty = ko.computed(function () {
                return buoy.dirtyFlag().isDirty();
            });
        },
        "{root}.Vms": "ExtendWithDestroy",
        "{root}.Vms[i]": function (vms) {
            vms.dirtyFlag = new ko.DirtyFlag([
                vms.IsInstalled,
                vms.Make,
                vms.Model,
                vms.SystemDescription,
                vms.SealsIntact,
                vms.Comments
            ], false);

            vms.isDirty = ko.computed(function () {
                return vms.dirtyFlag().isDirty();
            });
        },
        "{root}.OtherDevices": "ExtendWithDestroy",
        "{root}.OtherDevices[i]": function (device) {
            device.dirtyFlag = new ko.DirtyFlag([
                device.DeviceType,
                device.Description,
                device.IsInstalled,
                device.Usage,
                device.Make,
                device.Model,
                device.Comments
            ], false);

            device.isDirty = ko.computed(function () {
                return device.dirtyFlag().isDirty();
            });
        }
    },
    arrayChild: {
        "{root}.Buoys": "Id",
        "{root}.Vms": "Id",
        "{root}.OtherDevices": "Id"
    },
    shared: {
        /* Assumes item has an Id() observable to hold database PK value */
        ExtendWithDestroy: function (items) {
            items.Remove = function (item) {
                // If the item has an Id, mark for deletion
                if (item && item.Id && item.Id()) {
                    items.destroy(item);
                } else {
                    // If there is no Id, or it equals zero, remove it from the
                    // array so that it won't be sent to the server
                    items.remove(item);
                }
            };
        }
    }
};

/**
 * Serves as a default values for all category entities.
 */
tubs.electronicsCategoryDefaults = {
    Id: 0,
    IsInstalled: '',
    Usage: ''
};

/**
 * Serves as a default values for all buoy entities.
 * Also used in the construction of new observable buoy entities.
 */
tubs.electronicsBuoyDefaults = {
    Id: 0,
    DeviceType: '',
    IsInstalled: '',
    Usage: '',
    Make: '',
    Model: '',
    Comments: '',
    BuoyCount: null,
    _destroy: false,
    NeedsFocus: false
};

/**
 * Serves as a default values for all VMS entities.
 * Also used in the construction of new observable VMS entities.
 */
tubs.electronicsVmsDefaults = {
    Id: 0,
    DeviceType: 'Vms', // TODO Double check!
    IsInstalled: '',
    Usage: '',
    Make: '',
    Model: '',
    Comments: '',
    SystemDescription: '',
    SealsIntact: '',
    _destroy: false,
    NeedsFocus: false
};

/**
 * Serves as a default values for all other device entities.
 * Also used in the construction of new observable device entities.
 */
tubs.electronicsDeviceDefaults = {
    Id: 0,
    DeviceType: '',
    Description: '',
    IsInstalled: '',
    Usage: '',
    Make: '',
    Model: '',
    Comments: '',
    _destroy: false,
    NeedsFocus: false
};


/**
 * @param {data} data Electronics object model that may need missing properties.
 * 
 */
tubs.electronicsDefaults = function (data) {
    // The category objects use the .Name field to keep track of what
    // they are when they get back to the server.
    data.Gps = data.Gps || { Name: "GPS" };
    _.defaults(data.Gps, tubs.electronicsCategoryDefaults);

    data.TrackPlotter = data.TrackPlotter || { Name: "Track Plotter" };
    _.defaults(data.TrackPlotter, tubs.electronicsCategoryDefaults);

    data.DepthSounder = data.DepthSounder || { Name: "Depth Sounder" };
    _.defaults(data.DepthSounder, tubs.electronicsCategoryDefaults);

    data.SstGauge = data.SstGauge || { Name: "SST Gauge" };
    _.defaults(data.SstGauge, tubs.electronicsCategoryDefaults);

    data.Buoys = data.Buoys || [];
    data.Vms = data.Vms || [];
    data.OtherDevices = data.OtherDevices || [];

    return data;
};

/**
 * Electronics with associated buoys
 * @constructor
 * @param {object} data - Entity data
 */
tubs.ElectronicsBuoy = function (data) {
    'use strict';
    var self = this;
    data = data || {};
    _.defaults(data, tubs.electronicsBuoyDefaults);

    self.Id = ko.observable(data.Id);
    self.DeviceType = ko.observable(data.DeviceType);
    self.IsInstalled = ko.observable(data.IsInstalled);
    self.Usage = ko.observable(data.Usage);
    self.Make = ko.observable(data.Make);
    self.Model = ko.observable(data.Model);
    self.Comments = ko.observable(data.Comments);
    self.BuoyCount = ko.observable(data.BuoyCount);
    self._destroy = ko.observable(data._destroy);
    self.NeedsFocus = ko.observable(data.NeedsFocus);

    self.dirtyFlag = new ko.DirtyFlag([
        self.DeviceType,
        self.IsInstalled,
        self.Usage,
        self.Make,
        self.Model,
        self.Comments,
        self.BuoyCount
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

/**
 * VMS electronics
 * @constructor
 * @param {object} data - Entity data
 */
tubs.ElectronicsVms = function (data) {
    'use strict';
    var self = this;
    data = data || {};
    _.defaults(data, tubs.electronicsVmsDefaults);

    self.Id = ko.observable(data.Id);
    self.IsInstalled = ko.observable(data.IsInstalled);
    self.Usage = ko.observable(data.Usage);
    self.Make = ko.observable(data.Make);
    self.Model = ko.observable(data.Model);
    self.Comments = ko.observable(data.Comments);
    self.SystemDescription = ko.observable(data.SystemDescription);
    self.SealsIntact = ko.observable(data.SealsIntact);
    self._destroy = ko.observable(data._destroy);
    self.NeedsFocus = ko.observable(data.NeedsFocus);

    self.dirtyFlag = new ko.DirtyFlag([
        self.IsInstalled,
        self.Make,
        self.Model,
        self.SystemDescription,
        self.SealsIntact,
        self.Comments
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

/**
 * Other electronics
 * @constructor
 * @param {object} data - Entity data
 */
tubs.ElectronicsDevice = function (data) {
    'use strict';
    var self = this;
    data = data || {};
    _.defaults(data, tubs.electronicsDeviceDefaults);

    self.Id = ko.observable(data.Id);
    self.DeviceType = ko.observable(data.DeviceType);
    self.Description = ko.observable(data.Description);
    self.IsInstalled = ko.observable(data.IsInstalled);
    self.Usage = ko.observable(data.Usage);
    self.Make = ko.observable(data.Make);
    self.Model = ko.observable(data.Model);
    self.Comments = ko.observable(data.Comments);
    self._destroy = ko.observable(data._destroy);
    self.NeedsFocus = ko.observable(data.NeedsFocus);

    self.dirtyFlag = new ko.DirtyFlag([
        self.DeviceType,
        self.Description,
        self.IsInstalled,
        self.Usage,
        self.Make,
        self.Model,
        self.Comments
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

tubs.Electronics = function (data) {
    'use strict';

    var vm = ko.viewmodel.fromModel(
        tubs.electronicsDefaults(data),
        tubs.electronicsOptions
    );

    vm.dirtyFlag = new ko.DirtyFlag([
        vm.Gps,
        vm.TrackPlotter,
        vm.DepthSounder,
        vm.SstGauge,
        vm.Buoys,
        vm.Vms,
        vm.OtherDevices,
        vm.Communications,
        vm.Info
    ], false);

    vm.isDirty = ko.computed(function () {
        var hasDirtyChild = false;
        // Header first
        if (vm.dirtyFlag().isDirty()) {
            return true;
        }

        // TODO:  There's probably a smarter way to do this
        // where we loop while !hasDirtyChild.  The loop source would be
        // vm.Buoys(), vm.Vms(), vm.OtherDevices()

        hasDirtyChild = _.any(vm.Buoys(), function (buoy) {
            return buoy.isDirty();
        });

        if (hasDirtyChild) {
            return true;
        }

        hasDirtyChild = _.any(vm.Vms(), function (vms) {
            return vms.isDirty();
        });

        if (hasDirtyChild) {
            return true;
        }

        hasDirtyChild = _.any(vm.OtherDevices(), function (device) {
            return device.isDirty();
        });

        if (hasDirtyChild) {
            return true;
        }
    });

    vm.clearDirtyFlag = function () {
        // TODO: Would it be faster to check if buoy is dirty first?
        _.each(vm.Buoys(), function (buoy) {
            buoy.dirtyFlag().reset();
        });

        _.each(vm.Vms(), function (vms) {
            vms.dirtyFlag().reset();
        });

        _.each(vm.OtherDevices(), function (device) {
            device.dirtyFlag().reset();
        });

        vm.dirtyFlag().reset();
    };

    vm.addBuoy = function () {
        vm.Buoys.push(new tubs.ElectronicsBuoy({ NeedsFocus: true}));
    };

    vm.removeBuoy = function (buoy) {
        vm.Buoys.Remove(buoy);
    };

    vm.addVms = function () {
        vm.Vms.push(new tubs.ElectronicsVms({ NeedsFocus: true }));
    };

    vm.removeVms = function (vms) {
        vm.Vms.Remove(vms);
    };

    vm.addOtherDevice = function () {
        vm.OtherDevices.push(new tubs.ElectronicsDevice({ NeedsFocus: true }));
    };

    vm.removeOtherDevice = function (device) {
        vm.OtherDevices.Remove(device);
    };

    vm.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getElectronics(
                vm.TripId(),
                function (result) {
                    ko.viewmodel.updateFromModel(vm, tubs.electronicsDefaults(result));
                    vm.clearDirtyFlag();
                    toastr.success('Reloaded electronics data');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload electronics', xhr, status);
                }
            );
            complete();
        },
        canExecute: function (isExecuting) {
            return !isExecuting && vm.isDirty();
        }
    });

    vm.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.saveElectronics(
                vm.TripId(),
                vm,
                function (result) {
                    ko.viewmodel.updateFromModel(vm, tubs.electronicsDefaults(result));
                    vm.clearDirtyFlag();
                    toastr.success('Saved electronics data');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save electronics', xhr, status);
                }
            );
            complete();
        },
        canExecute: function (isExecuting) {
            // TODO isValid check
            return !isExecuting && vm.isDirty();
        }
    });

    return vm;
};