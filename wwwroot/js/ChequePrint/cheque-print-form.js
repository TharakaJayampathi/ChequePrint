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
    if (paymentMethod == 1) {
        chequeName = "CASH";
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

            $.ajax({
                url: "/api/chequeprint/cheque-print",
                method: "POST",
                data: JSON.stringify(checkPrint),
                contentType: "application/json; charset=utf-8",
                xhrFields: {
                    responseType: 'blob' 
                },
                success: function (response, status, xhr) {
                    var blob = new Blob([response], { type: 'application/pdf' });
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    var contentDisposition = xhr.getResponseHeader('Content-Disposition');
                    var filename = 'Cheque_Print.pdf';
                    if (contentDisposition) {
                        var match = contentDisposition.match(/filename\*?=([^;]+)/);
                        if (match && match[1]) {
                            filename = decodeURIComponent(match[1].replace(/["']/g, ''));
                        }
                    }
                    link.download = filename;
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);

                    toastr.success("", "Check Printed Successfully", { progressBar: true });
                    document.getElementById("pageOverlay").style.display = "none";
                    $('#btnPrint').prop("disabled", false);
                    $("#selectPaymentMethod").val(1).trigger('change.select2');
                    $('#txtChequeName').prop('disabled', true);
                    $("#txtChequeName").val('');
                    $("#txtDate").val('');
                    $("#txtAmount").val('');
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var errorMessage = "Something went wrong.";
                    try {
                        var response = JSON.parse(jqXHR.responseText);
                        errorMessage = response.message || errorMessage;
                    } catch (e) {
                        errorMessage = jqXHR.statusText || errorMessage;
                    }
                    console.log(errorMessage);
                    toastr.error(errorMessage);
                    document.getElementById("pageOverlay").style.display = "none";
                    $('#btnPrint').prop("disabled", false);
                    $("#selectPaymentMethod").val(1).trigger('change.select2');
                    $('#txtChequeName').prop('disabled', true);
                    $("#txtChequeName").val('');
                    $("#txtDate").val('');
                    $("#txtAmount").val('');
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