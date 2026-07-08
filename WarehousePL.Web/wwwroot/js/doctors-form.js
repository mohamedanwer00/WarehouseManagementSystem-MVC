/**
 * Doctor Form — Schedule management
 * Depends on: jQuery, Select2 (via applySelect2 in site.js), jquery-timepicker
 * Config object: doctorFormConfig (injected by Form.cshtml)
 */

$(function () {

    var index = 0;

    // ── timepicker ───────────────────────────────────────────────────────────

    function initTimepicker($input) {
        $input.timepicker({
            timeFormat: 'H:i',
            interval: 15,
            minTime: '0',
            maxTime: '23:45',
            startTime: '00:00',
            dynamic: false,
            dropdown: true,
            scrollbar: true
        });
    }

    // ── build a row DOM element ──────────────────────────────────────────────

    function buildRow(idx, dayVal, startVal, endVal, maxVal) {
        var daysHtml = '';
        doctorFormConfig.days.forEach(function (d) {
            var sel = (dayVal !== undefined && d.value == dayVal) ? ' selected' : '';
            daysHtml += '<option value="' + d.value + '"' + sel + '>' + d.text + '</option>';
        });

        return $(
            '<div class="row g-3 mb-4 js-schedule-row border rounded p-3 align-items-end">' +

              '<div class="col-md-3">' +
                '<label class="form-label">' + doctorFormConfig.labels.day + '</label>' +
                // js-select2 class lets applySelect2() in site.js handle this automatically
                '<select class="form-select js-select2"' +
                  ' name="DoctorSchedules[' + idx + '].WeekDay">' +
                  daysHtml +
                '</select>' +
              '</div>' +

              '<div class="col-md-3">' +
                '<label class="form-label">' + doctorFormConfig.labels.startTime + '</label>' +
                '<div class="input-group">' +
                  '<span class="input-group-text"><i class="bx bx-time-five"></i></span>' +
                  '<input type="text" class="form-control js-time-picker" placeholder="00:00"' +
                    ' name="DoctorSchedules[' + idx + '].StartTime"' +
                    ' value="' + (startVal || '') + '" />' +
                '</div>' +
              '</div>' +

              '<div class="col-md-3">' +
                '<label class="form-label">' + doctorFormConfig.labels.endTime + '</label>' +
                '<div class="input-group">' +
                  '<span class="input-group-text"><i class="bx bx-time-five"></i></span>' +
                  '<input type="text" class="form-control js-time-picker" placeholder="00:00"' +
                    ' name="DoctorSchedules[' + idx + '].EndTime"' +
                    ' value="' + (endVal || '') + '" />' +
                '</div>' +
              '</div>' +

              '<div class="col-md-2">' +
                '<label class="form-label">' + doctorFormConfig.labels.maxPatients + '</label>' +
                '<input type="number" min="1" class="form-control"' +
                  ' name="DoctorSchedules[' + idx + '].MaxPatientsPerDay"' +
                  ' value="' + (maxVal || '') + '" />' +
              '</div>' +

              '<div class="col-md-1">' +
                '<button type="button" class="btn btn-danger btn-sm w-100 js-remove-schedule">' +
                  doctorFormConfig.labels.remove +
                '</button>' +
              '</div>' +

            '</div>'
        );
    }

    // ── add row ──────────────────────────────────────────────────────────────

    function addRow(dayVal, startVal, endVal, maxVal) {
        var $row = buildRow(index, dayVal, startVal, endVal, maxVal);
        $('#schedules-container').append($row);

        // init timepicker on the time inputs in this row
        $row.find('.js-time-picker').each(function () {
            initTimepicker($(this));
        });

        // reuse the global applySelect2() from site.js — no duplication
        applySelect2();

        clearScheduleError();
        index++;
    }

    // ── re-index names after removal ─────────────────────────────────────────

    function reIndex() {
        $('#schedules-container .js-schedule-row').each(function (i) {
            $(this).find('[name]').each(function () {
                var n = $(this).attr('name');
                $(this).attr('name', n.replace(/DoctorSchedules\[\d+\]/, 'DoctorSchedules[' + i + ']'));
            });
        });
        index = $('#schedules-container .js-schedule-row').length;
    }

    // ── schedule error message ───────────────────────────────────────────────

    function showScheduleError(message) {
        if ($('#schedule-error').length === 0) {
            $('#schedules-container').before(
                '<p id="schedule-error" class="text-danger small mb-2">' +
                (message || doctorFormConfig.atLeastOneSchedule) +
                '</p>'
            );
        }
    }

    function clearScheduleError() {
        $('#schedule-error').remove();
    }

    // ── on load: render existing schedules (edit mode) or one empty row ──────

    if (doctorFormConfig.existing && doctorFormConfig.existing.length > 0) {
        doctorFormConfig.existing.forEach(function (s) {
            addRow(s.day, s.startTime, s.endTime, s.maxPatients);
        });
    } else {
        addRow();
    }

    // ── events ───────────────────────────────────────────────────────────────

    $('#btn-add-schedule').on('click', function () {
        addRow();
    });

    $('#schedules-container').on('click', '.js-remove-schedule', function () {
        $(this).closest('.js-schedule-row').remove();
        reIndex();
    });

    // prevent submit if no schedules
    $('form').on('submit', function (e) {

        let isValid = true;
        let days = [];

        clearScheduleError();

        $('#schedules-container .js-schedule-row').each(function () {

            let day = $(this).find('select').val();
            let start = $(this).find('input[name*="StartTime"]').val();
            let end = $(this).find('input[name*="EndTime"]').val();

            // duplicate days
            if (days.includes(day)) {
                showScheduleError("Duplicate days are not allowed");
                isValid = false;
                return false;
            }

            days.push(day);

            // invalid time
            if (start && end && start >= end) {
                showScheduleError("Invalid time range");
                isValid = false;
                return false;
            }
        });

        // no schedules
        if ($('#schedules-container .js-schedule-row').length === 0) {
            showScheduleError();
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault();
            e.stopImmediatePropagation();
            return false;
        }
    });

    $('#UserName').on('input', function () {
        let value = $(this).val();
        let arabicRegex = /[\u0600-\u06FF]/;

        if (arabicRegex.test(value)) {
            $(this).addClass('is-invalid');

            if ($('#username-error').length === 0) {
                $(this).after('<span id="username-error" class="text-danger">لا يُسمح باستخدام الأحرف العربية</span>');
            }
        } else {
            $(this).removeClass('is-invalid');
            $('#username-error').remove();
        }
    });

});
