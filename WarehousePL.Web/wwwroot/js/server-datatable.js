var ServerDataTable = (function () {
    'use strict';

    function parseDefaultOrder(value) {
        if (!value) return [[0, 'asc']];

        var parts = value.split(',');
        var column = parseInt(parts[0], 10);
        var direction = (parts[1] || 'asc').toLowerCase() === 'desc' ? 'desc' : 'asc';

        return [[isNaN(column) ? 0 : column, direction]];
    }

    function buildColumns($table) {
        var columns = [];

        $table.find('thead th').each(function () {
            var $th = $(this);
            columns.push({
                data: null,
                name: $th.data('name') || '',
                orderable: !$th.hasClass('js-no-sort'),
                searchable: !$th.hasClass('js-no-search'),
                defaultContent: ''
            });
        });

        return columns;
    }

    function parseRowHtml(rowHtml) {
        var $row = $(rowHtml.trim());
        if (!$row.is('tr')) {
            $row = $('<table><tbody>' + rowHtml + '</tbody></table>').find('tr').first();
        }

        return $row.find('td').map(function () {
            return $(this).html();
        }).get();
    }

    function formatExportCell(inner) {
        try {
            if (!inner || typeof inner !== 'string' || inner.trim().length === 0)
                return '';

            var parser = new DOMParser();
            var doc = parser.parseFromString(inner, 'text/html');
            var userNameElements = doc.querySelectorAll('.user-name');

            if (userNameElements.length > 0) {
                return Array.from(userNameElements).map(function (el) {
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

    function buildExportButtons() {
        var exportLabel = typeof localization !== 'undefined' ? localization.Export : 'Export';

        return ['print', 'excel'].map(function (type) {
            var icons = { print: 'bx-printer', excel: 'bxs-file-export' };
            var labels = { print: 'Print', excel: 'Excel' };
            var button = {
                extend: type,
                text: '<span class="d-flex align-items-center"><i class="icon-base bx ' + icons[type] + ' me-1"></i>' + labels[type] + '</span>',
                className: 'dropdown-item',
                exportOptions: {
                    columns: ':not(.js-no-export)',
                    format: {
                        body: formatExportCell
                    }
                }
            };

            if (type === 'print' && typeof config !== 'undefined') {
                button.customize = function (win) {
                    win.document.body.style.color = config.colors.headingColor;
                    win.document.body.style.borderColor = config.colors.borderColor;
                    win.document.body.style.backgroundColor = config.colors.bodyBg;
                    var table = win.document.body.querySelector('table');
                    table.classList.add('compact');
                    table.style.color = 'inherit';
                    table.style.borderColor = 'inherit';
                    table.style.backgroundColor = 'inherit';
                };
            }

            return button;
        });
    }

    function applyLayoutStyles(tableSelector) {
        var wrapperSelector = tableSelector + '_wrapper';

        setTimeout(function () {
            var elementsToModify = [
                { selector: wrapperSelector, classToAdd: 'p-2' },
                { selector: '.dt-buttons .btn', classToRemove: 'btn-secondary' },
                { selector: '.dt-search .form-control', classToRemove: 'form-control-sm', classToAdd: 'ms-4' },
                { selector: '.dt-length .form-select', classToRemove: 'form-select-sm' },
                { selector: '.dt-layout-table', classToRemove: 'row mt-2' },
                { selector: '.dt-layout-end', classToAdd: 'mt-0' },
                { selector: '.dt-layout-end .dt-search', classToAdd: 'mt-0 mt-md-6' },
                { selector: '.dt-layout-start', classToAdd: 'mt-0' },
                { selector: '.dt-layout-end .dt-buttons', classToAdd: 'mb-0' }
            ];

            elementsToModify.forEach(function (item) {
                var $el = $(item.selector);
                if (item.classToRemove) $el.removeClass(item.classToRemove);
                if (item.classToAdd) $el.addClass(item.classToAdd);
            });
        }, 100);
    }

    function buildLayout(tableTitle, createButton, tableSelector) {
        var layout = {
            top2Start: {
                rowClass: 'row card-header flex-column flex-md-row pb-0',
                features: []
            },
            top2End: {
                features: []
            }
        };

        if (tableTitle) {
            var title = document.createElement('h5');
            title.classList.add('card-title', 'mb-0', 'text-md-start', 'text-center', 'text-primary');
            title.innerHTML = tableTitle;
            layout.top2Start.features.push(title);
        }

        var exportLabel = typeof localization !== 'undefined' ? localization.Export : 'Export';
        var topButtons = [{
            extend: 'collection',
            className: 'btn btn-label-primary dropdown-toggle me-4',
            text: '<span class="d-flex align-items-center gap-2">' +
                '<i class="icon-base bx bx-export me-sm-1"></i>' +
                '<span class="d-none d-sm-inline-block">' + exportLabel + '</span>' +
                '</span>',
            buttons: buildExportButtons()
        }];

        if (createButton && !$(tableSelector).hasClass('js-hide-create')) {
            topButtons.push({
                text: '<span class="d-flex align-items-center gap-2 js-create-action"' +
                    ' data-mode="' + createButton.mode + '"' +
                    ' data-title="' + createButton.title + '"' +
                    ' data-url="' + createButton.url + '">' +
                    '<i class="icon-base bx bx-plus icon-sm"></i>' +
                    '<span class="d-none d-sm-inline-block">' + createButton.label + '</span>' +
                    '</span>',
                className: 'btn create-new btn-primary'
            });
        }

        layout.top2End.features.push({ buttons: topButtons });

        return layout;
    }

    function init(options) {
        var $table = $(options.tableSelector);
        if (!$table.length || $.fn.DataTable.isDataTable($table)) return null;

        var columns = buildColumns($table);
        var columnCount = columns.length;
        var dataColumns = Array.from({ length: columnCount }, function (_, index) {
            return {
                data: index,
                orderable: columns[index].orderable,
                searchable: columns[index].searchable,
                render: function (data) {
                    return data ?? '';
                }
            };
        });

        var columnDefs = [];
        $table.find('thead th').each(function (index) {
            var $th = $(this);
            var def = { targets: index };

            if ($th.hasClass('js-no-sort')) def.orderable = false;
            if ($th.hasClass('js-no-export')) def.className = (def.className || '') + ' js-no-export';

            columnDefs.push(def);
        });

        var listUrl = options.listUrl || $table.data('list-url');
        var defaultOrder = parseDefaultOrder(options.defaultOrder || $table.data('default-order'));
        var antiForgeryToken = options.antiForgeryToken || '';
        var customDataFn = options.customDataFn || function () { return {}; };

        var datatable = $table.DataTable({
            processing: true,
            serverSide: true,
            deferRender: true,
            order: defaultOrder,
            language: {
                url: options.languageUrl || (typeof isRTL !== 'undefined' && isRTL ? '/lib/datatable/ar.json' : '')
            },
            ajax: {
                url: listUrl,
                type: 'POST',
                data: function (d) {
                    d.__RequestVerificationToken = antiForgeryToken;
                    var customData = customDataFn();
                    $.extend(d, customData);
                    return d;
                },
                dataSrc: function (json) {
                    return (json.data || []).map(parseRowHtml);
                },
                error: function () {
                    if (typeof showErrorNotyf === 'function') {
                        showErrorNotyf(options.errorMessage);
                    }
                }
            },
            columns: dataColumns,
            columnDefs: columnDefs,
            layout: buildLayout(options.tableTitle, options.createButton, options.tableSelector)
        });

        applyLayoutStyles(options.tableSelector);
        return datatable;
    }

    return {
        init: init
    };
})();
