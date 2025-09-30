$(document).ready(function () {
    tinymce.init({
        selector: '#TemplateBody',
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