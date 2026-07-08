function initSearchPhoneModal(options) {
    const {
        searchUrl,
        createUrl,
        createPatientUrl,
        invalidPhoneMessage,
        requiredFieldMessage,
        errorMessage,
        bookLabel,
        genderMale,
        genderFemale,
        phoneNotFoundMessage,
        noPatientsFoundMessage
    } = options;

    $('#btnModalSearchPhone').on('click', performModalSearch);

    $('#modalPhoneNumber').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            performModalSearch();
        }
    });

    setTimeout(function() {
        $('#modalPhoneNumber').focus();
    }, 300);

    function performModalSearch() {
        var phone = $('#modalPhoneNumber').val().trim();
        var $error = $('#modalPhoneSearchError');
        var $spinner = $('#modalSearchSpinner');
        var $results = $('#modalSearchResults');
        var $noPatients = $('#modalNoPatientsMessage');

        $error.addClass('d-none').text('');
        $results.addClass('d-none');
        $noPatients.addClass('d-none');

        if (!phone) {
            showError(requiredFieldMessage);
            return;
        }

        if (!/^01[0125][0-9]{8}$/.test(phone)) {
            showError(invalidPhoneMessage);
            return;
        }

        var $btn = $('#btnModalSearchPhone');
        $btn.prop('disabled', true);
        $spinner.removeClass('d-none');

        $.get(searchUrl, { phoneNumber: phone })
            .done(function (result) {
                renderModalResults(result, phone);
            })
            .fail(function (xhr) {
                var msg = errorMessage;
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    msg = xhr.responseJSON.message;
                }
                showError(msg);
            })
            .always(function () {
                $spinner.addClass('d-none');
                $btn.prop('disabled', false);
            });
    }

    function showError(msg) {
        $('#modalPhoneSearchError').removeClass('d-none').text(msg);
    }

    function renderModalResults(result, phone) {
        var $list = $('#modalPatientsList');
        var $results = $('#modalSearchResults');
        var $noPatients = $('#modalNoPatientsMessage');
        var $addContainer = $('#modalAddPatientContainer');

        $list.empty();

        if (!result.contactId || !result.patients || result.patients.length === 0) {
            var message = (!result.contactId) ? phoneNotFoundMessage : noPatientsFoundMessage;
            $('#modalNoPatientsText').text(message);

            var addPatientUrl = createPatientUrl + '/' + phone;
            $('#modalNoPatientsAddBtn').attr('href', addPatientUrl);

            $noPatients.removeClass('d-none');
            return;
        }

        result.patients.forEach(function (patient) {
            var genderLabel = patient.gender === 1 ? genderMale : genderFemale;
            var lastDate = patient.lastAppointmentDate
                ? new Date(patient.lastAppointmentDate).toLocaleDateString('ar-EG')
                : '-';

            var bookUrl = createUrl +
                '?patientId=' + patient.id +
                '&contactId=' + result.contactId;

            var card = 
                '<div class="card border mb-3 shadow-sm hover-shadow">' +
                '  <div class="card-body p-3 d-flex justify-content-between align-items-center">' +
                '    <div>' +
                '      <h6 class="mb-1 text-primary fw-semibold"><i class="bx bx-user me-2"></i>' + escapeHtml(patient.name) + '</h6>' +
                '      <small class="text-muted"><i class="bx bx-calendar me-1"></i>' + patient.age + ' سنة | ' + genderLabel + ' | آخر موعد: ' + lastDate + '</small>' +
                '    </div>' +
                '    <div>' +
                '      <a href="' + bookUrl + '" class="btn btn-sm btn-primary">' +
                '        <i class="bx bx-calendar-plus me-1"></i> ' + bookLabel +
                '      </a>' +
                '    </div>' +
                '  </div>' +
                '</div>';

            $list.append(card);
        });

        var addPatientUrl = createPatientUrl + '/' + phone;
        $('#modalAddPatientBtn').attr('href', addPatientUrl);
        $addContainer.removeClass('d-none');
        $results.removeClass('d-none');
    }

    function escapeHtml(text) {
        return $('<div>').text(text).html();
    }
}
