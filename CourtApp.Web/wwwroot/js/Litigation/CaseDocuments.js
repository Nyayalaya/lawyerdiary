$(document).ready(function () {
    BindType('dynamicdata');

    $(document).on("change", ".doctypechange", function () {
        var id = $(this).attr('id').split("_")[1];
        BindDocument('Documents_' + id + '__DocId', $("#Documents_" + id + "__TypeId").val());
    });

    $('#CaseDocUpload').submit(function (e) {
        e.preventDefault();
        var form = $('#CaseDocUpload')[0];
        var formData = new FormData(form);

        Swal.fire({
            title: 'Uploading...',
            text: 'Please wait while the document is being uploaded.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        $.ajax({
            url: form.action,
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                Swal.close();

                if (response.success === true) {
                    Swal.fire({
                        title: 'Success!',
                        text: 'Document uploaded successfully!',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            refreshDocsTable(response.caseId, response.caseType);
                        }
                    });
                } else {
                    Swal.fire({
                        title: 'Failure!',
                        text: response.message || 'Upload failed.',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            error: function (xhr) {
                Swal.close();
                Swal.fire({
                    title: 'Error!',
                    text: xhr.responseText || 'An error occurred during upload.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        });
    });
});

function refreshDocsTable(caseId, caseType) {
    $.ajax({
        url: '/Litigation/CaseManage/GetUpdatedDocs?caseId=' + caseId + "&reft=" + caseType,
        type: 'GET',
        success: function (html) {
            const trimmedHtml = $.trim(html);
            if (trimmedHtml) {
                $('#tblCaseHistory tbody').html(trimmedHtml);
            } else {
                $('#tblCaseHistory tbody').html(
                    '<tr><td colspan="100%" class="text-center text-muted">No records found.</td></tr>'
                );
            }
        },
        error: function () {
            $('#tblCaseHistory tbody').html(
                '<tr><td colspan="100%" class="text-center text-danger">Failed to refresh document list.</td></tr>'
            );
        }
    });
}


function ConfirmDelete(DocId, fpath) {
    Swal.fire({
        title: "Are you sure?",
        text: "Do you want to delete the file: " + fpath + "?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            deleteFile(DocId, fpath);
        }
    });
}

function deleteFile(fileId, fpath) {
    Swal.fire({
        title: 'Deleting...',
        text: 'Please wait while the file is being deleted.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    fetch("/Litigation/CaseManage/DeleteDoc?id=" + fileId + "&fPath=" + encodeURIComponent(fpath), {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            Swal.close(); // Close the loader

            if (data.success) {
                Swal.fire("Deleted!", "Your file has been deleted.", "success")
                    .then(() => {
                        refreshDocsTable($("#CaseId").val(), $("#Reference").val());
                    });
            } else {
                Swal.fire("Error!", "Something went wrong while deleting the file.", "error");
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire("Error!", "A network error occurred while deleting the file.", "error");
        });
}


function removeKolom(id) {
    $(".removedoc_" + id).remove();
}

var incval = 0;

function defaultload() {
    var content = '<tr class="removedoc_' + incval + '">' +
        '<td><select class="form-control select2bs4 doctypechange" id="Documents_' + incval + '__TypeId" name="Documents[' + incval + '].TypeId"></select></td>' +
        '<td><select class="form-control select2bs4" id="Documents_' + incval + '__DocId" name="Documents[' + incval + '].DocId"></select></td>' +
        '<td><input type="date" id="OrderDate_' + incval + '__DocDate" name="Documents[' + incval + '].DocDate" class="form-control" /></td>' +
        '<td><input type="file" name="Documents[' + incval + '].Document" id="Documents_' + incval + '__Document" class="form-control" /></td>' +
        '<td><button class="add-btn-repeat btn btn-success" onclick="addElement()" type="button"><i class="fa fa-plus"></i></button></td></tr>';

    $('#tblDocs tbody').append(content);

    var data = $("#dynamicdata").html();
    $("#Documents_" + incval + "__TypeId").html('<option value="">Select</option>' + data);
    $("#Documents_" + incval + "__DocId").html('<option value="">Select</option>');

    $("#Documents_" + incval + "__TypeId").select2({
        placeholder: "Select a type",
        theme: "bootstrap4",
        escapeMarkup: function (m) {
            return m;
        }
    });

    $("#Documents_" + incval + "__DocId").select2({
        placeholder: "Select a document",
        theme: "bootstrap4",
        escapeMarkup: function (m) {
            return m;
        }
    });

    incval++;
}

function validation() {
    var count = incval - 1;
    var type = $("#Documents_" + count + "__TypeId").val();
    var doc = $("#Documents_" + count + "__DocId").val();
    var imgv = $("#Documents_" + count + "__Document").val();

    if (!type || !doc || !imgv) {
        return false;
    }

    return true;
}

function addElement() {
    if (!validation()) {
        alert("Please select all the fields. They are required.");
        return;
    }

    var content = '<tr class="removedoc_' + incval + '">' +
        '<td><select class="form-control select2bs4 doctypechange" id="Documents_' + incval + '__TypeId" name="Documents[' + incval + '].TypeId"></select></td>' +
        '<td><select class="form-control select2bs4" id="Documents_' + incval + '__DocId" name="Documents[' + incval + '].DocId"></select></td>' +
        '<td><input type="date" id="OrderDate_' + incval + '__DocDate" name="Documents[' + incval + '].DocDate" class="form-control" /></td>' +
        '<td><input type="file" name="Documents[' + incval + '].Document" id="Documents_' + incval + '__Document" class="form-control" /></td>' +
        '<td><button type="button" onclick="removeKolom(' + incval + ')" class="btn btn-warning"><i class="fa fa-trash"></i></button></td></tr>';

    $('#tblDocs tbody').append(content);

    var data = $("#dynamicdata").html();
    $("#Documents_" + incval + "__TypeId").html('<option value="">Select</option>' + data);
    $("#Documents_" + incval + "__DocId").html('<option value="">Select</option>');

    $("#Documents_" + incval + "__TypeId").select2({
        placeholder: "Select a type",
        theme: "bootstrap4",
        escapeMarkup: function (m) {
            return m;
        }
    });

    $("#Documents_" + incval + "__DocId").select2({
        placeholder: "Select a document",
        theme: "bootstrap4",
        escapeMarkup: function (m) {
            return m;
        }
    });

    incval++;
}

function BindType(ddl) {
    $.getJSON("/Litigation/casemanage/DOCTypes", function (data) {
        $("#" + ddl).empty();
        $("#" + ddl).append('<option value="">Select</option>');

        $.each(data, function (key, value) {
            $("#" + ddl).append(`<option value="${key}">${value}</option>`);
        });

        defaultload();
    });
}

function BindDocument(ddl, v) {
    $("#" + ddl).empty();
    $("#" + ddl).append('<option value="">Select</option>');

    $.getJSON("/Litigation/casemanage/DdlDOTypes?TypeId=" + v, function (data) {
        $.each(data.Data, function (i, item) {
            $("#" + ddl).append(`<option value="${item.Id}">${item.Name_En}</option>`);
        });
    });

    $("#" + ddl).select2({
        placeholder: "Select a document",
        theme: "bootstrap4",
        escapeMarkup: function (m) {
            return m;
        }
    });
}


function ReplaceDocument(id,docType,fileId) {
    Swal.fire({
        title: "Are you sure?",
        text: "Do you want to replace the file: " + fpath + "?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, replace it!"
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Replacing...',
                text: 'Please wait while the file is being replaced.',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            fetch("/Litigation/CaseManage/ReplaceDocument?docId=" + id + "&fPath=" + encodeURIComponent(fpath), {
                method: 'POST'
            })
                .then(response => response.json())
                .then(data => {
                    Swal.close(); // Close the loader

                    if (data.success) {
                        Swal.fire("Deleted!", "Your file has been deleted.", "success")
                            .then(() => {
                                refreshDocsTable($("#CaseId").val(), $("#Reference").val());
                            });
                    } else {
                        Swal.fire("Error!", "Something went wrong while deleting the file.", "error");
                    }
                })
                .catch(() => {
                    Swal.close();
                    Swal.fire("Error!", "A network error occurred while deleting the file.", "error");
                });
        }
    });
}