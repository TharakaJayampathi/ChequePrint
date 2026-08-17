$(document).ready(function () {
    console.log('Working');
    $("#selectPaymentMethod").val(1).trigger('change.select2');
})

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

$("#selectPaymentMethod").select2({
    placeholder: "-Please Select-"
});