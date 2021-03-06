﻿/**
 * @file Central location for defining data services.
 * Uses Amplify.js for loading data, jQuery
 * AJAX for saving.
 * @author Corey Cole <corey.cole@gmail.com>
 * @copyright Secretariat of the Pacific Community, 2013
 */

/// <reference path="../amplify.js" />
var tubs = tubs || {};

tubs.saveTimeout = 10000; /* 10 seconds */

// Since we're only using AmplifyJS for (re)load
// requests, the settings are very similar across
// all the definitions.  Using this for all
// requests ensures that timeout is consistent.
tubs.defaultNetworkSettings = {
    dataType: 'json',
    type: 'GET',
    timeout: 10000 /* 10 seconds */
};

// TODO appBase and start are still in the global namespace
var appBase = '/';
var start = $('#applicationHome');
if (start) {
    appBase = start.attr('href');
}

// Request for loading PS-2 data
amplify.request.define(
    "getSeaDay",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/Days/{DayNumber}/Edit' }
    )
);

// Request for loading PS-1 crew data
amplify.request.define(
    "getCrew",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/Crew/Edit' }
    )
);

// Request for loading PS-3 data
amplify.request.define(
    "getFishingSet",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/Sets/{SetNumber}/Edit' }
    )
);

// Request for loading PS-1/LL-1 electronics data
amplify.request.define(
    "getElectronics",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/Electronics/Edit' }
    )
);

// Request for loading GEN-1 (sighting) data
amplify.request.define(
    "getSightings",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/Sightings' }
    )
);

// Request for loading GEN-1 (transfer) data
amplify.request.define(
    "getTransfers",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/Transfers' }
    )
);

// Request for loading GEN-2 data
amplify.request.define(
    "getInteraction",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/GEN-2/{PageNumber}/Edit' }
    )
);

// Request for loading GEN-3 data
amplify.request.define(
    "getGen3",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/GEN-3' }
    )
);

// Request for loading GEN-5 (transfer) data
amplify.request.define(
    "getFad",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/GEN-5/Details?fadId={FadId}' }
    )
);

// Request for loading simple PS-1 data (not crew, wells, or electronics)
amplify.request.define(
    "getPs1",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/PS-1/Index' }
    )
);

// Request for loading simple LL-1 data (not electronics)
amplify.request.define(
    "getTripInfo",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/LL-1/Index' }
    )
);


// Request for loading page count data
amplify.request.define(
    "getPageCounts",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/PageCount' }
    )
);

// Request for loading LL-2/3 data
amplify.request.define(
    "getSetHaul",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/SetHaul/{SetNumber}/Edit' }
    )
);

// Request for loading LL-4 data
amplify.request.define(
    "getSampleLL",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/LL-4/{SetNumber}/Edit' }
    )
);

// Request for loading trip track
amplify.request.define(
    "getTripTrack",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/track.geojson' }
    )
);

// Request for loading trip track
amplify.request.define(
    "getTripPositions",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/positions.geojson' }
    )
);

// Request for loading Well Content data
amplify.request.define(
    "getWellNumber",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/WellContent/Edit' }
    )
);

// Request for loading PS-4 data
amplify.request.define(
    "getSampleHeader",
    "ajax",
    $.extend(
        tubs.defaultNetworkSettings,
        { url: appBase + 'Trip/{TripId}/PS-4/{SetNumber}/{PageNumber}/Edit' }
    )
);

/**
 * Load PS-2 data for a given sea day.
 * @param {Number} tripId Trip primary key
 * @param {Number} dayNumber Location of the day within the trip
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getSeaDay = function (tripId, dayNumber, success_cb, error_cb) {
    amplify.request({
        resourceId: "getSeaDay",
        data: { "TripId": tripId, "DayNumber": dayNumber },
        success: success_cb,
        error: error_cb
    });
};

tubs.getWellContent = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getWellNumber",
        data: { "TripId": tripId},
        success: success_cb,
        error: error_cb
    });
};

/**

 */
tubs.saveWellContent = function (tripId, wellContent, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/WellContent/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(wellContent),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
};

/**
 * Save PS-2 data for a given sea day.
 * @param {Number} tripId Trip primary key
 * @param {Number} dayNumber Location of the day within the trip
 * @param seaDay Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveSeaDay = function (tripId, dayNumber, seaDay, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/Days/' + dayNumber + '/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(seaDay),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
};

/**
 * Load LL-2/3 data for a given set.
 * @param {Number} tripId Trip primary key
 * @param {Number} setNumber Location of the set within the trip
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getSetHaul = function (tripId, setNumber, success_cb, error_cb) {
    amplify.request({
        resourceId: "getSetHaul",
        data: { "TripId": tripId, "SetNumber": setNumber },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save LL-2/3 data for a given set.
 * @param {Number} tripId Trip primary key
 * @param {Number} setNumber Location of the set within the trip
 * @param seaDay Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveSetHaul = function (tripId, setNumber, setHaul, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/SetHaul/' + setNumber + '/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(setHaul),
        timeout: tubs.saveTimeout
    }).done(success_cb)
        .fail(error_cb);
};


/**
 * Load crew for a given trip
 * @param {Number} tripId Trip primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getCrew = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getCrew",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save crew data for a given trip.
 * @param {Number} tripId Trip primary key
 * @param crew Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveCrew = function (tripId, crew, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/Crew/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(crew),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
    // TODO Use promise API
    //.done(success_cb)
    //.fail(error_cb);
};

/**
 * Load electronics for a given trip
 * @param {Number} tripId Trip primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getElectronics = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getElectronics",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save electronics data for a given trip.
 * @param {Number} tripId Trip primary key
 * @param electronics Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveElectronics = function (tripId, electronics, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/Electronics/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(electronics),
        timeout: tubs.saveTimeout
    }).done(success_cb)
        .fail(error_cb);
};

/**
 * Get PS-3 data for a given fishing set.
 * @param {Number} tripId Trip primary key
 * @param {Number} setNumber Location of the set within the trip
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getFishingSet = function (tripId, setNumber, success_cb, error_cb) {
    amplify.request({
        resourceId: "getFishingSet",
        data: { "TripId": tripId, "SetNumber": setNumber },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save PS-3 data for a given fishing set.
 * @param {Number} tripId Trip primary key
 * @param {Number} setNumber Location of the set within the trip
 * @param fishingSet Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveFishingSet = function (tripId, setNumber, fishingSet, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/Sets/' + setNumber + '/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(fishingSet),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
    // TODO Use promise API
    //.done(success_cb)
    //.fail(error_cb);
};

/**
 * Get GEN-1 Sightings for a given trip.
 * @param {Number} tripId Trip primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getSightings = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getSightings",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save GEN-1 Sightings for a given trip.
 * @param {Number} tripId Trip primary key
 * @param sightings Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveSightings = function (tripId, sightings, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/Sightings/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(sightings),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
    // TODO Use promise API
    //.done(success_cb)
    //.fail(error_cb);
};

/**
 * Get GEN-1 Transfers for a given trip.
 * @param {Number} tripId Trip primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getTransfers = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getTransfers",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save GEN-1 Transfers for a given trip.
 * @param {Number} tripId Trip primary key
 * @param transfers Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveTransfers = function (tripId, transfers, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/Transfers/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(transfers),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
    // TODO Use promise API
    //.done(success_cb)
    //.fail(error_cb);
};

/**
 * Get GEN-2 interaction for a given trip and page number.
 * @param {Number} tripId Trip primary key
 * @param {Number} pageNumber Location within the pages of GEN-2 data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getInteraction = function (tripId, pageNumber, success_cb, error_cb) {
    amplify.request({
        resourceId: "getInteraction",
        data: { "TripId": tripId, "PageNumber": pageNumber },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save GEN-2 interaction for a given trip.
 * @param {Number} tripId Trip primary key
 * @param interaction Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveInteraction = function (tripId, interaction, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/GEN-2/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(interaction),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
    // TODO Use promise API
    //.done(success_cb)
    //.fail(error_cb);
};

/**
 * Get GEN-5 details for a given trip and FAD record.
 * @param {Number} tripId Trip primary key
 * @param {Number} fadId FAD primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getFad = function (tripId, fadId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getFad",
        data: { "TripId": tripId, "FadId": fadId },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save GEN-5 FAD for a given trip.
 * @param {Number} tripId Trip primary key
 * @param fad Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveFad = function (tripId, fad, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/GEN-5/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(fad),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
    // TODO Use promise API
    //.done(success_cb)
    //.fail(error_cb);
};

/**
 * Get PS-1 details for a given trip.
 * @param {Number} tripId Trip primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getPs1 = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getPs1",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save PS-1 details for a given trip.
 * @param {Number} tripId Trip primary key
 * @param ps1 Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.savePs1 = function (tripId, ps1, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/PS-1/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(ps1),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
    // TODO Use promise API
    //.done(success_cb)
    //.fail(error_cb);
};

/**
 * Get LL-1 details for a given trip.
 * @param {Number} tripId Trip primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getTripInfo = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getTripInfo",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save LL-1 details for a given trip.
 * @param {Number} tripId Trip primary key
 * @param tripInfo Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveTripInfo = function (tripId, tripInfo, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/LL-1/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(tripInfo),
        timeout: tubs.saveTimeout
    })
        .done(success_cb)
        .fail(error_cb);
};



/**
 * Get page counts for a given trip.
 * @param {Number} tripId Trip primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getPageCounts = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getPageCounts",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save PS-1 details for a given trip.
 * @param {Number} tripId Trip primary key
 * @param pageCounts Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.savePageCounts = function (tripId, pageCounts, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/PageCount/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(pageCounts),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
    // TODO Use promise API
    //.done(success_cb)
    //.fail(error_cb);
};

/**
 * Get GEN-3 details for a given trip.
 * @param {Number} tripId Trip primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getGen3 = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getGen3",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save GEN-3 details for a given trip.
 * @param {Number} tripId Trip primary key
 * @param gen3 Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveGen3 = function (tripId, gen3, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/GEN-3/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(gen3),
        success: success_cb,
        error: error_cb,
        timeout: tubs.saveTimeout
    });
    // TODO Use promise API
    //.done(success_cb)
    //.fail(error_cb);
};

/**
 * Get LL-4 data for a given fishing set.
 * @param {Number} tripId Trip primary key
 * @param {Number} setNumber Location of the set within the trip
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getLonglineSample = function (tripId, setNumber, success_cb, error_cb) {
    amplify.request({
        resourceId: "getSampleLL",
        data: { "TripId": tripId, "SetNumber": setNumber },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save LL-4 data for a given fishing set.
 * @param {Number} tripId Trip primary key
 * @param {Number} setNumber Location of the set within the trip
 * @param sample Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.saveLonglineSample = function (tripId, setNumber, sample, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/LL-4/' + setNumber + '/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(sample),
        timeout: tubs.saveTimeout
    }).done(success_cb)
        .fail(error_cb);
};

/**
 * Get PS-4 header data for a given fishing set and page
 * @param {Number} tripId Trip primary key
 * @param {Number} setNumber Location of the set within the trip
 * @param {Number} pageNumber Location of form within the current set
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getPs4Header = function (tripId, setNumber, pageNumber, success_cb, error_cb) {
    amplify.request({
        resourceId: "getSampleHeader",
        data: { "TripId": tripId, "SetNumber": setNumber, "PageNumber": pageNumber },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Save PS-4 header data for a given fishing set.
 * @param {Number} tripId Trip primary key
 * @param {Number} setNumber Location of the set within the trip
 * @param {Number} pageNumber Location of form within the current set
 * @param header Knockout view model containing data
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.savePs4Header = function (tripId, setNumber, pageNumber, header, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/PS-4/' + setNumber + '/' + pageNumber + '/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(header),
        timeout: tubs.saveTimeout
    }).done(success_cb)
        .fail(error_cb);
};


/**
 * Get track for a given trip.
 * @param {Number} tripId Trip primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getTripTrack = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getTripTrack",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};

/**
 * Get all positions for a given trip.
 * @param {Number} tripId Trip primary key
 * @param success_cb Callback that handles returned data
 * @param error_cb Callback that handles error situation
 */
tubs.getTripPositions = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getTripPositions",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};