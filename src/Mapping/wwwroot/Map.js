var layers = [];
var mapMinZoom = 1;
var mapMaxZoom = 18;

function initMap(location) {
    
    window.map = L.map('mapid',
        {
            loadingControl: true,
        }).setView([location.latitude, location.longitude], 6);
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

function registerProjectLocations(geojsonstring) {
    var geojson = JSON.parse(geojsonstring);

    var geojsonMarkerOptions = {
        radius: 6,
        fillColor: "#ffffff",
        color: "#000000",
        weight: 1,
        opacity: 1,
        fillOpacity: 0.8
    };

    var myLayer = L.geoJSON(geojson,
        {
            pointToLayer: function(feature, latlng) {
                switch (feature.properties.category) {
                case "Soil Engineering":
                    geojsonMarkerOptions.fillColor = "#556B2F";
                    break;

                default:
                    geojsonMarkerOptions.fillColor = "#000000";
                    break;
                }
                return L.circleMarker(latlng, geojsonMarkerOptions);
            },
            onEachFeature: function(feature, layer) {
                var description;
                switch (feature.properties.category) {
                case "Soil Engineering":
                    description = "Geotechnical";
                    break;

                default:
                    description = "Unkown type";
                }


                layer.bindPopup("<p>" +
                    feature.properties.name +
                    "</p><p style=\"font-style: italic;color: DarkGrey;\">" +
                    description +
                    "</p><p><a href=\"https://deltekpim.jppuk.net/XWeb/entity/entity.aspx?ec=3&code=" +
                    feature.properties.id +
                    "\" target=\"_blank\">Open in deltek</a></p>");
            }
        });

    layers.push(["projects", myLayer]);
    return true;
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

    var osm_data = JSON.parse(osm_string);
    var geojson = osmtogeojson(osm_data);

    var myLayer = L.geoJSON(geojson, {
        style: function(feature) {
            switch (feature.properties.amenity) {
            case "school":
                return { color: "#29c3e8" }

            case "hospital":
                return { color: "#ff0000" }

            default:
                return { color: "#f7ff90" }
            }
        },
        onEachFeature: function (feature, layer) {
            layer.bindPopup("<h4>" + feature.properties.name + "</h4>" );
        }
    }).addTo(window.map);

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

//Travel layers
async function addTravelLayer(travelentry, locations) {

    var entry = layers.find(layer => layer[0] === travelentry.layerName);
    if (!(typeof entry === "undefined" || entry === null)) {
        removeTravelLayer(travelentry);
    } else {
        entry = [travelentry.layerName, null];
        layers.push(entry);
    }

    var url;
    var method;

    switch (travelentry.type) {
        case 0:
            url = "https://api.openrouteservice.org/v2/isochrones/driving-car";
            method = "car";
            break;

        case 1:
            url = "https://api.openrouteservice.org/v2/isochrones/foot-walking";
            method = "foot";
            break;

        case 2:
            url = "https://api.openrouteservice.org/v2/isochrones/cycling-regular";
            method = "cycle";
            break;

        default:
            return false;
    }
    
    var requestBody = {
        "locations": [[Number(locations.longitude), Number(locations.latitude)]],
        "range": [travelentry.range * 60],
        "interval": travelentry.interval * 60,
        "range_type": "time"
    }
    
    const response = await fetch(url, {
        method: 'POST', // *GET, POST, PUT, DELETE, etc.
        headers: {
            'Authorization': '5b3ce3597851110001cf624844f9a567079f465f93568e7ed6060c3f',
            'Content-Type': 'application/json',
            'Accept': 'application/json, application/geo+json, application/gpx+xml, img/png; charset=utf-8'
        },
        body: JSON.stringify(requestBody) // body data type must match "Content-Type" header
    });

    var data = await response.json();
    data.features = data.features.reverse();
    
    var myLayer = L.geoJSON(data,
        {
            style: {
                color: getRandomColor(),

            },
            onEachFeature: function (feature, layer) {
                var time = feature.properties.value / 60;
                layer.bindPopup(time + " minute travel time by " + method);
            }
        }
    ).addTo(window.map);

    entry[1] = myLayer;

    return true;
}

async function removeTravelLayer(travelentry) {
    var entry = layers.find(layer => layer[0] === travelentry.layerName);
    if (!(typeof entry === "undefined" || entry === null)) {
        entry[1].remove();
        entry[1] = null;
    }

    return true;
}

function getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}