/*
 * Custom Knockout bindings that can be shared go here.
 */

// http://stackoverflow.com/questions/5953222/how-to-convert-json-datetime-into-readable-date-time-using-knockout-and-custom
// Using answer provided by Mike Ward
// Updated to handle null or invalid values
ko.bindingHandlers.date = {
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        var date = moment(value);
        var isInput = (element instanceof HTMLInputElement);
        if (element instanceof HTMLInputElement) {
            if (date) {
                $(element).val(date.format("DD/MM/YY"));
            } else {
                $(element).val('');
            }
        } else {
            if (date) {
                $(element).text(date.format("DD/MM/YY"));
            } else {
                $(element).text('');
            }
        }
    }
};