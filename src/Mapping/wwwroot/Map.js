function initMap() {
    /*var mapBounds = new L.LatLngBounds(
        new L.LatLng(49.852539, -7.793077),
        new L.LatLng(60.894042, 1.790425));*/
    var mapMinZoom = 1;
    var mapMaxZoom = 18;
    window.map = L.map('mapid').setView([55.37329, -3.001326], 4);

    var osm = L.tileLayer('http://10.10.1.27/mapping/osm/tile/{z}/{x}/{y}.png',
        {
            minZoom: mapMinZoom,
            maxZoom: mapMaxZoom,
            //bounds: mapBounds,
            attribution: 'OSM Street Maps Data',
        }).addTo(map);

    var otm = L.tileLayer('http://{s}.tile.opentopomap.org/{z}/{x}/{y}.png ',
        {
            minZoom: mapMinZoom,
            maxZoom: mapMaxZoom,
            //bounds: mapBounds,
            attribution: 'optentopomap.org',
        });

    var historic = L.tileLayer('http://nls-{s}.tileserver.com/nls/{z}/{x}/{y}.jpg',
        {
            minZoom: mapMinZoom,
            maxZoom: mapMaxZoom,
            //bounds: mapBounds,
            attribution:
                'Historical Maps Layer, 1919-1947 from the <a href="http://maps.nls.uk/projects/api/">NLS Maps API</a>',
            opacity: 0.25,
            subdomains: '0123'
        });

    
    var radon = L.tileLayer('http://10.10.1.27/mapping/radon/tile/{z}/{x}/{y}.png',
        {
            maxZoom: 18,
            opacity: 0.4,
            attribution:
                'BGS Radon',
        });

    var fz2 = L.tileLayer('http://10.10.1.27/mapping/fz2/tile/{z}/{x}/{y}.png',
        {
            maxZoom: 18,
            opacity: 0.8,
            attribution:
                'EA Flood Map for Planning - Flood Zone 2 (September 2020)',
        });

    var fz3 = L.tileLayer('http://10.10.1.27/mapping/fz3/tile/{z}/{x}/{y}.png',
        {
            maxZoom: 18,
            opacity: 0.8,
            attribution:
                'EA Flood Map for Planning - Flood Zone 3 (September 2020)',
        });


    var basemaps = {
        "OSM": osm,
        "Topo": otm
    };

    var overlayMaps = {
        "1917 - 19??": historic,
        "Radon": radon,
        "Flood Zone 2": fz2,
        "Flood Zone 3": fz3
    };

    L.control.layers(basemaps, overlayMaps).addTo(window.map);
    L.control.scale().addTo(window.map);

    return true;
}

function setLocation(latitude, longitude, zoom) {
    window.map.flyTo([latitude, longitude], zoom);
    var marker = L.marker([latitude, longitude]).addTo(window.map);
    return true;
}

