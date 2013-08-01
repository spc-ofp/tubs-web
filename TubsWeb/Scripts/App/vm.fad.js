/** 
 * @file Knockout ViewModel for a single GEN-5 (floating object) record.
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * knockout.js
 * underscore.js
 * knockout.mapping plugin
 * KoLite plugins (asyncCommand, activity, dirtyFlag)
 * toastr
 */

/// <reference path="../knockout-2.3.0.debug.js" />
/// <reference path="../knockout.mapping-latest.js" />
/// <reference path="../tubs-common-extensions.js" />
/// <reference path="datacontext.js" />

/**
 * @namespace All view models are in the tubs namespace.
 */
var tubs = tubs || {};

/**
 * Knockout mapping for a FAD record.
 * DeploymentDate uses the ISODate extension
 * Latitude is extended with a custom latitude validator
 * Longitude is extended with a custom longitude validator
 */
tubs.FadMapping = {
    'DeploymentDate': {
        create: function (options) {
            return ko.observable(options.data).extend(tubs.dateExtension);
        }
    },
    'Latitude' : {
        create: function (options) {
            return ko.observable(options.data).extend(tubs.latitudeExtension);
        }
    },
    'Longitude': {
        create: function (options) {
            return ko.observable(options.data).extend(tubs.longitudeExtension);
        }
    },
    'MainMaterials': {
        create: function (options) {
            return new tubs.FadMaterial(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    },
    'Attachments': {
        create: function (options) {
            return new tubs.FadMaterial(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    }
};

/**
 * A single material item.  This could either be a main material
 * or an attachment.
 * @constructor
 * @param {object} data - FAD material data
 */
tubs.FadMaterial = function (data) {
    'use strict';
    var self = this;
    self.Id = ko.observable(data.Id || 0);
    self.MaterialCode = ko.observable(data.MaterialCode || null);
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.MaterialCode
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

/**
 * View model for the floating object record.
 * @constructor
 * @param {object} data - FAD data
 */
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
        if (self.dirtyFlag().isDirty()) {
            return true;
        }

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
        _.each(self.MainMaterials(), function (material) {
            material.dirtyFlag().reset();
        });

        _.each(self.Attachments(), function (attach) {
            attach.dirtyFlag().reset();
        });

        self.dirtyFlag().reset();
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

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getFad(
                self.TripId(),
                self.Id(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.FadMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded FAD details');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload FAD details', xhr, status);
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
            tubs.saveFad(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.FadMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved FAD details');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save FAD details', xhr, status);
                }
            );
            complete();
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    return self;
};