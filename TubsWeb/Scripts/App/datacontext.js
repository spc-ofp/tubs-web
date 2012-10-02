/*
 * datacontext.js provides a central location
 * for defining amplify requests.
 * Assumes Amplify.js is loaded (duh!)
 */

/// <reference path="../amplify.js" />
var tubs = tubs || {};

/*
 * After much screwing around, this looks to be
 * a portable solution.  MVC puts the virtual directory
 * app root into every page for us.
 */
var appBase = '/';
var start = $('#applicationHome');
if (start) {
    appBase = start.attr('href');
}

amplify.request.define("getSeaDay", "ajax", {
url: appBase + 'Trip/{TripId}/Days/{DayNumber}/Edit',
dataType: 'json',
type: 'GET'
});

// Not used while Amplify.JS issues worked out!
/*
amplify.request.define("saveSeaDay", "ajax", {
url: appBase + '/Trip/{TripId}/Days/{DayNumber}/Edit',
dataType: "json",
contentType: "application/json",
type: "POST"
});
*/

amplify.request.define("getCrew", "ajax", {
    url: appBase + 'Trip/{TripId}/Crew/Edit',
    dataType: 'json',
    type: 'GET'
});

amplify.request.define("getFishingSet", "ajax", {
    url: appBase + 'Trip/{TripId}/Sets/{SetNumber}/Edit',
    dataType: 'json',
    type: 'GET'
});

tubs.getSeaDay = function (tripId, dayNumber, success_cb, error_cb) {
    amplify.request({
        resourceId: "getSeaDay",
        data: { "TripId": tripId, "DayNumber": dayNumber },
        success: success_cb,
        error: error_cb
    });
};

// Amplify can't read parameters out of a JSON string, but when given
// an object, it URL encodes it.  Still, while we wait for an answer
// to this, we can still use this facade.
// This StackOverflow answer might have a workaround:
// http://stackoverflow.com/questions/10808312/amplify-js-crud-like-urls
tubs.saveSeaDay = function (tripId, dayNumber, seaDay, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/Days/' + dayNumber + '/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(seaDay),
        success: success_cb,
        error: error_cb
    });
};

tubs.getCrew = function (tripId, success_cb, error_cb) {
    amplify.request({
        resourceId: "getCrew",
        data: { "TripId": tripId },
        success: success_cb,
        error: error_cb
    });
};

tubs.saveCrew = function (tripId, crew, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/Crew/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(crew),
        success: success_cb,
        error: error_cb
    });
};

tubs.getFishingSet = function (tripId, setNumber, success_cb, error_cb) {
    amplify.request({
        resourceId: "getFishingSet",
        data: { "TripId": tripId, "SetNumber": setNumber },
        success: success_cb,
        error: error_cb
    });
};

tubs.saveFishingSet = function (tripId, setNumber, fishingSet, success_cb, error_cb) {
    var url = appBase + 'Trip/' + tripId + '/Sets/' + fishingSet + '/Edit';
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: ko.toJSON(fishingSet),
        success: success_cb,
        error: error_cb
    });
}