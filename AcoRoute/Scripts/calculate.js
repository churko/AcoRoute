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
        var personData = [person.personId, person.surname, person.name, person.address, person.latitude, person.longitude];
        peopleArray[i] = personData;
    }


    peopleTable = $('#peopleTable').DataTable({
        data: peopleArray,
        columns: [
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
                "width": "25%"
            },
            {
                "targets": [2],
                "width": "25%"
            },
            {
                "targets": [3],
                "width": "50%"
            },
            {
                "targets": [4],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [5],
                "visible": false,
                "searchable": false
            }]
    });

    $('#peopleTable tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
        var selectedRow = peopleTable.row(this).data();

        if ($(this).hasClass('selected')) {
            var point = createPoint(selectedRow[4], selectedRow[5]);
            var title = selectedRow[1] + ", " + selectedRow[2]
            var marker = addAddressMarker(point, title);
            var markerObj =
                {
                    personId: selectedRow[0],
                    marker: marker
                };
            markersArray.push(markerObj);

        }
        else {
            removeMarker(selectedRow[0]);
        }
    });

    destinationsTable = $('#destinationsTable').DataTable({
        data: destinationsArray,
        columns: [
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
                "width": "25%"
            },
            {
                "targets": [2],
                "width": "25%"
            },
            {
                "targets": [3],
                "width": "50%"
            },
            {
                "targets": [4],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [5],
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
    }
}

function removeDestinations() {
    var selectedToRemove = destinationsTable.rows('.selected').data();

    if (selectedToRemove.length > 0) {
        peopleArray.push.apply(peopleArray, selectedToRemove);
        peopleTable.clear().rows.add(peopleArray).draw();
        destinationsArray = destinationsArray.filter(function (destination) {
            return !peopleArray.includes(destination);
        });
        destinationsTable.clear().rows.add(destinationsArray).draw();
        for (var i = 0; i < peopleArray.length; i++) {
            removeMarker(peopleArray[i][0]);
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
