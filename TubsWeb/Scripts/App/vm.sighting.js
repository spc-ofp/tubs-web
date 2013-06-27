/*
* vm.sighting.js
* Knockout.js ViewModel for editing the sightings
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

tubs.sightingMapping = {
    'Sightings': {
        create: function (options) {
            return new tubs.sighting(options.data);
        }
    }
};

tubs.sighting = function (eventData) {
    'use strict';
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
    self.Bearing = ko.observable(eventData.Bearing || null).extend({ min: 0, max: 360 });
    self.Distance = ko.observable(eventData.Distance || null).extend({ min: 0 }); // No negative distance
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

tubs.SightingViewModel = function (data) {
    var self = this;
    ko.mapping.fromJS(data, tubs.sightingMapping, self);

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
        self.Sightings.push(new tubs.sighting({ "NeedsFocus": true }));
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
                    ko.mapping.fromJS(result, tubs.sightingMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded sightings');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload sightings', xhr, status);
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
            tubs.saveSightings(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.sightingMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved sightings');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to save sightings', xhr, status);
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