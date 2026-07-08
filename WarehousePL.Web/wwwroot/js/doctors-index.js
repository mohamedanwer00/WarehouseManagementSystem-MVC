function initDoctorsIndex(options) {
    const {
        errorMessage
    } = options;

    // ── Details Offcanvas ───────────────────────────────────────────────
    $(document).on('click', '.js-show-details', function () {
        var id = $(this).data('id');
        var name = $(this).find('.fw-semibold').text().trim();
        var offcanvas = new bootstrap.Offcanvas(document.getElementById('doctorDetailsOffcanvas'));

        $('#doctorDetailsName').text(name);
        $('#doctorDetailsBody').html(
            '<div class="text-center py-5"><div class="spinner-border text-primary" role="status"></div></div>'
        );

        offcanvas.show();

        $.get('/Doctors/DetailsPartial/' + id, function (html) {
            $('#doctorDetailsBody').html(html);
        }).fail(function () {
            $('#doctorDetailsBody').html(
                '<div class="alert alert-danger m-3">' + errorMessage + '</div>'
            );
        });
    });

    // ── Schedules Offcanvas ─────────────────────────────────────────────
    var _currentDoctorId = null;
    var _schedulesOffcanvas = new bootstrap.Offcanvas(document.getElementById('schedulesOffcanvas'));

    function loadSchedules(doctorId) {
        $('#schedulesBody').html(
            '<div class="text-center py-5"><div class="spinner-border text-info" role="status"></div></div>'
        );
        $.get('/Doctors/SchedulesPartial/' + doctorId, function (html) {
            $('#schedulesBody').html(html);

            // init flatpickr time pickers on the newly loaded inputs
            $('#schedulesBody .js-offcanvas-timepicker').each(function () {
                flatpickr(this, {
                    enableTime: true,
                    noCalendar: true,
                    dateFormat: 'H:i',
                    time_24hr: true,
                    minuteIncrement: 15,
                    // append inside offcanvas so z-index works correctly
                    appendTo: document.getElementById('schedulesOffcanvas')
                });
            });

            // init select2 — use dropdownParent to render inside offcanvas
            $('#schedulesBody .js-select2-offcanvas').each(function () {
                var $el = $(this);
                if ($el.hasClass('select2-hidden-accessible')) return;
                $el.select2({
                    width: '100%',
                    dropdownParent: $('#schedulesOffcanvas')
                });
            });
        }).fail(function () {
            $('#schedulesBody').html(
                '<div class="alert alert-danger m-3">' + errorMessage + '</div>'
            );
        });
    }

    $(document).on('click', '.js-manage-schedules', function () {
        _currentDoctorId = $(this).data('id');
        var name = $(this).data('name');
        $('#scheduleDoctorName').text(name);
        loadSchedules(_currentDoctorId);
        _schedulesOffcanvas.show();
    });

    // Add new schedule row form submit
    $(document).on('submit', '#add-schedule-form', function (e) {
        e.preventDefault();
        var $form = $(this);
        var $btn = $form.find('[type=submit]');
        $btn.prop('disabled', true);

        // serialize form data and append the CSRF token
        var formData = $form.serialize() +
            '&__RequestVerificationToken=' +
            encodeURIComponent($('input[name="__RequestVerificationToken"]').val());

        $.ajax({
            url: '/Doctors/AddSchedule',
            type: 'POST',
            data: formData,
            success: function (res) {
                if (res.success) {
                    loadSchedules(_currentDoctorId);
                } else {
                    showScheduleAlert(res.message || errorMessage, 'danger');
                    $btn.prop('disabled', false);
                }
            },
            error: function () {
                showScheduleAlert(errorMessage, 'danger');
                $btn.prop('disabled', false);
            }
        });
    });

    // Delete a single schedule
    $(document).on('click', '.js-delete-schedule', function () {
        var scheduleId = $(this).data('id');
        var $btn = $(this);
        $btn.prop('disabled', true);

        $.ajax({
            url: '/Doctors/DeleteSchedule',
            type: 'POST',
            data: {
                id: scheduleId,
                '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                if (res.success) {
                    loadSchedules(_currentDoctorId);
                } else {
                    showScheduleAlert(res.message || errorMessage, 'danger');
                    $btn.prop('disabled', false);
                }
            },
            error: function () {
                showScheduleAlert(errorMessage, 'danger');
                $btn.prop('disabled', false);
            }
        });
    });

    function showScheduleAlert(msg, type) {
        var $alert = $('<div class="alert alert-' + type + ' alert-dismissible" role="alert">' +
            msg +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>');
        $('#schedules-alert-area').html($alert);
    }
}
