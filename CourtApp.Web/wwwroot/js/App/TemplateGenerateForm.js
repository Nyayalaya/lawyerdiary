$(document).ready(function () {
    $("#FormId").select2({
        placeholder: "Select a form name",
        theme: "bootstrap4",
        allowClear: true,
        escapeMarkup: function (m) {
            return m;
        }
    });

    $("#FormId").on("change", function () {
        var selectedText = $("#FormId option:selected").text();
        if ($("#TemplateName").val() == "")
            $("#TemplateName").val(selectedText + "-");
    });
});

$(document).ready(function () {
    tinymce.init({
        selector: '#TemplateBody',
        height: 500,
        menubar: true,
        plugins: [
            'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview', 'anchor',
            'searchreplace', 'visualblocks', 'visualchars', 'code', 'fullscreen',
            'insertdatetime', 'media', 'table', 'help', 'wordcount',
            'print', 'hr', 'pagebreak', 'nonbreaking', 'autosave','paste'
        ],
        paste_as_text: false,
        paste_enable_default_filters: false,
        paste_retain_style_properties: 'all', // preserve inline formatting
        valid_elements: '*[*]',               // allow all tags and attributes
        valid_styles: { '*': 'color,font-size,font-family,background,font-weight,font-style,text-decoration' },
        paste_data_images: true, 

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

document.getElementById('viewTagsBtn').addEventListener('click', function (e) {
    e.preventDefault(); // prevent default anchor behavior

    const selectedFormId = document.getElementById('FormId').value;

    if (!selectedFormId) {
        alert("Please select a form first.");
        return;
    }

    const url = `/admin/templatebuilder/ViewTag?formId=${encodeURIComponent(selectedFormId)}`;

    // Open the link in a new tab
    window.open(url, '_blank');
});