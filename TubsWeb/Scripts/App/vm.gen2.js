/*
* vm.gen2.js
* Knockout.js ViewModel for editing GEN-2 data
* Depends on:
* jquery
* json2 (for down-level browser support)
* knockout
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

tubs.Gen2Event = function (data) {
    var self = this;

    // Set common properties
    self.Id = ko.observable(data.Id || 0);
    self.TripId = ko.observable(data.TripId);
    self.PageNumber = ko.observable(data.PageNumber || 0);
    self.ShipsDate = ko.observable(data.ShipsDate || '').extend({ isoDate: 'DD/MM/YY' });
    self.ShipsTime = ko.observable(data.ShipsTime || '');
    self.Latitude = ko.observable(data.Latitude || '').extend({ pattern: '^[0-8][0-9]{3}\.?[0-9]{3}[NnSs]$' });
    self.Longitude = ko.observable(data.Longitude || '').extend({ pattern: '^[0-1]\\d{4}\.?\\d{3}[EeWw]$' });
    self.SpeciesCode = ko.observable(data.SpeciesCode || '');
    self.SpeciesDescription = ko.observable(data.SpeciesDescription || '');
    self.ActionName = ko.observable(data.ActionName || 'add');

    return self;
}

tubs.Gen2LandedEvent = function (data) {
    var self = this;
    ko.utils.extend(self, new tubs.Gen2Event(data));

    self.addPattern = /add/i;
    // This is very important.  The "interactionType" property is used to assist the
    // MVC model binder choose the correct concrete instance for the abstract class that
    // is the incoming parameter to save.
    self.interactionType = ko.observable('TubsWeb.ViewModels.Gen2LandedViewModel');

    // Set landed properties
    self.LandedConditionCode = ko.observable(data.LandedConditionCode || '');
    self.LandedConditionDescription = ko.observable(data.LandedConditionDescription || '');
    self.LandedHandling = ko.observable(data.LandedHandling || '');
    self.LandedLength = ko.observable(data.LandedLength || null);
    self.LandedLengthCode = ko.observable(data.LandedLengthCode || '');
    self.LandedSexCode = ko.observable(data.LandedSexCode || '');
    self.DiscardedConditionCode = ko.observable(data.DiscardedConditionCode || '');
    self.DiscardedConditionDescription = ko.observable(data.DiscardedConditionDescription || '');
    self.RetrievedTagNumber = ko.observable(data.RetrievedTagNumber || '');
    self.RetrievedTagType = ko.observable(data.RetrievedTagType || '');
    self.RetrievedTagOrganization = ko.observable(data.RetrievedTagOrganization || '');
    self.PlacedTagNumber = ko.observable(data.PlacedTagNumber || '');
    self.PlacedTagType = ko.observable(data.PlacedTagType || '');
    self.PlacedTagOrganization = ko.observable(data.PlacedTagOrganization || '');

    // Knockout Mapping would do this for us, but we're not using mapping here
    self.ConditionCodes = data.ConditionCodes;
    self.SexCodes = data.SexCodes;
    self.LengthCodes = data.LengthCodes;

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

    self.isAdd = ko.computed(function () {
        return self.addPattern.test(self.ActionName());
    });

    // Only show the "Next Day" button for any Edit
    // or an Add that has been stored in the database
    self.showNextEventButton = ko.computed(function () {
        return !self.isAdd() || (self.Id() != 0);
    });

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getInteraction(
                self.TripId(),
                self.PageNumber(),
                function (result) {
                    // TODO How do we reload without using mapping?
                    self.clearDirtyFlag();
                    toastr.info('Reloaded GEN-2 event');
                    complete();
                },
                function (xhr, status, error) {
                    tubs.notify('Failed to reload GEN-2 event', xhr, status);
                    complete();
                }
            );
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    // Somehow need to pass interactionType = 'Gen2LandedViewModel'
    // outside the JSON data for this request
    // The cheap fix for this is to add distinct endpoints
    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.saveInteraction(
                self.TripId(),
                self,
                function (result) {
                    // TODO Reload
                    self.clearDirtyFlag();
                    toastr.success('Saved GEN-2');
                    complete();
                },
                function (xhr, status, error) {
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

}