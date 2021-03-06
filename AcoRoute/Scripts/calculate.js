﻿var people
var peopleArray = [];
var destinationsArray = [];
var markersArray = [];
var peopleTable;
var destionationsTable;

function initializeTables(param) {
    people = param;

    for (var i = 0; i < people.length; i++) {
        var person = people[i];
        var personData = [0, "", person.personId, person.surname, person.name, person.address, person.latitude, person.longitude];
        peopleArray[i] = personData;
    }


    peopleTable = $('#peopleTable').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ registros por página",
            "zeroRecords": "No hay registros para mostrar",
            "info": "Mostrando página _PAGE_ de _PAGES_",
            "infoEmpty": "No hay registros disponibles",
            "infoFiltered": "(filtrado desde _MAX_ registros totales)",
            "search": "Buscar",
            "paginate": {
                "previous": "Anterior",
                "next": "Siguiente"
            }
        },
        data: peopleArray,
        columns: [
            { title: "BeginEnd" },
            { title: "" },
            { title: "PersonId" },
            { title: "Apellido" },
            { title: "Nombre" },
            { title: "Domicilio" },
            { title: "Latitude" },
            { title: "Longitude" }
        ],
        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [1],
                "visible": false,
                "searchable": false,
                "sortable": false
            },
            {
                "targets": [2],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [3],
                "width": "25%"
            },
            {
                "targets": [4],
                "width": "25%"
            },
            {
                "targets": [5],
                "width": "50%"
            },
            {
                "targets": [6],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [7],
                "visible": false,
                "searchable": false
            }]
    });

    $('#peopleTable tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
        var selectedRow = peopleTable.row(this).data();

        if ($(this).hasClass('selected')) {
            var point = createPoint(selectedRow[6], selectedRow[7]);
            var title = selectedRow[3] + ", " + selectedRow[4]
            var marker = addAddressMarker(point, title);
            var markerObj =
                {
                    personId: selectedRow[2],
                    marker: marker
                };
            markersArray.push(markerObj);

        }
        else {
            removeMarker(selectedRow[2]);
        }
    });

    destinationsTable = $('#destinationsTable').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ registros por página",
            "zeroRecords": "No hay registros para mostrar",
            "info": "Mostrando página _PAGE_ de _PAGES_",
            "infoEmpty": "No hay registros disponibles",
            "infoFiltered": "(filtrado desde _MAX_ registros totales)",
            "search": "Buscar",
            "paginate": {
                "previous": "Anterior",
                "next": "Siguiente"
            }
        },
        data: destinationsArray,
        columns: [
            { title: "BeginEnd" },
            { title: "" },
            { title: "PersonId" },
            { title: "Apellido" },
            { title: "Nombre" },
            { title: "Domicilio" },
            { title: "Latitude" },
            { title: "Longitude" }
        ],
        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [1],
                "visible": true,
                "searchable": false,
                "sortable": false,
                "width": "10%"
            },
            {
                "targets": [2],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [3],
                "width": "25%"
            },
            {
                "targets": [4],
                "width": "25%"
            },
            {
                "targets": [5],
                "width": "50%"
            },
            {
                "targets": [6],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [7],
                "visible": false,
                "searchable": false
            }]
    });

    $('#destinationsTable tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });
}



function addDestinations() {
    var selectedToAdd = peopleTable.rows('.selected').data();
    if (selectedToAdd.length > 0) {
        destinationsArray.push.apply(destinationsArray, selectedToAdd);
        destinationsTable.clear().rows.add(destinationsArray).draw();
        peopleArray = peopleArray.filter(function (person) {
            return !destinationsArray.includes(person);
        });
        peopleTable.clear().rows.add(peopleArray).draw();
        $("#btnSetOrigin").removeClass("disabled");
        $("#btnSetDestination").removeClass("disabled");
    }
}

function removeDestinations() {
    var selectedToRemove = destinationsTable.rows('.selected').data();

    if (selectedToRemove.length > 0) {
        for (var i = 0; i < selectedToRemove.length; i++) {
            selectedToRemove[0][0] = 0;
            selectedToRemove[0][1] = '';
        }
        peopleArray.push.apply(peopleArray, selectedToRemove);
        peopleTable.clear().rows.add(peopleArray).draw();
        destinationsArray = destinationsArray.filter(function (destination) {
            return !peopleArray.includes(destination);
        });
        destinationsTable.clear().rows.add(destinationsArray).draw();
        for (var i = 0; i < peopleArray.length; i++) {
            removeMarker(peopleArray[i][2]);
        }
        if (destinationsArray.length == 0) {
            $("#btnSetOrigin").addClass("disabled");
            $("#btnSetDestination").addClass("disabled");
        }
    }
}

function removeMarker(personId) {
    var markerObj = markersArray.filter(function (markerObj) {
        return markerObj.personId == personId;
    })[0];
    if (markerObj) {
        removeAddressMarker(markerObj.marker);
        var index = markersArray.indexOf(markerObj);
        if (index > -1) {
            markersArray.splice(index, 1);
        }
    }
}

function setOrigin() {
    var selectedRow = destinationsTable.rows('.selected').data();
    if (selectedRow.length != 1) {
        showAlert('Error', 'Seleccione una fila')
        return
    }

    var destinations = destinationsTable.rows().data();
    for (var i = 0; i < destinations.length; i++) {
        if (destinations[i][0] == 2) {
            destinations[i][0] = 0;
            destinations[i][1] = '';
        }
    }

    selectedRow[0][0] = 2;
    selectedRow[0][1] = '<i class="fa fa-flag fa-lg text-origin"></i>';

    destinationsTable.clear().rows.add(destinationsArray).order([[0, "desc"]]).draw(false);
    enableDisableCalculate();
}

function setEndDestination() {
    var selectedRow = destinationsTable.rows('.selected').data();
    if (selectedRow.length != 1) {
        showAlert('Error', 'Seleccione una fila')
        return
    }

    var destinations = destinationsTable.rows().data();
    for (var i = 0; i < destinations.length; i++) {
        if (destinations[i][0] == 1) {
            destinations[i][0] = 0;
            destinations[i][1] = '';
        }
    }

    selectedRow[0][0] = 1;
    selectedRow[0][1] = '<i class="fa fa-flag-checkered fa-lg text-destination"></i>';

    destinationsTable.clear().rows.add(destinationsArray).order([[0, "desc"]]).draw(false);
    enableDisableCalculate();
}

function calculateRoute() {
    var destinationsCoordinates = [];
    var originCoordinates = [];
    var endDestinationCoordinates = [];
    var destinations = destinationsTable.rows().data();
    for (var i = 0; i < destinations.length; i++) {
        var singleCoordinates = [destinations[i][6], destinations[i][7], destinations[i][2]];
        destinationsCoordinates[i] = singleCoordinates;

        switch (destinations[i][0]) {
            case 3:
                originCoordinates = singleCoordinates;
                endDestinationCoordinates = singleCoordinates;
                break;
            case 2:
                originCoordinates = singleCoordinates;
                break;
            case 1:
                endDestinationCoordinates = singleCoordinates;
        }
    }

    var param = {
        Points: destinationsCoordinates,
        StartCoord: originCoordinates,
        EndCoord: endDestinationCoordinates
    }

    $.ajax({
        type: "POST",
        url: "/Routes/CalculateRoute/",
        data: param,
        dataType: "json"
    }).done((response) => {
        var minLat = response.routeResult[0].Latitude;
        var maxLat = response.routeResult[0].Latitude;
        var minLong = response.routeResult[0].Longitude;
        var maxLong = response.routeResult[0].Longitude;
        var wayPoints = [];

        var origin = {
            lat: response.routeResult[0].Latitude,
            lng: response.routeResult[0].Longitude
        }
        for (var i = 1; i < response.routeResult.length; i++) {
            minLat = response.routeResult[i].Latitude < minLat ? response.routeResult[i].Latitude : minLat;
            maxLat = response.routeResult[i].Latitude > maxLat ? response.routeResult[i].Latitude : maxLat;
            minLong = response.routeResult[i].Longitude < minLong ? response.routeResult[i].Longitude : minLong;
            maxLong = response.routeResult[i].Longitude > maxLong ? response.routeResult[i].Longitude : maxLong;
            wayPoints.push({
                location: new google.maps.LatLng(response.routeResult[i].Latitude, response.routeResult[i].Longitude),
                stopover: true
            });
        }

        wayPoints.pop();
        var destination = {
            lat: response.routeResult[response.routeResult.length - 1].Latitude,
            lng: response.routeResult[response.routeResult.length - 1].Longitude
        }

        var centerLat = (minLat + maxLat) / 2;
        var centerLong = (minLong + maxLong) / 2;

        var directionsRequest = {
            origin: origin,
            destination: destination,
            travelMode: 'DRIVING',
            waypoints: wayPoints,
            provideRouteAlternatives: false
        }

        $(document).ready(function () {
            createMap(15, createPoint(centerLat, centerLong), false, "map2")
            directionsDisplay.setMap(map);
            $("#divCalculation").addClass("hidden");
            $("#divRoute").removeClass("hidden");
            directionsService.route(directionsRequest, (result, status) => {
                if (status == 'OK') {
                    var instructions = '';
                    for (var i = 0; i < result.routes[0].legs.length; i++) {
                        var leg = result.routes[0].legs[i];
                        for (var j = 0; j < leg.steps.length; j++) {
                            var step = leg.steps[j];

                            instructions += "<li>" + step.instructions + "<br />";
                        }
                    }
                    $("#instructions").html(instructions);
                    directionsDisplay.setDirections(result);                    
                }
            });
        });



    });
}

function enableDisableCalculate() {

    var origin = false;
    var endDestination = false;

    var destinations = destinationsTable.rows().data();
    for (var i = 0; i < destinations.length; i++) {
        origin = origin || destinations[i][0] == 2;
        endDestination = endDestination || destinations[i][0] == 1;
    }

    if (origin && endDestination) {
        $("#btnCalculate").removeClass("disabled");
    } else {
        $("#btnCalculate").addClass("disabled");
    }

}