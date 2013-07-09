/*
* vm.gen2.js
* Knockout.js ViewModel for editing GEN-2 data
* Depends on:
* jquery
* knockout
* knockout.asyncCommand (makes it easier to show user activity)
* knockout.dirtyFlag (avoid unneccesary saves)
* knockout.activity (fancy UI gadget)
* knockout.mapping
* amplify (local storage and Ajax mapping
* toastr (user notification)
* knockout.custom-bindings (date binding)
*/

// All the view models are in the tubs namespace
var tubs = tubs || {};

tubs.Gen2Event = function (data) {
    var self = this;

    // Set common properties
    self.Id = ko.observable(data.Id || 0);
    self.TripId = ko.observable(data.TripId);
    self.PageNumber = ko.observable(data.PageNumber || 0);
    self.ShipsDate = ko.observable(data.ShipsDate || '').extend({ isoDate: 'DD/MM/YY' });
    self.ShipsTime = ko.observable(data.ShipsTime || '');
    self.Latitude =
        ko.observable(data.Latitude || '').extend({
            pattern: '^[0-8][0-9]{3}\.?[0-9]{3}[NnSs]$'
        });
    self.Longitude =
        ko.observable(data.Longitude || '').extend({
            pattern: '^[0-1]\\d{4}\.?\\d{3}[EeWw]$'
        });
    self.SpeciesCode = ko.observable(data.SpeciesCode || '');
    self.SpeciesDescription = ko.observable(data.SpeciesDescription || '');
    self.ActionName = ko.observable(data.ActionName || 'add');
    self.InteractionType = ko.observable(data.InteractionType || '');

    self.addPattern = /add/i;

    self.isAdd = ko.computed(function () {
        return self.addPattern.test(self.ActionName());
    });

    // Only show the "Next Event" button for any Edit
    // or an Add that has been stored in the database
    self.showNextEventButton = ko.computed(function () {
        return !self.isAdd() || (self.Id() !== 0);
    });

    return self;
};

tubs.Gen2LandedEvent = function (data) {
    var self = this;

    ko.utils.extend(self, new tubs.Gen2Event(data));
    // If necessary, we can add a custom mapping function
    ko.mapping.fromJS(data, {}, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.ShipsDate,
        self.ShipsTime,
        self.Latitude,
        self.Longitude,
        self.SpeciesCode,
        self.SpeciesDescription,
        self.LandedConditionCode,
        self.LandedConditionDescription,
        self.LandedHandling,
        self.LandedLength,
        self.LandedLengthCode,
        self.LandedSexCode,
        self.DiscardedConditionCode,
        self.DiscardedConditionDescription,
        self.RetrievedTagNumber,
        self.RetrievedTagType,
        self.RetrievedTagOrganization,
        self.PlacedTagNumber,
        self.PlacedTagType,
        self.PlacedTagOrganization,
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getInteraction(
                self.TripId(),
                self.PageNumber(),
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded GEN-2 event');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload GEN-2 event', xhr, status);
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
            tubs.saveInteraction(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.success('Saved GEN-2');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to save GEN-2', xhr, status);
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

tubs.Gen2GearMapping = {
    'StartOfInteraction': {
        create: function (options) {
            return new tubs.Gen2SpeciesGroup(options.data);
        }
    },
    'EndOfInteraction': {
        create: function (options) {
            return new tubs.Gen2SpeciesGroup(options.data);
        }
    }
};

tubs.Gen2SpeciesGroup = function (data) {
    var self = this;
    self.Id = ko.observable(data.Id || 0);
    self.Count = ko.observable(data.Count || null);
    self.ConditionCode = ko.observable(data.ConditionCode || '');
    self.Description = ko.observable(data.Description || '');
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);
    self._destroy = ko.observable(data._destroy || false); //ignore jslint

    self.dirtyFlag = new ko.DirtyFlag([
        self.Count,
        self.ConditionCode,
        self.Description,
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    return self;
};

tubs.Gen2GearEvent = function (data) {
    var self = this;

    ko.utils.extend(self, new tubs.Gen2Event(data));
    // If necessary, we can add a custom mapping function
    ko.mapping.fromJS(data, tubs.Gen2GearMapping, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.ShipsDate,
        self.ShipsTime,
        self.Latitude,
        self.Longitude,
        self.SpeciesCode,
        self.SpeciesDescription,
        self.VesselActivity,
        self.VesselActivityDescription,
        self.StartOfInteraction, // Add/Remove only
        self.EndOfInteraction, // Add/Remove only
        self.InteractionDescription,
    ], false);

    self.isDirty = ko.computed(function () {
        // Avoid iterating over the events if the header
        // has changed
        if (self.dirtyFlag().isDirty()) { return true; }
        // Check each child event, bailing on the first
        // dirty child.
        var hasDirtyChild = false;
        $.each(self.StartOfInteraction(), function (i, evt) { //ignore jslint
            if (evt.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        if (hasDirtyChild) { return true; }
        $.each(self.EndOfInteraction(), function (i, evt) { //ignore jslint
            if (evt.isDirty()) {
                hasDirtyChild = true;
                return false;
            }
        });
        return hasDirtyChild;
    });

    self.clearDirtyFlag = function () {
        $.each(self.StartOfInteraction(), function (index, value) { //ignore jslint
            value.dirtyFlag().reset();
        });
        $.each(self.EndOfInteraction(), function (index, value) { //ignore jslint
            value.dirtyFlag().reset();
        });
        self.dirtyFlag().reset();
    };

    self.addStartItem = function () {
        self.StartOfInteraction.push(new tubs.Gen2SpeciesGroup({ "NeedsFocus": true }));
    };

    self.removeStartItem = function (evt) {
        if (evt && evt.Id()) {
            self.StartOfInteraction.destroy(evt);
        } else {
            self.StartOfInteraction.remove(evt);
        }
    };

    self.addEndItem = function () {
        self.EndOfInteraction.push(new tubs.Gen2SpeciesGroup({ "NeedsFocus": true }));
    };

    self.removeEndItem = function (evt) {
        if (evt && evt.Id()) {
            self.EndOfInteraction.destroy(evt);
        } else {
            self.EndOfInteraction.remove(evt);
        }
    };

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getInteraction(
                self.TripId(),
                self.PageNumber(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.Gen2GearMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded GEN-2 event');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload GEN-2 event', xhr, status);
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
            tubs.saveInteraction(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.Gen2GearMapping, self);
                    self.clearDirtyFlag();
                    toastr.success('Saved GEN-2');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to save GEN-2', xhr, status);
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

tubs.GenSightedEvent = function (data) {
    var self = this;
    ko.utils.extend(self, new tubs.Gen2Event(data));
    // If necessary, we can add a custom mapping function
    ko.mapping.fromJS(data, {}, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.ShipsDate,
        self.ShipsTime,
        self.Latitude,
        self.Longitude,
        self.SpeciesCode,
        self.SpeciesDescription,
        self.VesselActivity,
        self.VesselActivityDescription,
        self.NumberSighted,
        self.NumberOfAdults,
        self.NumberOfJuveniles,
        self.SightingLength,
        self.SightingDistance,
        self.SightingBehavior,
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    // Commands
    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getInteraction(
                self.TripId(),
                self.PageNumber(),
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded GEN-2 event');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload GEN-2 event', xhr, status);
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
            tubs.saveInteraction(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, {}, self);
                    self.clearDirtyFlag();
                    toastr.success('Saved GEN-2');
                    complete();
                },
                function (xhr, status) {
                    tubs.notify('Failed to save GEN-2', xhr, status);
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