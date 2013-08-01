/** 
 * @file Common Knockout bindings
 * @copyright 2013, Secretariat of the Pacific Community
 * @author Corey Cole <coreyc@spc.int>
 *
 * Depends on:
 * knockout.js
 */

/// <reference name="../knockout-2.3.0.debug.js" />

/**
 * Extend Knockout to store dates using ISO-8601, 
 * but display (and accept) in arbitrary formats.
 * [Original implementation]{@link http://www.reddnet.net/knockout-js-extender-for-dates-in-iso-8601-format/}
 * [JSFiddle]{@link http://jsfiddle.net/StephenRedd/SXRyr/}
 * @param {HTMLElement} target target HTMLElement
 * @param {string} [formatString=DD/MM/YY] Date format string
 */
ko.extenders.isoDate = function (target, formatString) {
    formatString = formatString || 'DD/MM/YY';
    target.formattedDate = ko.computed({
        read: function () {
            var dt;
            if (!target()) {
                return;
            }
            dt = moment(target());
            return dt.format(formatString);
        },
        write: function (value) {
            var shortDate,
                dt,
                formatted;
            if (value) {
                // Use the presence of a slash character to determine if the date
                // is not in ISO8601 format.
                shortDate = value.indexOf("/") > 0;
                dt = shortDate ? moment(value, formatString) : moment(value);
                formatted = dt.format();
                target(formatted);
            }
        }
    });

    //initialize with current value
    target.formattedDate(target());

    //return the computed observable
    return target;
};
