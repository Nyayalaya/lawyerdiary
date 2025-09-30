var btn_delete = '<button type="button" onclick="removeKolom($(this))" class="btn btn-warning"><i class="fa fa-trash" aria-hidden="true"></i></button>';
var btn_add = '<button class="add-btn-repeat btn btn-success" onclick="addElement($(this))" type="button"><i class="fa fa-plus" aria-hidden="true"></i></button>';
function removeKolom(e) {
    e.parents('.kolom').remove();
}
function addElement1(e) {
    var trlength = $('#tblFormFields tbody tr').length;
    var clonedRow = $('#tblFormFields tbody tr:last').clone();
    clonedRow.find('input').each(function (i) {
        if (i === 0) {
            NameAtr = "Form.Fields[" + trlength + "].Key";
            IdAtr = "Form_Fields_" + trlength + "__Key";
        }
        if (i === 1) {
            NameAtr = "Form.Fields[" + trlength + "].Name";
            IdAtr = "Form_Fields_" + trlength + "__Name";
        }
        if (i === 2) {
            NameAtr = "Form.Fields[" + trlength + "].DefaultVal";
            IdAtr = "Form_Fields_" + trlength + "__DefaultVal";
        }

        if (i === 3) {
            NameAtr = "Form.Fields[" + trlength + "].IsRequire";
            IdAtr = "Form_Fields_" + trlength + "__IsRequire";
        }
        if (i === 4) {
            NameAtr = "Form.Fields[" + trlength + "].DispOrder";
            IdAtr = "Form_Fields_" + trlength + "__DispOrder";
        }
        if (i === 5) {
            NameAtr = "Form.Fields[" + trlength + "].Placeholder";
            IdAtr = "Form_Fields_" + trlength + "__Placeholder";
        }
        if (i === 6) {
            NameAtr = "Form.Fields[" + trlength + "].length.Min";
            IdAtr = "Form_Fields_" + trlength + "__length.Min";
        }
        if (i === 7) {
            NameAtr = "Form.Fields[" + trlength + "].length.Max";
            IdAtr = "Form_Fields_" + trlength + "__length.Max";
        }
        $(this).attr('name', NameAtr).attr('id', IdAtr);
        clonedRow.find("input").val("");
    });
    clonedRow.find('select').each(function (i) {
        NameAtr = "Form.Fields[" + trlength + "].Type";
        IdAtr = "Form_Fields_" + trlength + "__Type";
        $(this).attr('name', NameAtr).attr('id', IdAtr);
        clonedRow.find("select").val("");
    });

    clonedRow.find('button').find('button').replaceWith(btn_add);
    clonedRow.find('button.add-btn-repeat').replaceWith(btn_delete);
    $('#tblFormFields tbody').append(clonedRow);
}

function SelectBox() {
    clonedRow.find('select').each(function (i) {
        NameAtr = "Form.Fields[" + trlength + "].Type";
        IdAtr = "Form_Fields_" + trlength + "__Type";
        $(this).attr('name', NameAtr).attr('id', IdAtr);
        clonedRow.find("select").val("");
    });
}
var index = 0;
function addElement(e) {
    var $lastRow = $("#tblFormFields tbody tr:last");

    var lblName = $lastRow.find("input[id*='__Name']").val();

    // 1️⃣ Validate Type is selected
    var typeVal = $lastRow.find("select[id*='__Type']").val();
    if (!typeVal || typeVal === "--Select--") {
        Swal.fire("Please select the type!");
        return; // stop row creation
    }

    // 2️⃣ Validate Tag uniqueness
    var currentTag = $lastRow.find("input[id*='__Tag']").val();
    if (currentTag) {
        var duplicate = false;
        $("#tblFormFields tbody tr").not($lastRow).each(function () {
            var tagVal = $(this).find("input[id*='__Tag']").val();
            if (tagVal && tagVal === currentTag) {
                duplicate = true;
                return false; // break loop
            }
        });

        if (duplicate) {
            Swal.fire("Label/Tag value is duplicate, please enter another value in the label input!");
            return; // stop row creation
        }
    }

    var tblRows = $("#tblFormFields tbody tr").length;
    if (tblRows === 1)
        ++index;
    else
        index = $("#tblFormFields tbody tr").length;
    var key = generateGUID();
    var html = "";
    html += "<tr>";
    html += "<td><input type='hidden' name='Form.Fields[" + index + "].Key' value='" + key + "' />" +
        "<input type='text' name='Form.Fields[" + index + "].Name' id='Form_Fields_" + index + "__Name' width='300px' placeholder='Enter label name' class='form-control' title='Please enter field label name!' /></td>";
    html += "<td><select name='Form.Fields[" + index + "].Type' id='Form_Fields_" + index + "__Type'  class='form-control' title='Please enter type name!'><option>--Select--</option></select></td>";
    html += "<td><input type = 'text' name = 'Form.Fields[" + index + "].DefaultVal' id = 'Form_Fields_" + index + "__DefaultVal' width = '300px' placeholder = 'Enter Default value (In case of Dropdown provide comma seprated values)' class='form-control' title = 'Enter Default value (In case of Dropdown provide comma seprated values)' /></td >";

    // ✅ Tag column (auto generated)
    html += "<td><input type = 'text' name='Form.Fields[" + index + "].Tag' id='Form_Fields_" + index + "__Tag'  readonly/></td>";


    html += "<td><button type='button' class='btn btn-warning delete'><i class='fa fa-trash' aria-hidden='true'></i></button>";
   // html += "<button type='button' class='btn btn-success add'><i class='fa fa-plus'></i></button>";
    html += "</td>";
    html += "</tr>";
    $(".data-repeater").append(html);

    FieldType('Form_Fields_' + index + '__Type');
    $("#tblFormFields tbody tr").not(":last").find(".add").remove();
}

// Apply Title Case while typing
$(document).on("input", "input[id*='__Name']", function () {
    let cursorPos = this.selectionStart; // keep caret position
    let val = $(this).val();

    // Title Case each word
    val = val.replace(/\w\S*/g, function (txt) {
        return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
    });

    $(this).val(val);
    this.setSelectionRange(cursorPos, cursorPos); // restore caret
});

// Auto generate tag from label input
$(document).on("input", "input[id*='__Name']", function () {
    debugger;
    var id = $(this).attr("id");   // e.g. Form_Fields_0__Name
    var index = id.match(/Form_Fields_(\d+)__Name/)[1]; // extract 0,1,2...

    var labelText = $(this).val().trim().toLowerCase();

    if (labelText) {
        var cleaned = labelText.replace(/\s+/g, "");
        var tag = "#frm-" + cleaned + "#";

        // show in table
        $("#Form_Fields_" + index + "__Tag").val(tag);

        // also store in hidden input for form post
        $("#Form_Fields_" + index + "__Tag").val(tag);
    } else {
        $("#tag_" + index).text("");
        $("#Form_Fields_" + index + "__Tag").val("");
    }
});



$(document).on("click", '.delete', function () {
    debugger;
    --index;
    $(this).closest('tr').remove();
});

function generateGUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function FieldType(ddl) {
    $.getJSON("/admin/generateform/FieldType", function (data) {
        debugger;
        $("#" + ddl).empty();
        $("#" + ddl).append("<option>--Select--</option>");
        $.each(data, function (key, value) {
            $("#" + ddl).append('<option value=' + value.Id + '>' + value.Name + '</option>');
        });
    });
}

$('#tbody').on('click', '.remove', function () {
    $(this).parent('td.text-center').parent('tr.rowClass').remove();
});

