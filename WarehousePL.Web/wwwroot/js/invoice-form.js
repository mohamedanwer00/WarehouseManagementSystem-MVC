function initInvoiceForm(options) {
    const { getAppointmentsUrl } = options;

    var $patientSelect = $('#PatientId');
    var $appointmentSelect = $('#AppointmentId');

    $patientSelect.on('change', function () {
        var patientId = $(this).val();
        $appointmentSelect.empty().append('<option value="">-- Select Appointment --</option>').prop('disabled', true);

        // Trigger change to update Select2 UI
        $appointmentSelect.trigger('change');

        if (!patientId) {
            return;
        }

        $.get(getAppointmentsUrl, { patientId: patientId })
            .done(function (data) {
                if (data && data.length > 0) {
                    data.forEach(function (app) {
                        $appointmentSelect.append($('<option>', {
                            value: app.value,
                            text: app.text
                        }));
                    });
                    $appointmentSelect.prop('disabled', false);
                } else {
                    $appointmentSelect.append('<option value="">No unpaid appointments found</option>');
                }
                $appointmentSelect.trigger('change');
            })
            .fail(function () {
                showErrorMessage();
            });
    });
}
