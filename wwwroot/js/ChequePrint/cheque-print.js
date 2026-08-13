$("#btnAddAttachment").on("click", function () {
    var input = document.getElementById("txtAttachment");
    var formData = new FormData();
    if (!input.files || input.files.length === 0) {
        toastr.error("No file selected.");
        return;
    }
    $('#btnText').hide();
    $('#btnSpinner').show();
    $('#btnAddAttachment').prop('disabled', true);

    var files = input.files;
    for (var i = 0; i < files.length; i++) {
        formData.append("Files", files[i]);
    }
    $.ajax({
        url: '/api/ChequePrint/ChequePrintAttachmentUpload',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            console.log(response);
            toastr.success("File uploaded successfully!");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var errorMessage = jqXHR.responseJSON && jqXHR.responseJSON.message
                ? jqXHR.responseJSON.message
                : "Something went wrong.";
            toastr.error(errorMessage);
        },
        complete: function () {
            $('#btnText').show();
            $('#btnSpinner').hide();
            $('#btnAddAttachment').prop('disabled', false);
            document.getElementById('txtAttachment').value = '';
        }
    });
});