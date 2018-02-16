var people
var peopleArray = [];
var destinationsArray = [];
var markersArray = [];
var peopleTable;
var destionationsTable;

function initializeTables(param) {
    people = param;

    for (var i = 0; i < people.length; i++) {
        var person = people[i];
        var personData = [0,"",person.personId, person.surname, person.name, person.address, person.latitude, person.longitude];
        peopleArray[i] = personData;
    }


    peopleTable = $('#peopleTable').DataTable({
        data: peopleArray,
        columns: [  
            { title: "BeginEnd" },
            { title: "" },
            { title: "PersonId" },
            { title: "Surname" },
            { title: "Name" },
            { title: "Address" },
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
        data: destinationsArray,
        columns: [
            { title: "BeginEnd" },
            { title: "" },
            { title: "PersonId" },
            { title: "Surname" },
            { title: "Name" },
            { title: "Address" },
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
        if (destinationsArray.length == 0)
        {
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

    var rows = destinationsTable.rows().data();
    for (var i = 0; i < rows.length; i++) {
        if (rows[i][0] == 2 || rows[i][0] == 3) {
            rows[i][0] -= 2;
            rows[i][1] = rows[i][1].replace('<i class="fa fa-flag fa-lg text-origin"></i> ','');
        }
    }

    selectedRow[0][0] += 2;
    selectedRow[0][1] = '<i class="fa fa-flag fa-lg text-origin"></i> ' + selectedRow[0][1];

    destinationsTable.clear().rows.add(destinationsArray).order([[0, "desc"]]).draw(false);
}

function setEndDestination() {
    var selectedRow = destinationsTable.rows('.selected').data();
    if (selectedRow.length != 1) {
        showAlert('Error', 'Seleccione una fila')
        return
    }

    var rows = destinationsTable.rows().data();
    for (var i = 0; i < rows.length; i++) {
        if (rows[i][0] == 1 || rows[i][0] == 3) {
            rows[i][0] -= 1;
            rows[i][1] = rows[i][1].replace('<i class="fa fa-flag-checkered fa-lg text-destination"></i>','');
        }
    }

    selectedRow[0][0] += 1;
    selectedRow[0][1] = selectedRow[0][1] + '<i class="fa fa-flag-checkered fa-lg text-destination"></i>';

    destinationsTable.clear().rows.add(destinationsArray).order([[0, "desc"]]).draw(false);
}