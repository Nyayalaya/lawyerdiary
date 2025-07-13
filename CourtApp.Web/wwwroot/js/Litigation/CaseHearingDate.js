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
        var caseId = $(this).find('input[type="hidden"]').val();
        var nextDate = $(this).find('input[type="date"]').val();
        var procDate = $(this).find('input[name="bt.ProceedingDate"]').val();
        if (nextDate !== "")
            caseDataList.push({
                CaseId: caseId,
                HearingDt: nextDate,
                ProcDt: procDate
            });
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