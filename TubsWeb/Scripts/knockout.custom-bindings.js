/*
 * Custom Knockout bindings that can be shared go here.
 */

/// <reference name="../knockout-2.1.0.debug.js" />

// http://stackoverflow.com/questions/5953222/how-to-convert-json-datetime-into-readable-date-time-using-knockout-and-custom
// Using answer provided by Mike Ward
// Updated to handle null or invalid values
ko.bindingHandlers.date = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var allBindings = allBindingsAccessor();
        var valueUnwrapped = ko.utils.unwrapObservable(valueAccessor());
        // I'm using day/month/year as a default.  Don't like it?  Set formatString in your binding!
        var formatString = allBindings.formatString || 'DD/MM/YY';

        // Convert the value using moment.js
        var date = moment(valueUnwrapped);

        // Use a blank string if moment.js couldn't figure it out
        var dateString = date ? date.format(formatString) : '';
        if (element.type) {
            $(element).val(dateString);
        } else {
            $(element).text(dateString);
        }
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        // This might be the trick here:
        // http://stackoverflow.com/questions/7704268/formatting-rules-for-numbers-in-knockoutjs
        if (element.type) {
            var newValue = $(element).val();
            var valueUnwrapped = ko.utils.unwrapObservable(valueAccessor());
            //console.log("Writing new value");
            //valueAccessor()(newValue);
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
            if (!target()) {
                return;
            }
            var dt = moment(target());
            return dt.format(formatString);
        },
        write: function (value) {
            if (value) {
                var shortDate = value.indexOf("/") > 0;
                // TODO:  Replace hard-coded value with 'formatString' argument?
                var dt = shortDate ? moment(value, "DD/MM/YY") : moment(value);
                //console.log("value: " + value);
                //console.log("dt: " + dt);
                var formatted = dt.format();
                //console.log("formatted: " + formatted);
                target(formatted);
            }
        }
    });

    //initialize with current value
    target.formattedDate(target());

    //return the computed observable
    return target;
};
