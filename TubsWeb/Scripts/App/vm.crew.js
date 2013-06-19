/// <reference path="~/Scripts/knockout-2.2.1.js" />
/// <reference path="~/Scripts/knockout.mapping-latest.js" />
/// <reference path="~/Scripts/knockout.command.js" />

/*
* vm.crew.js
* Knockout.js ViewModel for editing a PS-2 Daily Log
* Depends on:
* jquery
* underscore.js (cleaner array ops)
* knockout
* knockout.mapping (automatically maps JSON)
* knockout.asyncCommand (makes it easier to show user activity)
* knockout.dirtyFlag (avoid unneccesary saves)
* knockout.activity (fancy UI gadget)
* amplify (local storage and Ajax mapping
* toastr (user notification)
* knockout.custom-bindings (date binding)
*/

// All the view models are in the tubs namespace
var tubs = tubs || {};

tubs.psCrewMapping = {
    'Hands': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    },
    'Captain': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    },
    'Navigator': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    },
    'Mate': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    },
    'ChiefEngineer': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    },
    'AssistantEngineer': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    },
    'DeckBoss': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    },
    'Cook': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    },
    'HelicopterPilot': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    },
    'SkiffMan': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    },
    'WinchMan': {
        create: function (options) {
            return new tubs.CrewMember(options.data);
        }
    }
};

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
}

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
        vm.Hands.push(ko.viewmodel.fromModel({NeedsFocus: true}, tubs.crewMemberOptions));
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
                function (xhr, status, error) {
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
                function (xhr, status, error) {
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

tubs.CrewMember = function (data) {
    'use strict';
    var self = this;
    self.Id = ko.observable(data.Id || 0);
    self.Job = ko.observable(data.Job); // Numeric enum value, not string description
    self.Name = ko.observable(data.Name);
    self.Nationality = ko.observable(data.Nationality).extend({ maxLength: 2 });
    self.Years = ko.observable(data.Years);
    self.Comments = ko.observable(data.Comments);
    // This is used to set focus on the most recently added CrewMember
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Name,
        self.Nationality,
        self.Years,
        self.Comments
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

tubs.psCrewViewModel = function (data) {
    var self = this;
    ko.mapping.fromJS(data, tubs.psCrewMapping, self);
    self.dirtyFlag = new ko.DirtyFlag([
        self.Captain,
        self.Navigator,
        self.Mate,
        self.ChiefEngineer,
        self.AssistantEngineer,
        self.DeckBoss,
        self.Cook,
        self.HelicopterPilot,
        self.SkiffMan,
        self.WinchMan,
        self.Hands
    ], false);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }

        return _.any(self.Hands(), function (hand) { //ignore jslint
            return hand.isDirty();
        });
    });

    // Clear the dirty flag for the this entity, plus all the
    // child entities stored in the Events observableArray
    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
        _.each(self.Hands(), function (hand) { //ignore jslint
            hand.dirtyFlag().reset();
        });
    };

    // Operations
    // Create a new 'Other Crew' line item and tell the UI that it needs focus
    // This is handy for mouse-free data entry
    self.addHand = function () {
        // http://knockoutjs.com/documentation/hasfocus-binding.html
        // Knockout.js is awesome.  The 'hasfocus' binding allows us to set
        // focus on a newly created item, but we can set focus elsewhere
        // when the page loads.
        self.Hands.push(new tubs.CrewMember({ "NeedsFocus": true }));
    };

    /* This function takes advantage of the RoR integration
     * in Knockout.  A call to .destroy(...) sets the "_destroy" property
     * to true.  Knockout then ignores the entity in the 'foreach' binding.
     * This removes the requirement to manage deleted Ids on the client and
     * the server.
     * NOTE:  We're not using destroy on events that have an Id of zero.
     * If the user had added a row and then realized they didn't need it,
     * we wouldn't want to ship garbage data back to the server and complicate
     * the validation.
     */
    self.removeHand = function (hand) {
        if (hand && hand.Id()) {
            self.Hands.destroy(hand);
        } else {
            self.Hands.remove(hand);
        }
    };

    // Getting a working reload is going to require a working JSON mapping
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getCrew(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    complete();
                },
                function (xhr, status, error) {
                    tubs.notify('Failed to reload crew', xhr, status);
                    complete();
                }
            );
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.saveCrew(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved crew details');
                    complete();
                },
                function (xhr, status, error) {
                    tubs.notify('Failed to save crew details', xhr, status);
                    complete();
                }
            );
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });
    return self;
};