/*
* vm.transfer.js
* Knockout.js ViewModel for editing the transfers
* portion of the GEN-1 form.
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
* spc.utilities (String hashCode)
*/

// All the view models are in the tubs namespace
var tubs = tubs || {};
"use strict";

tubs.transferMapping = {
    'Transfers': {
        create: function (options) {
            return new tubs.transfer(options.data);
        }
    }
};

tubs.transfer = function (eventData) {
    var self = this;
    self.Id = ko.observable(eventData.Id || 0);
    self.DateOnly = ko.observable(eventData.DateOnly || null).extend({ isoDate: 'DD/MM/YY' });
    self.TimeOnly = ko.observable(eventData.TimeOnly || '').extend({ pattern: '^[0-2][0-9][0-5][0-9]$' });
    self.Latitude = ko.observable(eventData.Latitude || '').extend({ pattern: '^[0-8][0-9]{3}\.?[0-9]{3}[NnSs]$' });
    self.Longitude = ko.observable(eventData.Longitude || '').extend({ pattern: '^[0-1]\\d{4}\.?\\d{3}[EeWw]$' });
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
    self._destroy = ko.observable(eventData._destroy || false);

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

    self.dirtyFlag = new ko.DirtyFlag([
        self.Transfers
    ], false);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        var hasDirtyChild = false;
        $.each(self.Transfers(), function (i, evt) {
            if (evt.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
    });

    self.clearDirtyFlag = function () {
        $.each(self.Transfers(), function (index, value) {
            value.dirtyFlag().reset();
        });
    };

    // Operations
    // Create a new GEN-1 Transfer line item
    self.addEvent = function () {
        self.Transfers.push(new tubs.transfer({ "NeedsFocus": true }));
    };

    self.removeEvent = function (evt) {
        if (evt && evt.Id()) { self.Transfers.destroy(evt); }
        else { self.Transfers.remove(evt); }
    }

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getTransfers(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.transferMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded transfers');
                    complete();
                },
                function (xhr, status, error) {
                    tubs.notify('Failed to reload transfers', xhr, status);
                    complete();
                });
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
                    ko.mapping.fromJS(result, tubs.transferMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved transfers');
                    complete();
                },
                function (xhr, status, error) {
                    tubs.notify('Failed to save transfers', xhr, status);
                    complete();
                });
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    return self;
};