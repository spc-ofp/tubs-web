/*
 * vm.pagecount.js
 * Knockout.js ViewModel for editing the number of forms in this trip
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
"use strict";

tubs.PageCountViewModel = function (data) {
    var self = this;
    ko.mapping.fromJS(data, {}, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Ps1Count,
        self.Ps2Count,
        self.Ps3Count,
        self.Ps4Count,
        self.Ps5Count,
        self.Gen1Count,
        self.Gen2Count,
        self.Gen3Count,
        self.Gen4Count,
        self.Gen5Count,
        self.Gen6Count,
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    // Clear the dirty flag for the this entity
    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getPageCounts(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded page counts');
                    complete();
                },
                function (xhr, status, error) {
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
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved page counts');
                    complete();
                },
                function (xhr, status, error) {
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