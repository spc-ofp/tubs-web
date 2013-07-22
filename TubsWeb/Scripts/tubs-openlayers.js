/*
 * After some investigation (including a custom MBTiles server using MVC), it looks like
 * Leaflet is best used when additional data is available as GeoJSON.  The KML story
 * is pretty crappy, and for some reason Leaflet isn't pulling down tiles as fast as
 * it should over an internal network.
 */

/// <reference path="OpenLayers.js" />

var map,
    kml, // kml will be used in the click handler here, but will be defined closer to use
    epsg4326 = new OpenLayers.Projection("EPSG:4326");
OpenLayers.IMAGE_RELOAD_ATTEMPTS = 3;

OpenLayers.Control.Click = OpenLayers.Class(OpenLayers.Control, {
    defaultHandlerOptions: {
        'single': true,
        'double': false,
        'pixelTolerance': 0,
        'stopSingle': false,
        'stopDouble': false
    },

    initialize: function (options) {
        this.handlerOptions = OpenLayers.Util.extend(
            {}, this.defaultHandlerOptions
        );
        OpenLayers.Control.prototype.initialize.apply(
            this, arguments
        );
        this.handler = new OpenLayers.Handler.Click(
            this, {
                'click': this.trigger
            }, this.handlerOptions
        );
    },

    // TODO:  Set up a div with lat/lon in the map tab and fill in on click
    // Should be useful for correcting data.
    // http://gis.stackexchange.com/questions/18984/how-to-register-onclick-handler-for-all-kml-feaures-in-openlayers
    trigger: function (e) {
        if (e.target._featureId) {
            var feature = kml.getFeatureById(e.target._featureId);
            console.log(feature);
            // feature.data.name has the event description (e.g. "Transit", "Sighting of XYZZY")
        }
        var lonlat = map.getLonLatFromPixel(e.xy).transform(map.getProjectionObject(), epsg4326);
        alert("You clicked near " + lonlat.lat + ", " + lonlat.lon);
    }

});