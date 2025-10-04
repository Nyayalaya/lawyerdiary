var btn_delete = '<button type="button" onclick="removeKolom($(this))" class="btn btn-warning"><i class="fa fa-trash" aria-hidden="true"></i></button>';
var btn_add = '<button class="add-btn-repeat btn btn-success" onclick="addElement($(this))" type="button"><i class="fa fa-plus" aria-hidden="true"></i></button>';

$("#HeadId").select2({
    placeholder: "Select a proceeding type",
    theme: "bootstrap4",
    escapeMarkup: function (m) {
        return m;
    }
});
$("#SubHeadId").select2({
    placeholder: "Select a proceeding",
    theme: "bootstrap4",
    escapeMarkup: function (m) {
        return m;
    }
});
$("#StageId").select2({
    placeholder: "Select a stage",
    theme: "bootstrap4",
    escapeMarkup: function (m) {
        return m;
    }
});
$("#HeadId").on("change", function () {
    $("#SubHeadId").empty();
    $.getJSON("/Litigation/CaseManage/DdlSubProcHeads?Id=" + $("#HeadId").val(), function (data) {
        $.each(data.Data, function (i, item) {
            $("#SubHeadId").append(`<option /><option value="${item.Id}">${item.Name_En}</option>`);
        });
    });
});


$("#ProcWork_Workdt_0__WorkTypeId").on("change", function () {
    $("#ProcWork_Workdt_0__WorkId").empty();
    $.getJSON("/Litigation/CaseWork/DdlSubWorks?WorkId=" + $("#ProcWork_Workdt_0__WorkTypeId").val(), function (data) {
        $.each(data.Data, function (i, item) {
            $("#ProcWork_Workdt_0__WorkId").append(`<option value="${item.Id}">${item.Name_En}</option>`);
        });
    });
});
function BindDynamicWork(WorkTypeddlID, WorkddlID) {    
    $("#" + WorkddlID).empty();
    $.getJSON("/Litigation/CaseWork/DdlSubWorks?WorkId=" + $("#" + WorkTypeddlID).val(), function (data) {
        $.each(data.Data, function (i, item) {
            $("#" + WorkddlID).append(`<option value="${item.Id}">${item.Name_En}</option>`);
        });
    });
    $("#" + WorkTypeddlID).select2({
        placeholder: "Select a work type",
        theme: "bootstrap4",
        escapeMarkup: function (m) {
            return m;
        }
    });
    $("#" + WorkddlID).select2({
        placeholder: "Select a work",
        theme: "bootstrap4",
        escapeMarkup: function (m) {
            return m;
        }
    });
}


function addElement(e) {
    var trlength = $('#tblWorks tbody tr').length;
    var clonedRow = $('#tblWorks tbody tr:last').clone();
    
    clonedRow.find('select').each(function (i) {        
        var NameAtr, IdAtr;
        //$(this).removeAttr('data-select2-id').select2();
        if (i === 0) {
            NameAtr = "ProcWork.Workdt[" + trlength + "].WorkTypeId";
            IdAtr = "ProcWork_Workdt_" + trlength + "__WorkTypeId";
        }
        if (i === 1) {
            NameAtr = "ProcWork.Workdt[" + trlength + "].WorkId";
            IdAtr = "ProcWork_Workdt_" + trlength + "__WorkId";
        }        
        $(this).attr('name', NameAtr).attr('id', IdAtr);
        //$(this).removeAttr('data-select2-id').select2();
        clonedRow.find('button.add-btn-repeat').replaceWith(btn_delete); // Assuming btn_delete is defined
        clonedRow.find('button').find('button').replaceWith(btn_add); // Assuming btn_add is defined
        $('#tblWorks tbody').append(clonedRow);
        //$(this).attr('data-select2-id').select2();
        
        if (i === 0) {
            $("#ProcWork_Workdt_" + trlength + "__WorkTypeId").on('change', function () {
                BindDynamicWork("ProcWork_Workdt_" + trlength + "__WorkTypeId", "ProcWork_Workdt_" + trlength + "__WorkId"); // Call your function here
            });
        }
    });
}
function removeKolom(e) {   
    e.closest('tr').remove();
}


var rowCount = 1;
$("#btnAddMore").on("click", function () {
    alert("Hello");
    var newRow = $('#workRow_0').clone();
    newRow.attr('id', 'workRow_' + rowCount);

    newRow.find('select#WorkTypeId_0')
        .attr('id', 'WorkTypeId_' + rowCount)
        .attr('name', "ProcWork.Workdt[" + rowCount + "].WorkTypeId")
        .val('')
        .removeClass('select2-hidden-accessible');

    newRow.find('select#WorkId_0')
        .attr('id', 'WorkId_' + rowCount)
        .attr('name', "ProcWork.Workdt[" + rowCount + "].WorkId")
        .val('')
        .removeClass('select2-hidden-accessible');

    newRow.find('.select2bs4').select2({
        theme: 'bootstrap4'
    });

    $('#workRows').append(newRow);
    rowCount++;
});

$(document).on('change', '[id^=WorkTypeId_]', function () {
    var selectedWorkTypeId = $(this).val(); // Get selected WorkTypeId
    var workIdDropdown = $(this).closest('.workRow').find('[id^=WorkId_]'); // Find associated WorkId dropdown

    if (selectedWorkTypeId) {
        // Example: Fetch data for WorkId dropdown (replace with actual AJAX call)
        $.ajax({
            url: '/Litigation/CaseWork/DdlSubWorks', // Replace with your API endpoint
            method: 'GET',
            data: { WorkId: selectedWorkTypeId },
            success: function (data) {
                // Populate the WorkId dropdown
                workIdDropdown.empty(); // Clear existing options
                workIdDropdown.append('<option value="">Select Work</option>'); // Add placeholder option
                $.each(data.Data, function (index, work) {
                    debugger;
                    workIdDropdown.append('<option value="' + work.Id + '">' + work.Name_En + '</option>');
                });

                // Reinitialize select2 for the updated dropdown
                workIdDropdown.select2({
                    theme: 'bootstrap4'
                });
            },
            error: function () {
                alert('Failed to load works. Please try again.');
            }
        });
    } else {
        // Clear WorkId dropdown if no WorkTypeId selected
        workIdDropdown.empty().append('<option value="">Select Work</option>').select2({
            theme: 'bootstrap4'
        });
    }
});

// $("#btnSubmit").on("click", function () {
//     Upseart();
// });
// Upseart = function () {
//     debugger;
//     var SelectedCaseId = [];
//     $('#tblUserCase tbody tr').each(function () {
//         if ($(this).find("input[type='checkbox'").is(":checked")) {
//             SelectedCaseId.push($(this).find('input[type="hidden"]').val());
//         }
//     });
//     var url = $("#create-form").attr("action");
//     var formData = new FormData();
//     formData.append("HeadId", $("#HeadId").val());
//     formData.append("SelectedSubHeads", $("#SubHeadId").val());
//     formData.append("StageId", $("#StageId").val());
//     formData.append("NextDate", $("#NextDate").val());
//     formData.append("SelectedCases", SelectedCaseId);
//     $.ajax({
//         type: 'POST',
//         url: url,
//         data: formData,
//         processData: false,
//         contentType: false
//     }).done(function (response) {

//     });
// }
