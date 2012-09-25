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
// https://github.com/ericmbarnard/Knockout-Validation
// https://github.com/ericmbarnard/Knockout-Validation/wiki/Native-Rules
//
tubs.psEvent = function (eventData) {
    var self = this;
    self.EventId = ko.observable(eventData.EventId || 0);
    self.Time = ko.observable(eventData.Time || '').extend({ required: true, pattern: '^[0-2][0-9][0-5][0-9]$' });
    self.Latitude = ko.observable(eventData.Latitude || '').extend({ pattern: '^[0-8][0-9]{3}\.?[0-9]{3}[NnSs]$' });
    self.Longitude = ko.observable(eventData.Longitude || '').extend({ pattern: '^[0-1]\\d{4}\.?\\d{3}[EeWw]$' });
    self.EezCode = ko.observable(eventData.EezCode || '').extend({ minLength: 2, maxLength: 2 });
    self.ActivityCode = ko.observable(eventData.ActivityCode || '');
    self.WindSpeed = ko.observable(eventData.WindSpeed || '').extend({ min: 0 }); // No negative wind speeds
    self.WindDirection = ko.observable(eventData.WindDirection || '').extend({ min: 0, max: 360 });
    self.SeaCode = ko.observable(eventData.SeaCode || '');
    // NOTE:  Knockout is very sensitive to data types
    // If the value is coerced to a string, that will count as a 'change'
    // In this case, I settled on nullable int in the viewModel, with an actual
    // JavaScript null if no value is provided (probably can get away without, but it's a good reminder
    // in the event I switch back to a string)
    self.DetectionCode = ko.observable(eventData.DetectionCode || null);
    self.AssociationCode = ko.observable(eventData.AssociationCode || null);
    self.FadNumber = ko.observable(eventData.FadNumber || '');
    self.BuoyNumber = ko.observable(eventData.BuoyNumber || '');
    self.Comments = ko.observable(eventData.Comments || '');
    self.HasSet = ko.observable(eventData.HasSet || false);
    self.NeedsFocus = ko.observable(eventData.NeedsFocus || false);
    // If there's an associated set, force the user to affirm that
    // the activity should be deleted.
    self.CanDeleteAnswer = ko.observable(eventData.CanDeleteAnswer || '');
    self._destroy = ko.observable(eventData._destroy || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Time,
        self.Latitude,
        self.Longitude,
        self.EezCode,
        self.ActivityCode,
        self.WindSpeed,
        self.WindDirection,
        self.SeaCode,
        self.DetectionCode,
        self.AssociationCode,
        self.FadNumber,
        self.BuoyNumber,
        self.Comments
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

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
        self.Events // This is only for the add/remove
    ], false);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        // TODO It might be better to for-each this and bail
        // out of the loop on the first dirty element
        var dirtyEvents = $.grep(self.Events(), function (n, i) {
            return (!n.isDirty());
        });
        return dirtyEvents.length === 0;
    });

    // Clear the dirty flag for the this entity, plus all the
    // child entities stored in the Events observableArray
    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
        $.each(self.Events(), function (index, value) {
            value.dirtyFlag().reset();
        });
    };

    // Operations
    // Create a new PS-2 line item and tell the UI that it needs focus
    // This is handy for mouse-free data entry
    self.addEvent = function () {
        self.Events.push(new tubs.psEvent({ "NeedsFocus": true }));
    };

    // This function takes advantage of the RoR integration
    // in Knockout.  A call to .destroy(...) sets the "_destroy" property
    // to true.  Knockout then ignores the entity in the 'foreach' binding.
    // This removes the requirement to manage deleted Ids on the client and
    // the server.
    // NOTE:  We're not using destroy on events that have an Id of zero.
    // If the user had added a row and then realized they didn't need it,
    // we wouldn't want to ship garbage data back to the server and complicate
    // the validation.
    self.removeEvent = function (evt) {
        if (evt && evt.EventId()) { self.Events.destroy(evt); }
        else { self.Events.remove(evt); }
    }

    self.reload = function () {
        amplify.request("getSeaDay", function (data) {
            // http://jsfiddle.net/jearles/wgZ59/49/
            // NOTE:  This only works if the entity is mappable from JSON
            // Even then, there might still be some issues
            // TODO:  Test the hell out of this!
            ko.mapping.fromJS(data, tubs.psSeaDayMapping, self);
            self.clearDirtyFlag();
        });
    }

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            amplify.request({
                resourceId: "getSeaDay",
                success: function (result) {
                    ko.mapping.fromJS(result, tubs.psSeaDayMapping, self);
                    self.clearDirtyFlag();
                    complete();
                },
                error: function (xhr, status, error) {
                    toastr.error(error, 'Failed to reload daily log');
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
                resourceId: "saveSeaDay",
                data: ko.toJSON(self),
                success: function (result) {
                    self.clearDirtyFlag();
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
    });
    return self;
};