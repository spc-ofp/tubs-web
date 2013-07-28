/** 
 * @file Knockout ViewModel for purse seine crew
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
 * Knockout mapping for a single crew member.
 * Uses the tubs.CrewMember constructor to create
 * a crew member from data, and the primary key
 * (Id field) to identify an existing object.
 */
tubs.psCrewMemberMapping = {
    create: function (options) {
        return new tubs.CrewMember(options.data);
    },
    key: function (data) {
        return ko.utils.unwrapObservable(data.Id);
    }
}

/**
 * Knockout mapping for the entire crew.
 * Uses the shared tubs.psCrewMemberMapping for each
 * crew member.
 */
tubs.psCrewMapping = {
    'Hands': tubs.psCrewMemberMapping,
    'Captain': tubs.psCrewMemberMapping,
    'Navigator': tubs.psCrewMemberMapping,
    'Mate': tubs.psCrewMemberMapping,
    'ChiefEngineer': tubs.psCrewMemberMapping,
    'AssistantEngineer': tubs.psCrewMemberMapping,
    'DeckBoss': tubs.psCrewMemberMapping,
    'Cook': tubs.psCrewMemberMapping,
    'HelicopterPilot': tubs.psCrewMemberMapping,
    'SkiffMan': tubs.psCrewMemberMapping,
    'WinchMan': tubs.psCrewMemberMapping
}

/**
 * Purse seine crew member
 * @constructor
 * @param {object} data - Crew member data
 */
tubs.CrewMember = function (data) {
    'use strict';
    var self = this;
    self.Id = ko.observable(data.Id || 0);
    self.Job = ko.observable(data.Job); // Numeric enum value, not string description
    self.Name = ko.observable(data.Name);
    self.Nationality = ko.observable(data.Nationality).extend({ maxLength: 2 });
    self.Years = ko.observable(data.Years);
    self.Comments = ko.observable(data.Comments);
    self.NeedsFocus = ko.observable(data.NeedsFocus || false);

    self.dirtyFlag = new ko.DirtyFlag([
        self.Name,
        self.Nationality,
        self.Years,
        self.Comments
    ], false);

    self.isDirty = ko.computed(function () {
        return self.dirtyFlag().isDirty();
    });

    return self;
};

/**
 * View model representing all crew for a single trip.
 * @constructor
 * @param {object} data - Crew data
 */
tubs.psCrewViewModel = function (data) {
    'use strict';
    var self = this;
    ko.mapping.fromJS(data, tubs.psCrewMapping, self);
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
        if (self.dirtyFlag().isDirty()) { return true; }

        return _.any(self.Hands(), function (hand) { //ignore jslint
            return hand.isDirty();
        });
    });

    self.clearDirtyFlag = function () {        
        _.each(self.Hands(), function (hand) { //ignore jslint
            hand.dirtyFlag().reset();
        });
        self.dirtyFlag().reset();
    };

    self.addHand = function () {
        self.Hands.push(new tubs.CrewMember({ "NeedsFocus": true }));
    };

    self.removeHand = function (hand) {
        if (hand && hand.Id()) {
            self.Hands.destroy(hand);
        } else {
            self.Hands.remove(hand);
        }
    };

    self.reloadCommand = ko.asyncCommand({
        execute: function (complete) {
            tubs.getCrew(
                self.TripId(),
                function (result) {
                    ko.mapping.fromJS(result, tubs.psCrewMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Reloaded crew details');
                },
                function (xhr, status) {
                    tubs.notify('Failed to reload crew', xhr, status);
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
            tubs.saveCrew(
                self.TripId(),
                self,
                function (result) {
                    ko.mapping.fromJS(result, tubs.psCrewMapping, self);
                    self.clearDirtyFlag();
                    toastr.info('Saved crew details');
                },
                function (xhr, status) {
                    tubs.notify('Failed to save crew details', xhr, status);
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