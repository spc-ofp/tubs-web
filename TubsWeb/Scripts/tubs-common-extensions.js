/** 
 * @file Common Knockout extensions
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * knockout.js
 * knockout.validation
 * knockout-custom-bindings (for ISO date)
 */

/// <reference name="../knockout-2.3.0.debug.js" />
/// <reference name="../knockout-custom-bindings.js" />
/// <reference name="../knockout.validation.debug.js" />
var tubs = tubs || {};

/**
 * Apply a regular expression to 24 hour time values.
 * Knockout validation creates a regex from a string parameter, so
 * create a RegExp directly to make the implicit explicit.
 */
tubs.timeExtension = {
    pattern: {
        message: 'Must be a valid 24 hour time',
        params: new RegExp('^(20|21|22|23|[01][0-9])[0-5][0-9]$')
    },
    maxLength: 4
};

/**
 * Extend date values with the ISO date binding.
 * This binding adds a 'formattedDate' property
 * and allows the date to capture a full ISO8601 data
 * while displaying a simplified date format for entry
 * operators.
 */
tubs.dateExtension = {
    isoDate: 'DD/MM/YY'
};

/**
 * Common Knockout mapping plugin creation function for
 * date values.
 */
tubs.mappedDate = {
    create: function (options) {
        return ko.observable(options.data).extend(tubs.dateExtension);
    }
};

/**
 * Extend latitude string with a regular expression validator
 * that conforms to standard 'SPC' latitude format.
 */
tubs.latitudeExtension = {
    pattern: {
        message: 'Latitude should be ddmm.mmmN or ddmm.mmmS',
        params: new RegExp('^[0-8][0-9]{3}\.?[0-9]{3}[NS]$', 'i') //ignore jslint
    },
    maxLength: 9
};

/**
 * Extend latitude string with a regular expression validator
 * that conforms to standard 'SPC' latitude format.
 */
tubs.longitudeExtension = {
    pattern: {
        message: 'Longitude should be dddmm.mmmE or dddmm.mmmW',
        params: new RegExp('^[0-1]\\d{4}\.?\\d{3}[EW]$', 'i') //ignore jslint
    },
    maxLength: 10
};