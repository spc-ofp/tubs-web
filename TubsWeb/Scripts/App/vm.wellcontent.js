/*
 * vm.wellcontent.js
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

tubs.WellContentMapping = {
    'WellContentItems': {
        create: function (options) {
            return new tubs.WellContent(options.data);
        }
    }
};

tubs.WellContent = function (data) {
    'use strict';
    var self = this;
    self.Id = ko.observable(data.Id || 0);
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);
    self._destroy = ko.observable(data._destroy || false);
    self.Comment = ko.observable(data.Comment || '');
    self.Content = ko.observable(data.Content || '');
    self.Capacity = ko.observable(data.Capacity || null);
    self.Location = ko.observable(data.Location || '');
    self.WellNumber = ko.observable(data.WellNumber || 0);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Comment,
        self.Content,
        self.Capacity,
        self.Location,
        self.WellNumber,
        self.Id
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    return self;
};

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

    // Clear the dirty flag for the this entity
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

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getWellContent(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.WellContentMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded well contents');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload well contents', xhr, status);
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
            tubs.saveWellContent(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.WellContentMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved well contents');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to save well contents', xhr, status);
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