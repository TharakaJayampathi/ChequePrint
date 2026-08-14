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
            responseType: 'blob' // important: tells the browser to expect binary data
        },
        success: function (blob, status, xhr) {
            // Try to pull the real file name from the Content-Disposition header
            var disposition = xhr.getResponseHeader('Content-Disposition');
            var fileName = 'ChequePrintLetters.zip';
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

            toastr.success("Cheque print letters generated successfully!");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            // jQuery gives us the blob here too since responseType is 'blob',
            // so we need to read it back out as text/JSON to get the error message
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
                // when responseType is blob, jQuery won't auto-parse JSON errors —
                // read the blob manually as a fallback
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