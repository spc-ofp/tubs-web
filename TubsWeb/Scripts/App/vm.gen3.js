/** 
 * @file Knockout ViewModel for editing a GEN-3 form
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * knockout.js
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
 * Knockout mapping for GEN-3 form data.
 * WARNING:  The v2007 workbook, with it's use of "0" as a key
 * for all the incidents, breaks when using a "key" function
 * in knockout.mapping.
 */
tubs.gen3Mapping = {
    'Notes': {
        create: function (options) {
            return new tubs.Gen3Note(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    },
    'Incidents': {
        create: function (options) {
            return new tubs.Gen3Incident(options.data);
        }
    }
};

/**
 * A single GEN-3 incident (recorded as a yes/no and general code)
 * @constructor
 * @param {object} incidentData - Incident data
 */
tubs.Gen3Incident = function (incidentData) {
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

/**
 * A single GEN-3 note.
 * @constructor
 * @param {object} noteData - Note data
 */
tubs.Gen3Note = function (noteData) {
    'use strict';
    var self = this;
    self.Id = ko.observable(noteData.Id || 0);
    self.Date = ko.observable(noteData.Date || null).extend(tubs.dateExtension);
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

/**
 * View model for all GEN-3 incidents and notes for a single trip.
 * @constructor
 * @param {object} data - GEN-3 data
 */
tubs.Gen3ViewModel = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, tubs.gen3Mapping, self);

    self.clear = function () {
        if (self.Notes) {
            self.Notes([]);
        }
        if (self.Incidents) {
            self.Incidents([]);
        }
    };

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

    // TODO Replace with underscore
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
        self.Notes.push(new tubs.Gen3Note({ "NeedsFocus": true }));
    };

    self.removeNote = function (evt) {
        if (evt && evt.Id()) {
            self.Notes.destroy(evt);
        } else {
            self.Notes.remove(evt);
        }
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getGen3(
                self.TripId(),
                function (result) {
                    // There's a problem here with mapping back into the
                    // array.  It needs fixing, but we can ignore for the
                    // time being.
                    //self.clear();
                    ko.mapping.fromJS(result, tubs.gen3Mapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded GEN-3');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload GEN-3', xhr, status);
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
            tubs.saveGen3(
                self.TripId(),
                self,
                function (result) {
                    // There's a problem here with mapping back into the
                    // array.  It needs fixing, but we can ignore for the
                    // time being.
                    //self.clear();
                    ko.mapping.fromJS(result, tubs.gen3Mapping, self);
                    self.clearDirtyFlag();
                    toastr.success('GEN-3 saved');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save GEN-3', xhr, status);
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
