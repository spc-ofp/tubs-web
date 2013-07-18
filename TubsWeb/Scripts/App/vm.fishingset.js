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
/// <reference path="../knockout-2.1.0.debug.js"  />

// All the view models are in the tubs namespace
var tubs = tubs || {};

tubs.psSetMapping = {
    'LogbookDate': {
        create: function (options) {
            return ko.observable(options.data).extend(tubs.dateExtension);
        }
    },
    'SkiffOff': {
        create: function (options) {
            return ko.observable(options.data).extend(tubs.dateExtension);
        }
    },
    'WinchOnDateOnly': {
        create: function (options) {
            return ko.observable(options.data).extend(tubs.dateExtension);
        }
    },
    'RingsUpDateOnly': {
        create: function (options) {
            return ko.observable(options.data).extend(tubs.dateExtension);
        }
    },
    'BeginBrailingDateOnly': {
        create: function (options) {
            return ko.observable(options.data).extend(tubs.dateExtension);
        }
    },
    'EndBrailingDateOnly': {
        create: function (options) {
            return ko.observable(options.data).extend(tubs.dateExtension);
        }
    },
    'EndOfSetDateOnly': {
        create: function (options) {
            return ko.observable(options.data).extend(tubs.dateExtension);
        }
    },
    'ByCatch': {
        create: function (options) {
            return new tubs.psSetCatch(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    },
    'TargetCatch': {
        create: function (options) {
            return new tubs.psSetCatch(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    }
};


tubs.psSetCatch = function (catchData) {
    'use strict';
    var self = this;
    self.Id = ko.observable(catchData.Id || 0);
    self._destroy = ko.observable(catchData._destroy || false); //ignore jslint
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
};

// This is the actual Purse Seine Sea Day view model
// Any functions/properties/etc. that belong on the view model
// are defined here.
tubs.psSet = function (data) {
    'use strict';
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
    ], false);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        // Check each child, bailing on the first
        // dirty child.
        var hasDirtyChild = false;
        $.each(self.ByCatch(), function (i, sc) { //ignore jslint
            if (sc.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        if (hasDirtyChild) { return hasDirtyChild; }
        $.each(self.TargetCatch(), function (i, sc) { //ignore jslint
            if (sc.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        return hasDirtyChild;
    });

    // Clear the dirty flag for the this entity, plus all the
    // child entities stored in the ByCatch and TargetCatch observableArrays
    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
        $.each(self.ByCatch(), function (index, value) { //ignore jslint
            value.dirtyFlag().reset();
        });
        $.each(self.TargetCatch(), function (index, value) { //ignore jslint
            value.dirtyFlag().reset();
        });
    };

    // The clear function preps this ViewModel for being reloaded
    self.clear = function () {
        if (self.ByCatch) {
            self.ByCatch([]);
        }
        if (self.TargetCatch) {
            self.TargetCatch([]);
        }
    };

    // Compute total catch, not to store, but as a possible check
    self.computedCatch = ko.computed(function () {
        var brail1 = 0,
            brail2 = 0;
        if ($.isNumeric(self.SizeOfBrail1()) && $.isNumeric(self.SumOfBrail1())) {
            brail1 = self.SizeOfBrail1() * self.SumOfBrail1();
            if (brail1 > 0) {
                brail1 = Math.round(brail1 * 1000) / 1000;
            }
        }
        if ($.isNumeric(self.SizeOfBrail2()) && $.isNumeric(self.SumOfBrail2())) {
            brail2 = self.SizeOfBrail2() * self.SumOfBrail2();
            if (brail2 > 0) {
                brail2 = Math.round(brail2 * 1000) / 1000;
            }
        }
        return brail1 + brail2;
    });

    // Show a warning if the entered and computed values don't match (within a half ton)
    self.showCatchTotalNote = ko.computed(function () {
        var retval = false,
            delta;
        if ($.isNumeric(self.SumOfBrail1()) || $.isNumeric(self.SumOfBrail2())) {
            delta = Math.abs(self.computedCatch() - self.TotalCatch());
            retval = delta > 0.5;
        }
        return retval;
    });

    // Operations
    self.addByCatch = function () {
        self.ByCatch.push(new tubs.psSetCatch({ "NeedsFocus": true }));
    };

    self.addTargetCatch = function () {
        self.TargetCatch.push(new tubs.psSetCatch({ "NeedsFocus": true }));
    };

    self.removeByCatch = function (sc) {
        if (sc && sc.Id()) {
            self.ByCatch.destroy(sc);
        } else {
            self.ByCatch.remove(sc);
        }
    };

    self.removeTargetCatch = function (sc) {
        if (sc && sc.Id()) {
            self.TargetCatch.destroy(sc);
        } else {
            self.TargetCatch.remove(sc);
        }
    };

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getFishingSet(
                self.TripId(),
                self.SetNumber(),
                function (result) {
                    self.clear();
                    ko.mapping.fromJS(result, tubs.psSetMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded set details');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload set details', xhr, status);
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
                    self.clear();
                    ko.mapping.fromJS(result, tubs.psSetMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved set details');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to save set details', xhr, status);
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