/*
 * PS-2 line item
 */
function SeaDayEvent() {
    var self = this;

    self.time = ko.observable("");
    self.latitude = ko.observable("");
    self.longitude = ko.observable("");
    self.eezCode = ko.observable("");
    self.activityCode = ko.observable("");
    self.windSpeed = ko.observable("");
    self.windDirection = ko.observable("");
    self.seaCode = ko.observable("");
    self.detectionCode = ko.observable("");
    self.associationCode = ko.observable("");
    self.fadNumber = ko.observable("");
    self.buoyNumber = ko.observable("");
    self.comments = ko.observable("");
    self.isNew = ko.observable(false);

    // http://www.barelyfitz.com/blog/archives/2006/03/07/230/
    for (var n in arguments[0]) { self[n] = arguments[0][n]; }
}

/*
 * Entire PS-2 page
 */
function SeaDayViewModel() {
    var self = this;

    self.shipsTime = ko.observable("");
    self.utcTime = ko.observable("");
    self.anchoredWithNoSchool = ko.observable("");
    self.anchoredWithSchool = ko.observable("");
    self.freeFloatingWithNoSchool = ko.observable("");
    self.freeFloatingWithSchool = ko.observable("");
    self.freeSchool = ko.observable("");
    self.hasGen3 = ko.observable("");
    self.diaryPage = ko.observable("");
    

    self.lineItems = ko.observableArray([
        new SeaDayEvent()
    ]);

    // Operations
    self.addLineItem = function () {
        // http://knockoutjs.com/documentation/hasfocus-binding.html
        // Knockout.js is awesome.  The 'hasfocus' binding allows us to set
        // focus on a newly created item, but we can set focus elsewhere
        // when the page loads.
        self.lineItems.push(new SeaDayEvent({ "isNew": true }));
    }

    self.removeLineItem = function (lineItem) { self.lineItems.remove(lineItem) }

    // TODO:  Add save operation that packages up
    // view model and ships to controller as JSON
}

ko.applyBindings(new SeaDayViewModel());