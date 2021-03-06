﻿/* tubs-unobtrusive.js
 * Integrate Microsoft's unobstrusive validation with the Twitter Bootstrap look and feel.
 * Assumes that jQuery and jquery-unobtrusive-ajax have already been referenced.
 */

$(document).ready(function () {
    var esettings,
        originalFunction;

    esettings = $.data($('form')[0], 'validator').settings;
    // Get a handle to the original errorPlacement function
    originalFunction = esettings.errorPlacement;
    esettings.errorPlacement = function (error, inputElement) {
        var id,
            controlGroup;

        // Although you have access to the form via $(this),
        // getElementById should be efficient
        // NOTE:  This assumes that the name and id properties are the same
        id = "#" + inputElement[0].name;
        controlGroup = $(id).closest("div.control-group");
        // error[0].innerHTML contains the validation error message
        if (0 === error[0].innerHTML.length) {
            controlGroup.removeClass("error").addClass("success");
        } else {
            controlGroup.removeClass("success").addClass("error");
        }
        // Call the original function
        originalFunction(error, inputElement);
    };
});