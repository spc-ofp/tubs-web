/*
 * vm.sethaul.js
 * Knockout.js ViewModel for editing an LL-2/3 form
 * Depends on:
 * TODO
 */

/// <reference name="../knockout-2.1.0.debug.js" />
/// <reference name="../knockout-custom-bindings.js" />

// All the view models are in the tubs namespace
var tubs = tubs || {};

/*
 * Use the knockout.viewmodel plugin to map the SetHaul
 * view model.
 * http://coderenaissance.github.io/knockout.viewmodel/
 * TODO:  Adding a Comment or IntermediateHaulPosition
 * Work out if it is done on the VM or is a shared function...
 */
tubs.setHaulOptions = {
    extend: {
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
        }
    }
};

// Probably best to create observable entities for SetHaulComment and
// SetHaulPosition.  Maybe even do so above setHaulOptions and use them in
// the mapping where possible?

tubs.SetHaul = function (data) {
    'use strict';
    // Use knockout.viewmodel to manage most of the mapping
    var vm = ko.viewmodel.fromModel(data, tubs.setHaulOptions);

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
        vm.EndOfHaul, // Not sure this will stick around...
        vm.Comments,
        vm.IntermediateHaulPositions
    ], false);

    vm.isAdd = ko.computed(function () {
        return /add/i.test(vm.ActionName());
    });

    vm.showNextItem = ko.computed(function () {
        // TODO: Take HasNext into account?
        return !vm.isAdd() || vm.SetId() !== 0;
    });

    vm.isDirty = ko.computed(function () {
        if (vm.dirtyFlag().isDirty()) {
            return true;
        }
        var hasDirtyChild = false;
        // Look for dirty comments
        $.each(vm.Comments(), function (i, c) { //ignore jslint
            if (c.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        if (hasDirtyChild) { return hasDirtyChild; }
        // Look for dirty intermediate positions
        $.each(vm.IntermediateHaulPositions(), function (i, p) { //ignore jslint
            if (p.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });

        return hasDirtyChild;
    });

    vm.clearDirtyFlag = function () {
        vm.dirtyFlag().reset();
    };

    // TODO: Implement using correct jQuery promise functions
    vm.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            complete();
        },

        canExecute: function (isExecuting) {
            return !isExecuting && vm.isDirty();
        }
    });

    // TODO: Implement
    vm.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            complete();
        },

        canExecute: function (isExecuting) {
            return !isExecuting && vm.isDirty();
        }
    });

    return vm;
};