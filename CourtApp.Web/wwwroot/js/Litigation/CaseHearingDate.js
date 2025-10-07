$(document).ready(function () {
    //JqueryDataTable('tblCaseWohd', 'User Data List', '0,1,2,3,4,5,6,7');
    JqueryDataTable('tblCaseWohd', {
        serverSide: false, // Client-side processing since data is rendered already
        searching: true,
        processing: false,
        orderable: false,
        columns: [
            { orderable: false }, // Sno
            null, // Court
            null, // Type
            null, // No
            null, // Year
            null, // Title
            null, // Stage
            null, // Proceeding Date
            { orderable: false } // Next Date (with input box)
        ]
    });

});


$("#btnUpdate").on("click", function (e) {
    debugger;
    e.preventDefault(); 
    var caseDataList = [];

    $('#tblCaseWohd tbody tr').each(function () {
       
        var $row = $(this);

        var caseId = $row.find('input[name="CaseId"]').val();

        var caseNo = $row.find('input[name="CaseNo"]').val();
        var caseYear = $row.find('input[name="CaseYear"]').val();

        var nextDateInput = $row.find('input[type="date"]');
        var currentNextDate = nextDateInput.val();
        var originalNextDate = nextDateInput.data('original-next');

        // ProceedingDate stays same, just retrieve it
        var procDate = $row.find('input[name="ProceedingDate"]').val();
        var isPrnt = $row.find('input[name="IsParent"]').val();

        var caseNoYear = caseNo + "/" + caseYear;

        // Only add to list if NextHearingDate is changed
        if (currentNextDate !== originalNextDate) {
            caseDataList.push({
                CaseId: caseId,
                HearingDt: currentNextDate,
                ProcDt: procDate,
                IsParent: isPrnt,
                CaseNoYear: caseNoYear
            });
        }
    });

    if (caseDataList.length > 0) {
        $.ajax({
            url: "/Litigation/casemanage/UpdateHearingDate",
            type: "POST",
            data: { casedts: caseDataList },
            success: function (response) {
                if (response.Success) {
                    Swal.fire({
                        title: "Hearing date updated for the selected cases!",
                        icon: "success"
                    });
                } else if (response.InvalidCaseNos) {
                    // Build invalid case message
                    let invalidCases = response.InvalidCaseNos.join(", ");
                    Swal.fire({
                        title: "Validation Error!",
                        text: `These cases have invalid hearing dates: ${invalidCases}. Proceed with valid cases?`,
                        icon: "warning",
                        showCancelButton: true,
                        confirmButtonText: "Yes, update valid cases",
                        cancelButtonText: "No, cancel"
                    }).then((result) => {
                        if (result.isConfirmed) {
                            // Send valid cases only
                            $.ajax({
                                url: "/Litigation/casemanage/UpdateHearingDate",
                                type: "POST",
                                data: { casedts: response.ValidCases },
                                success: function (res) {
                                    if (res.Success) {
                                        Swal.fire({
                                            title: "Valid cases updated successfully!",
                                            icon: "success"
                                        });
                                    }
                                },
                                error: function (xhr) {
                                    console.error('An error occurred:', xhr.responseText);
                                }
                            });
                        }
                    });
                }
            },
            error: function (xhr) {
                console.error('An error occurred:', xhr.responseText);
            }
        });
    } else {
        Swal.fire({
            title: "Kindly fill the Hearing date before submit!",
            icon: "error"
        });
    }
});