/** 
 * @file Knockout ViewModel for editing an LL-2/3 form
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

// All the view models are in the tubs namespace
var tubs = tubs || {};

/**
 * Knockout ViewModel options for mapping JavaScript object
 * to full view model.
 * The mapping is intended to:
 * Set a validation regular expression on all properties named 'LocalTime'.
 * Set a validation regular expression on all properties named 'Latitude'.
 * Set a validation regular expression on all properties named 'Longitude'.
 * Add a 'Remove' method to the Comments observable array.
 * Add a 'Remove' method to the IntermediateHaulPositions array.
 * Add a KoLite dirty flag to all members of the Comments observable array.
 * Add a KoLite dirty flag to all members of the IntermediateHaulPositions array.
 * Track members of the Comments observable array using their intrinsic 'Id' property.
 * Track members of the IntermediateHaulPositions array using their intrinsic 'Id' property.
 * Extend the 'ShipsDate' property with a function that displays the date in DD/MM/YY format.
 * Extend the 'UtcDate' property with a function that displays the date in DD/MM/YY format.
 */
tubs.setHaulOptions = {
    extend: {
        "DateOnly": "IsoDate",
        "LocalTime": "TimePattern",
        "Latitude": "LatitudePattern",
        "Longitude": "LongitudePattern",
        "{root}.Comments": "ExtendWithDestroy",
        "{root}.Comments[i]": function (comment) {
            comment.dirtyFlag = new ko.DirtyFlag([
                comment.LocalTime,
                comment.Details
            ], false);

            comment.isDirty = ko.computed(function () {
                return comment.dirtyFlag().isDirty();
            });
        },
        "{root}.IntermediateHaulPositions": "ExtendWithDestroy",
        "{root}.IntermediateHaulPositions[i]": function (position) {
            position.dirtyFlag = new ko.DirtyFlag([
                position.LocalTime,
                position.Latitude,
                position.Longitude
            ], false);

            position.isDirty = ko.computed(function () {
                return position.dirtyFlag().isDirty();
            });
        }
    },
    arrayChild: {
        "{root}.Comments": "Id",
        "{root}.IntermediateHaulPositions": "Id"
    },
    custom: {
        "{root}.ShipsDate": {
            map: function (shipsDate) {
                return ko.observable(shipsDate).extend({ isoDate: 'DD/MM/YY' });
            }
        },
        "{root}.UtcDate": {
            map: function (utcDate) {
                return ko.observable(utcDate).extend({ isoDate: 'DD/MM/YY' });
            }
        }
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
            time.extend({ pattern: '^(20|21|22|23|[01][0-9])[0-5][0-9]$' });
        },
        /* Validate latitude with a simple regex.  It allows invalid values but it's a decent first pass. */
        LatitudePattern: function (latitude) {
            latitude.extend({ pattern: '^[0-8][0-9]{3}\.?[0-9]{3}[NnSs]$' });
        },
        /* Validate longitude with a simple regex.  It allows invalid values but it's a decent first pass. */
        LongitudePattern: function (longitude) {
            longitude.extend({ pattern: '^[0-1]\\d{4}\.?\\d{3}[EeWw]$' });
        },
        /* Add formattedDate property to full date object */
        IsoDate: function (date) {
            date.extend({ isoDate: 'DD/MM/YY' });
        }
    }
};

/**
 * Serves as a default values for all comments entities.
 * Also used in the construction of new observable comment entities.
 */
tubs.setHaulCommentDefaults = {
    Id: 0,
    DateOnly: null,
    LocalTime: null,
    Details: null,
    _destroy: false,
    NeedsFocus: false
};

/**
 * Serves as a default values for all position entities.
 * Also used in the construction of new observable position entities.
 */
tubs.setHaulPositionDefaults = {
    Id: 0,
    DateOnly: null,
    LocalTime: null,
    Latitude: null,
    Longitude: null,
    EezCode: null,
    _destroy: false,
    NeedsFocus: false
};

/**
 * Serves as a default values for all bait values.
 * Also used in the construction of new observable bait values.
 */
tubs.setHaulBaitDefaults = {
    Species: null,
    Weight: null,
    Hooks: null,
    DyedBlue: null
};

/**
 * @param {data} data SetHaul object model that may need gain missing properties.
 * 
 */
tubs.setHaulDefaults = function (data) {
    // defaults can't deal with null objects
    data.StartOfSet = data.StartOfSet || {};
    _.defaults(data.StartOfSet, tubs.setHaulPositionDefaults);

    data.EndOfSet = data.EndOfSet || {};
    _.defaults(data.EndOfSet, tubs.setHaulPositionDefaults);

    data.StartOfHaul = data.StartOfHaul || {};
    _.defaults(data.StartOfHaul, tubs.setHaulPositionDefaults);

    data.EndOfHaul = data.EndOfHaul || {};
    _.defaults(data.EndOfHaul, tubs.setHaulPositionDefaults);

    // Create an array containing 5 objects
    // Backend will ignore anything not filled in
    data.Baits = data.Baits || [];
    while (data.Baits.length < 5) {
        data.Baits.push(_.clone(tubs.setHaulBaitDefaults));
    }

    data.Comments = data.Comments || [];
    data.IntermediateHaulPositions = data.IntermediateHaulPositions || [];

    return data;
};

/**
 * Comment line item on the LL-2/3 form.
 * @constructor
 * @param {object} data - Line item data
 */
tubs.SetHaulComment = function (data) {
    'use strict';
    var self = this;
    data = data || {};
    _.defaults(data, tubs.setHaulCommentDefaults);

    self.Id = ko.observable(data.Id);
    self.DateOnly = ko.observable(data.DateOnly).extend({ isoDate: 'DD/MM/YY' });
    self.LocalTime = ko.observable(data.LocalTime).extend({ pattern: '^(20|21|22|23|[01][0-9])[0-5][0-9]$' });
    self.Details = ko.observable(data.Details);
    self._destroy = ko.observable(data._destroy);
    self.NeedsFocus = ko.observable(data.NeedsFocus);

    self.dirtyFlag = new ko.DirtyFlag([
        self.DateOnly,
        self.LocalTime,
        self.Details
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

/**
 * Set or Haul position line item on the LL-2/3 form
 * @constructor
 * @param {object} data - Position data
 */
tubs.SetHaulPosition = function (data) {
    'use strict';
    var self = this;
    data = data || {};
    _.defaults(data, tubs.setHaulPositionDefaults);

    self.Id = ko.observable(data.Id);
    self.DateOnly = ko.observable(data.DateOnly).extend({ isoDate: 'DD/MM/YY' });
    self.LocalTime = ko.observable(data.LocalTime).extend({ pattern: '^(20|21|22|23|[01][0-9])[0-5][0-9]$' });
    self.Latitude = ko.observable(data.Latitude).extend({ pattern: '^[0-8][0-9]{3}\.?[0-9]{3}[NnSs]$' });
    self.Longitude = ko.observable(data.Longitude).extend({ pattern: '^[0-1]\\d{4}\.?\\d{3}[EeWw]$' });
    self.EezCode = ko.observable(data.EezCode);
    self._destroy = ko.observable(data._destroy);
    self.NeedsFocus = ko.observable(data.NeedsFocus);

    self.dirtyFlag = new ko.DirtyFlag([
        self.DateOnly,
        self.LocalTime,
        self.Latitude,
        self.Longitude
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

/**
 *
 */
tubs.SetHaul = function (data) {
    'use strict';

    // Use knockout.viewmodel to manage most of the mapping
    // 
    var vm = ko.viewmodel.fromModel(tubs.setHaulDefaults(data), tubs.setHaulOptions);

    // Setting the dirty flag in the viewmodel options is a bridge too far...
    // I'm fine with extending the instance versus adding this via prototype since there should
    // only ever be one SetHaul viewmodel on a page
    vm.dirtyFlag = new ko.DirtyFlag([
        vm.HooksPerBasket,
        vm.TotalBaskets,
        vm.TotalHooks,
        vm.FloatlineLength,
        vm.LineSettingSpeed,
        vm.LineSettingSpeedUnit,
        vm.BranchlineSetInterval,
        vm.DistanceBetweenBranchlines,
        vm.BranchlineLength,
        vm.VesselSpeed,
        vm.SharkLineCount,
        vm.SharkLineLength,
        vm.WasTdrDeployed,
        vm.IsTargetingTuna,
        vm.IsTargetingSwordfish,
        vm.IsTargetingShark,
        vm.LightStickCount,
        vm.TotalObservedBaskets,
        vm.HasGen3Event,
        vm.DiaryPage,
        vm.ShipsDate,
        vm.ShipsTime,
        vm.UtcDate,
        vm.UtcTime,
        vm.UnusualDetails,
        vm.StartEndPositionsObserved,
        vm.StartOfSet,
        vm.EndOfSet,
        vm.StartOfHaul,
        vm.EndOfHaul,
        vm.Baits,
        vm.Comments,
        vm.IntermediateHaulPositions
    ], false);

    vm.isAdd = ko.computed(function () {
        return (/add/i).test(vm.ActionName());
    });

    vm.showNextItem = ko.computed(function () {
        // TODO: Take HasNext into account?
        return !vm.isAdd() || vm.SetId() !== 0;
    });

    vm.isDirty = ko.computed(function () {
        var hasDirtyChild = false;
        //console.log("isDirty [header]" + vm.dirtyFlag().isDirty());
        if (vm.dirtyFlag().isDirty()) {
            return true;
        }
        
        hasDirtyChild = _.any(vm.Comments(), function (comment) {
            return comment.isDirty();
        });
        //console.log("isDirty [Comments]" + hasDirtyChild);

        if (hasDirtyChild) {
            return true;
        }

        hasDirtyChild = _.any(vm.IntermediateHaulPositions(), function (position) {
            return position.isDirty();
        });
        //console.log("isDirty [Positions]" + hasDirtyChild);

        return hasDirtyChild;
    });

    vm.clearDirtyFlag = function () {
        vm.dirtyFlag().reset();
    };

    vm.addComment = function () {
        var previous,
            dateOnly;
        // Comment gets DateOnly from previous comment
        // If no previous comment, DateOnly comes from Set value of DateOnly
        previous = _.last(vm.Comments());
        if (previous && previous.DateOnly) {
            dateOnly = previous.DateOnly();
        } else {
            dateOnly = vm.ShipsDate();
        }
        vm.Comments.push(new tubs.SetHaulComment({ DateOnly: dateOnly, NeedsFocus: true }));
    };

    vm.removeComment = function (comment) {
        vm.Comments.Remove(comment);
    };

    vm.addPosition = function () {
        var dateOnly = null,
            previous;

        // There's probably a smart way to do this with a while loop, but
        // this should work for now, although it will need a specific unit test.

        // Position gets DateOnly from previous position
        // If no previous position, DateOnly comes from Set value of DateOnly
        previous = _.last(vm.IntermediateHaulPositions());
        if (previous && previous.DateOnly) {
            dateOnly = previous.DateOnly();
        }
        // Try StartOfHaul
        if (null === dateOnly && vm.StartOfHaul) {
            dateOnly = vm.StartOfHaul.DateOnly();
        }
        // Try EndOfSet
        if (null === dateOnly && vm.EndOfSet) {
            dateOnly = vm.EndOfSet.DateOnly();
        }
        // Try StartOfSet
        if (null === dateOnly && vm.StartOfSet) {
            dateOnly = vm.StartOfSet.DateOnly();
        }
        if (null === dateOnly) {
            dateOnly = vm.ShipsDate();
        }

        vm.IntermediateHaulPositions.push(new tubs.SetHaulPosition({ DateOnly: dateOnly, NeedsFocus: true }));
    };

    vm.removePosition = function (position) {
        vm.IntermediateHaulPositions.Remove(position);
    };

    // Set default values for the "named" positions when ship's date is changed
    vm.ShipsDate.subscribe(function () {
        if (vm.ShipsDate()) {
            if (!vm.StartOfSet.DateOnly()) {
                vm.StartOfSet.DateOnly(vm.ShipsDate());
            }
            if (!vm.EndOfSet.DateOnly()) {
                vm.EndOfSet.DateOnly(vm.ShipsDate());
            }
            if (!vm.StartOfHaul.DateOnly()) {
                vm.StartOfHaul.DateOnly(vm.ShipsDate());
            }
        }
    });

    // Set the start-of-set time automatically
    vm.ShipsTime.subscribe(function () {
        if (vm.ShipsTime() && !vm.StartOfSet.LocalTime()) {
            vm.StartOfSet.LocalTime(vm.ShipsTime());
        }
    });

    vm.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getSetHaul(
                vm.TripId(),
                vm.SetNumber(),
                function (result) {
                    ko.viewmodel.updateFromModel(vm, tubs.setHaulDefaults(result));
                    vm.clearDirtyFlag();
                    toastr.success('Reloaded Set/Haul data');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload Set/Haul data', xhr, status);
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
            tubs.saveSetHaul(
                vm.TripId(),
                vm.SetNumber(),
                vm,
                function (result) {
                    ko.viewmodel.updateFromModel(vm, tubs.setHaulDefaults(result));
                    vm.clearDirtyFlag();
                    toastr.success('Set/Haul data saved');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save Set/Haul data', xhr, status);
                }
            );
            complete();
        },
        canExecute: function (isExecuting) {
            return !isExecuting && vm.isDirty();
        }
    });

    return vm;
};