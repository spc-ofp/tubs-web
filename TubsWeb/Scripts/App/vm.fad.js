/*
 * vm.fad.js
 * Knockout.js ViewModel for editing a single GEN-5 record
 * Depends on:
 * jquery
 * knockout
 * knockout.mapping (automatically maps JSON)
 * knockout.asyncCommand (makes it easier to show user activity)
 * knockout.dirtyFlag (avoid unneccesary saves)
 * knockout.activity (fancy UI gadget)
 * amplify (local storage and Ajax mapping
 * toastr (user notification)
 * knockout.custom-bindings (date binding)
 */

// All the view models are in the tubs namespace
var tubs = tubs || {};

// TODO:  Extend Lat/Lon with appropriate regex
tubs.FadMapping = {
    'DeploymentDate': {
        create: function (options) {
            return ko.observable(options.data).extend({ isoDate: 'DD/MM/YY' });
        }
    },
    'MainMaterials': {
        create: function (options) {
            return new tubs.FadMaterial(options.data);
        }
    },
    'Attachments': {
        create: function (options) {
            return new tubs.FadMaterial(options.data);
        }
    }
};

tubs.FadMaterial = function (data) {
    'use strict';
    var self = this;
    self.Id = ko.observable(data.Id || 0);
    self.MaterialCode = ko.observable(data.MaterialCode || null);
    // This is used to set focus on the most recently added FadMaterial
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.MaterialCode
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

tubs.FadViewModel = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, tubs.FadMapping, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.ObjectNumber,
        self.OriginCode,
        self.DeploymentDate,
        self.Latitude,
        self.Longitude,
        self.SsiTrapped,
        self.AsFoundCode,
        self.AsLeftCode,
        self.Depth,
        self.Length,
        self.Width,
        self.BuoyNumber,
        self.Markings,
        self.Comments,
        self.MainMaterials, // Add/Remove only
        self.Attachments    // Add/Remove only
    ], false);

    self.isDirty = ko.computed(function () {
        var hasDirtyChild = false;
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }

        hasDirtyChild = _.any(self.MainMaterials(), function (material) {
            return material.isDirty();
        });

        if (hasDirtyChild) {
            return hasDirtyChild;
        }

        hasDirtyChild = _.any(self.Attachments(), function (attach) {
            return attach.isDirty();
        });

        return hasDirtyChild;
    });

    // Clear the dirty flag for the this entity, plus all the
    // child entities stored in the MainMaterials and Attachments observableArrays
    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();

        _.each(self.MainMaterials(), function (material) {
            material.dirtyFlag().reset();
        });

        _.each(self.Attachments(), function (attach) {
            attach.dirtyFlag().reset();
        });
    };

    // Operations
    self.addMainMaterial = function () {
        self.MainMaterials.push(new tubs.FadMaterial({ "NeedsFocus": true }));
    };

    self.addAttachment = function () {
        self.Attachments.push(new tubs.FadMaterial({ "NeedsFocus": true }));
    };

    self.removeMainMaterial = function (fm) {
        if (fm && fm.Id()) {
            self.MainMaterials.destroy(fm);
        } else {
            self.MainMaterials.remove(fm);
        }
    };

    self.removeAttachment = function (fm) {
        if (fm && fm.Id()) {
            self.Attachments.destroy(fm);
        } else {
            self.Attachments.remove(fm);
        }
    };

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getFad(
                self.TripId(),
                self.Id(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.FadMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded FAD details');
                    complete();
                },
                function (xhr, status, error) {
                    tubs.notify('Failed to reload FAD details', xhr, status);
                    complete();
                }
            );
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    // tubs.saveFad = function (tripId, fad, success_cb, error_cb)
    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.saveFad(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.FadMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved FAD details');
                    complete();
                },
                function (xhr, status, error) {
                    tubs.notify('Failed to save FAD details', xhr, status);
                    complete();
                }
            );
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    return self;
};