/** 
 * @file Knockout ViewModel for editing GEN-2 data
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
 * Basic special species interaction object.
 * @constructor
 * @param {object} data - Shared data for all interaction types.
 */
tubs.Gen2Event = function (data) {
    'use strict';
    var self = this;

    // Set common properties
    self.Id = ko.observable(data.Id || 0);
    self.TripId = ko.observable(data.TripId);
    self.PageNumber = ko.observable(data.PageNumber || 0);
    self.ShipsDate = ko.observable(data.ShipsDate || '').extend(tubs.dateExtension);
    self.ShipsTime = ko.observable(data.ShipsTime || '').extend(tubs.timeExtension);
    self.Latitude = ko.observable(data.Latitude || '').extend(tubs.latitudeExtension);
    self.Longitude = ko.observable(data.Longitude || '').extend(tubs.longitudeExtension);
    self.SpeciesCode = ko.observable(data.SpeciesCode || '');
    self.SpeciesDescription = ko.observable(data.SpeciesDescription || '');
    self.ActionName = ko.observable(data.ActionName || 'add');
    self.InteractionType = ko.observable(data.InteractionType || '');

    self.addPattern = /add/i;

    self.isAdd = ko.computed(function () {
        return self.addPattern.test(self.ActionName());
    });

    // Only show the "Next Event" button for any Edit
    // or an Add that has been stored in the database
    self.showNextEventButton = ko.computed(function () {
        return !self.isAdd() || (self.Id() !== 0);
    });

    return self;
};

/**
 * Interaction in which the species in question is landed on deck.
 * @constructor
 * @param {object} data - Landing data
 */
tubs.Gen2LandedEvent = function (data) {
    var self = this;

    ko.utils.extend(self, new tubs.Gen2Event(data));
    ko.mapping.fromJS(data, {}, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.ShipsDate,
        self.ShipsTime,
        self.Latitude,
        self.Longitude,
        self.SpeciesCode,
        self.SpeciesDescription,
        self.LandedConditionCode,
        self.LandedConditionDescription,
        self.LandedHandling,
        self.LandedLength,
        self.LandedLengthCode,
        self.LandedSexCode,
        self.DiscardedConditionCode,
        self.DiscardedConditionDescription,
        self.RetrievedTagNumber,
        self.RetrievedTagType,
        self.RetrievedTagOrganization,
        self.PlacedTagNumber,
        self.PlacedTagType,
        self.PlacedTagOrganization,
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getInteraction(
                self.TripId(),
                self.PageNumber(),
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded GEN-2 event');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload GEN-2 event', xhr, status);
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
            tubs.saveInteraction(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.success('Saved GEN-2');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save GEN-2', xhr, status);
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

/**
 * Knockout mapping for interaction in which the species
 * in question interacts with the gear.
 */
tubs.Gen2GearMapping = {
    'StartOfInteraction': {
        create: function (options) {
            return new tubs.Gen2SpeciesGroup(options.data);
        }
    },
    'EndOfInteraction': {
        create: function (options) {
            return new tubs.Gen2SpeciesGroup(options.data);
        }
    }
};

/**
 * Gear interactions can have interactions with multiple animals.
 * Observers are instructed to group the animals together based on
 * any commonality.
 * @constructor
 * @param {object} data - Species group data
 */
tubs.Gen2SpeciesGroup = function (data) {
    var self = this;
    self.Id = ko.observable(data.Id || 0);
    self.Count = ko.observable(data.Count || null);
    self.ConditionCode = ko.observable(data.ConditionCode || '');
    self.Description = ko.observable(data.Description || '');
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);
    self._destroy = ko.observable(data._destroy || false); //ignore jslint

    self.dirtyFlag = new ko.DirtyFlag([
        self.Count,
        self.ConditionCode,
        self.Description,
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
 * Special species interaction during which the animal(s)
 * interact with the vessel or gear.
 * @constructor
 * @param {object} data - Gear interaction data
 */
tubs.Gen2GearEvent = function (data) {
    var self = this;

    ko.utils.extend(self, new tubs.Gen2Event(data));
    ko.mapping.fromJS(data, tubs.Gen2GearMapping, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.ShipsDate,
        self.ShipsTime,
        self.Latitude,
        self.Longitude,
        self.SpeciesCode,
        self.SpeciesDescription,
        self.VesselActivity,
        self.VesselActivityDescription,
        self.StartOfInteraction, // Add/Remove only
        self.EndOfInteraction, // Add/Remove only
        self.InteractionDescription,
    ], false);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        // Check each child event, bailing on the first
        // dirty child.
        var hasDirtyChild = false;
        $.each(self.StartOfInteraction(), function (i, evt) { //ignore jslint
            if (evt.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        if (hasDirtyChild) { return true; }
        $.each(self.EndOfInteraction(), function (i, evt) { //ignore jslint
            if (evt.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        return hasDirtyChild;
    });

    self.clearDirtyFlag = function () {
        $.each(self.StartOfInteraction(), function (index, value) { //ignore jslint
            value.dirtyFlag().reset();
        });
        $.each(self.EndOfInteraction(), function (index, value) { //ignore jslint
            value.dirtyFlag().reset();
        });
        self.dirtyFlag().reset();
    };

    self.addStartItem = function () {
        self.StartOfInteraction.push(new tubs.Gen2SpeciesGroup({ "NeedsFocus": true }));
    };

    self.removeStartItem = function (evt) {
        if (evt && evt.Id()) {
            self.StartOfInteraction.destroy(evt);
        } else {
            self.StartOfInteraction.remove(evt);
        }
    };

    self.addEndItem = function () {
        self.EndOfInteraction.push(new tubs.Gen2SpeciesGroup({ "NeedsFocus": true }));
    };

    self.removeEndItem = function (evt) {
        if (evt && evt.Id()) {
            self.EndOfInteraction.destroy(evt);
        } else {
            self.EndOfInteraction.remove(evt);
        }
    };

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getInteraction(
                self.TripId(),
                self.PageNumber(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.Gen2GearMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded GEN-2 event');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload GEN-2 event', xhr, status);
                    complete();
                }
            );
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.saveInteraction(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.Gen2GearMapping, self);
                    self.clearDirtyFlag();
                    toastr.success('Saved GEN-2');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to save GEN-2', xhr, status);
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

/**
 * Special species interaction during which the animal(s)
 * is only sighted.
 * @constructor
 * @param {object} data - Sighting data
 */
tubs.GenSightedEvent = function (data) {
    var self = this;
    ko.utils.extend(self, new tubs.Gen2Event(data));
    ko.mapping.fromJS(data, {}, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.ShipsDate,
        self.ShipsTime,
        self.Latitude,
        self.Longitude,
        self.SpeciesCode,
        self.SpeciesDescription,
        self.VesselActivity,
        self.VesselActivityDescription,
        self.NumberSighted,
        self.NumberOfAdults,
        self.NumberOfJuveniles,
        self.SightingLength,
        self.SightingDistance,
        self.SightingBehavior,
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getInteraction(
                self.TripId(),
                self.PageNumber(),
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded GEN-2 event');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload GEN-2 event', xhr, status);
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
            tubs.saveInteraction(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.success('Saved GEN-2');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save GEN-2', xhr, status);
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