$(document).ready(function () {
    let productsList = [];

    const urls = {
        getProducts: '/PurchaseInvoices/GetProducts',
        getWarehouses: '/PurchaseInvoices/GetWarehouses',
        getCashBoxes: '/PurchaseInvoices/GetCashBoxes',
        getUnits: '/PurchaseInvoices/GetUnits',
        getPurchasePrice: '/PurchaseInvoices/GetPurchasePrice'
    };

    function buildProductOptions(selectedProductId) {
        let options = '<option value="">اختر الصنف</option>';
        $.each(productsList, function (i, p) {
            const selected = selectedProductId && p.value == selectedProductId ? ' selected' : '';
            options += `<option value="${p.value}"${selected}>${p.text}</option>`;
        });
        return options;
    }

    function buildRowHtml(index, item) {
        const productId = item ? item.productId : null;
        const quantity = item ? item.quantity : 1;
        const purchasePrice = item ? item.purchasePrice : 0;
        const discount = item ? (item.discount ?? 0) : 0;

        return `
            <tr>
                <td>
                    <select name="Items[${index}].ProductId" class="form-select product-select" data-index="${index}">
                        ${buildProductOptions(productId)}
                    </select>
                </td>
                <td>
                    <select name="Items[${index}].ProductUnitId" class="form-select unit-select" data-index="${index}">
                        <option value="">اختر الوحدة</option>
                    </select>
                </td>
                <td>
                    <input type="number" step="0.01" min="0.01" name="Items[${index}].Quantity" class="form-control text-center qty-input" value="${quantity}" />
                </td>
                <td>
                    <input type="number" step="0.01" min="0" name="Items[${index}].PurchasePrice" class="form-control text-center price-input" value="${purchasePrice}" />
                </td>
                <td>
                    <input type="number" step="0.01" min="0" name="Items[${index}].Discount" class="form-control text-center item-discount-input" value="${discount}" />
                </td>
                <td>
                    <input type="number" readonly class="form-control text-center item-total bg-light" value="0" />
                </td>
                <td>
                    <button type="button" class="btn btn-sm btn-outline-danger btn-remove"><i class="bx bx-trash"></i></button>
                </td>
            </tr>
        `;
    }

    function loadUnitsForRow(row, productId, selectedUnitId) {
        const unitSelect = row.find(".unit-select");
        unitSelect.empty().append('<option value="">اختر الوحدة</option>');

        if (!productId) return $.Deferred().resolve().promise();

        return $.get(urls.getUnits, { productId: productId }).then(function (data) {
            $.each(data, function (i, item) {
                const selected = selectedUnitId && item.value == selectedUnitId ? ' selected' : '';
                unitSelect.append($(`<option value="${item.value}"${selected}>${item.text}</option>`));
            });
        });
    }

    function addRow(item) {
        const index = $("#itemsTable tbody tr").length;
        const row = $(buildRowHtml(index, item));
        $("#itemsTable tbody").append(row);

        if (!item || !item.productId) return $.Deferred().resolve().promise();

        return loadUnitsForRow(row, item.productId, item.productUnitId).then(function () {
            calculateRowTotal(row);
        });
    }

    function loadExistingItems() {
        const items = window.existingItems || [];
        if (!items.length) return;

        const chain = items.reduce(function (promise, item) {
            return promise.then(function () {
                return addRow(item);
            });
        }, $.Deferred().resolve());

        chain.then(function () {
            calculateInvoiceTotals();
        });
    }

    $.get(urls.getProducts, function (data) {
        productsList = data;
        loadExistingItems();
    });

    $("#BranchId").on("change", function () {
        let branchId = $(this).val();

        $("#WarehouseId").empty().append('<option value="">اختر المخزن</option>');
        $("#CashBoxId").empty().append('<option value="">اختر الخزنة</option>');

        if (!branchId) return;

        $.get(urls.getWarehouses, { branchId: branchId }, function (data) {
            $.each(data, function (i, item) {
                $("#WarehouseId").append($('<option>', { value: item.value, text: item.text }));
            });
        });

        $.get(urls.getCashBoxes, { branchId: branchId }, function (data) {
            $.each(data, function (i, item) {
                $("#CashBoxId").append($('<option>', { value: item.value, text: item.text }));
            });
        });
    });

    $("#btnAddRow").on("click", function () {
        addRow(null);
    });

    // 4. تحميل وحدات المنتج واختيار الوحدة الأساسية تلقائياً
    $(document).on("change", ".product-select", function () {
        let productId = $(this).val();
        let row = $(this).closest("tr");
        let unitSelect = row.find(".unit-select");

        unitSelect.empty().append('<option value="">اختر الوحدة</option>');
        row.find(".price-input").val(0); // إعادة تصفير السعر عند تغيير المنتج

        if (!productId) return;

        $.get(urls.getUnits, { productId: productId }, function (data) {
            let defaultUnitId = null;

            $.each(data, function (i, item) {
                // إضافة الخيار للقائمة
                let option = $('<option>', { value: item.value, text: item.text });

                // تحديد الوحدة الأساسية إذا كانت معلمة كـ Default أو أخذ أول وحدة كافتراضية
                if (item.isDefault || i === 0) {
                    option.attr('selected', 'selected');
                    defaultUnitId = item.value;
                }

                unitSelect.append(option);
            });

            // إذا تم اختيار وحدة أساسية تلقائياً، نجلب سعرها فوراً!
            if (defaultUnitId) {
                unitSelect.val(defaultUnitId).trigger('change');
            }
        });
    });

    $(document).on("change", ".unit-select", function () {
        let unitId = $(this).val();
        let row = $(this).closest("tr");

        if (!unitId) return;

        $.get(urls.getPurchasePrice, { productUnitId: unitId }, function (price) {
            row.find(".price-input").val(price);
            calculateRowTotal(row);
        });
    });

    $(document).on("input", ".qty-input, .price-input, .item-discount-input", function () {
        calculateRowTotal($(this).closest("tr"));
    });

    $(document).on("input", "#Discount, #Paid", function () {
        calculateInvoiceTotals();
    });

    $(document).on("click", ".btn-remove", function () {
        $(this).closest("tr").remove();
        reindexRows();
        calculateInvoiceTotals();
    });

    function calculateRowTotal(row) {
        let qty = parseFloat(row.find(".qty-input").val()) || 0;
        let price = parseFloat(row.find(".price-input").val()) || 0;
        let discount = parseFloat(row.find(".item-discount-input").val()) || 0;

        let total = (qty * price) - discount;
        row.find(".item-total").val(total > 0 ? total.toFixed(2) : 0);

        calculateInvoiceTotals();
    }

    function calculateInvoiceTotals() {
        let itemsTotal = 0;
        $(".item-total").each(function () {
            itemsTotal += parseFloat($(this).val()) || 0;
        });

        let invoiceDiscount = parseFloat($("#Discount").val()) || 0;
        let netTotal = itemsTotal - invoiceDiscount;
        netTotal = netTotal > 0 ? netTotal : 0;

        $("#TotalAmount").val(netTotal.toFixed(2));

        let paid = parseFloat($("#Paid").val()) || 0;
        let remaining = netTotal - paid;

        $("#Remaining").val(remaining.toFixed(2));
    }

    function reindexRows() {
        $("#itemsTable tbody tr").each(function (i, row) {
            $(row).find(".product-select").attr("name", `Items[${i}].ProductId`);
            $(row).find(".unit-select").attr("name", `Items[${i}].ProductUnitId`);
            $(row).find(".qty-input").attr("name", `Items[${i}].Quantity`);
            $(row).find(".price-input").attr("name", `Items[${i}].PurchasePrice`);
            $(row).find(".item-discount-input").attr("name", `Items[${i}].Discount`);
        });
    }
});
