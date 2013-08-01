/** 
 * @file Knockout ViewModel for editing a PS-3 (set) form.
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * jQuery (isNumeric)
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
 * Common Knockout mapping for target catch and bycatch.
 * Same object model for both, with the only difference
 * being which list they go into.
 */
tubs.psCatchMapping = {
    create: function (options) {
        return new tubs.psSetCatch(options.data);
    },
    key: function (data) {
        return ko.utils.unwrapObservable(data.Id);
    }
};

/**
 * Knockout mapping for entire purse seine set.
 * Date values are extended with ISODate extension.
 * Target catch and bycatch are mapped with common
 * tubs.psCatchMapping options.
 */
tubs.psSetMapping = {
    'LogbookDate': tubs.mappedDate,
    'SkiffOff': tubs.mappedDate,
    'WinchOnDateOnly': tubs.mappedDate,
    'RingsUpDateOnly': tubs.mappedDate,
    'BeginBrailingDateOnly': tubs.mappedDate,
    'EndBrailingDateOnly': tubs.mappedDate,
    'EndOfSetDateOnly': tubs.mappedDate,
    'ByCatch': tubs.psCatchMapping,
    'TargetCatch': tubs.psCatchMapping
};

/**
 * Single catch record.  Includes fields common to target
 * and bycatch records.
 * @constructor
 * @param {object} catchData - Catch record data.
 */
tubs.psSetCatch = function (catchData) {
    'use strict';
    var self = this;
    self.Id = ko.observable(catchData.Id || 0);
    self.SpeciesCode = ko.observable(catchData.SpeciesCode || '');
    self.FateCode = ko.observable(catchData.FateCode || '');
    self.ObservedWeight = ko.observable(catchData.ObservedWeight || null);
    self.ObservedCount = ko.observable(catchData.ObservedCount || null);
    self.LogbookWeight = ko.observable(catchData.LogbookWeight || null);
    self.LogbookCount = ko.observable(catchData.LogbookCount || null);
    self.Comments = ko.observable(catchData.Comments || null);
    self.NeedsFocus = ko.observable(catchData.NeedsFocus || false);
    self._destroy = ko.observable(catchData._destroy || false); //ignore jslint

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

/**
 * View model representing a single purse seine set.
 * @constructor
 * @param {object} data - Set data
 */
tubs.psSet = function (data) {
    'use strict';
    var self = this;
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
        var hasDirtyChild = false;
        if (self.dirtyFlag().isDirty()) {
            return true;
        }

        hasDirtyChild = _.any(self.ByCatch(), function (item) { //ignore jslint
            return item.isDirty();
        });

        if (hasDirtyChild) {
            return hasDirtyChild;
        }

        return _.any(self.TargetCatch(), function (item) { //ignore jslint
            return item.isDirty();
        });
    });

    self.clearDirtyFlag = function () {
        _.each(self.ByCatch(), function (item) { //ignore jslint
            item.dirtyFlag().reset();
        });
        _.each(self.TargetCatch(), function (item) { //ignore jslint
            item.dirtyFlag().reset();
        });
        self.dirtyFlag().reset();
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

    // Compute total catch, not to store, but as a quality check
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
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload set details', xhr, status);
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
            tubs.saveFishingSet(
                self.TripId(),
                self.SetNumber(),
                self,
                function (result) {
                    self.clear();
                    ko.mapping.fromJS(result, tubs.psSetMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved set details');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save set details', xhr, status);
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