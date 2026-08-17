$(document).ready(function () {
    console.log('Working');
    $("#selectPaymentMethod").val(1).trigger('change.select2');
    $('#txtChequeName').prop('disabled', true);
    $("#txtChequeName").val('');
})

$("#btnPrint").on("click", async function () {
    var paymentMethod = $('#selectPaymentMethod').val() || null;
    var chequeName = $('#txtChequeName').val().trim() || null;
    var date = $('#txtDate').val() || null;
    var amount = $('#txtAmount').val() || null;

    console.log("paymentMethod", paymentMethod);
    console.log("chequeName", chequeName);
    console.log("date", date);
    console.log("amount", amount);

    // Field Validation
    if (!paymentMethod) {
        toastr.error("", "Payment Method is mandatory", { progressBar: true });
        return false;
    }
    if (!chequeName && paymentMethod == 2) {
        toastr.error("", "Cheque Name is mandatory", { progressBar: true });
        return false;
    }
    if (!date) {
        toastr.error("", "Date is mandatory", { progressBar: true });
        return false;
    }
    if (!amount) {
        toastr.error("", "Amount is mandatory", { progressBar: true });
        return false;
    }

    let checkPrint = {};
    checkPrint.paymentMethod = paymentMethod;
    checkPrint.chequeName = chequeName;
    checkPrint.date = date;
    checkPrint.amount = amount;

    console.log("Check Print Details:");
    console.log(checkPrint);

    var title = "Are you sure you want to Print?"
    swal({
        title: title,
        text: "",
        icon: "warning",
        buttons: ['No', 'Yes'],
        dangerMode: true,
    }).then(function (isConfirm) {
        if (isConfirm) {
            document.getElementById("pageOverlay").style.display = "block";
            $('#btnPrint').prop("disabled", true);
            return $.ajax({
                url: "/api/chequeprint/cheque-print",
                method: "POST",
                data: JSON.stringify(checkPrint),
                contentType: "application/json; charset=utf-8",
                success: function (response) {
                    toastr.success("", "Check Printed Successfully", { progressBar: true });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var errorMessage = jqXHR.responseJSON && jqXHR.responseJSON.message
                        ? jqXHR.responseJSON.message
                        : "Something went wrong.";
                    console.log(errorMessage);
                    toastr.error(errorMessage);
                    document.getElementById("pageOverlay").style.display = "none";
                    $('#btnPrint').prop("disabled", false);
                },
                complete: function () {
                }
            });
        } else {
            swal("Cancelled", "", "error");
        }
    })

});

$('#txtAmount').on('input', function () {
    let value = this.value;
    const regex = /^\d*(\.\d{0,2})?$/;
    if (regex.test(value)) {
        $(this).data('lastValidValue', value);
    } else {
        this.value = $(this).data('lastValidValue') || '';
    }
});

$('#txtAmount').on('keypress', function (e) {
    if (e.key === '-') {
        e.preventDefault();
    }
});

$('#selectPaymentMethod').on('change', async function () {
    var paymentMethod = $('#selectPaymentMethod').val() || null;
    if (paymentMethod == 2) {
        $('#txtChequeName').prop('disabled', false);
    }
    else {
        $('#txtChequeName').prop('disabled', true);
        $("#txtChequeName").val('');
    }
})

$("#selectPaymentMethod").select2({
    placeholder: "-Please Select-"
});