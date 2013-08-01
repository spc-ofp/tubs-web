/** 
 * @file Knockout ViewModel for the number of forms in this workbook
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
 * Knockout mapping for page counts
 */
tubs.PageCountMapping = {
    'PageCounts': {
        create: function (options) {
            return new tubs.PageCount(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    }
};

/**
 * Page count for a single form type.
 * @constructor
 * @param {object} data
 */
tubs.PageCount = function (data) {
    'use strict';
    var self = this;
    self.Id = ko.observable(data.Id || 0);
    self.Key = ko.observable(data.Key || null);
    self.Value = ko.observable(data.Value || 0);
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Key,
        self.Value
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
 * View model for all page counts in a trip.
 * @constructor
 * @param {object} data
 */
tubs.PageCountViewModel = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, tubs.PageCountMapping, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.PageCounts // Add/Remove only
    ], false);

    self.isDirty = ko.computed(function () {
        if (self.dirtyFlag().isDirty()) {
            return true;
        }

        var hasDirtyChild = false;
        ko.utils.arrayForEach(self.PageCounts(), function (item) {
            if (item.isDirty()) {
                hasDirtyChild = true;
            }
        });
        return hasDirtyChild;
    });

    self.clearDirtyFlag = function () {
        _.each(self.PageCounts(), function (item) {
            item.clearDirtyFlag();
        });
        self.dirtyFlag().reset();
    };

    self.addPageCount = function () {
        self.PageCounts.push(new tubs.PageCount({ "NeedsFocus": true }));
    };

    self.removePageCount = function (pageCount) {
        if (pageCount && pageCount.Id()) {
            self.PageCounts.destroy(pageCount);
        } else {
            self.PageCounts.remove(pageCount);
        }
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getPageCounts(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.PageCountMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded page counts');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload page counts', xhr, status);
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
            tubs.savePageCounts(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.PageCountMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved page counts');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save page counts', xhr, status);
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