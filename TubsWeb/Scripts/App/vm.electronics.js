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
/// <reference name="../knockout.mapping-latest.debug.js" />
/// <reference name="../tubs-custom-bindings.js" />
/// <reference name="datacontext.js" />

// All the view models are in the tubs namespace
var tubs = tubs || {};

tubs.electronicsMapping = {
    'Buoys': {
        create: function (options) {
            return new tubs.ElectronicsBuoy(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    },
    'Vms': {
        create: function (options) {
            return new tubs.ElectronicsVms(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    },
    'OtherDevices': {
        create: function (options) {
            return new tubs.ElectronicsDevice(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
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
 * Ensure that incoming electronics viewmodel data has everything for correct
 * binding/rendering.
 * @param {data} data Electronics object model that may need missing properties.
 */
tubs.addElectronicsDefaults = function (data) {
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

    self.Id = ko.observable(data.Id || 0);
    self.DeviceType = ko.observable(data.DeviceType || '');
    self.IsInstalled = ko.observable(data.IsInstalled || '');
    self.Usage = ko.observable(data.Usage || '');
    self.Make = ko.observable(data.Make || '');
    self.Model = ko.observable(data.Model || '');
    self.Comments = ko.observable(data.Comments || '');
    self.BuoyCount = ko.observable(data.BuoyCount || null);
    self._destroy = ko.observable(data._destroy || false);
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);

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

    self.Id = ko.observable(data.Id || 0);
    self.DeviceType = ko.observable('VMS');
    self.IsInstalled = ko.observable(data.IsInstalled || '');
    self.Make = ko.observable(data.Make || '');
    self.Model = ko.observable(data.Model || '');
    self.Comments = ko.observable(data.Comments || '');
    self.SystemDescription = ko.observable(data.SystemDescription || '');
    self.SealsIntact = ko.observable(data.SealsIntact || '');
    self._destroy = ko.observable(data._destroy || false);
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);

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

    self.Id = ko.observable(data.Id || 0);
    self.DeviceType = ko.observable(data.DeviceType || '');
    self.Description = ko.observable(data.Description || '');
    self.IsInstalled = ko.observable(data.IsInstalled || '');
    self.Usage = ko.observable(data.Usage || '');
    self.Make = ko.observable(data.Make || '');
    self.Model = ko.observable(data.Model || '');
    self.Comments = ko.observable(data.Comments || '');
    self._destroy = ko.observable(data._destroy || false);
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);

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
    var self = this;

    ko.mapping.fromJS(tubs.addElectronicsDefaults(data), tubs.electronicsMapping, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Gps,
        self.TrackPlotter,
        self.DepthSounder,
        self.SstGauge,
        self.Buoys,
        self.Vms,
        self.OtherDevices,
        self.Communications,
        self.Info
    ], false);

    self.isDirty = ko.computed(function () {
        var hasDirtyChild = false;
        // Header first
        if (self.dirtyFlag().isDirty()) {
            return true;
        }

        // TODO:  There's probably a smarter way to do this
        // where we loop while !hasDirtyChild.  The loop source would be
        // self.Buoys(), self.Vms(), self.OtherDevices()

        hasDirtyChild = _.any(self.Buoys(), function (buoy) {
            return buoy.isDirty();
        });

        if (hasDirtyChild) {
            return true;
        }

        hasDirtyChild = _.any(self.Vms(), function (vms) {
            return vms.isDirty();
        });

        if (hasDirtyChild) {
            return true;
        }

        hasDirtyChild = _.any(self.OtherDevices(), function (device) {
            return device.isDirty();
        });

        return hasDirtyChild;
    });

    self.clearDirtyFlag = function () {
        // TODO: Would it be faster to check if buoy is dirty first?
        _.each(self.Buoys(), function (buoy) {
            buoy.dirtyFlag().reset();
        });

        _.each(self.Vms(), function (vms) {
            vms.dirtyFlag().reset();
        });

        _.each(self.OtherDevices(), function (device) {
            device.dirtyFlag().reset();
        });

        self.dirtyFlag().reset();
    };

    self.addBuoy = function () {
        self.Buoys.push(new tubs.ElectronicsBuoy({ NeedsFocus: true }));
    };

    self.removeBuoy = function (buoy) {
        if (buoy && buoy.Id()) {
            self.Buoys.destroy(buoy);
        } else {
            self.Buoys.remove(buoy);
        }
    };

    self.addVms = function () {
        self.Vms.push(new tubs.ElectronicsVms({ NeedsFocus: true }));
    };

    self.removeVms = function (vms) {
        if (vms && vms.Id()) {
            self.Vms.destroy(vms);
        } else {
            self.Vms.remove(vms);
        }
    };

    self.addOtherDevice = function () {
        self.OtherDevices.push(new tubs.ElectronicsDevice({ NeedsFocus: true }));
    };

    self.removeOtherDevice = function (device) {
        if (device && device.Id()) {
            self.OtherDevices.destroy(device);
        } else {
            self.OtherDevices.remove(device);
        }
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getElectronics(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(tubs.addElectronicsDefaults(result), tubs.electronicsMapping, self);
                    self.clearDirtyFlag();
                    toastr.success('Reloaded electronics data');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload electronics', xhr, status);
                }
            );
            complete();
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.saveElectronics(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(tubs.addElectronicsDefaults(result), tubs.electronicsMapping, self);
                    self.clearDirtyFlag();
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
            return !isExecuting && self.isDirty();
        }
    });

    return self;
};