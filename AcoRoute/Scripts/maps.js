/*map related*/

var map;
var initialPosition;
var geocoder;

function createMap(initialZoom) {

    initialPosition = new google.maps.LatLng(0, 0);
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


    //se crea el mapa
    map = new google.maps.Map(document.getElementById("map"), mapOptions);
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
    var address = $("#" + addressElement)[0].value
    if (address) {
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);
                var marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location
                });
                $("#" + latElement)[0].value = marker.position.lat();
                $("#" + lngElement)[0].value = marker.position.lng();
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