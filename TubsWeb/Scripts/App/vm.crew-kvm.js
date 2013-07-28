/*
* vm.crew-kvm.js
* Knockout.js ViewModel for editing Purse Seine crew.
* This version uses the viewmodel plugin for mapping.
* Depends on:
* underscore.js (cleaner array ops)
* knockout
* knockout.viewmodel (automatically maps JSON)
* knockout.asyncCommand (makes it easier to show user activity)
* knockout.dirtyFlag (avoid unneccesary saves)
* toastr (user notification)
*/

/// <reference path="~/Scripts/knockout-2.3.0.debug.js" />
/// <reference path="~/Scripts/knockout.viewmodel.js" />
/// <reference path="~/Scripts/knockout.command.js" />

// All the view models are in the tubs namespace
var tubs = tubs || {};

// knockout.viewmodel options for mapping a single
// crewmember
tubs.crewMemberOptions = {
    extend: {
        "{root}.Id": function (id) {
            return ko.observable(id || 0);
        },
        "{root}.Job": function (job) {
            return ko.observable(job);
        },
        "{root}.Name": function (name) {
            return ko.observable(name);
        },
        "{root}.Nationality": function (nationality) {
            return ko.observable(nationality).extend({ maxLength: 2 });
        },
        "{root}.Years": function (years) {
            return ko.observable(years);
        },
        "{root}.Comments": function (comments) {
            return ko.observable(comments);
        },
        "{root}.NeedsFocus": function (needsFocus) {
            return ko.observable(needsFocus || false);
        }
    }
};

// knockout.viewmodel options for mapping entire crew
tubs.crewOptions = {
    extend: {
        "{root}.Captain": "CrewDirtyFlag",
        "{root}.Navigator": "CrewDirtyFlag",
        "{root}.Mate": "CrewDirtyFlag",
        "{root}.ChiefEngineer": "CrewDirtyFlag",
        "{root}.AssistantEngineer": "CrewDirtyFlag",
        "{root}.DeckBoss": "CrewDirtyFlag",
        "{root}.Cook": "CrewDirtyFlag",
        "{root}.HelicopterPilot": "CrewDirtyFlag",
        "{root}.SkiffMan": "CrewDirtyFlag",
        "{root}.WinchMan": "CrewDirtyFlag",
        "{root}.Hands[i]": "CrewDirtyFlag"
    },
    shared: {
        CrewDirtyFlag: function (crew) {
            crew.dirtyFlag = new ko.DirtyFlag([
                crew.Name,
                crew.Nationality,
                crew.Years,
                crew.Comments
            ], false);

            crew.isDirty = ko.computed(function () {
                return crew.dirtyFlag().isDirty();
            });

            return crew;
        }
    }
};

// knockout.viewmodel version of crew mapping
tubs.PurseSeineCrewViewModel = function (data) {
    'use strict';
    var vm = ko.viewmodel.fromModel(data, tubs.crewOptions);

    vm.dirtyFlag = new ko.DirtyFlag([
        vm.Captain,
        vm.Navigator,
        vm.Mate,
        vm.ChiefEngineer,
        vm.AssistantEngineer,
        vm.DeckBoss,
        vm.Cook,
        vm.HelicopterPilot,
        vm.SkiffMan,
        vm.WinchMan,
        vm.Hands
    ], false);

    vm.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (vm.dirtyFlag().isDirty()) { return true; }

        return _.any(vm.Hands(), function (hand) {
            return hand.isDirty();
        });
    });

    // Clear the dirty flag for the this entity, plus all the
    // child entities stored in the Events observableArray
    vm.clearDirtyFlag = function () {
        vm.dirtyFlag().reset();
        _.each(vm.Hands(), function (hand) {
            hand.dirtyFlag().reset();
        });
    };

    vm.addHand = function () {
        vm.Hands.push(ko.viewmodel.fromModel({ NeedsFocus: true }, tubs.crewMemberOptions));
    };

    vm.removeHand = function (hand) {
        if (hand && hand.Id()) {
            vm.Hands.destroy(hand);
        } else {
            vm.Hands.remove(hand);
        }
    };

    // Getting a working reload is going to require a working JSON mapping
    vm.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getCrew(
                vm.TripId(),
                function (result) {
                    ko.viewmodel.updateFromModel(vm, result);
                    vm.clearDirtyFlag();
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload crew', xhr, status);
                    complete();
                }
            );
        },
        canExecute: function (isExecuting) {
            return !isExecuting && vm.isDirty();
        }
    });

    vm.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.saveCrew(
                vm.TripId(),
                vm,
                function (result) {
                    ko.viewmodel.updateFromModel(vm, result);
                    vm.clearDirtyFlag();
                    toastr.info('Saved crew details');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to save crew details', xhr, status);
                    complete();
                }
            );
        },

        canExecute: function (isExecuting) {
            return !isExecuting && vm.isDirty();
        }
    });


    return vm;

};