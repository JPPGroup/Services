var layers = [];
var mapMinZoom = 1;
var mapMaxZoom = 18;

function initMap() {
    
    window.map = L.map('mapid',
        {
            loadingControl: true,
        }).setView([52.332510, -0.897930], 6);
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

    if (layerDefinition.interactive) {
        if (registerGeoJson(layerDefinition)) {
            return true;
        }
    }

    registerTileLayer(layerDefinition);

    return true;
}

function registerGeoJson(layerDefinition) {
    fetch(layerDefinition.interactiveURL)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            return response.json();
        }).then(async data => {

            const response = await fetch(layerDefinition.interactiveStyleURL);
            const stylejson = await response.json();

            var layer = L.geoJSON(data, {
                style: function (feature) {
                    var field = Reflect.get(feature.properties, stylejson.property);
                    var rule = stylejson.rules.find(ruleentry => ruleentry.key === field);

                    if (typeof rule === "undefined" || rule === null) {
                        return {
                            color: "rgba(0, 0, 0, 0)",
                            fillOpacity: layerDefinition.opacity,
                            stroke: false
                        };
                        
                    } else {
                        return {
                            color: rule.fill,
                            fillOpacity: layerDefinition.opacity,
                            stroke: false
                        };
                    }
                },
                onEachFeature: function (feature, layer) {

                    var field = Reflect.get(feature.properties, stylejson.property);
                    var rule = stylejson.rules.find(ruleentry => ruleentry.key === field);

                    if (typeof rule === "undefined" || rule === null) {
                        return;
                    } else {
                        layer.bindPopup("<h4>" + rule.title + "</h4><p>" + rule.description + "</p>");
                    }
                }
            });

            layers.push([layerDefinition.layerName + "Interactive", layer]);

            return true;
        })
        .catch(error => {
            return false;
        });
}

function registerTileLayer(layerDefinition) {
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
}

function setLayerState(layerName, active) {

    var target = layers.find(layer => layer[0] === layerName)[1];
    if (active) {
        target.addTo(window.map);
    } else {
        target.remove();
    }

    return true;
}

function addQueryLayer(osm_string) {
    var entry = layers.find(layer => layer[0] === "querylayer");
    if (typeof entry === "undefined" || entry === null) {
        entry = ["querylayer", null];
        layers.push(entry);
    } else {
        entry[1].remove();
    }

    osm_data = JSON.parse(osm_string);
    geojson = osmtogeojson(osm_data);

    var myLayer = L.geoJSON().addTo(window.map);
    myLayer.addData(geojson);

    entry[1] = myLayer;

    return true;
}

function getBounds() {
    bounds = window.map.getBounds();

    return {
        North: bounds.getNorth(),
        South: bounds.getSouth(),
        East: bounds.getEast(),
        West: bounds.getWest()
    }
}
