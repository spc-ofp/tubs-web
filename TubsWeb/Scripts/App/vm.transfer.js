﻿/** 
 * @file Knockout ViewModel for editing transfers to/from vessels at sea
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
 * Knockout mapping for collection transfers view model.
 */
tubs.transferMapping = {
    'Transfers': {
        create: function (options) {
            return new tubs.Transfer(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    }
};

/**
 * A transfer to/from a single vessel as recorded on a GEN-1 form.
 * @constructor
 * @param {object} eventData - Transfer data
 */
tubs.Transfer = function (eventData) {
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
    self.Skipjack = ko.observable(eventData.Skipjack || null);
    self.Yellowfin = ko.observable(eventData.Yellowfin || null);
    self.Bigeye = ko.observable(eventData.Bigeye || null);
    self.Mixed = ko.observable(eventData.Mixed || null);
    self.ActionCode = ko.observable(eventData.ActionCode || '');
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
        self.Skipjack,
        self.Yellowfin,
        self.Bigeye,
        self.Mixed,
        self.ActionCode,
        self.Comments
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

tubs.TransferViewModel = function (data) {
    var self = this;
    ko.mapping.fromJS(data, tubs.transferMapping, self);

    // The clear function preps this ViewModel for being reloaded
    self.clear = function () {
        if (self.Transfers) {
            self.Transfers([]);
        }
    };

    self.dirtyFlag = new ko.DirtyFlag([
        self.Transfers
    ], false);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        var hasDirtyChild = false;
        $.each(self.Transfers(), function (i, evt) { //ignore jslint
            if (evt.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
    });

    self.clearDirtyFlag = function () {
        $.each(self.Transfers(), function (index, value) { //ignore jslint
            value.dirtyFlag().reset();
        });
    };

    // Operations
    // Create a new GEN-1 Transfer line item
    self.addEvent = function () {
        self.Transfers.push(new tubs.Transfer({ "NeedsFocus": true }));
    };

    self.removeEvent = function (evt) {
        if (evt && evt.Id()) {
            self.Transfers.destroy(evt);
        } else {
            self.Transfers.remove(evt);
        }
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getTransfers(
                self.TripId(),
                function (result) {
                    self.clear();
                    ko.mapping.fromJS(result, tubs.transferMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded transfers');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload transfers', xhr, status);
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
            tubs.saveTransfers(
                self.TripId(),
                self,
                function (result) {
                    self.clear();
                    ko.mapping.fromJS(result, tubs.transferMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved transfers');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save transfers', xhr, status);
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