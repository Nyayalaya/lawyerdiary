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
    tinymce.baseURL = "/lib/tinymce";
    tinymce.init({
        selector: '#TemplateBody',
        height: 500,
        menubar: true,
        plugins: [
            'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview', 'anchor',
            'searchreplace', 'visualblocks', 'visualchars', 'code', 'fullscreen',
            'insertdatetime', 'media', 'table', 'help', 'wordcount',
            'print', 'hr', 'pagebreak', 'nonbreaking', 'autosave', 'paste'
        ],
        paste_as_text: false,
        paste_enable_default_filters: false,
        paste_retain_style_properties: 'all',
        valid_elements: '*[*]',
        fontsize_formats: '5pt 6pt 7pt 8pt 9pt 10pt 11pt 12pt 13pt 14pt 16pt 18pt 20pt 22pt 24pt 26pt 28pt 36pt 48pt',
        valid_styles: {
            '*': 'color,font-size,font-family,background,font-weight,font-style,text-decoration,text-align,line-height'
        },
        paste_data_images: true,
        toolbar:
            'undo redo | formatselect | fontselect fontsizeselect | ' +
            'bold italic underline strikethrough forecolor backcolor | ' +
            'alignleft aligncenter alignright alignjustify | ' +
            'lineheight | ' +
            'bullist numlist outdent indent | ' +
            'table | hr pagebreak | ' +
            'link image media | removeformat code preview print | help',
        content_style: `
        body { font-family:Helvetica,Arial,sans-serif; font-size:14px; padding:20px; }
        hr.tiny-ruler { border: none; border-top: 1px dashed #888; margin: 20px 0; }
    `,
        setup: function (editor) {
            //-------------------------------------------------------
            // 3️⃣ Tab = Indent / Shift+Tab = Outdent
            //-------------------------------------------------------
            editor.on('keydown', function (e) {
                if (e.key === 'Tab') {
                    e.preventDefault();
                    editor.execCommand(e.shiftKey ? 'Outdent' : 'Indent');
                }
            });
        },

        table_toolbar:
            'tableprops tabledelete | tableinsertrowbefore tableinsertrowafter tabledeleterow | ' +
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