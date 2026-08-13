$("#btnAddAttachment").on("click", function () {
    var input = document.getElementById("txtAttachment");
    var formData = new FormData();
    if (!input.files || input.files.length === 0) {
        toastr.error("No file selected.");
        return;
    }
    $('#btnAddAttachment').attr('disabled', true);
    $('#btnAttachmentSubmit').attr('disabled', true);
    $('#lblSpinner').show();
    var files = input.files;
    for (var i = 0; i < files.length; i++) {
        formData.append("Files", files[i]);
    }
    $.ajax({
        url: '/api/AdminRequest/AdminRequestAttachmentUpload',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            console.log(response);
            attachmentLines = $.merge(attachmentLines, response);
            $.each(attachmentLines, function (i, item) {
                item.no = i + 1;
            });
            bindAttachmentGrid(attachmentLines);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var errorMessage = jqXHR.responseJSON && jqXHR.responseJSON.message
                ? jqXHR.responseJSON.message
                : "Something went wrong.";
            toastr.error(errorMessage);
        },
        complete: function () {
            $('#btnAddAttachment').attr('disabled', false);
            $('#btnAttachmentSubmit').attr('disabled', false);
            $('#lblSpinner').hide();
            document.getElementById('txtAttachment').value = '';
        }
    });
});