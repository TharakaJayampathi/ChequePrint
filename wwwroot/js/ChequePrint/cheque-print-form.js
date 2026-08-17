$(document).ready(function () {
    console.log('Working');
    $("#selectPaymentMethod").val(1).trigger('change.select2');
})

$("#selectPaymentMethod").select2({
    placeholder: "-Please Select-"
});