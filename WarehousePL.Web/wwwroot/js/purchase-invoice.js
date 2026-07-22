$(document).ready(function () {
    let productsList = [];

    // تعريف مسارات الـ API (تعدل حسب الـ Controller والروتينج عندك)
    const urls = {
        getProducts: '/PurchaseInvoices/GetProducts',
        getWarehouses: '/PurchaseInvoices/GetWarehouses',
        getCashBoxes: '/PurchaseInvoices/GetCashBoxes',
        getUnits: '/PurchaseInvoices/GetUnits',
        getPurchasePrice: '/PurchaseInvoices/GetPurchasePrice'
    };

    // 1. تحميل المنتجات مرة واحدة لاستخدامها في الأسطر
    $.get(urls.getProducts, function (data) {
        productsList = data;
    });

    // 2. تحديث المخازن والخزن عند تغيير الفرع
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

    // 3. إضافة سطر جديد بالجدول
    $("#btnAddRow").on("click", function () {
        let index = $("#itemsTable tbody tr").length;

        let productOptions = '<option value="">اختر الصنف</option>';
        $.each(productsList, function (i, p) {
            productOptions += `<option value="${p.value}">${p.text}</option>`;
        });

        let rowHtml = `
            <tr>
                <td>
                    <select name="Items[${index}].ProductId" class="form-select product-select" data-index="${index}">
                        ${productOptions}
                    </select>
                </td>
                <td>
                    <select name="Items[${index}].ProductUnitId" class="form-select unit-select" data-index="${index}">
                        <option value="">اختر الوحدة</option>
                    </select>
                </td>
                <td>
                    <input type="number" step="0.01" min="0.01" name="Items[${index}].Quantity" class="form-control text-center qty-input" value="1" />
                </td>
                <td>
                    <input type="number" step="0.01" min="0" name="Items[${index}].PurchasePrice" class="form-control text-center price-input" value="0" />
                </td>
                <td>
                    <input type="number" step="0.01" min="0" name="Items[${index}].Discount" class="form-control text-center item-discount-input" value="0" />
                </td>
                <td>
                    <input type="number" readonly class="form-control text-center item-total bg-light" value="0" />
                </td>
                <td>
                    <button type="button" class="btn btn-sm btn-outline-danger btn-remove"><i class="bx bx-trash"></i></button>
                </td>
            </tr>
        `;

        $("#itemsTable tbody").append(rowHtml);
    });

    // 4. تحميل وحدات المنتج عند اختياره
    $(document).on("change", ".product-select", function () {
        let productId = $(this).val();
        let row = $(this).closest("tr");
        let unitSelect = row.find(".unit-select");

        unitSelect.empty().append('<option value="">اختر الوحدة</option>');

        if (!productId) return;

        $.get(urls.getUnits, { productId: productId }, function (data) {
            $.each(data, function (i, item) {
                unitSelect.append($('<option>', { value: item.value, text: item.text }));
            });
        });
    });

    // 5. جلب سعر الشراء للوحدة المختارة
    $(document).on("change", ".unit-select", function () {
        let unitId = $(this).val();
        let row = $(this).closest("tr");

        if (!unitId) return;

        $.get(urls.getPurchasePrice, { productUnitId: unitId }, function (price) {
            row.find(".price-input").val(price);
            calculateRowTotal(row);
        });
    });

    // 6. أحداث إعادة الحسابات
    $(document).on("input", ".qty-input, .price-input, .item-discount-input", function () {
        calculateRowTotal($(this).closest("tr"));
    });

    $(document).on("input", "#Discount, #Paid", function () {
        calculateInvoiceTotals();
    });

    // 7. حذف السطر وإعادة ترقيم الفهارس
    $(document).on("click", ".btn-remove", function () {
        $(this).closest("tr").remove();
        reindexRows();
        calculateInvoiceTotals();
    });

    // --- دوال الحسابات والإدارة ---

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