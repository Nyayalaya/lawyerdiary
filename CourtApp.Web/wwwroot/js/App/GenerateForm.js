// ------------------------------
// REMOVE all OLD DELETE handlers
// ------------------------------
/*
var btn_delete = '<button type="button" onclick="removeKolom($(this))" ...';
function removeKolom(e) { ... }
$('#tbody').on("click", ".remove", function(){ ... });
*/

// ------------------------------
// NEW SAFE DELETE BUTTON
// ------------------------------
var btn_delete = '<button type="button" class="btn btn-warning delete"><i class="fa fa-trash"></i></button>';
var btn_add = '<button type="button" class="btn btn-success add"><i class="fa fa-plus"></i></button>';

// ------------------------------
// DELETE Row (SAFE & CLEAN)
// ------------------------------
$(document).on("click", ".delete", function () {
    $(this).closest("tr").remove();
    reorderRows();
});


// ------------------------------
// ADD NEW ROW
// ------------------------------
var index = 0;

function addElement(e) {

    var $lastRow = $("#tblFormFields tbody tr:last");

    // Validation
    var typeVal = $lastRow.find("select[id*='__Type']").val();
    if (!typeVal || typeVal === "--Select--") {
        Swal.fire("Please select the type!");
        return;
    }

    // Duplicate Tag Check
    var currentTag = $lastRow.find("input[id*='__Tag']").val();
    var duplicate = false;

    $("#tblFormFields tbody tr").not($lastRow).each(function () {
        if ($(this).find("input[id*='__Tag']").val() === currentTag)
            duplicate = true;
    });

    if (duplicate) {
        Swal.fire("Tag is duplicate! Enter another label.");
        return;
    }

    index = $("#tblFormFields tbody tr").length;
    var key = generateGUID();

    var html = `
<tr>
    <td>
        <input type="hidden" name="Form.Fields[${index}].Key" value="${key}" />
        <input type="text" name="Form.Fields[${index}].Name" id="Form_Fields_${index}__Name"
            class="form-control" placeholder="Enter label name" />
    </td>

    <td>
        <select name="Form.Fields[${index}].Type" id="Form_Fields_${index}__Type" class="form-control">
            <option>--Select--</option>
        </select>
    </td>

    <td>
        <input type="text" name="Form.Fields[${index}].DefaultVal"
            id="Form_Fields_${index}__DefaultVal" class="form-control"
            placeholder="Enter default value" />
    </td>

    <td>
        <input type="text" name="Form.Fields[${index}].Tag"
            id="Form_Fields_${index}__Tag" class="form-control" readonly />
    </td>

    <td>
        ${btn_delete}
    </td>
</tr>`;

    $("#tblFormFields tbody").append(html);

    FieldType("Form_Fields_" + index + "__Type");
}



// ------------------------------
// REORDER ROWS AFTER DELETE
// ------------------------------
function reorderRows() {
    $("#tblFormFields tbody tr").each(function (i) {

        $(this).find("input, select").each(function () {

            let oldName = $(this).attr("name");
            if (oldName)
                $(this).attr("name", oldName.replace(/Form\.Fields\[\d+\]/, "Form.Fields[" + i + "]"));

            let oldId = $(this).attr("id");
            if (oldId)
                $(this).attr("id", oldId.replace(/Form_Fields_\d+__/, "Form_Fields_" + i + "__"));
        });
    });

    index = $("#tblFormFields tbody tr").length;
}



// ------------------------------
// TITLE CASE LABEL
// ------------------------------
$(document).on("input", "input[id*='__Name']", function () {

    let cursorPos = this.selectionStart;
    let val = $(this).val();

    val = val.replace(/\w\S*/g, txt =>
        txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase()
    );

    $(this).val(val);
    this.setSelectionRange(cursorPos, cursorPos);
});


// ------------------------------
// AUTO TAG GENERATOR
// ------------------------------
$(document).on("input", "input[id*='__Name']", function () {

    var id = $(this).attr("id");
    var i = id.match(/Form_Fields_(\d+)__Name/)[1];

    var txt = $(this).val().trim().toLowerCase();
    txt = txt.replace(/\s+/g, "");

    $("#Form_Fields_" + i + "__Tag").val(txt ? "#frm-" + txt + "#" : "");
});


// ------------------------------
// FIELD TYPE DROPDOWN LOADER
// ------------------------------
function FieldType(ddl) {
    $.getJSON("/admin/generateform/FieldType", function (data) {

        $("#" + ddl).empty().append("<option>--Select--</option>");

        data.forEach(x => {
            $("#" + ddl).append(`<option value="${x.Id}">${x.Name}</option>`);
        });

    });
}


// ------------------------------
// GUID GENERATOR
// ------------------------------
function generateGUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
