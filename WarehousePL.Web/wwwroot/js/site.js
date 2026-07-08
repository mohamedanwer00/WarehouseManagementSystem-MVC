var updatedRow;
var datatable;
createButtonData = {};
const notyf = new Notyf({
    duration: 2000,
    position: {
        x: 'right',
        y: 'bottom'
    },
    dismissible: true,
    ripple: true
});

function showErrorNotyf(message = localization.errorMessage) {

    notyf.error(message);
}
function showSuccessNotyf(message = localization.successMessage) {
    notyf.success(message);
}
function showErrorMessage(response = localization.errorMessage) {
    Swal.fire({
        icon: 'error',
        title: localization.error,
        text: response.responseText !== undefined ? response.responseText : response,
        showClass: {
            popup: 'animate__animated animate__bounceIn'
        },
        customClass: {
            confirmButton: 'btn btn-primary'
        },
    });
}
function showSuccessMessage(message = localization.successMessage) {
    Swal.fire({
        icon: 'success',
        title: localization.success,
        text: message,
        showClass: {
            popup: 'animate__animated animate__bounceIn'
        },
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}
function onModalBegin() {
    disableSubmitButton($('#Modal').find(':submit'));
}
function disableSubmitButton(btn) {
    $(btn).attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}
function onModalSuccess(rowHtml) {
    showSuccessMessage();
    $('#Modal').modal('hide');

    if (datatable && datatable.settings && datatable.settings()[0] && datatable.settings()[0].oFeatures.bServerSide) {
        datatable.ajax.reload(null, false);
        return;
    }

    var $newRow = $(rowHtml);
    let addedRow;

    if (updatedRow !== undefined) {
        datatable.row(updatedRow).remove().draw();

        addedRow = datatable.row.add($newRow).draw().node();

        requestAnimationFrame(() => {
            $(addedRow).addClass('animate__animated animate__flash');
        });

        updatedRow = undefined;
    } else {
        addedRow = datatable.row.add($newRow).draw().node();

        requestAnimationFrame(() => {
            $(addedRow).addClass('animate__animated animate__flash');
        });
    }

    $(addedRow).on('animationend', function () {
        $(this).removeClass('animate__animated animate__flash');
    });
}

function onModalComplete() {
    $('body :submit').removeAttr('disabled').removeAttr('data-kt-indicator');
}

// DataTables
function setDataTable() {
    let tableTitle = document.createElement('h5');
    tableTitle.classList.add('card-title', 'mb-0', 'text-md-start', 'text-center', 'text-primary');
    tableTitle.innerHTML = createButtonData.address;
    if (!$.fn.DataTable.isDataTable('.js-table')) {
        datatable = $('.js-table').DataTable({
            order: [[2, 'desc']],
            language: {
                url: (isRTL ? '/lib/datatable/ar.json' : ''),
            },
            columnDefs: [
                { orderable: false, targets: -1 }
            ],
            layout: {
                top2Start: {
                    rowClass: 'row card-header flex-column flex-md-row pb-0',
                    features: [tableTitle]
                },
                top2End: {
                    features: [
                        {
                            buttons: (function () {
                                var btns = [
                                    {
                                        extend: 'collection',
                                        className: 'btn btn-label-primary dropdown-toggle me-4',
                                        text: `<span class="d-flex align-items-center gap-2">
                                          <i class="icon-base bx bx-export me-sm-1"></i>
                                          <span class="d-none d-sm-inline-block">${localization.Export}</span>
                                          </span>`,
                                        buttons: ['print', 'excel'].map(type => {
                                            const icons = {
                                                print: 'bx-printer',
                                                excel: 'bxs-file-export'
                                            };
                                            const labels = {
                                                print: 'Print',
                                                excel: 'Excel'
                                            };

                                            return {
                                                extend: type,
                                                text: `<span class="d-flex align-items-center"><i class="icon-base bx ${icons[type]} me-1"></i>${labels[type]}</span>`,
                                                className: 'dropdown-item',
                                                exportOptions: {
                                                    columns: ':not(.js-no-export)',
                                                    format: { // quetion
                                                        body: function (inner) {
                                                            try {
                                                                if (!inner || typeof inner !== 'string' || inner.trim().length === 0)
                                                                    return '';

                                                                const parser = new DOMParser();
                                                                const doc = parser.parseFromString(inner, 'text/html');

                                                                const userNameElements = doc.querySelectorAll('.user-name');
                                                                if (userNameElements.length > 0) {
                                                                    return Array.from(userNameElements).map(el => {
                                                                        return (
                                                                            el.querySelector('.fw-medium')?.textContent ||
                                                                            el.querySelector('.d-block')?.textContent ||
                                                                            el.textContent
                                                                        ).trim();
                                                                    }).join(' ');
                                                                }

                                                                return (doc.body.textContent || doc.body.innerText || '').trim();
                                                            } catch (e) {
                                                                console.error('Error parsing export cell:', e);
                                                                return inner ?? '';
                                                            }
                                                        }
                                                    }
                                                },
                                                ...(type === 'print' && {
                                                    customize: function (win) {
                                                        win.document.body.style.color = config.colors.headingColor;
                                                        win.document.body.style.borderColor = config.colors.borderColor;
                                                        win.document.body.style.backgroundColor = config.colors.bodyBg;
                                                        const table = win.document.body.querySelector('table');
                                                        table.classList.add('compact');
                                                        table.style.color = 'inherit';
                                                        table.style.borderColor = 'inherit';
                                                        table.style.backgroundColor = 'inherit';
                                                    }
                                                })
                                            };
                                        })
                                    }
                                ];

                                if (!$('.js-table').hasClass('js-hide-create')) {
                                    btns.push({
                                        text: `
                                        <span class="d-flex align-items-center gap-2 js-create-action"
                                              data-mode="${createButtonData.mode}"
                                              data-title="${createButtonData.title}" 
                                              data-url="${createButtonData.url}">
                                          <i class="icon-base bx bx-plus icon-sm"></i>
                                          <span class="d-none d-sm-inline-block">${createButtonData.label}</span>
                                        </span>
                                        `,
                                        className: 'btn create-new btn-primary'
                                    });
                                }

                                return btns;
                            })()
                        }
                    ]
                }
            }
        });
    }

    setTimeout(() => {
        const elementsToModify = [
            { selector: '.dt-buttons .btn', classToRemove: 'btn-secondary' },
            { selector: '.dt-search .form-control', classToRemove: 'form-control-sm', classToAdd: 'ms-4' },
            { selector: '.dt-length .form-select', classToRemove: 'form-select-sm' },
            { selector: '.dt-layout-table', classToRemove: 'row mt-2' },
            { selector: '.dt-layout-end', classToAdd: 'mt-0' },
            { selector: '.dt-layout-end .dt-search', classToAdd: 'mt-0 mt-md-6' },
            { selector: '.dt-layout-start', classToAdd: 'mt-0' },
            { selector: '.dt-layout-end .dt-buttons', classToAdd: 'mb-0' },
            { selector: '#DataTables_Table_0_wrapper', classToAdd: 'pe-5 ps-5' }

        ];

        elementsToModify.forEach(item => {
            const $el = $(item.selector);
            if (item.classToRemove) {
                $el.removeClass(item.classToRemove);
            }
            if (item.classToAdd) {
                $el.addClass(item.classToAdd);
            }
        });
    }, 100);
}

//Select2
function applySelect2() {
    $('.js-select2').each(function () {
        const $select = $(this);

        // skip if already initialized
        if ($select.hasClass('select2-hidden-accessible')) return;

        const $modalParent = $select.closest('#Modal');
        const options = {
            width: '100%'
        };

        if ($modalParent.length > 0) {
            options.dropdownParent = $modalParent;
        }

        $select.select2(options);

        $select.on('select2:select', function () {
            $('form').not('#SignOut').validate().element('#' + $(this).attr('id'));
        });
    });
}

$(document).on('click', '.js-create-action', function () {

    const mode = $(this).data('mode');
    const url = $(this).data('url');
    const title = $(this).data('title');

    if (mode === 'modal') {

        const btn = $('<button>')
            .addClass('js-render-modal')
            .attr('data-title', title)
            .attr('data-url', url);

        $('body').append(btn);

        btn.trigger('click');

        btn.remove();

    } else {
        window.location.href = url;
    }
});

$(document).ready(function () {
    var $dataDiv = $('#create-button-data');
    createButtonData = {
        address: $dataDiv.data('address'),
        title: $dataDiv.data('title'),
        url: $dataDiv.data('url'),
        label: $dataDiv.data('label'),
        mode: $dataDiv.data('mode')
    };
    //DataTables
    setDataTable();

    applySelect2();
    // Message
    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);
    }

    var errorMessage = $('#ErrorMessage').text();
    if (errorMessage !== '') {
        showErrorMessage(errorMessage);
    }

    //Handle Toggle Status
    $('body').delegate('.js-toggle-status', 'click', function () {
        var btn = $(this);
        var tableElement = btn.closest('table');
        var datatable = $(tableElement).DataTable();

        var successMsg = btn.data('success-message');
        var errorMsg = btn.data('error-message');
        var confirmMsg = btn.data('confirm-message') || localization.confirm;

        bootbox.confirm({
            message: confirmMsg,
            buttons: {
                confirm: {
                    label: localization.yes,
                    className: 'btn-danger'
                },
                cancel: {
                    label: localization.no,
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.ajax({
                        url: btn.data('url'),
                        type: 'POST',
                        dataType: 'html',
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (rowHtml) {
                            var row = btn.parents('tr');

                            if (datatable.settings()[0].oFeatures.bServerSide) {
                                datatable.ajax.reload(null, false);
                            } else if (rowHtml && rowHtml.indexOf('<tr') >= 0) {
                                var $newRow = $(rowHtml.trim());
                                datatable.row(row).remove();
                                var addedRow = datatable.row.add($newRow).draw(false).node();

                                requestAnimationFrame(function () {
                                    $(addedRow).addClass('animate__animated animate__flash');
                                });

                                $(addedRow).on('animationend', function () {
                                    $(this).removeClass('animate__animated animate__flash');
                                });
                            } else {
                                row.addClass('animate__animated animate__fadeOut');
                                setTimeout(function () {
                                    datatable.row(row).remove().draw(false);
                                }, 500);
                            }

                            showSuccessMessage(successMsg);
                        },
                        error: function () {
                            showErrorMessage(errorMsg);
                        }
                    });
                }
            }
        });
    });
    // Restore (soft deleted)
    $('body').delegate('.js-restore', 'click', function () {
        var btn = $(this);
        var tableElement = btn.closest('table');
        var datatable = $(tableElement).DataTable();

        bootbox.confirm({
            message: localization.confirm,
            buttons: {
                confirm: {
                    label: localization.yes,
                    className: 'btn-danger'
                },
                cancel: {
                    label: localization.no,
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.ajax({
                        url: btn.data('url'),
                        type: 'POST',
                        dataType: 'html',
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (rowHtml) {
                            var row = btn.parents('tr');

                            if (datatable.settings()[0].oFeatures.bServerSide) {
                                datatable.ajax.reload(null, false);
                            } else if (rowHtml && rowHtml.indexOf('<tr') >= 0) {
                                var $newRow = $(rowHtml.trim());
                                datatable.row(row).remove();
                                var addedRow = datatable.row.add($newRow).draw(false).node();

                                requestAnimationFrame(function () {
                                    $(addedRow).addClass('animate__animated animate__flash');
                                });

                                $(addedRow).on('animationend', function () {
                                    $(this).removeClass('animate__animated animate__flash');
                                });
                            }

                            showSuccessMessage();
                        },
                        error: function () {
                            showErrorMessage();
                        }
                    });
                }
            }
        });
    });
    //Handle Toggle Status
    $('body').delegate('.js-toggle-active', 'click', function () {
        var btn = $(this);

        bootbox.confirm({
            message: localization.confirm,
            buttons: {
                confirm: {
                    label: localization.yes,
                    className: 'btn-danger'
                },
                cancel: {
                    label: localization.no,
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function () {
                            var status = btn.parents('tr').find('.js-status');
                            var newText = status.text().trim() === "Active" ? "InActive" : "Active";
                            status.text(newText);
                            status.toggleClass("bg-label-success bg-label-danger");
                            row.addClass('animate__animated animate__fadeOut');
                            showSuccessMessage();
                        },
                        error: function () {
                            showErrorMessage();
                        }
                    });
                }
            }
        });
    });
    //Handle bootstrap modal
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');

        modal.find('#ModalLabel').text(btn.data('title'));

        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        }

        $.get({
            url: btn.data('url'),
            success: function (form) {
                modal.find('.modal-body').html(form);
                $.validator.unobtrusive.parse(modal);
                applySelect2();
            },
            error: function () {
                showErrorMessage();
            }
        });

        modal.modal('show');
    });
});