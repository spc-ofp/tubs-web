/**
 * @file Knockout ViewModel for editing an LL-1 form
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * knockout.js
 * knockout.viewmodel plugin
 */

/// <reference name="../knockout-2.1.0.debug.js" />

/**
 * Namespace for all TUBS javascript
 * @namespace tubs
 */
var tubs = tubs || {};

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
            complete();
        },
        canExecute: function (isExecuting) {
            return !isExecuting && vm.isDirty();
        }
    });

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