/*
* vm.fishingset.js
* Knockout.js ViewModel for editing a PS-3
* Depends on:
* jquery
* json2
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

tubs.psSetMapping = {};

tubs.psSetCatch = function (catchData) {
    var self = this;
}

// This is the actual Purse Seine Sea Day view model
// Any functions/properties/etc. that belong on the view model
// are defined here.
tubs.psSet = function (data) {
    var self = this;
    // Map the incoming JSON in 'data' to self, using
    // the options in psSetMapping
    ko.mapping.fromJS(data, tubs.psSetMapping, self);

    // TODO Add fields
    self.dirtyFlag = new ko.DirtyFlag([], false, tubs.hashFunction);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        // Check each child, bailing on the first
        // dirty child.
        var hasDirtyChild = false;
        /*
        $.each(self.Events(), function (i, evt) {
        if (evt.isDirty()) {
        hasDirtyChild = true;
        return false;
        }
        });
        */
        return hasDirtyChild;
    });

    // Clear the dirty flag for the this entity, plus all the
    // child entities stored in the Events observableArray
    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
        //$.each(self.Events(), function (index, value) {
        //    value.dirtyFlag().reset();
        //});
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            complete();
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            complete();
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    return self;
}