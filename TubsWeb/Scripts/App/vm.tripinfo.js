/**
 * @file Knockout ViewModel for editing an LL-1 form
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * knockout.js
 * knockout.viewmodel plugin
 */

/// <reference name="../underscore.js" />
/// <reference name="../knockout-2.3.0.debug.js" />
/// <reference path="datacontext.js" />

/**
 * Namespace for all TUBS javascript
 * @namespace tubs
 */
var tubs = tubs || {};

/**
 * View model for LL-1 form
 * @constructor
 * @param {object} data - LL-1 data
 */
tubs.TripInfo = function (data) {
    'use strict';

    var vm = ko.viewmodel.fromModel(data);

    vm.dirtyFlag = new ko.DirtyFlag([
        vm.VesselDeparturePort,
        vm.VesselDepartureDate,
        vm.Characteristics,
        vm.Nationality,
        vm.Gear,
        vm.Refrigeration,
        vm.Comments,
        vm.HasWasteDisposal,
        vm.WasteDisposalDescription,
        vm.Inspection
    ], false);

    vm.isDirty = ko.computed(function () {
        return vm.dirtyFlag().isDirty();
    });

    vm.clearDirtyFlag = function () {
        vm.dirtyFlag().reset();
    };

    vm.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getTripInfo(
                vm.TripId(),
                function (result) {
                    ko.viewmodel.updateFromModel(vm, result);
                    vm.clearDirtyFlag();
                    toastr.info('Reloaded LL-1 details');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload LL-1 details', xhr, status);
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
            tubs.saveTripInfo(
                vm.TripId(),
                vm,
                function (result) {
                    ko.viewmodel.updateFromModel(vm, result);
                    vm.clearDirtyFlag();
                    toastr.info('LL-1 saved');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save LL-1 details', xhr, status);
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