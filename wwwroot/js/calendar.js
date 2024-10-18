var customerTable;
var idToPopulate;
document.addEventListener('DOMContentLoaded', function () {

    LoadCalendar()
    LoadFormattedDate()

    $('#selectClientBtn').click(function () {
        LoadCustomersDataTable()
    })

    $('.resetDataTable').click(function () {
        customerTable.destroy()
    })

});

function LoadCalendar() {
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        locale: 'el',
        themeSystem: 'bootstrap5',
        initialView: 'dayGridMonth',
        selectable: true,
        aspectRatio: 2, // change this for scale
        views: {
            dayGrid: {
                dayMaxEventRows: 5
            }
        },
        events: function (info, successCallback, failureCallback) {
            $.ajax({
                url: 'api/therapies/get-therapies',
                type: 'GET',
                dataType: 'json',

                success: function (response) {
                    console.log(response)
                    var events = response.map(function (event) {
                        // Convert UTC start date to Athens local time
                        var localStartDate = moment.utc(event.therapyDto.startDate).tz('Europe/Athens').format();
                        return {
                            id: event.therapyDto.id,
                            title: event.firstname + " " + event.lastname,
                            start: localStartDate,
                            end: event.endDate // Assuming event.endDate is already in the correct format
                        };
                    });
                    successCallback(events); // Pass events to FullCalendar
                },
                error: function (xhr, status, error) {
                    // Handle errors
                    console.error('Error fetching events:', error);
                    failureCallback(error);
                }
            });
        },
        eventDidMount: function (info) {
            var today = new Date();
            var eventDate = new Date(info.event.start);
            if (eventDate < today) {
                info.el.classList.add('past-event');
            } else {
                info.el.classList.add('future-event');
            }
        },
        eventColor: '#8e6368',
        eventTimeFormat: {
            hour: '2-digit',
            minute: '2-digit',
            meridiem: false
        },
        eventDisplay: "block",
        dateClick: function (info) {
            $('#clearFormBtn').trigger('click');
            $('#editTherapyForm').hide();

            console.log(info)
            dotvvm.viewModels.root.viewModel.AppointmentDate(info.date)
            $('#newTherapyForm').show()
        },
        eventClick: function (info) {
            dotvvm.viewModels.root.viewModel.TherapyIdToPopulate(info.event._def.publicId)
            document.querySelectorAll('.fc-event').forEach(function (eventEl) {
                eventEl.classList.remove('active-event');
            });
            info.el.classList.add('active-event');
            $('#clearFormBtn').trigger('click');
            $('#editTherapyBtn').trigger('click');
            $('#newTherapyForm').hide()
            console.log(info)
            $('#editTherapyForm').show();
        },




    });
    calendar.render();

}

// DataTable
function LoadCustomersDataTable() {
    customerTable = $('#customerTable').DataTable({
        ajax: {
            url: 'api/customers/get-customers',
            dataSrc: '',
            type: 'GET',
            contentType: "application/json",
            dataType: "json",
            data: function (data) {
                return JSON.stringify(data);
            }
        },
        select: 'single',
        responsive: true,
        layout: {
            topStart: {
                buttons: [
                    {
                        text: '<i class="bi bi-plus-lg"></i> Νέο',
                        action: function (e, dt, node, config) {
                            $('#editModal').modal('toggle')
                            $('#addCustomerModal').modal('toggle')
                        }
                    },
                ]
            }
        },
        columns: [
            { data: 'Firstname', title: 'Όνομα' },
            { data: 'Lastname', title: 'Επώνυμο' },
            { data: 'MobileNumber', title: 'Κινητό τηλ.' },
            { data: 'Age', title: 'Ημ. Γέννησης' },
            { data: 'Email', title: 'E-mail' },
            { data: 'Medication', title: 'Αγωγή' }
        ],
        columnDefs: [
            {
                targets: 3,
                render: function (data, type, row) {
                    var startDate = moment("1900-01-01", "YYYY-MM-DD");
                    var endDate = moment();
                    // Parse the date from the data
                    var date = moment(data, "YYYY-MM-DD");

                    // Check if the date is within the specified range
                    if (date.isValid() && date.isBetween(startDate, endDate, null, '[]')) {
                        // If valid and within range, return formatted date
                        return date.format('DD/MM/YYYY');
                    } else {
                        // Otherwise, return an empty string
                        return '';
                    }
                }

            }
        ]

    });
    customerTable.on('select', function (e, dt, type, indexes) {
        let rowData = customerTable
            .rows(indexes)
            .data()
            .toArray();
        idToPopulate = rowData[0].Id
        dotvvm.viewModels.root.viewModel.CustomerIdToAppoint(idToPopulate)
    })
}

function LoadFormattedDate() {
    var today = new Date();

    // Format the date as yyyy-MM-dd
    var year = today.getFullYear();
    var month = ('0' + (today.getMonth() + 1)).slice(-2); // Months are zero-indexed
    var day = ('0' + today.getDate()).slice(-2);
    var formattedDate = year + '-' + month + '-' + day;
    // Set the value of the date picker
    $('#dobPicker').val(formattedDate);
}

// Validation
const forms = document.querySelectorAll('.needs-validation')
Array.from(forms).forEach(form => {
    form.addEventListener('submit', event => {
        if (!form.checkValidity()) {
            event.preventDefault()
            event.stopPropagation()
        }
        if (form.checkValidity()) {
            if (form.id === "newTherapyForm") {
                $('#createTherapy').click();
            } else if (form.id === "createCustomerForm") {
                $('#createCustomer').click();
            }
        }
        event.preventDefault()
        event.stopPropagation()

        form.classList.add('was-validated')
    }, false)
})