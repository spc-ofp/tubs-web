/*
 * Custom Knockout bindings that can be shared go here.
 */

/// <reference name="../knockout-2.1.0.debug.js" />

// http://stackoverflow.com/questions/5953222/how-to-convert-json-datetime-into-readable-date-time-using-knockout-and-custom
// Using answer provided by Mike Ward
// Updated to handle null or invalid values
ko.bindingHandlers.date = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var allBindings,
            valueUnwrapped,
            formatString,
            date,
            dateString;

        allBindings = allBindingsAccessor();
        valueUnwrapped = ko.utils.unwrapObservable(valueAccessor());
        formatString = allBindings.formatString || 'DD/MM/YY';

        // Convert the value using moment.js
        date = moment(valueUnwrapped);

        // Use a blank string if moment.js couldn't figure it out
        dateString = date ? date.format(formatString) : '';
        if (element.type) {
            $(element).val(dateString);
        } else {
            $(element).text(dateString);
        }
    },
    /*
     * Is this binding still in use?  Also, can we ignore the allBindingsAccessor argument?
     */
    update: function (element, valueAccessor, allBindingsAccessor) {
        // This might be the trick here:
        // http://stackoverflow.com/questions/7704268/formatting-rules-for-numbers-in-knockoutjs
        if (element.type) {
            var newValue = $(element).val(),
                valueUnwrapped = ko.utils.unwrapObservable(valueAccessor());
        }
        // ko.bindingHandlers.text.update(element, function() { return formattedValue; });
    }
};

// Extender from here:
// http://www.reddnet.net/knockout-js-extender-for-dates-in-iso-8601-format/
// JSFiddle:
// http://jsfiddle.net/StephenRedd/SXRyr/
ko.extenders.isoDate = function (target, formatString) {
    /// <summary>Extend Knockout to store dates using ISO-8601, but display (and accept) in arbitrary formats</summary>
    /// <param name="target" optional="false" type="ko.observable"></param>
    /// <param name="formatString" optional="false" type="String"></param>
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
                shortDate = value.indexOf("/") > 0;
                // TODO:  Replace hard-coded value with 'formatString' argument?
                dt = shortDate ? moment(value, "DD/MM/YY") : moment(value);
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
