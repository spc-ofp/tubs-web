
String.prototype.endsWith = function (str) {
    return (this.match(str + "$") === str);
};

$(document).ready(function () {
    /* Stolen from here */
    /* http://stackoverflow.com/questions/2335553/jquery-how-to-catch-enter-key-and-change-event-to-tab */
    $(".autotab").on('keyup', function () {
        var val,
            inputs;

        val = $(this).val();
        if (val.length === $(this).attr("maxlength")) {
            inputs = $(this).closest('form').find(':input:visible');
            inputs.eq(inputs.index(this) + 1).focus();
        }
    });
});

//
// This one doesn't clear hidden values
// http://weblogs.asp.net/haithamkhedre/archive/2010/11/22/blank-out-a-form-with-jquery.aspx
function clearFormEx(formName) {
    $(':input', formName)
                .not(':button, :submit, :reset, :hidden')
                .val('')
                .removeAttr('checked')
                .removeAttr('selected');
};

// Clear any form values
// Used for the Ajax "add to table" archetype.
function clearForm(formName) {
    $(':input', formName).each(function () {
        var type = this.type.toLowerCase();
        if (type === 'text' || type === 'password' || type === 'textarea' || type === 'range') {
            this.value = "";
        } else if (type === 'hidden') {
            // Fixes problem with clearing out numeric hidden values that map to not-nullable
            // properties in the data access layer
            if (this.name.endsWith(".Id")) {
                this.value = "0";
            } else {
                this.value = "";
            }
        } else if (type === 'select-one' || type === 'select-multi') {
            this.selectedIndex = -1;
        } else if (type === 'radio' || type === 'checkbox') {
            if (this.checked) {
                this.checked = false;
            }
        }
    });
};