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

/// <reference name="../knockout-2.1.0.debug.js" />
/// <reference name="../knockout-custom-bindings.js" />
/// <reference name="../underscore.js" />

// All the view models are in the tubs namespace
var tubs = tubs || {};

// TODO: Extract into a single script in the tubs namespace
tubs.timeExtension = {
    pattern: {
        message: 'Must be a valid 24 hour time',
        params: '^(20|21|22|23|[01][0-9])[0-5][0-9]$'
    },
    maxLength: 4
};

/**
 * Knockout ViewModel options for mapping JavaScript object.
 * The mapping is intended to:
 * Set a validation regular expression on all properties named 'TimeOnly'.
 * Add a 'Remove' method to the Details observable array.
 * Add a KoLite dirty flag to all members of the Details observable array.
 * Track members of the Details observable array using their intrinsic 'Id' property.
 * Extend the 'DateOnly' property with a function that displays the date in DD/MM/YY format.
 */
tubs.longlineSampleOptions = {
    extend: {
        "DateOnly": "IsoDate",
        "TimeOnly": "TimePattern",
        "{root}.Details": "ExtendWithDestroy",
        "{root}.Details[i]": function (detail) {
            detail.dirtyFlag = new ko.DirtyFlag([
                detail.DateOnly,
                detail.TimeOnly,
                detail.HookNumber,
                detail.SpeciesCode,
                detail.CaughtCondition,
                detail.DiscardedCondition,
                detail.Length,
                detail.LengthCode,
                detail.Weight,
                detail.WeightCode,
                detail.FateCode,
                detail.SexCode,
                detail.Comments
            ], false);

            detail.isDirty = ko.computed(function () {
                return detail.dirtyFlag().isDirty();
            });
        }
    },
    arrayChild: {
        "{root}.Details": "Id"
    },
    shared: {
        /* Assumes item has an Id() observable to hold database PK value */
        ExtendWithDestroy: function (items) {
            items.Remove = function (item) {
                // If the item has an Id, mark for deletion
                if (item && item.Id && item.Id()) {
                    items.destroy(item);
                } else {
                    // If there is no Id, or it equals zero, remove it from the
                    // array so that it won't be sent to the server
                    items.remove(item);
                }
            };
        },
        /* Validate time of day with a simple regex. */
        TimePattern: function (time) {
            time.extend(tubs.timeExtension);
        },
        /* Add formattedDate property to full date object */
        IsoDate: function (date) {
            date.extend({ isoDate: 'DD/MM/YY' });
        }
    }
};

/**
 * Serves as a default values for all detail entities.
 * Also used in the construction of new observable detail entities.
 */
tubs.longlineSampleDetailDefaults = {
    Id: 0,
    SampleNumber: 0,
    DateOnly: null,
    TimeOnly: null,
    HookNumber: null,
    SpeciesCode: '',
    CaughtCondition: null,
    DiscardedCondition: null,
    Length: null,
    LengthCode: '',
    Weight: null,
    WeightCode: '',
    FateCode: '',
    SexCode: '',
    Comments: '',
    _destroy: false,
    NeedsFocus: false
};

/**
 * @param {data} data Long Line catch monitoring object model that may need gain missing properties.
 * 
 */
tubs.longlineSampleDefaults = function (data) {
    data.Details = data.Details || [];
    return data;
};

/**
 * Sample line item on the LL-4 form.
 * @constructor
 * @param {object} data - Line item data
 */
tubs.LongLineSampleDetail = function (data) {
    'use strict';
    var self = this;
    data = data || {};
    _.defaults(data, tubs.longlineSampleDetailDefaults);

    self.Id = ko.observable(data.Id);
    self.SampleNumber = ko.observable(data.SampleNumber);
    self.DateOnly = ko.observable(data.DateOnly).extend({ isoDate: 'DD/MM/YY' });
    self.TimeOnly = ko.observable(data.TimeOnly);
    self.HookNumber = ko.observable(data.HookNumber);
    self.SpeciesCode = ko.observable(data.SpeciesCode);
    self.CaughtCondition = ko.observable(data.CaughtCondition);
    self.DiscardedCondition = ko.observable(data.DiscardedCondition);
    self.Length = ko.observable(data.Length);
    self.LengthCode = ko.observable(data.LengthCode);
    self.Weight = ko.observable(data.Weight);
    self.WeightCode = ko.observable(data.WeightCode);
    self.FateCode = ko.observable(data.FateCode);
    self.SexCode = ko.observable(data.SexCode);
    self.Comments = ko.observable(data.Comments);

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

    // Use knockout.viewmodel to manage most of the mapping
    var vm = ko.viewmodel.fromModel(
        tubs.longlineSampleDefaults(data),
        tubs.longlineSampleOptions
    );

    vm.dirtyFlag = new ko.DirtyFlag([
        vm.MeasuringInstrument,
        vm.Details
    ], false);

    vm.isAdd = ko.computed(function () {
        return (/add/i).test(vm.ActionName());
    });

    vm.showNextItem = ko.computed(function () {
        return !vm.isAdd() && vm.HasNext();
    });

    vm.isDirty = ko.computed(function () {
        var hasDirtyChild = false;
        if (vm.dirtyFlag().isDirty()) {
            return true;
        }

        hasDirtyChild = _.any(vm.Details(), function (detail) {
            return detail.isDirty();
        });

        return hasDirtyChild;
    });

    vm.clearDirtyFlag = function () {
        _.each(vm.Details(), function (detail) {
            detail.dirtyFlag().reset();
        });
        vm.dirtyFlag().reset();
    };

    vm.addDetail = function () {
        var previous,
            dateOnly;
        // Get the date from the previous entry
        previous = _.last(vm.Details());
        if (previous && previous.DateOnly) {
            dateOnly = previous.DateOnly();
        }
        // If no previous entry, get the date from the start of haul
        if (null === dateOnly) {
            dateOnly = vm.HaulDate();
        }

        vm.Details.push(new tubs.LongLineSampleDetail({ DateOnly: dateOnly, NeedsFocus: true }));
    };

    vm.removeDetail = function (detail) {
        vm.Details.Remove(detail);
    };

    vm.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getLonglineSample(
                vm.TripId(),
                vm.SetNumber(),
                function (result) {
                    ko.viewmodel.updateFromModel(vm, tubs.longlineSampleDefaults(result));
                    vm.clearDirtyFlag();
                    toastr.success('Reloaded LL-4 data');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload LL-4 data', xhr, status);
                }
            );
            complete();
        },
        canExecute: function (isExecuting) {
            return !isExecuting && vm.isDirty();
        }
    });

    vm.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.saveLonglineSample(
                vm.TripId(),
                vm.SetNumber(),
                vm,
                function (result) {
                    ko.viewmodel.updateFromModel(vm, tubs.longlineSampleDefaults(result));
                    vm.clearDirtyFlag();
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
            return !isExecuting && vm.isDirty();
        }
    });

    return vm;
};