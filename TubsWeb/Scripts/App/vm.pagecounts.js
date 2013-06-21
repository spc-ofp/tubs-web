/*
 * vm.pagecount.js
 * Knockout.js ViewModel for editing the number of forms in this trip
 * Depends on:
 * jquery
 * knockout
 * knockout.mapping (automatically maps JSON)
 * knockout.command (makes it easier to show user activity)
 * knockout.dirtyFlag (avoid unneccesary saves)
 * knockout.activity (fancy UI gadget)
 * amplify (local storage and Ajax mapping
 * toastr (user notification)
 */

// All the view models are in the tubs namespace
var tubs = tubs || {};

tubs.PageCountMapping = {
    'PageCounts': {
        create: function (options) {
            return new tubs.PageCount(options.data);
        }
    }
};

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

tubs.PageCountViewModel = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, tubs.PageCountMapping, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.PageCounts // Add/Remove only
    ], false);

    self.isDirty = ko.computed(function () {
        // Add/Remove from PageCounts
        if (self.dirtyFlag().isDirty()) {
            return true;
        }
        // Iterate over PageCounts
        var hasDirtyChild = false;
        ko.utils.arrayForEach(self.PageCounts(), function (item) {
            if (item.isDirty()) {
                hasDirtyChild = true;
            }
        });
        return hasDirtyChild;
    });

    // Clear the dirty flag for the this entity
    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
        _.each(self.PageCounts(), function (item) {
            item.clearDirtyFlag();
        });
        //ko.utils.arrayForEach(self.PageCounts(), function (item) {
        //    item.clearDirtyFlag();
        //});
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

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getPageCounts(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.PageCountMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded page counts');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload page counts', xhr, status);
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
            tubs.savePageCounts(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.PageCountMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved page counts');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to save page counts', xhr, status);
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