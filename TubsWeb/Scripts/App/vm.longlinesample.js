/** 
 * @file Knockout ViewModel for editing an LL-4 form
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * knockout.js
 * underscore.js
 * knockout.viewmodel plugin
 * knockout-custom-bindings (for ISO date)
 */

/// <reference name="../underscore.js" />
/// <reference name="../knockout-2.3.0.debug.js" />
/// <reference name="../knockout.mapping-latest.debug.js" />
/// <reference name="../tubs-custom-bindings.js" />
/// <reference name="datacontext.js" />

/**
 * @namespace All view models are in the tubs namespace.
 */
var tubs = tubs || {};

/**
 * Knockout mapping for a single sample (catch record).
 */
tubs.longlineSampleMapping = {
    'Details': {
        create: function (options) {
            return new tubs.LongLineSampleDetail(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.Id);
        }
    }
};

/**
 * Sample line item on the LL-4 form.
 * @constructor
 * @param {object} data - Line item data
 */
tubs.LongLineSampleDetail = function (data) {
    'use strict';
    var self = this;

    self.Id = ko.observable(data.Id || 0);
    self.SampleNumber = ko.observable(data.SampleNumber || 0);
    self.DateOnly = ko.observable(data.DateOnly || null).extend(tubs.dateExtension);
    self.TimeOnly = ko.observable(data.TimeOnly || '').extend(tubs.timeExtension);
    self.HookNumber = ko.observable(data.HookNumber || null);
    self.SpeciesCode = ko.observable(data.SpeciesCode || '');
    self.CaughtCondition = ko.observable(data.CaughtCondition || '');
    self.DiscardedCondition = ko.observable(data.DiscardedCondition || '');
    self.Length = ko.observable(data.Length || null);
    self.LengthCode = ko.observable(data.LengthCode || '');
    self.Weight = ko.observable(data.Weight || null);
    self.WeightCode = ko.observable(data.WeightCode || '');
    self.FateCode = ko.observable(data.FateCode || '');
    self.SexCode = ko.observable(data.SexCode || '');
    self.Comments = ko.observable(data.Comments || '');
    self._destroy = ko.observable(data._destroy || false);
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.DateOnly,
        self.TimeOnly,
        self.HookNumber,
        self.SpeciesCode,
        self.CaughtCondition,
        self.DiscardedCondition,
        self.Length,
        self.LengthCode,
        self.Weight,
        self.WeightCode,
        self.FateCode,
        self.SexCode,
        self.Comments
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

/**
 * ViewModel for complete LL-4 form
 * @constructor
 * @param {object} data - ViewModel data
 */
tubs.LongLineSample = function (data) {
    'use strict';
    var self = this;

    data = data || {};
    ko.mapping.fromJS(data, tubs.longlineSampleMapping, self);

    // The clear function preps this ViewModel for being reloaded
    self.clear = function () {
        if (self.Details) {
            self.Details([]);
        }
    };

    self.dirtyFlag = new ko.DirtyFlag([
        self.MeasuringInstrument,
        self.Details
    ], false);

    self.isAdd = ko.computed(function () {
        return (/add/i).test(self.ActionName());
    });

    self.showNextItem = ko.computed(function () {
        return !self.isAdd() && self.HasNext();
    });

    self.isDirty = ko.computed(function () {
        var hasDirtyChild = false;
        if (self.dirtyFlag().isDirty()) {
            return true;
        }

        hasDirtyChild = _.any(self.Details(), function (detail) {
            return detail.isDirty();
        });

        return hasDirtyChild;
    });

    self.clearDirtyFlag = function () {
        _.each(self.Details(), function (detail) {
            detail.dirtyFlag().reset();
        });
        self.dirtyFlag().reset();
    };

    self.addDetail = function () {
        var previous = null,
            dateOnly = null;
        // Get the date from the previous entry
        previous = _.last(self.Details());
        if (previous && previous.DateOnly) {
            dateOnly = previous.DateOnly();
        }
        // If no previous entry, get the date from the start of haul
        if (null === dateOnly) {
            dateOnly = self.HaulDate();
        }

        // Defaults for Fate and Sex are based on a user request to keep the same
        // functionality present in the existing legacy system
        self.Details.push(
            new tubs.LongLineSampleDetail({
                DateOnly: dateOnly,
                NeedsFocus: true,
                FateCode: 'RGG',
                SexCode: 'U'
            })
        );
    };

    self.removeDetail = function (detail) {
        if (detail && detail.Id()) {
            self.Details.destroy(detail);
        } else {
            self.Details.remove(detail);
        }
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getLonglineSample(
                self.TripId(),
                self.SetNumber(),
                function (result) {
                    self.clear();
                    ko.mapping.fromJS(result, tubs.longlineSampleMapping, self);
                    self.clearDirtyFlag();
                    toastr.success('Reloaded LL-4 data');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload LL-4 data', xhr, status);
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
            tubs.saveLonglineSample(
                self.TripId(),
                self.SetNumber(),
                self,
                function (result) {
                    self.clear();
                    ko.mapping.fromJS(result, tubs.longlineSampleMapping, self);
                    self.clearDirtyFlag();
                    toastr.success('Saved LL-4 data');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save LL-4 data', xhr, status);
                }
            );
            complete();
        },
        canExecute: function (isExecuting) {
            // TODO Deny save if validation fails
            return !isExecuting && self.isDirty();
        }
    });

    return self;
};