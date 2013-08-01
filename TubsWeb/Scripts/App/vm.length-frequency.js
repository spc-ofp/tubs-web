/** 
 * @file Knockout ViewModel for a PS-4 form header
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * knockout.js
 * underscore.js
 * knockout.mapping plugin
 * KoLite plugins (asyncCommand, activity, dirtyFlag)
 * toastr
 *
 * NOTE:  The volume of data present on a PS-4 precludes the
 * use of a full Knockout ViewModel per page
 * (a full page would be several hundred observables).
 * There is a separate ViewModel for a single _column_ of
 * purse seine samples.
 */

/// <reference path="../knockout-2.3.0.debug.js" />
/// <reference path="../knockout.mapping-latest.js" />
/// <reference path="datacontext.js" />

/**
 * @namespace All view models are in the tubs namespace.
 */
var tubs = tubs || {};

/**
 * Knockout mapping for a PS-4 form header.
 * Only the 'Brails' collection needs special mapping.
 * Everything else can be managed by the default values.
 */
tubs.Ps4HeaderMapping = {
    'Brails': {
        create: function (options) {
            return new tubs.Brail(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Number);
        }
    }
};

/**
 * Entity representing a single brail coming onboard during
 * a set.
 * @constructor
 * @param {object} data - Brail data
 */
tubs.Brail = function (data) {
    'use strict';
    var self = this;
    self.Number = ko.observable(data.Number || 1);
    self.Fullness = ko.observable(data.Fullness || null);
    self.Samples = ko.observable(data.Samples || null);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Fullness,
        self.Samples
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

/**
 * View model for the PS-4 header.
 * @constructor
 * @param {object} data - PS-4 header data
 */
tubs.Ps4HeaderViewModel = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, tubs.Ps4HeaderMapping, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.SampleType,
        self.OtherCode,
        self.ProtocolComments,
        self.GrabTarget,
        self.SpillBrailNumber,
        self.SpillNumberFishMeasured,
        self.WhichBrail,
        self.PageCount,
        self.MeasuringInstrument,
        self.CalibratedThisSet,
        self.FullBrailCount,
        self.SevenEighthsBrailCount,
        self.ThreeQuartersBrailCount,
        self.TwoThirdsBrailCount,
        self.OneHalfBrailCount,
        self.OneThirdBrailCount,
        self.OneQuarterBrailCount,
        self.OneEighthBrailCount,
        self.TotalBrails,
        self.SumOfAllBrails
    ], false);

    self.isDirty = ko.computed(function () {
        var hasDirtyChild = false;
        if (self.dirtyFlag().isDirty()) {
            return true;
        }

        hasDirtyChild = _.any(self.Brails(), function (item) { //ignore jslint
            return item.isDirty();
        });
        return hasDirtyChild;
    });

    self.clearDirtyFlag = function () {
        ko.utils.arrayForEach(self.Brails(), function (item) {
            item.dirtyFlag().reset();
        });
        self.dirtyFlag().reset();
    };

    self.isGrab = ko.computed(function () {
        return "Grab" === self.SampleType();
    });

    self.isSpill = ko.computed(function () {
        return "Spill" === self.SampleType();
    });

    self.isOther = ko.computed(function () {
        return "Other" === self.SampleType();
    });

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getPs4Header(
                self.TripId(),
                self.SetNumber(),
                self.PageNumber(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.Ps4HeaderMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded PS-4');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload PS-4', xhr, status);
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
            tubs.savePs4Header(
                self.TripId(),
                self.SetNumber(),
                self.PageNumber(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.Ps4HeaderMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved PS-4');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save PS-4', xhr, status);
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