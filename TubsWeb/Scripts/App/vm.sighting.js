/** 
 * @file Knockout ViewModel for editing sightings of other vessels
 * during a trip.
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
/// <reference path="../tubs-common-extensions.js" />
/// <reference path="datacontext.js" />

/**
 * @namespace All view models are in the tubs namespace.
 */
var tubs = tubs || {};

/**
 * Knockout mapping for collection of sightings.
 */
tubs.sightingMapping = {
    'Sightings': {
        create: function (options) {
            return new tubs.VesselSighting(options.data);
        }
    }
};

/**
 * A sighting of a single vessel as recorded on a GEN-1 form.
 * @constructor
 * @param {object} eventData - Sighting data
 */
tubs.VesselSighting = function (eventData) {
    'use strict';
    var self = this;
    self.Id = ko.observable(eventData.Id || 0);
    self.DateOnly =
        ko.observable(eventData.DateOnly || null).extend(tubs.dateExtension);
    self.TimeOnly =
        ko.observable(eventData.TimeOnly || '').extend(tubs.timeExtension);
    self.Latitude =
        ko.observable(eventData.Latitude || '').extend(tubs.latitudeExtension);
    self.Longitude =
        ko.observable(eventData.Longitude || '').extend(tubs.longitudeExtension);
    self.VesselId = ko.observable(eventData.VesselId || 0);
    self.Name = ko.observable(eventData.Name || '');
    self.Ircs = ko.observable(eventData.Ircs || '');
    self.CountryCode = ko.observable(eventData.CountryCode || '');
    self.TypeCode = ko.observable(eventData.TypeCode || null);
    self.Bearing = ko.observable(eventData.Bearing || null).extend({ min: 0, max: 360 });
    // No negative distance
    self.Distance = ko.observable(eventData.Distance || null).extend({ min: 0 });
    self.ActionCode = ko.observable(eventData.ActionCode || '');
    self.PhotoFrame = ko.observable(eventData.PhotoFrame || '');
    self.Comments = ko.observable(eventData.Comments || '');
    self.NeedsFocus = ko.observable(eventData.NeedsFocus || false);
    self._destroy = ko.observable(eventData._destroy || false); //ignore jslint

    self.dirtyFlag = new ko.DirtyFlag([
        self.DateOnly,
        self.TimeOnly,
        self.Latitude,
        self.Longitude,
        self.VesselId,
        self.Name,
        self.Ircs,
        self.CountryCode,
        self.CountryCode,
        self.TypeCode,
        self.Bearing,
        self.Distance,
        self.ActionCode,
        self.PhotoFrame,
        self.Comments
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

/**
 * View model for all sightings in a single trip
 * @constructor
 * @param {object} data - Sightings data
 */
tubs.SightingViewModel = function (data) {
    var self = this;
    ko.mapping.fromJS(data, tubs.sightingMapping, self);

    // The clear function preps this ViewModel for being reloaded
    self.clear = function () {
        if (self.Sightings) {
            self.Sightings([]);
        }
    };

    self.dirtyFlag = new ko.DirtyFlag([
        self.Sightings
    ], false);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        var hasDirtyChild = false;
        $.each(self.Sightings(), function (i, evt) { //ignore jslint
            if (evt.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
    });

    self.clearDirtyFlag = function () {
        $.each(self.Sightings(), function (index, value) { //ignore jslint
            value.dirtyFlag().reset();
        });
    };

    // Operations
    // Create a new GEN-1 Sighting line item
    self.addEvent = function () {
        self.Sightings.push(new tubs.VesselSighting({ "NeedsFocus": true }));
    };

    self.removeEvent = function (evt) {
        if (evt && evt.Id()) {
            self.Sightings.destroy(evt);
        } else {
            self.Sightings.remove(evt);
        }
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getSightings(
                self.TripId(),
                function (result) {
                    self.clear();
                    ko.mapping.fromJS(result, tubs.sightingMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded sightings');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload sightings', xhr, status);
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
            tubs.saveSightings(
                self.TripId(),
                self,
                function (result) {
                    self.clear();
                    ko.mapping.fromJS(result, tubs.sightingMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved sightings');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save sightings', xhr, status);
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