var table;

$(document).ready(function () {
    LoadCustomersDataTable()
    LoadFormattedDate()
    
    var idToPopulate = '';
    $('#editButton').click(function () {
        $('#editButton').hide();
        $('#deleteButton').css('visibility', 'hidden');
        $('.enable-input').attr('disabled', false);
        $('#saveChanges').show();

    });

    $('#saveChangesValidated').click(function () {
        $('#editButton').show();
        $('#deleteButton').css('visibility', 'visible');
        $('.enable-input').attr('disabled', true);
        $('#saveChanges').hide();

    });

    $('#closeModal').click(function () {
        $('#deleteButton').css('visibility', 'visible');
        $('#editButton').css('visibility', 'visible');
        $('.enable-input').attr('disabled', true);
    })


});

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

function LoadCustomersDataTable() {
    table = $('#myTable').DataTable({
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
                            $('#addCustomerModal').modal('toggle')
                        }
                    },
                    {
                        text: '<i class="bi bi-person-lines-fill"></i> Καρτέλα Πελάτη',
                        attr: {
                            Id: 'editBtn',
                        },
                        action: function (e, dt, node, config) {
                            var t = dt.row({ selected: true }).data();
                            $('#populateModalBtn').trigger('click')
                            console.log(t.Id)
                            dotvvm.viewModels.root.viewModel.IdToPopulate(t.Id)
                        }
                    }

                ]
            }
        },
        columns: [
            { data: 'Firstname', title: 'Όνομα' },
            { data: 'Lastname', title: 'Επώνυμο' },
            { data: 'MobileNumber', title: 'Κινητό τηλ.' },
            { data: 'Age', title: 'Ημ. Γέννησης' },
            { data: 'Gender', title: 'Φύλλο' },
            { data: 'Email', title: 'E-mail' },
            { data: 'Address', title: 'Διεύθυνση' },
            { data: 'City', title: 'Πόλη' },
            { data: 'PostalCode', title: 'Τ.Κ.' },
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
    //Disable edit if no customer is selected
    var buttons = table.buttons(['editBtn', '.delete']);
    if (table.rows({ selected: true }).indexes().length === 0) {
        buttons.disable();
    }
    else {
        buttons.enable();
    }
}
//<i class="bi bi-pencil"></i>

/*const saveChanges = document.getElementById('saveChanges')
const form = document.getElementById('editCustomerForm')
// Loop over them and prevent submission


    saveChanges.addEventListener('click', event => {
        if (!form.checkValidity()) {
            event.preventDefault()
            event.stopPropagation()
        }
        if (form.checkValidity()) {
            if (form.id === "editCustomerForm") {
                $('#saveChangesValidated').click();
            } else if (form.id === "createCustomerForm") {
                $('#createCustomer').click();
            }
        }
        event.preventDefault()
        event.stopPropagation()

        form.classList.add('was-validated')
    }, false)*/

const forms = document.querySelectorAll('.needs-validation')

// Loop over them and prevent submission
Array.from(forms).forEach(form => {
    form.addEventListener('submit', event => {
        if (!form.checkValidity()) {
            event.preventDefault()
            event.stopPropagation()
        }
        if (form.checkValidity()) {
            if (form.id === "editCustomerForm") {
                $('#saveChangesValidated').click();
            } else if (form.id === "createCustomerForm") {
                $('#createCustomer').click();
            }
        }
        event.preventDefault()
        event.stopPropagation()

        form.classList.add('was-validated')
    }, false)
})