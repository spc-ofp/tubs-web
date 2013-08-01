/** 
 * @file Knockout ViewModel for PS-1 well content
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

/// <reference name="../underscore.js" />
/// <reference name="../knockout-2.3.0.debug.js" />
/// <reference path="../knockout.mapping-latest.js" />
/// <reference path="datacontext.js" />

/**
 * @namespace All view models are in the tubs namespace.
 */
var tubs = tubs || {};

/**
 * Knockout mapping for all well content.
 */
tubs.WellContentMapping = {
    'WellContentItems': {
        create: function (options) {
            return new tubs.WellContent(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    }
};

/**
 * Content for a single well.
 * @constructor
 * @parameter {object} data - Well content data
 */
tubs.WellContent = function (data) {
    'use strict';
    var self = this;
    self.Id = ko.observable(data.Id || 0);
    self.Comment = ko.observable(data.Comment || '');
    self.Content = ko.observable(data.Content || '');
    self.Capacity = ko.observable(data.Capacity || null);
    self.Location = ko.observable(data.Location || '');
    self.WellNumber = ko.observable(data.WellNumber || 0);
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);
    self._destroy = ko.observable(data._destroy || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Comment,
        self.Content,
        self.Capacity,
        self.Location,
        self.WellNumber
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    return self;
};

/**
 * View model representing all well content recorded on PS-1 form.
 * @constructor
 * @parameter {object} data - Well content data
 */
tubs.WellContentViewModel = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, tubs.WellContentMapping, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.WellContentItems // Add/Remove only
    ], false);

    self.isDirty = ko.computed(function () {
        // Add/Remove from WellContents
        if (self.dirtyFlag().isDirty()) {
            return true;
        }
        // Iterate over WellContentItems
        var hasDirtyChild = false;
        ko.utils.arrayForEach(self.WellContentItems(), function (item) {
            if (item.isDirty()) {
                hasDirtyChild = true;
            }
        });
        return hasDirtyChild;
    });

    self.clearDirtyFlag = function () {
        ko.utils.arrayForEach(self.WellContentItems(), function (item) {
            item.clearDirtyFlag();
        });
        self.dirtyFlag().reset();
    };

    self.addWellContent = function () {
        self.WellContentItems.push(new tubs.WellContent({ "NeedsFocus": true }));
    };

    self.removeWellContent = function (wellContent) {
        if (wellContent && wellContent.Id()) {
            self.WellContentItems.destroy(wellContent);
        } else {
            self.WellContentItems.remove(wellContent);
        }
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getWellContent(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.WellContentMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded well contents');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload well contents', xhr, status);
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
            tubs.saveWellContent(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.WellContentMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved well contents');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save well contents', xhr, status);
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