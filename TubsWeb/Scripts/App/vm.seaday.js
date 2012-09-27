/*
 * vm.seaday.js
 * Knockout.js ViewModel for editing a PS-2 Daily Log
 * Depends on:
 * jquery
 * json2
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
    // IsLocked has two functions:  On the UI side, it notifies the user
    // that the activity can't be deleted.  On the database side, it will be our
    // okay to delete associated data.
    self.IsLocked = ko.observable(eventData.IsLocked || false);
    self.HasSet = ko.observable(eventData.HasSet || false);
    self.NeedsFocus = ko.observable(eventData.NeedsFocus || false);
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

    self.toggleUnlock = function () {
        self.IsLocked(!self.IsLocked());
    };

    return self;
};

// This is a dirty, dirty hack!
// Convert the Knockout object to JSON string
// Then, convert the JSON string into an object
// (These two steps strip the 'observable' stuff off the object)
// Pull the last value off the end of the object
// (This assumes that .Events() is the last entity in the dirtyFlag list
// of objects to track)
// For each entity, delete an existing value of 'IsLocked'
// Then put the modified array back
// Finally, convert it to a string so it can be compared.
// Bleah!
tubs.hashFunction = function (trackedObject) {
    var stringVal = ko.toJSON(trackedObject);
    var workingCopy = JSON.parse(stringVal);
    var events = workingCopy.pop();
    $.each(events, function (index, value) {
        delete value['IsLocked'];
    });
    workingCopy.push(events);
    stringVal = JSON.stringify(workingCopy);
    return stringVal;
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
    ], false, tubs.hashFunction);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        // Check each child event, bailing on the first
        // dirty child.
        var hasDirtyChild = false;
        $.each(self.Events(), function (i, evt) {
            if (evt.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        return hasDirtyChild;
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