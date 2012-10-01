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


tubs.psSetMapping = {
    'LogbookDate': {
        create: function (options) {
            return ko.observable(options.data).extend({ isoDate: 'DD/MM/YY' });
        }
    },
    'ByCatch': {
        create: function (options) {
            return new tubs.psSetCatch(options.data);
        }
    },
    'TargetCatch': {
        create: function (options) {
            return new tubs.psSetCatch(options.data);
        }
    }
};

tubs.psSetCatch = function (catchData) {
    var self = this;
    self.Id = ko.observable(catchData.Id || 0);
    self._destroy = ko.observable(catchData._destroy || false);
    self.SpeciesCode = ko.observable(catchData.SpeciesCode || '');
    self.FateCode = ko.observable(catchData.FateCode || '');
    self.ObservedWeight = ko.observable(catchData.ObservedWeight || null);
    self.ObservedCount = ko.observable(catchData.ObservedCount || null);
    self.LogbookWeight = ko.observable(catchData.LogbookWeight || null);
    self.LogbookCount = ko.observable(catchData.LogbookCount || null);
    self.Comments = ko.observable(catchData.Comments || null);
    self.NeedsFocus = ko.observable(catchData.NeedsFocus || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.SpeciesCode,
        self.FateCode,
        self.ObservedWeight,
        self.ObservedCount,
        self.LogbookWeight,
        self.LogbookCount,
        self.Comments
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
}

// This is the actual Purse Seine Sea Day view model
// Any functions/properties/etc. that belong on the view model
// are defined here.
tubs.psSet = function (data) {
    var self = this;
    // Map the incoming JSON in 'data' to self, using
    // the options in psSetMapping
    ko.mapping.fromJS(data, tubs.psSetMapping, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.SkiffOffTimeOnly,
        self.WinchOnTimeOnly,
        self.RingsUpTimeOnly,
        self.BeginBrailingTimeOnly,
        self.EndBrailingTimeOnly,
        self.EndOfSetTimeOnly,
        self.SumOfBrail1,
        self.SumOfBrail2,
        self.TotalCatch,
        self.ContainsSkipjack,
        self.ContainsYellowfin,
        self.ContainsLargeYellowfin,
        self.ContainsBigeye,
        self.ContainsLargeBigeye,
        self.SkipjackPercentage,
        self.YellowfinPercentage,
        self.LargeYellowfinPercentage,
        self.LargeYellowfinCount,
        self.BigeyePercentage,
        self.LargeBigeyePercentage,
        self.LargeBigeyeCount,
        self.TonsOfSkipjackObserved,
        self.TonsOfYellowfinObserved,
        self.TonsOfBigeyeObserved,
        self.RecoveredTagCount,
        self.Comments,
        self.ByCatch, // Add/remove only
        self.TargetCatch // Add/remove only
    ], false /* TODO: Add appropriate hash function */);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        // Check each child, bailing on the first
        // dirty child.
        var hasDirtyChild = false;
        $.each(self.ByCatch(), function (i, sc) {
            if (sc.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        if (hasDirtyChild) { return hasDirtyChild; }
        $.each(self.TargetCatch(), function (i, sc) {
            if (sc.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        return hasDirtyChild;
    });

    // Clear the dirty flag for the this entity, plus all the
    // child entities stored in the Events observableArray
    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
        $.each(self.ByCatch(), function (index, value) {
            value.dirtyFlag().reset();
        });
        $.each(self.TargetCatch(), function (index, value) {
            value.dirtyFlag().reset();
        });
    };

    // Operations
    self.addByCatch = function () {
        self.ByCatch.push(new tubs.psSetCatch({ "NeedsFocus": true }));
    };

    self.addTargetCatch = function () {
        self.TargetCatch.push(new tubs.psSetCatch({ "NeedsFocus": true }));
    };

    self.removeByCatch = function (sc) {
        if (sc && sc.Id()) { self.ByCatch.destroy(sc); }
        else { self.ByCatch.remove(sc); }
    };

    self.removeTargetCatch = function (sc) {
        if (sc && sc.Id()) { self.TargetCatch.destroy(sc); }
        else { self.TargetCatch.remove(sc); }
    };

    // tubs.getFishingSet = function (tripId, setNumber, success_cb, error_cb)
    // tubs.saveFishingSet = function (tripId, setNumber, fishingSet, success_cb, error_cb)

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getFishingSet(
                self.TripId(),
                self.SetNumber(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.psSetMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded set details');
                    complete();
                },
                function (xhr, status, error) {
                    toastr.error(error, 'Failed to reload set details');
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
            tubs.saveFishingSet(
                self.TripId(),
                self.SetNumber(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.psSetMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved set details');
                    complete();
                },
                function (xhr, status, error) {
                    toastr.error(error, 'Failed to save set details');
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