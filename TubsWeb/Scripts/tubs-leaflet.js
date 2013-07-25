/**
 * @file Leaflet mapping functionality
 * @author Corey Cole <corey.cole@gmail.com>
 * @copyright Secretariat of the Pacific Community, 2013
 */

/// <reference path="leaflet-src.js" />

var tubs = tubs || {};

TripMap = function (elem) {
    'use strict';
    var track = L.geoJson(), // Trip track
        positions = L.geoJson(), // Trip positions
        tiles, // Tile server layer
        map,
        base,
        overlays;

    // TODO:  Add a method of specifying a different layer if necessary
    tiles = L.tileLayer('http://nouofputil01/mbtiles/test/{z}/{x}/{y}.png', {
        maxZoom: 9,
        attribution: 'Map data &copy; CROP'
    });

    // TODO:  Center the map appropriately
    map = L.Map(elem, {
        layers: [tiles, track, positions]
    }).setZoom(4); // Zoom level 4 should cover a big part of the W&C Pacific

    // This object allows us to name the base layer
    base = {
        "CROP": tiles
    };

    // These are layers that can be enabled/disabled via the layer control
    overlays = {
        "Track": track,
        "Positions": positions
    };

    L.control.layers(base, overlays).addTo(map);

    // The scale control included with Leaflet only does km and miles
    // There's a third party control (leaflet.customscale) that can handle
    // custom units including nautical miles
    L.control.scale().addTo(map);

    return {
        map: map,
        track: track,
        positions: positions
    };
}();