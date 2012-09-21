/*
 * Custom Knockout bindings that can be shared go here.
 */

// http://stackoverflow.com/questions/5953222/how-to-convert-json-datetime-into-readable-date-time-using-knockout-and-custom
// Using answer provided by Mike Ward
// Updated to handle null or invalid values
ko.bindingHandlers.date = {
    init: function (element, valueAccessor) {

    },

    update: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor();
        var allBindings = allBindingsAccessor();
        // I'm using day/month/year as a default.  Don't like it?  Set formatString in your binding!
        var formatString = allBindings.formatString || 'DD/MM/YY';

        // Convert the value using moment.js
        var date = moment(value);

        // Use a blank string if moment.js couldn't figure it out
        var dateString = date ? date.format(formatString) : '';

        var isInput = (element instanceof HTMLInputElement);
        if (isInput) {
            $(element).val(dateString);
        } else {
            $(element).text(dateString);
        }
    }
};