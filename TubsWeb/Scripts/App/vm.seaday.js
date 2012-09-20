/*
 * vm.seaday.js
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

// This mapping is used to override the default
// JSON mapper for the named property (or properties).
// In this case, the 'Events' property of the mapped JSON
// object are converted via the anonymous function in
// 'create'.
tubs.psSeaDayMapping = {
    'Events': {
        create: function (options) {
            return new tubs.psEvent(options.data);
        }
    }
};

//
// Adding validation is much easier via manual mapping
//
tubs.psEvent = function (eventData) {
    var self = this;
    self.EventId = ko.observable(eventData.EventId || 0);
    self.Time = ko.observable(eventData.Time || '').extend({ required: true, pattern: '^[0-2]\d[0-5]\d$' });
    self.Latitude = ko.observable(eventData.Latitude || '').extend({ pattern: '^[0-8]\d{3}\.?\d{3}[NnSs]$' });
    self.Longitude = ko.observable(eventData.Longitude || '').extend({ pattern: '^[0-1]\d{4}\.?\d{3}[EeWw]$' });
    self.EezCode = ko.observable(eventData.EezCode || '').extend({ minLength: 2, maxLength: 2});
    self.ActivityCode = ko.observable(eventData.ActivityCode || '');
    self.WindSpeed = ko.observable(eventData.WindSpeed || '').extend({ min: 0}); // No negative wind speeds
    self.WindDirection = ko.observable(eventData.WindDirection || '').extend({ min: 0, max: 360 });
    self.SeaCode = ko.observable(eventData.SeaCode || '');
    self.DetectionCode = ko.observable(eventData.DetectionCode || '');
    self.AssociationCode = ko.observable(eventData.AssociationCode || '');
    self.FadNumber = ko.observable(eventData.FadNumber || '');
    self.BuoyNumber = ko.observable(eventData.BuoyNumber || '');
    self.Comments = ko.observable(eventData.Comments || '');
    self.NeedsFocus = ko.observable(eventData.NeedsFocus || false);

    return self;
};

// This is the actual Purse Seine Sea Day view model
// Any functions/properties/etc. that belong on the view model
// are defined here.
tubs.psSeaDay = function (data) {
    var self = this;
    // Map the incoming JSON in 'data' to self, using
    // the options in psSeaDayMapping
    ko.mapping.fromJS(data, tubs.psSeaDayMapping, self);

    // Keep track of any events that need to be deleted
    // TODO Confirm that toJS will serialize a non observable field.
    self.DeletedEvents = ko.observableArray([]);

    // Define the fields that are watched to determine the 'dirty' state
    self.dirtyFlag = new ko.DirtyFlag([
        self.ShipsDate,
        self.ShipsTime,
        self.UtcDate,
        self.UtcTime,
        self.AnchoredWithNoSchool,
        self.AnchoredWithSchool,
        self.FreeFloatingWithNoSchool,
        self.FreeFloatingWithSchool,
        self.FreeSchool,
        self.HasGen3Event,
        self.DiaryPage,
        self.Events // This is currently only add/remove
    ]);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    // Operations
    // Create a new PS-2 line item and tell the UI that it needs focus
    // This is handy for mouse-free data entry
    self.addEvent = function () {
        self.Events.push(new tubs.psEvent({ "NeedsFocus": true }));
    };

    self.removeEvent = function (evt) {
        if (evt && evt.EventId()) { self.DeletedEvents.push(evt.EventId()); }
        self.Events.remove(evt);
    };

    self.reload = function () {
        amplify.request("getSeaDay", function (data) {
            // http://jsfiddle.net/jearles/wgZ59/49/
            // NOTE:  This only works if the entity is mappable from JSON
            // Even then, there might still be some issues
            // TODO:  Test the hell out of this!
            ko.mapping.fromJS(data, tubs.psSeaDayMapping, self);
            self.dirtyFlag().reset();
        });
    }

    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            amplify.request({
                resourceId: "saveSeaDay",
                data: ko.toJSON(self),
                success: function (result) {
                    self.dirtyFlag().reset();
                    toastr.success('Daily log saved');
                    complete();
                },
                error: function (xhr, status, error) {
                    toastr.error(error, 'Failed to save daily log');
                    complete();
                }
            });
        },

        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    })

    return self;
}