function initInvoicesIndex(options) {
    var datatable;

    // Initialize select2
    applySelect2();

    var $dataDiv = $('#create-button-data');
    datatable = ServerDataTable.init({
        tableSelector: '#invoicesTable',
        listUrl: $('#invoicesTable').data('list-url'),
        defaultOrder: $('#invoicesTable').data('default-order'),
        antiForgeryToken: $('#invoicesAntiforgeryToken').val(),
        tableTitle: $dataDiv.data('address'),
        createButton: {
            mode: $dataDiv.data('mode'),
            title: $dataDiv.data('title'),
            url: $dataDiv.data('url'),
            label: $dataDiv.data('label')
        },
        customDataFn: function() {
            return {
                filterInvoiceStatus: $('#filterInvoiceStatus').val() || 0
            };
        },
        errorMessage: typeof localization !== 'undefined' ? localization.errorMessage : ''
    });

    // Filter change event
    $('#filterInvoiceStatus').on('change', function() {
        if (datatable) {
            datatable.ajax.reload();
        }
    });
}
