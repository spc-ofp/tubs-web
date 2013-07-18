// Lefalet shortcuts for common tile providers
// https://gist.github.com/mourner/1804938

var osmAttr = '&copy; <a href="http://openstreetmap.org">OpenStreetMap</a> contributors, <a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>';

L.TileLayer.Common = L.TileLayer.extend({
    initialize: function (options) {
        L.TileLayer.prototype.initialize.call(this, this.url, options);
    }
});

L.TileLayer.OpenStreetMap = L.TileLayer.Common.extend({
    url: 'http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
    options: { attribution: osmAttr }
});