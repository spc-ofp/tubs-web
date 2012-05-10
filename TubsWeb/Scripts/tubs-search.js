/* tubs-search 
 * Search TUBS trips using the DocumentCloud VisualSearch widget.
 */

$(document).ready(function () {
    var allObserverPrograms = [
        'AROB', 'ASOB', 'AUOB', 'CKOB', 'FAOB', 'FJOB', 'FMOB', 'HWOB',
        'KIOB', 'MHOB', 'NCOB', 'NROB', 'NUOB', 'NZOB', 'PFOB', 'PGOB',
        'PWOB', 'SBOB', 'SPOB', 'TOOB', 'TTOB', 'UNOB', 'VUOB', 'WFOB',
        'WSOB', 'VNOB'
    ];

    var visualSearch = VS.init({
        container: $('#visual_search'),
        query: '',
        callbacks: {
            search: function (query, searchCollection) {
                var $query = $('#search_query');
                $query.stop().animate({ opacity: 1 }, { duration: 300, queue: false });
                $query.html('<span class="raquo">&raquo;</span> You searched for: <b>' + searchCollection.serialize() + '</b>');
                clearTimeout(window.queryHideDelay);
                window.queryHideDelay = setTimeout(function () {
                    $query.animate({
                        opacity: 0
                    }, {
                        duration: 1000,
                        queue: false
                    });
                }, 2000);
            },
            facetMatches: function (callback) {
                callback([
                    'program', 'flag', 'observer', 'year', 'vessel'
                ]);
            },
            valueMatches: function (facet, searchTerm, callback) {
                switch (facet) {
                    case 'program':
                        callback(allObserverPrograms);
                        break;
                    case 'flag':
                        callback([]);
                        break;
                    case 'observer':
                        callback([]);
                        break;
                    case 'year':
                        callback([]);
                        break;
                    case 'vessel':
                        callback([]);
                        break;
                }
            }
        }
    });
});