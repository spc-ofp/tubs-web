/*
 * errorlogger.js provides a common error notification facility.
 * Depends on toastr.
 */

var tubs = tubs || {};

tubs.notify = function (header, xhr, status) {

    // If we got a bad request status, then that's the
    // application talking.
    var errorMessage = 'Application error', // Generic error message
        errors;
    if ("timeout" === status) {

        // Not great from a UX perspective, but good enough for them to tell us
        // what the error message says...
        errorMessage = 'Server timed out';
    }
    if (xhr.status === 400 && xhr.responseText) {
        try {
            errors = $.parseJSON(xhr.responseText);
            if (errors && errors.length > 0) {
                // If there's only one message, then don't put it into a list
                if (errors.length === 1) {
                    errorMessage = errors[0];
                } else {
                    errorMessage = "<ul><li>" + errors.join("</li><li>") + "</li></ul>";
                }
            }
        } catch (err) {
            // TODO Do something with this error
            // In the meantime, the default value will get stuck in there
        }
    }

    toastr.error(errorMessage, header);
};