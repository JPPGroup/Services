var layers = [];
var mapMinZoom = 1;
var mapMaxZoom = 18;

function initMap() {
    /*var mapBounds = new L.LatLngBounds(
        new L.LatLng(49.852539, -7.793077),
        new L.LatLng(60.894042, 1.790425));*/
    
    window.map = L.map('mapid').setView([52.332510, -0.897930], 6);
    window.map.createPane('base');
    window.map.getPane('base').style.zIndex = 150;

    layers.push(["locationMarker", L.marker([52.332510, -0.897930]).addTo(window.map)]);

    L.control.scale().addTo(window.map);

    return true;
}

function setLocation(latitude, longitude, zoom) {
    window.map.flyTo([latitude, longitude], zoom);
    var target = layers.find(layer => layer[0] === "locationMarker")[1];
    target.remove();

    layers.find(layer => layer[0] === "locationMarker")[1] = L.marker([latitude, longitude]).addTo(window.map);
    return true;
}

function registerLayer(layerDefinition) {
    
    var newLayer = L.tileLayer(layerDefinition.url,
        {
            minZoom: mapMinZoom,
            maxZoom: mapMaxZoom,
            opacity: layerDefinition.opacity,
            attribution: layerDefinition.attribution
        });

    if (layerDefinition.baseLayer) {
        newLayer.options.pane = "base";
    }

    layers.push([layerDefinition.layerName, newLayer]);

    return true;
}

function setLayerState(layerName, active) {

    var target = layers.find(layer => layer[0] === layerName)[1];
    if (active) {
        target.addTo(window.map);
    } else {
        target.remove();
    }

    /*for(var layer in layers) {
        if (layer[0] == layerName) {
            if (active) {
                layer[1].addTo(window.map);
            } else {
                layer[1].remove();
            }
        }
    }*/

    return true;
}
