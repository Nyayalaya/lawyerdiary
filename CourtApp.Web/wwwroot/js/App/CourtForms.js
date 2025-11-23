$(document).ready(function () {
    
    $("#StateId").select2({
        placeholder: "Select a state",
        theme: "bootstrap4",
        allowClear: true,
        escapeMarkup: function (m) {
            return m;
        }
    });
    $("#CaseTypeId").select2({
        placeholder: "Select a Case Type",
        theme: "bootstrap4",
        allowClear: true,
        escapeMarkup: function (m) {
            return m;
        }
    });
    $("#CourtTypeId").select2({
        placeholder: "Select a court type",
        theme: "bootstrap4",
        allowClear: true,
        escapeMarkup: function (m) {
            return m;
        }
    });

    $("#LanguageCode").select2({
        placeholder: "Select a court language",
        theme: "bootstrap4",
        allowClear: true,
        escapeMarkup: function (m) {
            return m;
        }
    });

    $("#CaseCategoryId").select2({
        placeholder: "Select case category",
        theme: "bootstrap4",
        allowClear: true,
        escapeMarkup: function (m) {
            return m;
        }
    });

    $("#StateId").on("change", function () {
        BindCaseCategory($("#CourtTypeId").val());
        BindLanguages($("#StateId").val());
    });

    $("#CaseCategoryId").on("change", function () {        
        BindCaseType($("#CaseCategoryId").val());
    });
});
BindCaseType = function (caseCategoryId) {
    $("#CaseTypeId").empty()
    $("#CaseTypeId").append(`<option /><option value="00000000-0000-0000-0000-000000000000">----All----</option>`);
    $.getJSON("/Litigation/CaseManage/LoadTypeOfCase?natureId=" + caseCategoryId, function (data) {        
        $.each(data.Data, function (i, item) {
            $("#CaseTypeId").append(`<option /><option value="${item.Id}">${item.Name_En}</option>`);
        });
    });
}


BindCaseCategory = function (CategoryId) {
    $("#CaseCategoryId").empty()
    $("#CaseCategoryId").append(`<option /><option value="00000000-0000-0000-0000-000000000000">----All----</option>`);
    $.getJSON("/Litigation/CaseManage/LoadCaseCategory?CourtTypeId=" + CategoryId, function (data) {
        $.each(data, function (i, item) {
            $("#CaseCategoryId").append(`<option /><option value="${item.Id}">${item.Name_En}</option>`);
        });
    });
}

BindLanguages = function (StateId) {
    $("#LanguageCode").empty();
    $.getJSON("/LawyerDiary/CourtForm/LoadLanguages?StateId=" + StateId, function (data) {
        debugger;
        $.each(data, function (i, item) {
            $("#LanguageCode").append(`<option /><option value="${item.Code}">${item.Name}</option>`);
        });
    });
}

$(document).ready(function () {
    tinymce.init({
        selector: '#FormTemplate',
        height: 500,
        menubar: true,
        plugins: [
            'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview', 'anchor',
            'searchreplace', 'visualblocks', 'visualchars', 'code', 'fullscreen',
            'insertdatetime', 'media', 'table', 'help', 'wordcount',
            'print', 'hr', 'pagebreak', 'nonbreaking', 'autosave'
        ],
        toolbar: 'undo redo | formatselect | fontselect fontsizeselect | ' +
            'bold italic underline strikethrough forecolor backcolor | ' +
            'alignleft aligncenter alignright alignjustify | ' +
            'bullist numlist outdent indent | table | hr pagebreak | ' +
            'link image media | removeformat code preview print | help',
        content_style: `
            body { font-family:Helvetica,Arial,sans-serif; font-size:14px; padding:20px; }
            hr.tiny-ruler { border: none; border-top: 1px dashed #888; margin: 20px 0; }
        `,
        setup: function (editor) {
            editor.on('keydown', function (e) {
                if (e.key === 'Tab') {
                    e.preventDefault();
                    if (e.shiftKey) {
                        editor.execCommand('Outdent');
                    } else {
                        editor.execCommand('Indent');
                    }
                }
            });
        },
        table_toolbar: 'tableprops tabledelete | tableinsertrowbefore tableinsertrowafter tabledeleterow | ' +
            'tableinsertcolbefore tableinsertcolafter tabledeletecol',
        branding: false
    });
});



