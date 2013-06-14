/*
 * vm.gen3.js
 * Knockout.js ViewModel for editing a GEN-3
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
/// <reference path="../knockout-2.1.0.debug.js"  />

// All the view models are in the tubs namespace
var tubs = tubs || {};

tubs.gen3Mapping = {
    'Notes': {
        create: function (options) {
            return new tubs.gen3Note(options.data);
        }
    },
    'Incidents': {
        create: function (options) {
            return new tubs.gen3Incident(options.data);
        }
    }
};


tubs.gen3Incident = function (incidentData) {
    'use strict';
    var self = this;
    self.Id = ko.observable(incidentData.Id || 0);
    self.QuestionCode = ko.observable(incidentData.QuestionCode);
    self.Answer = ko.observable(incidentData.Answer);
    self.JournalPage = ko.observable(incidentData.JournalPage);

    self.dirtyFlag = new ko.DirtyFlag([
        self.QuestionCode,
        self.Answer,
        self.JournalPage
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    return self;
};

tubs.gen3Note = function (noteData) {
    'use strict';
    var self = this;
    self.Id = ko.observable(noteData.Id || 0);
    self.Date = ko.observable(noteData.Date || null).extend({ isoDate: 'DD/MM/YY' });
    self.Comments = ko.observable(noteData.Comments || null);
    self._destroy = ko.observable(noteData._destroy || false); //ignore jslint
    self.NeedsFocus = ko.observable(noteData.NeedsFocus || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Date,
        self.Comments
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    return self;
};

tubs.gen3 = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, tubs.gen3Mapping, self);

    // TODO:  Adding Incidents to DirtyFlag allows save
    // at the cost of a false positive
    self.dirtyFlag = new ko.DirtyFlag([
        self.Incidents,
        self.Notes // Add/Remove only
    ], false);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        // Check each child, bailing on the first
        // dirty child.
        var hasDirtyChild = false;
        ko.utils.arrayForEach(self.Incidents(), function (item) {
            if (item.isDirty()) {
                hasDirtyChild = true;
            }
        });
        if (hasDirtyChild) { return hasDirtyChild; }

        ko.utils.arrayForEach(self.Notes(), function (item) {
            if (item.isDirty()) {
                hasDirtyChild = true;
            }
        });
        return hasDirtyChild;
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
        $.each(self.Incidents(), function (index, value) { //ignore jslint
            value.clearDirtyFlag();
        });
        $.each(self.Notes(), function (index, value) { //ignore jslint
            value.clearDirtyFlag();
        });
    };

    self.addNote = function () {
        self.Notes.push(new tubs.gen3Note({ "NeedsFocus": true }));
    };

    // This function takes advantage of the RoR integration
    // in Knockout.  A call to .destroy(...) sets the "_destroy" property
    // to true.  Knockout then ignores the entity in the 'foreach' binding.
    // This removes the requirement to manage deleted Ids on the client and
    // the server.
    // NOTE:  We're not using destroy on events that have an Id of zero.
    // If the user had added a row and then realized they didn't need it,
    // we wouldn't want to ship garbage data back to the server and complicate
    // the validation.
    self.removeNote = function (evt) {
        if (evt && evt.Id()) { self.Notes.destroy(evt); }
        else { self.Notes.remove(evt); }
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getGen3(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.gen3Mapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded GEN-3');
                    complete();
                },
                function (xhr, status, error) {
                    tubs.notify('Failed to reload GEN-3', xhr, status);
                    complete();
                });
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.saveGen3(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.gen3Mapping, self);
                    self.clearDirtyFlag();
                    toastr.success('GEN-3 saved');
                    complete();
                },
                function (xhr, status, error) {
                    tubs.notify('Failed to save GEN-3', xhr, status);
                    complete();
                });
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    return self;
};
