/*
* vm.crew.js
* Knockout.js ViewModel for editing a PS-2 Daily Log
* Depends on:
* jquery
* json2 (for down-level browser support)
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

tubs.CrewMember = function (data) {
    var self = this;
    self.Id = ko.observable(data.Id);
    self.Job = ko.observable(data.Job); // Numeric enum value, not string description
    self.Name = ko.observable(data.Name);
    self.Nationality = ko.observable(data.Nationality);
    self.Years = ko.observable(data.Years);
    self.Comments = ko.observable(data.Comments);
    // This is used to set focus on the most recently added CrewMember
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Name,
        self.Nationality,
        self.Years,
        self.Comments,
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
}

tubs.psCrewViewModel = function (data) {
    var self = this;

    self.TripId = ko.observable(data.TripId || 0);

    // Senior Crew
    self.Captain = ko.observable(new tubs.CrewMember(data.Captain));
    self.Navigator = ko.observable(new tubs.CrewMember(data.Navigator));
    self.Mate = ko.observable(new tubs.CrewMember(data.Mate));
    self.ChiefEngineer = ko.observable(new tubs.CrewMember(data.ChiefEngineer));
    self.AssistantEngineer = ko.observable(new tubs.CrewMember(data.AssistantEngineer));
    self.DeckBoss = ko.observable(new tubs.CrewMember(data.DeckBoss));
    self.Cook = ko.observable(new tubs.CrewMember(data.Cook));
    self.HelicopterPilot = ko.observable(new tubs.CrewMember(data.HelicopterPilot));
    self.SkiffMan = ko.observable(new tubs.CrewMember(data.SkiffMan));
    self.WinchMan = ko.observable(new tubs.CrewMember(data.WinchMan));

    // Other Crew
    var tmpHands = [];
    $.map(data.Hands, function (n, i) {
        tmpHands.push(new tubsCrew.CrewMember(n));
    });
    self.Hands = ko.observableArray(tmpHands);

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
        // TODO It might be better to for-each this and bail
        // out of the loop on the first dirty element
        var dirtyHands = $.grep(self.Hands(), function (n, i) {
            return (!n.isDirty());
        });
        return dirtyHands.length === 0;
    });

    // Clear the dirty flag for the this entity, plus all the
    // child entities stored in the Events observableArray
    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
        $.each(self.Hands(), function (index, value) {
            value.dirtyFlag().reset();
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
    }

    // This function takes advantage of the RoR integration
    // in Knockout.  A call to .destroy(...) sets the "_destroy" property
    // to true.  Knockout then ignores the entity in the 'foreach' binding.
    // This removes the requirement to manage deleted Ids on the client and
    // the server.
    // NOTE:  We're not using destroy on events that have an Id of zero.
    // If the user had added a row and then realized they didn't need it,
    // we wouldn't want to ship garbage data back to the server and complicate
    // the validation.
    self.removeHand = function (hand) {
        if (hand && evt.Id()) { self.Hands.destroy(hand); }
        else { self.Hands.remove(hand); }
    };

    self.save = function () {
        $.ajax({
            url: "/Trip/" + self.TripId() + "/Crew/Edit",
            dataType: "json",
            contentType: "application/json",
            data: ko.toJSON(self),
            type: "POST",
            success: function (result) {
                toastr.success('Saved crew data');
            },
            error: function (xhr, status, error) {
                toastr.error(error, 'Failed to save crew data');
            }
        });
        //alert("Saving..." + ko.toJSON(self));
    };

    // Getting a working reload is going to require a working JSON mapping
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            amplify.request({
                resourceId: "getCrew",
                success: function (result) {
                    ko.mapping.fromJS(result, {/* Replace this with the mapping...*/}, self);
                    self.clearDirtyFlag();
                    complete();
                },
                error: function (xhr, status, error) {
                    toastr.error(error, 'Failed to reload crew');
                    complete();
                }
            });
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            amplify.request({
                resourceId: "saveCrew",
                data: ko.toJSON(self),
                success: function (result) {
                    self.clearDirtyFlag();
                    toastr.success('Crew saved');
                    complete();
                },
                error: function (xhr, status, error) {
                    toastr.error(error, 'Failed to save crew');
                    complete();
                }
            });
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });
    return self;
}