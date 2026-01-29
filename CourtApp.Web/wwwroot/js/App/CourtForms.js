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
        height: 600,
        menubar: 'file edit view insert format tools table help',

        plugins: [
            'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview', 'anchor',
            'searchreplace', 'visualblocks', 'visualchars', 'code', 'fullscreen',
            'insertdatetime', 'media', 'table', 'help', 'wordcount',
            'pagebreak', 'nonbreaking', 'autosave', 'print'
        ],

        // Fixed toolbar with all items visible upfront
        toolbar: [
            'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | forecolor backcolor',
            'alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | table hr pagebreak',
            'pagesetup | link image media | removeformat code preview print | help'
        ],

        // Content styles for different page sizes
        content_style: `
            body {
                font-family: Arial, Helvetica, sans-serif;
                font-size: 14px;
                background: #f5f5f5;
                padding: 20px;
                margin: 0;
            }
            .align-left {
    text-align: left !important;
}

.align-center {
    text-align: center !important;
}

.align-right {
    text-align: right !important;
}

.align-justify {
    text-align: justify !important;
}

            .page {
                background: white;
                margin: 20px auto;
                padding: 20mm;
                box-shadow: 0 2px 8px rgba(0,0,0,0.15);
                position: relative;
            }

            /* Standard Paper Sizes */
            .page-a4 { 
                width: 210mm; 
                min-height: 297mm; 
            }
            
            .page-a3 { 
                width: 297mm; 
                min-height: 420mm; 
            }
            
            .page-letter { 
                width: 8.5in; 
                min-height: 11in; 
            }
            
            .page-legal { 
                width: 8.5in; 
                min-height: 14in; 
            }
            
            .page-executive { 
                width: 7.25in; 
                min-height: 10.5in; 
            }
            
            .page-tabloid { 
                width: 11in; 
                min-height: 17in; 
            }

            /* Envelope Sizes */
            .env-10 { 
                width: 9.5in; 
                min-height: 4.125in; 
                
            }
            
            .env-monarch { 
                width: 7.5in; 
                min-height: 3.875in; 
            }
            
            .env-dl { 
                width: 220mm; 
                min-height: 110mm; 
               
            }
            
            .env-c5 { 
                width: 229mm; 
                min-height: 162mm; 
                
            }
            
            .env-c6 {
                width: 162mm;
                min-height: 114mm;
                
            }

            /* Page indicator */
            .page::before {
                content: attr(data-page-type);
                position: absolute;
                top: 5px;
                right: 10px;
                font-size: 10px;
                color: #999;
                text-transform: uppercase;
            }

            /* Print styles */
            @media print {
                body { 
                    background: white; 
                    padding: 0;
                }
                
                .page {
                    margin: 0;
                    padding: 0;
                    box-shadow: none;
                    page-break-after: always;
                }
                
                .page::before {
                    display: none;
                }
            }
        `,

        setup: function (editor) {

            // Register Page Setup dropdown button (visible in toolbar)
            editor.ui.registry.addSplitButton('pagesetup', {
                text: 'Page Setup',
                icon: 'page-break',
                tooltip: 'Select page size',

                onAction: function () {
                    // Default action - set to A4
                    setPage(editor, 'page-a4', 'A4 (210mm × 297mm)');
                },

                onItemAction: function (api, value) {
                    setPage(editor, value.pageClass, value.pageName);
                },

                fetch: function (callback) {
                    var items = [
                        // Standard Paper Sizes
                        {
                            type: 'choiceitem',
                            text: '📄 A4 (210mm × 297mm)',
                            value: { pageClass: 'page-a4', pageName: 'A4' }
                        },
                        {
                            type: 'choiceitem',
                            text: '📄 A3 (297mm × 420mm)',
                            value: { pageClass: 'page-a3', pageName: 'A3' }
                        },
                        {
                            type: 'choiceitem',
                            text: '📄 Letter (8.5" × 11")',
                            value: { pageClass: 'page-letter', pageName: 'Letter' }
                        },
                        {
                            type: 'choiceitem',
                            text: '📄 Legal (8.5" × 14")',
                            value: { pageClass: 'page-legal', pageName: 'Legal' }
                        },
                        {
                            type: 'choiceitem',
                            text: '📄 Executive (7.25" × 10.5")',
                            value: { pageClass: 'page-executive', pageName: 'Executive' }
                        },
                        {
                            type: 'choiceitem',
                            text: '📄 Tabloid (11" × 17")',
                            value: { pageClass: 'page-tabloid', pageName: 'Tabloid' }
                        },

                        // Separator
                        {
                            type: 'separator'
                        },

                        // Envelopes
                        {
                            type: 'choiceitem',
                            text: '✉️ Envelope #10 (9.5" × 4.125")',
                            value: { pageClass: 'env-10', pageName: 'Envelope #10' }
                        },
                        {
                            type: 'choiceitem',
                            text: '✉️ Envelope Monarch (7.5" × 3.875")',
                            value: { pageClass: 'env-monarch', pageName: 'Envelope Monarch' }
                        },
                        {
                            type: 'choiceitem',
                            text: '✉️ Envelope DL (220mm × 110mm)',
                            value: { pageClass: 'env-dl', pageName: 'Envelope DL' }
                        },
                        {
                            type: 'choiceitem',
                            text: '✉️ Envelope C5 (229mm × 162mm)',
                            value: { pageClass: 'env-c5', pageName: 'Envelope C5' }
                        },
                        {
                            type: 'choiceitem',
                            text: '✉️ Envelope C6 (162mm × 114mm)',
                            value: { pageClass: 'env-c6', pageName: 'Envelope C6' }
                        }
                    ];

                    callback(items);
                }
            });

            // Initialize editor content
            editor.on('init', function () {
                const existingContent = editor.getContent({ format: 'html' }).trim();

                if (existingContent.length > 0) {
                    // Edit mode: wrap only if not already wrapped
                    if (!existingContent.includes('class="page"')) {
                        editor.setContent(
                            existingContent 
                        );
                    }
                } else {
                    // Create mode: new document
                    editor.setContent(
                        '<div class="page page-a4" data-page-type="A4"><p><br></p></div>'
                    );
                }

                console.log('TinyMCE initialized with page setup');
            });

            // Tab key handling
            editor.on('keydown', function (e) {
                if (e.key === 'Tab') {
                    e.preventDefault();
                    editor.execCommand(e.shiftKey ? 'Outdent' : 'Indent');
                }
            });

            // Add custom print command
            editor.ui.registry.addButton('print', {
                text: 'Print',
                icon: 'print',
                tooltip: 'Print document',
                onAction: function () {
                    printEditor(editor);
                }
            });
        },

        // Additional configuration
        toolbar_mode: 'sliding',  // Makes toolbar items slide out if space is limited
        toolbar_sticky: true,     // Keeps toolbar visible when scrolling
        branding: false,
        promotion: false,

        // Better default settings
        default_link_target: '_blank',
        link_assume_external_targets: true,

        // Image upload settings (optional)
        automatic_uploads: true,
        images_upload_url: '/upload-image',  // Update with your upload endpoint

        // File picker settings
        file_picker_types: 'image',

        // Valid elements - allow all
        valid_elements: '*[*]',
        extended_valid_elements: 'div[*]',

        // Format options
        formats: {
            alignleft: { selector: 'p,h1,h2,h3,h4,h5,h6,td,th,div,ul,ol,li,table,img', classes: 'align-left' },
            aligncenter: { selector: 'p,h1,h2,h3,h4,h5,h6,td,th,div,ul,ol,li,table,img', classes: 'align-center' },
            alignright: { selector: 'p,h1,h2,h3,h4,h5,h6,td,th,div,ul,ol,li,table,img', classes: 'align-right' },
            alignjustify: { selector: 'p,h1,h2,h3,h4,h5,h6,td,th,div,ul,ol,li,table,img', classes: 'align-justify' }
        }
    });
});

// Function to change page size
function setPage(editor, pageClass, pageName) {
    const body = editor.getBody();
    const page = body.querySelector('.page');

    if (page) {
        // Remove all existing page classes
        page.className = page.className.replace(/page-[\w-]+|env-[\w-]+/g, '');


        // Add new page class
        page.className = 'page ' + pageClass;

        // Update page type indicator
        page.setAttribute('data-page-type', pageName || pageClass);

        // Notify user
        editor.notificationManager.open({
            text: 'Page size changed to: ' + (pageName || pageClass),
            type: 'success',
            timeout: 2000
        });

        console.log('Page size changed to:', pageClass);
    } else {
        console.error('Page div not found');
    }
}

// Function to print editor content
function printEditor(editor) {
    const content = editor.getContent();
    const printWindow = window.open('', '_blank');

    printWindow.document.write(`
        <!DOCTYPE html>
        <html>
        <head>
            <title>Print Document</title>
            <style>
                body {
                    font-family: Arial, Helvetica, sans-serif;
                    font-size: 14px;
                    margin: 0;
                    padding: 0;
                }
                
                .page {
                    background: white;
                    padding: 20mm;
                    page-break-after: always;
                }
                
                .page:last-child {
                    page-break-after: auto;
                }
                
                /* Paper sizes for print */
                @page {
                    margin: 0;
                }
                
                .page-a4 { width: 210mm; }
                .page-letter { width: 8.5in; }
                .page-legal { width: 8.5in; }
                .env-10 { width: 9.5in; }
                
                @media print {
                    body { margin: 0; }
                    .page { margin: 0; box-shadow: none; }
                }
            </style>
        </head>
        <body>
            ${content}
        </body>
        </html>
    `);

    printWindow.document.close();

    // Wait for content to load then print
    setTimeout(function () {
        printWindow.focus();
        printWindow.print();
        // Don't close automatically - let user close after printing
        // printWindow.close();
    }, 500);
}



//$(document).ready(function () {
//    tinymce.init({
//        selector: '#FormTemplate',
//        height: 500,
//        menubar: true,

//        plugins: [
//            'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview', 'anchor',
//            'searchreplace', 'visualblocks', 'visualchars', 'code', 'fullscreen',
//            'insertdatetime', 'media', 'table', 'help', 'wordcount',
//            'pagebreak', 'nonbreaking', 'autosave'
//            // ❌ REMOVED hr, print
//        ],

//        toolbar:
//            'undo redo | blocks fontfamily fontsize | ' +
//            'bold italic underline strikethrough forecolor backcolor | ' +
//            'alignleft aligncenter alignright alignjustify | ' +
//            'bullist numlist outdent indent | table | hr pagebreak | ' +
//            'pagesetup | ' +
//            'link image media | removeformat code preview print | help',

//        content_style: `
//                body {
//                    font-family: Helvetica, Arial, sans-serif;
//                    font-size: 14px;
//                    background: #f2f2f2;
//                    padding: 20px;
//                }

//                .page {
//                    background: #fff;
//                    margin: 20px auto;
//                    padding: 20mm;
//                    box-shadow: 0 0 6px rgba(0,0,0,0.2);
//                }

//                /* Standard pages */
//                .page-a4 { width: 210mm; min-height: 297mm; }
//                .page-a3 { width: 297mm; min-height: 420mm; }
//                .page-letter { width: 216mm; min-height: 279mm; }
//                .page-legal { width: 216mm; min-height: 356mm; }
//                .page-executive { width: 184mm; min-height: 267mm; }
//                .page-tabloid { width: 279mm; min-height: 432mm; }

//                /* Envelopes */
//                .env-10 { width: 241mm; min-height: 105mm; }
//                .env-monarch { width: 191mm; min-height: 98mm; }
//                .env-dl { width: 220mm; min-height: 110mm; }
//                .env-c5 { width: 229mm; min-height: 162mm; }

//                @media print {
//                    body { background: none; }
//                    .page {
//                        margin: 0;
//                        box-shadow: none;
//                        page-break-after: always;
//                    }
//                }
//        `,

//        setup: function (editor) {

//            editor.ui.registry.addMenuButton('pagesetup', {
//                text: 'Page Setup',
//                fetch: function (callback) {
//                    callback([
//                        {
//                            type: 'menuitem',
//                            text: 'A4',
//                            onAction: function () {
//                                setPage(editor, 'page-a4');
//                            }
//                        },
//                        {
//                            type: 'menuitem',
//                            text: 'Letter',
//                            onAction: function () {
//                                setPage(editor, 'page-letter');
//                            }
//                        },
//                        {
//                            type: 'menuitem',
//                            text: 'Legal',
//                            onAction: function () {
//                                setPage(editor, 'page-legal');
//                            }
//                        },
//                        {
//                            type: 'menuitem',
//                            text: 'Executive',
//                            onAction: function () {
//                                setPage(editor, 'page-executive');
//                            }
//                        },
//                        {
//                            type: 'menuitem',
//                            text: 'Tabloid',
//                            onAction: function () {
//                                setPage(editor, 'page-tabloid');
//                            }
//                        },
//                        {
//                            type: 'menuitem',
//                            text: 'A3',
//                            onAction: function () {
//                                setPage(editor, 'page-a3');
//                            }
//                        },
                        
//                        {
//                            type: 'menuitem',
//                            text: 'Envelope #10',
//                            onAction: function () {
//                                setPage(editor, 'env-10');
//                            }
//                        },
//                        {
//                            type: 'menuitem',
//                            text: 'Envelope Monarch',
//                            onAction: function () {
//                                setPage(editor, 'env-monarch');
//                            }
//                        },
//                        {
//                            type: 'menuitem',
//                            text: 'Envelope DL',
//                            onAction: function () {
//                                setPage(editor, 'env-dl');
//                            }
//                        }
//                    ]);
//                }
//            });

//            editor.on('init', function () {

//                const existingContent = editor.getContent({ format: 'html' }).trim();

//                // Edit mode: content already exists
//                if (existingContent.length > 0) {

//                    // Wrap existing content only if not already wrapped
//                    if (!existingContent.includes('class="page"')) {
//                        editor.setContent(
//                            '<div class="page page-a4">' + existingContent + '</div>'
//                        );
//                    }

//                } else {
//                    // Create mode: empty editor
//                    editor.setContent(
//                        '<div class="page page-a4"><p></p></div>'
//                    );
//                }
//            });

//            editor.on('keydown', function (e) {
//                if (e.key === 'Tab') {
//                    e.preventDefault();
//                    editor.execCommand(e.shiftKey ? 'Outdent' : 'Indent');
//                }
//            });
//        },

//        branding: false
//    });
//});

//function setPage(editor, pageClass) {
//    const page = editor.getBody().querySelector('.page');
//    if (page) {
//        page.className = 'page ' + pageClass;
//    }
//}




//$(document).ready(function () {
//    tinymce.init({
//        selector: '#FormTemplate',
//        height: 500,
//        menubar: true,
//        plugins: [
//            'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview', 'anchor',
//            'searchreplace', 'visualblocks', 'visualchars', 'code', 'fullscreen',
//            'insertdatetime', 'media', 'table', 'help', 'wordcount',
//            'print', 'hr', 'pagebreak', 'nonbreaking', 'autosave'
//        ],
//        toolbar: 'undo redo | formatselect | fontselect fontsizeselect | ' +
//            'bold italic underline strikethrough forecolor backcolor | ' +
//            'alignleft aligncenter alignright alignjustify | ' +
//            'bullist numlist outdent indent | table | hr pagebreak | ' +
//            'link image media | removeformat code preview print | help',
//        content_style: `
//            body { font-family:Helvetica,Arial,sans-serif; font-size:14px; padding:20px; }
//            hr.tiny-ruler { border: none; border-top: 1px dashed #888; margin: 20px 0; }
//        `,
//        setup: function (editor) {
//            editor.on('keydown', function (e) {
//                if (e.key === 'Tab') {
//                    e.preventDefault();
//                    if (e.shiftKey) {
//                        editor.execCommand('Outdent');
//                    } else {
//                        editor.execCommand('Indent');
//                    }
//                }
//            });
//        },
//        table_toolbar: 'tableprops tabledelete | tableinsertrowbefore tableinsertrowafter tabledeleterow | ' +
//            'tableinsertcolbefore tableinsertcolafter tabledeletecol',
//        branding: false
//    });
//});



