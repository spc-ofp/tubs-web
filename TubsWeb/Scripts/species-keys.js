/**
 * Depends on jQuery Hotkeys plugin
 * Mousetrap is a similar library but has no support for the function keys
 * http://craig.is/killing/mice
 * Keypress is another good library, but again, no function key support
 * http://dmauro.github.com/Keypress/
 */

$(document).ready(function () {
    var stopF1,
        isIE = (document.all ? true : false), // Yes, this is a hack
        hotkeys = ['f1', 'f2', 'f3', 'f4', 'f5', 'f6', 'f7', 'f8', 'f9', 'f10'],
        lookup = {
            112: 'YFT', /* F1 */
            113: 'BET', /* F2 */
            114: 'ALB', /* F3 */
            115: 'SKJ', /* F4 */
            116: 'RRU', /* F5 */
            117: 'DOL', /* F6 */
            118: 'MLS', /* F7 */
            119: 'SFA', /* F8 */
            120: 'FAL', /* F9 */
            121: 'MSD'  /* F10 */
        };

    stopF1 = function (evt) {
        if (112 === evt.keyCode) {
            document.onhelp = function () {
                return (false);
            };
            window.onhelp = function () {
                return (false);
            };
            evt.returnValue = false;
            evt.keyCode = 0;
            evt.preventDefault();
            evt.stopPropagation();
        }
    };

    /*
     * I'm not super happy to bind this to document.  What I'd really like
     * is to export the function and then use a more specific container binding.
     * That said, I'll live with the disappointment for a while...
     */
    $(document).on('keydown', 'input.species', hotkeys, function (e) {
        var val = lookup[e.keyCode];
        if (val) {
            // Set the input value
            this.value = val;
            // Prevent the event from bubbling up
            // IE doesn't let us stop this from bubbling by returning false
            // Sledgehammer courtesy StackOverflow
            // http://stackoverflow.com/questions/9019278/
            if (isIE) {
                stopF1(e);
            }
            return false; //ignore jslint
        } else {
            return true;
        }
    });
});