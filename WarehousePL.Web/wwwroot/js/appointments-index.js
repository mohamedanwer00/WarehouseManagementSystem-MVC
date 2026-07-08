var appointmentsIndex = (function () {
    'use strict';

    var datatable;
    var isTodayOnly = true;

    function loadDoctors(getDoctorsUrl) {
        var $filterDoctor = $('#filterDoctor');
        if (!$filterDoctor.length) return;

        $.ajax({
            url: getDoctorsUrl,
            type: 'GET',
            success: function (doctors) {
                doctors.forEach(function (doctor) {
                    $filterDoctor.append('<option value="' + doctor.value + '">' + doctor.text + '</option>');
                });
                applySelect2();
            }
        });
    }

    function getCustomData() {
        return {
            isTodayOnly: isTodayOnly,
            filterDoctorId: $('#filterDoctor').val() || 0,
            filterStatus: $('#filterStatus').val() || 0,
            filterFromDate: $('#filterFromDate').val() || '',
            filterToDate: $('#filterToDate').val() || ''
        };
    }

    function toggleDateRangeFilters(show) {
        if (show) {
            $('#dateRangeFromDiv, #dateRangeToDiv').show();
        } else {
            $('#dateRangeFromDiv, #dateRangeToDiv').hide();
            // Clear the date inputs when hiding
            $('#filterFromDate').val('');
            $('#filterToDate').val('');
        }
    }

    function init(options) {
        var $dataDiv = $('#create-button-data');
        
        loadDoctors(options.getDoctorsUrl);

        // Initialize select2
        applySelect2();

        // Initialize flatpickr
        var flatpickrOptions = {
            dateFormat: 'Y-m-d',
            locale: typeof isRTL !== 'undefined' && isRTL ? 'ar' : 'en',
            onChange: function() {
                if (datatable) {
                    datatable.ajax.reload();
                }
            }
        };
        flatpickr('#filterFromDate', flatpickrOptions);
        flatpickr('#filterToDate', flatpickrOptions);

        // Initially hide date range filters (since today's tab is active)
        toggleDateRangeFilters(false);

        datatable = ServerDataTable.init({
            tableSelector: '#appointmentsTable',
            listUrl: $('#appointmentsTable').data('list-url'),
            defaultOrder: $('#appointmentsTable').data('default-order'),
            antiForgeryToken: $('#appointmentsAntiforgeryToken').val(),
            tableTitle: $dataDiv.data('address'),
            createButton: {
                mode: $dataDiv.data('mode'),
                title: $dataDiv.data('title'),
                url: $dataDiv.data('url'),
                label: $dataDiv.data('label')
            },
            customDataFn: getCustomData,
            errorMessage: options.errorMessage
        });

        // Tab click event
        $('#appointmentsTabs .nav-link').on('click', function () {
            var tabType = $(this).data('type');
            isTodayOnly = tabType === 'today';
            toggleDateRangeFilters(!isTodayOnly);
            if (datatable) {
                datatable.ajax.reload();
            }
        });

        // Filter change events
        $('#filterDoctor, #filterStatus').on('change', function () {
            if (datatable) {
                datatable.ajax.reload();
            }
        });
    }

    return {
        init: init
    };
})();

$(function () {
    // This is kept for backward compatibility with the existing script block in the view
});