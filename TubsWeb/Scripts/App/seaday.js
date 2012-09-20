/// <reference path="~/Scripts/knockout-2.1.0.js" />
var tubsSeaDay = tubsSeaDay = tubsSeaDay || {};

// NOTE:  I'm violating JavaScript conventions
// for property naming to match up with the names in the
// CLR ViewModel.

// Basic activity entity
tubsSeaDay.Activity = function (data) {
    var self = this;
    // UX property
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);
}

tubsSeaDay.SeaDayViewModel = function (data) {
    var self = this;

    self.TripId = ko.observable(data.TripId || 0);
    self.DayId = ko.observable(data.DayId || 0);

    var tmpActivities = [];
    $.map(data.Events, function (n, i) {
        tmpActivities.push(new tubsSeaDay.Activity(n));
    });
    self.Events = ko.observableArray(tmpActivities);
}