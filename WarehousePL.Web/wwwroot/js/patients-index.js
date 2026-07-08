$(function () {
    var $dataDiv = $('#create-button-data');

    ServerDataTable.init({
        tableSelector: '#patientsTable',
        listUrl: $('#patientsTable').data('list-url'),
        defaultOrder: $('#patientsTable').data('default-order'),
        antiForgeryToken: $('#patientsAntiforgeryToken').val(),
        tableTitle: $dataDiv.data('address'),
        createButton: {
            mode: $dataDiv.data('mode'),
            title: $dataDiv.data('title'),
            url: $dataDiv.data('url'),
            label: $dataDiv.data('label')
        },
        errorMessage: typeof localization !== 'undefined' ? localization.errorMessage : ''
    });

    var $offcanvasEl = document.getElementById('patientDetailsOffcanvas');
    var offcanvas = $offcanvasEl ? new bootstrap.Offcanvas($offcanvasEl) : null;
    var errorMessage = $('#patientsDetailsErrorMessage').val() || 'Error';

    function loadPatientDetails(id, name) {
        if (!offcanvas) return;

        $('#patientDetailsName').text(name);
        $('#patientDetailsBody').html(
            '<div class="text-center py-5"><div class="spinner-border text-primary" role="status"></div></div>'
        );

        offcanvas.show();

        $.get('/Patients/DetailsPartial/' + id, function (html) {
            $('#patientDetailsBody').html(html);
        }).fail(function () {
            $('#patientDetailsBody').html(
                '<div class="alert alert-danger m-3">' + errorMessage + '</div>'
            );
        });
    }

    $(document).on('click', '.js-show-details', function () {
        var id = $(this).data('id');
        var name = $(this).data('name') || $(this).text().trim();
        loadPatientDetails(id, name);
    });
});
