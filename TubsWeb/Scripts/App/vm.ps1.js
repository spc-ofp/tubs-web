/** 
 * @file Knockout ViewModel for editing a subset of PS-1 data
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * knockout.js
 * underscore.js
 * knockout.mapping plugin
 * KoLite plugins (asyncCommand, activity, dirtyFlag)
 * toastr
 */

/// <reference name="../underscore.js" />
/// <reference name="../knockout-2.3.0.debug.js" />
/// <reference path="../knockout.mapping-latest.js" />
/// <reference path="datacontext.js" />

/**
 * @namespace All view models are in the tubs namespace.
 */
var tubs = tubs || {};

/**
 * Knockout mapping for entire view model.
 */
tubs.Ps1Mapping = {
    'Characteristics': {
        create: function (options) {
            return new tubs.Ps1VesselCharacteristics(options.data);
        }
    },
    'Gear': {
        create: function (options) {
            return new tubs.Ps1Gear(options.data);
        }
    },
    'Inspection': {
        create: function (options) {
            return new tubs.SafetyInspection(options.data);
        }
    }
};

/**
 * Vessel data as recorded on PS-1 form.
 * @constructor
 * @param {object} data - Vessel data
 */
tubs.Ps1VesselCharacteristics = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, {}, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Owner,
        self.RegistrationNumber,
        self.CountryCode,
        self.Ircs,
        self.Length,
        self.LengthUnits,
        self.GrossTonnage,
        self.SpeedboatCount,
        self.AuxiliaryBoatCount,
        self.TenderBoatAnswer,
        self.SkiffMake,
        self.SkiffPower,
        self.CruiseSpeed,
        self.HelicopterMake,
        self.HelicopterModel,
        self.HelicopterRegistration,
        self.HelicopterRange,
        self.HelicopterRangeUnits,
        self.HelicopterColor,
        self.HelicopterServiceOtherCount
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    return self;
};

/**
 * Fishing gear as recorded on PS-1 form.
 * @constructor
 * @param {object} data - Gear data
 */
tubs.Ps1Gear = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, {}, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.PowerblockMake,
        self.PowerblockModel,
        self.PurseWinchMake,
        self.PurseWinchModel,
        self.NetDepth,
        self.NetDepthUnits,
        self.NetLength,
        self.NetLengthUnits,
        self.NetStripCount,
        self.NetMeshSize,
        self.NetMeshUnits,
        self.Brail1Capacity,
        self.Brail2Capacity,
        self.BrailType
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    return self;
};

/**
 * Safety gear as recorded on PS-1 form.
 * @constructor
 * @param {object} data - Safety gear data
 */
tubs.SafetyInspection = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, {}, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.LifejacketProvided,
        self.LifejacketSizeOk,
        self.LifejacketAvailability,
        self.BuoyCount,
        self.Epirb406Count,
        self.Epirb406Expiration,
        self.OtherEpirbType,
        self.OtherEpirbCount,
        self.OtherEpirbExpiration,
        self.LifeRaft1Capacity,
        self.LifeRaft1Inspection,
        self.LifeRaft1LastOrDue,
        self.LifeRaft2Capacity,
        self.LifeRaft2Inspection,
        self.LifeRaft2LastOrDue,
        self.LifeRaft3Capacity,
        self.LifeRaft3Inspection,
        self.LifeRaft3LastOrDue,
        self.LifeRaft4Capacity,
        self.LifeRaft4Inspection,
        self.LifeRaft4LastOrDue
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    self.clearDirtyFlag = function () {
        self.dirtyFlag().reset();
    };

    return self;
};

/**
 * View model for PS-1 form
 * @constructor
 * @param {object} data - PS-1 data
 */
tubs.Ps1ViewModel = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, tubs.Ps1Mapping, self);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Page1Comments,
        self.HasWasteDisposal,
        self.WasteDisposalDescription,
        self.PermitNumbers,
        self.Gear,
        self.Characteristics,
        self.Inspection
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    // Clear the dirty flag for the this entity, plus all the
    // child entities
    self.clearDirtyFlag = function () {
        self.Gear.clearDirtyFlag();
        self.Characteristics.clearDirtyFlag();
        self.Inspection.clearDirtyFlag();
        self.dirtyFlag().reset();
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getPs1(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.Ps1Mapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded PS-1 details');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload PS-1 details', xhr, status);
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
            tubs.savePs1(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.Ps1Mapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved PS-1 details');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save PS-1 details', xhr, status);
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
