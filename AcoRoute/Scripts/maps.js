/*map related*/

var map;
var geocoder;
var addressMarker;
var routeMarkers = [];

function createMap(initialZoom, initialPosition, addMarker = false) {


    geocoder = new google.maps.Geocoder();

    var mapOptions = {
        zoom: initialZoom,
        mapTypeControl: false,
        navigationControl: true,
        navigationControlOptions: {
            style: google.maps.NavigationControlStyle.DEFAULT
        },
        scaleControl: false,
        center: initialPosition,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    map = new google.maps.Map(document.getElementById("map"), mapOptions);

    if (addMarker) {
        addAddressMarker(initialPosition);
    }

    //se crea el mapa
    
}

function createPoint(lat, lng) {
    return new google.maps.LatLng(lat, lng);
}

function centerMap(point, zoom) {
    map.setCenter(point);
    map.setZoom(zoom);
}

function findAddress(addressElement, latElement, lngElement, wrngElement = null) {
    //var address = document.getElementById('Address').value;
    if (addressMarker) {
        addressMarker.SetMap(null);
        addressMarker = null;
    }

    var address = $("#" + addressElement)[0].value
    if (address) {
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);
                addressMarker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location
                });
                $("#" + latElement)[0].value = addressMarker.position.lat();
                $("#" + lngElement)[0].value = addressMarker.position.lng();
            } else {
                if (wrngElement) {
                    $(wrngElement).removeClass("hidden");
                }
                else {
                    alert('Geocode was not successful for the following reason: ' + status);
                }
            }
        });
    }
}

function addAddressMarker(point) {
    addressMarker = new google.maps.Marker({
        map: map,
        position: point
    });
    map.setCenter(point);
}


function addRouteMarker(point) {
    routeMarkers.push(new google.maps.Marker({
        map: map,
        position: point
    }));
    map.setCenter(point);
}

function deleteRouteMarkers() {
    if (routeMarkers.length > 0) {
        for (i in routeMarkers) {
            routeMarkers[i].setMap(null);            
        }
        routeMarkers = [];
    }
}

/* /map related */

/* not map related*/

function hideElement(element) {
    $(element).addClass("hidden");
}

function showAlert(title,message) {
    bootbox.alert({
        size: "small",
        title: title,
        message: message
    })
}

/* /not map related */