/*
 * spc.utilities.js
 * Common SPC utilities.
 *
 * Has a dependency on the following libraries:
 * jQuery 
 */

// Might as well start out as good citizens...
var spc = spc || {};

// typeAheadFactory eases the friction of working with
// the Twitter Bootstrap typeahead component
// It has been (lightly) tested with Twitter Bootstrap 2.1.1
// It is expected that the sourceUrl parameter allows 'GET'
// requests, that it meets the minimum standards of working
// with jQuery UI (each returned item has a 'label' and 'value'
// attribute, and that the remote service uses the parameter
// name 'term' to pass in string data.
//
// onSelect should be a function that takes two parameters
// (label and data).  'label' is the text to be displayed in
// the textbox, 'data' is a single entry from the remote service
//
// The following StackOverflow resources were invaluable in figuring this out:
// http://stackoverflow.com/questions/9496314/bootstrap-typeahead-js-add-a-listener-on-select-event
// http://stackoverflow.com/questions/9232748/twitter-bootstrap-typeahead-ajax-example
spc.typeAheadFactory = function (sourceUrl, onSelect) {
    var self = this,
        source,
        updater;

    self.sourceUrl = sourceUrl;
    self.onSelect = onSelect;
    // Possibles holds all the data that comes back from
    // the remote server.  Bootstrap typeahead only deals
    // with the label text, so this will be used to map
    // from the label text to a selected item.
    self.possibles = [];

    source = function (query, process) {
        return $.get(self.sourceUrl, { term: query }, function (data) {
            // Truncate possibles
            self.possibles.length = 0;
            var labels = [];
            $.each(data, function (index, value) { //ignore jslint
                labels.push(value.label);
                self.possibles.push(value);
            });
            return process(labels);
        });
    };

    updater = function (item) {
        var selected = $.grep(self.possibles, function (n, i) { //ignore jslint
            return n.label === item;
        });
        if (selected.length > 0) {
            self.onSelect(item, selected[0]);
        }
        // TODO:  Set this to empty?
        return item;
    };

    // Default 'minLength' is set to two.
    // Overwrite in returned value if necessary
    // items set to 10
    return {
        source: source,
        updater: updater,
        items: 10,
        minLength: 2
    };
};