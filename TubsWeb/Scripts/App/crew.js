/// <reference path="~/Scripts/knockout-2.1.0.js" />
var tubsCrew = tubsCrew = tubsCrew || {};

// NOTE:  I'm violating JavaScript conventions
// for property naming to match up with the names in the
// CLR ViewModel.

// Basic crewmember entity
tubsCrew.CrewMember = function (data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.Job = ko.observable(data.Job); // Numeric enum value, not string description
    self.Name = ko.observable(data.Name);
    self.Nationality = ko.observable(data.Nationality);
    self.Years = ko.observable(data.Years);
    self.Comments = ko.observable(data.Comments);
    // This is used to set focus on the most recently added CrewMember
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);
}

// ViewModel for purse seine crew (some of these jobs don't apply to other methods)
tubsCrew.PSCrewViewModel = function (data) {
    var self = this;

    self.TripId = ko.observable(data.TripId || 0);

    // Senior Crew
    self.Captain = ko.observable(new tubsCrew.CrewMember(data.Captain));
    self.Navigator = ko.observable(new tubsCrew.CrewMember(data.Navigator));
    self.Mate = ko.observable(new tubsCrew.CrewMember(data.Mate));
    self.ChiefEngineer = ko.observable(new tubsCrew.CrewMember(data.ChiefEngineer));
    self.AssistantEngineer = ko.observable(new tubsCrew.CrewMember(data.AssistantEngineer));
    self.DeckBoss = ko.observable(new tubsCrew.CrewMember(data.DeckBoss));
    self.Cook = ko.observable(new tubsCrew.CrewMember(data.Cook));
    self.HelicopterPilot = ko.observable(new tubsCrew.CrewMember(data.HelicopterPilot));
    self.SkiffMan = ko.observable(new tubsCrew.CrewMember(data.SkiffMan));
    self.WinchMan = ko.observable(new tubsCrew.CrewMember(data.WinchMan));

    // Other Crew
    var tmpHands = [];
    $.map(data.Hands, function (n, i) {
        tmpHands.push(new tubsCrew.CrewMember(n));
    });
    self.Hands = ko.observableArray(tmpHands);

    // Operations
    self.addHand = function () {
        // http://knockoutjs.com/documentation/hasfocus-binding.html
        // Knockout.js is awesome.  The 'hasfocus' binding allows us to set
        // focus on a newly created item, but we can set focus elsewhere
        // when the page loads.
        self.Hands.push(new tubsCrew.CrewMember({ "NeedsFocus": true }));
    }

    self.removeHand = function (hand) { self.Hands.remove(hand) }

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
    }

}
