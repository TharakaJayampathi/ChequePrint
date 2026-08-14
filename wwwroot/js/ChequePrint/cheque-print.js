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
        xhrFields: {
            responseType: 'blob'
        },
        success: function (blob, status, xhr) {
            var disposition = xhr.getResponseHeader('Content-Disposition');
            var fileName = 'ChequePrint.zip';
            if (disposition && disposition.indexOf('filename=') !== -1) {
                var match = disposition.match(/filename\*?=(?:UTF-8'')?["']?([^"';]+)["']?/i);
                if (match && match[1]) {
                    fileName = decodeURIComponent(match[1]);
                }
            }

            var downloadUrl = window.URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = downloadUrl;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(downloadUrl);

            toastr.success("Cheque Print generated successfully!");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.responseText) {
                try {
                    var errJson = JSON.parse(jqXHR.responseText);
                    toastr.error(errJson.message || "Something went wrong.");
                } catch (e) {
                    toastr.error("Something went wrong.");
                }
            } else if (jqXHR.responseJSON && jqXHR.responseJSON.message) {
                toastr.error(jqXHR.responseJSON.message);
            } else {
                if (jqXHR.response instanceof Blob) {
                    var reader = new FileReader();
                    reader.onload = function () {
                        try {
                            var errJson = JSON.parse(reader.result);
                            toastr.error(errJson.message || "Something went wrong.");
                        } catch (e) {
                            toastr.error("Something went wrong.");
                        }
                    };
                    reader.readAsText(jqXHR.response);
                } else {
                    toastr.error("Something went wrong.");
                }
            }
        },
        complete: function () {
            $('#btnText').show();
            $('#btnSpinner').hide();
            $('#btnAddAttachment').prop('disabled', false);
            document.getElementById('txtAttachment').value = '';
        }
    });
});