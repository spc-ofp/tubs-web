/**
 * @file Knockout ViewModel for editing a PS-2 form
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 */

/// <reference name="../underscore.js" />
/// <reference name="../knockout-2.3.0.debug.js" />
/// <reference name="../knockout.mapping-latest.debug.js" />
/// <reference name="../tubs-custom-bindings.js" />
/// <reference name="../tubs-common-extensions.js" />
/// <reference name="datacontext.js" />

/**
 * @namespace All view models are in the tubs namespace.
 */
var tubs = tubs || {};

/**
 * Knockout mapping for entire sea day (PS-2 form)
 */
tubs.psSeaDayMapping = {
    'Events': {
        create: function (options) {
            return new tubs.psEvent(options.data);
        },
        key: function (data) {
            return ko.utils.unwrapObservable(data.EventId);
        }
    },
    'ShipsDate': tubs.mappedDate,
    'UtcDate': tubs.mappedDate
};

/**
 * A single event during purse seine trip.
 * @constructor
 * @param {object} eventData - ViewModel data
 */
tubs.psEvent = function (eventData) {
    'use strict';
    var self = this;

    self.EventId = ko.observable(eventData.EventId || 0);
    self.Gen5Id = ko.observable(eventData.Gen5Id || 0);
    self.Time = ko.observable(eventData.Time || '').extend(tubs.timeExtension);
    self.Latitude =
        ko.observable(eventData.Latitude || '').extend(tubs.latitudeExtension);
    self.Longitude =
        ko.observable(eventData.Longitude || '').extend(tubs.longitudeExtension);
    self.EezCode =
        ko.observable(eventData.EezCode || '').extend({ minLength: 2, maxLength: 2 });
    self.ActivityCode = ko.observable(eventData.ActivityCode || '');
    self.WindSpeed = ko.observable(eventData.WindSpeed || null);
    self.WindDirection = ko.observable(eventData.WindDirection || null);
    self.SeaCode = ko.observable(eventData.SeaCode || '');
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
    self.HasGen5 = ko.observable(eventData.HasGen5 || false);
    self.NeedsFocus = ko.observable(eventData.NeedsFocus || false);
    self._destroy = ko.observable(eventData._destroy || false); //ignore jslint

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

/**
 * ViewModel for an an entire day at sea.
 * @constructor
 * @param {object} data - ViewModel data
 */
tubs.psSeaDay = function (data) {
    'use strict';
    var self = this;
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
    ], false, tubs.seaDayHashFunction);

    self.isAdd = ko.computed(function () {
        return (/add/i).test(self.ActionName());
    });

    // Only show the "Next Day" button for any Edit
    // or an Add that has been stored in the database
    self.showNextDayButton = ko.computed(function () {
        return !self.isAdd() || (self.DayId() !== 0);
    });

    // The clear function preps this ViewModel for being reloaded
    self.clear = function () {
        if (self.Events) {
            self.Events([]);
        }
    };

    self.isDirty = ko.computed(function () {
        var hasDirtyChild = false;
        if (self.dirtyFlag().isDirty()) {
            return true;
        }

        hasDirtyChild = _.any(self.Events(), function (evt) {
            return evt.isDirty();
        });
        return hasDirtyChild;
    });

    // Clear the dirty flag for the this entity, plus all the
    // child entities stored in the Events observableArray
    self.clearDirtyFlag = function () {
        _.each(self.Events(), function (evt) {
            evt.dirtyFlag().reset();
        });
        self.dirtyFlag().reset();
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
        if (evt && evt.EventId()) {
            self.Events.destroy(evt);
        } else {
            self.Events.remove(evt);
        }
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getSeaDay(
                self.TripId(),
                self.DayNumber(),
                function (result) {
                    self.clear();
                    ko.mapping.fromJS(result, tubs.psSeaDayMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded daily log');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload daily log', xhr, status);
                }
            );
            complete();
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });

    self.saveCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.saveSeaDay(
                self.TripId(),
                self.DayNumber(),
                self,
                function (result) {
                    self.clear();
                    ko.mapping.fromJS(result, tubs.psSeaDayMapping, self);
                    self.clearDirtyFlag();
                    toastr.success('Daily log saved');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save daily log', xhr, status);
                }
            );
            complete();
        },
        canExecute: function (isExecuting) {
            return !isExecuting && self.isDirty();
        }
    });
    return self;
};

/**
 * This function is an opt-out hash function.
 * http://stackoverflow.com/questions/4910567
 */
tubs.psSeaDayReplacer = function (key, value) {
    if (key === "IsLocked") {
        return undefined;
    }
    return value;
};

//
// After much yak-shaving, it looks like this is the easiest way
// to manage this.
// Write the state of the watched fields (set up in seaDay)
// to JSON, using the tubs.psSeaDayReplacer function to remove any
// fields down at the Event level that we don't consider
// when marking the whole day as 'dirty'.
tubs.seaDayHashFunction = function (seaDay) {
    return ko.toJSON(seaDay, tubs.psSeaDayReplacer);
};

