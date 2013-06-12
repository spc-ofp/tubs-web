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
"use strict";

/*
 * Use the knockout.viewmodel plugin to map the SetHaul
 * view model.
 */
tubs.setHaulOptions = {
    extend: {
        "{root}.Comments": function (comments) {
            /* Remove looks like it's working, but there's still the issue of creating a comment... */
            comments.Remove = function (comment) {
                if (comment && comment.Id && comment.Id()) {
                    comments.destroy(comment);
                } else {
                    comments.remove(comment)
                };
            };
            /* Add might belong on the ViewModel itself... */
            comments.Add = function (comment) {
                /* TODO: Set NeedsFocus to true */
                comments.push(comment);
            };
            
            return comments;
        },
        "{root}.Comments[i]": function (comment) {
            comment.dirtyFlag = new ko.DirtyFlag([
                comment.LocalTime,
                comment.Details,
            ], false);

            comment.isDirty = ko.computed(function () {
                return comment.dirtyFlag().isDirty();
            });
        },
        "{root}.IntermediateHaulPositions": function (positions) {
            positions.Remove = function (position) {
                if (position && position.Id && position.Id()) {
                    positions.destroy(position);
                } else {
                    positions.remove(position)
                };
            };
            positions.Add = function (position) {
                positions.push(position);
            };
            return positions;
        },
        "{root}.IntermediateHaulPositions[i]": function (position) {
            position.dirtyFlag = new ko.DirtyFlag([
                position.LocalTime,
                position.Latitude,
                position.Longitude,
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
                /* There is still the issue of whether this needs an unmap... */
                return ko.observable(shipsDate).extend({ isoDate: 'DD/MM/YY' });
            }
        },
        "{root}.UtcDate": {
            map: function (utcDate) {
                return ko.observable(utcDate).extend({ isoDate: 'DD/MM/YY' });
            }
        }
    }
};

tubs.SetHaul = function (data) {
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
        vm.Comments, // Array membership, not changes to children
        vm.IntermediateHaulPositions // Array membership, not changes to children
    ], false);

    vm.isDirty = ko.computed(function () {
        if (vm.dirtyFlag().isDirty()) {
            return true;
        }
        var hasDirtyChild = false;
        // Look for dirty comments
        $.each(vm.Comments(), function (i, c) {
            if (c.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        if (hasDirtyChild) { return hasDirtyChild; }
        // Look for dirty intermediate positions
        $.each(vm.IntermediateHaulPositions(), function (i, p) {
            if (p.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });

        return hasDirtyChild;
    });

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