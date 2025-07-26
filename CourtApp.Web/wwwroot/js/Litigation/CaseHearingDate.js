$(document).ready(function () {
    //JqueryDataTable('tblCaseWohd', 'User Data List', '0,1,2,3,4,5,6,7');
    JqueryDataTable('tblCaseWohd', {
        serverSide: false, // Client-side processing since data is rendered already
        searching: true,
        processing: false,
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
    e.preventDefault(); 
    var caseDataList = [];

    $('#tblCaseWohd tbody tr').each(function () {
        var $row = $(this);

        var caseId = $row.find('input[name="CaseId"]').val();

        var nextDateInput = $row.find('input[type="date"]');
        var currentNextDate = nextDateInput.val();
        var originalNextDate = nextDateInput.data('original-next');

        // ProceedingDate stays same, just retrieve it
        var procDate = $row.find('input[name="ProceedingDate"]').val();
        var isPrnt = $row.find('input[name="IsParent"]').val();

        // Only add to list if NextHearingDate is changed
        if (currentNextDate !== originalNextDate) {
            caseDataList.push({
                CaseId: caseId,
                HearingDt: currentNextDate,
                ProcDt: procDate,
                IsParent: isPrnt
            });
        }
    });
    
    if (caseDataList.length > 0) {
        $.ajax({
            url: "/Litigation/casemanage/UpdateHearingDate",
            type: "POST",
            data: { casedts: caseDataList },
            success: function (response) {
                if (response.Succeeded) {
                    Swal.fire({
                        title: "Hearing date updated for the selected cases!",
                        text: "Ok!",
                        icon: "success"
                    });
                }
                else {
                    Swal.fire({
                        title: "Good job!",
                        text: "You clicked the button!",
                        icon: "error"
                    });
                }
            },
            error: function (xhr) {
                console.error('An error occurred:', xhr.responseText);
            }
        });
    }
    else {
        Swal.fire({
            title: "Kindly fill the Hearing date before submit!",
            text: "You clicked the button!",
            icon: "error"
        });
    }
});