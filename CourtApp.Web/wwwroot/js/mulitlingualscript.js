var table;
var isHindiMode = true;
var krutiAlert;

$(document).ready(function () {

    // ================================
    // FUNCTION TO SHOW/HIDE KRUTI ALERT
    // ================================
    function showKrutiAlert() {
        if (!krutiAlert) {
            krutiAlert = Swal.fire({
                title: 'Important!',
                text: 'Kruti Dev 010 font must be installed on this machine for proper typing.',
                icon: 'info',
                toast: true,
                position: 'top-end',
                showConfirmButton: true,
                timer: 0, // persistent
                timerProgressBar: false,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            });
        }
    }

    function hideKrutiAlert() {
        if (krutiAlert) {
            Swal.close();
            krutiAlert = null;
        }
    }

    // ================================
    // DataTable init
    // ================================
    if (!$.fn.DataTable.isDataTable('#tblConversionData')) {
        table = $('#tblConversionData').DataTable({
            pageLength: 10,
            stateSave: true,
            ordering: false,
            dom:
                "<'row mb-2'<'col-md-6 d-flex align-items-center'l>" +
                "<'col-md-6 d-flex justify-content-end'fB>>" +
                "rt" +
                "<'row mt-2'<'col-md-6'i><'col-md-6'p>>",

            buttons: [
                {
                    extend: 'excelHtml5',
                    text: '<i class="fas fa-file-export me-1"></i> Export',
                    className: 'btn btn-sm btn-success', // green small button
                    title: null,
                    exportOptions: {
                        columns: [0, 1],
                        modifier: {
                            page: 'all'
                        },
                        

                        // ✅ MUST be here
                        format: {
                            body: function (data, row, column, node) {
                                var input = $('input', node);
                                return input.length ? input.val() : data;
                            }
                        }
                    }
                },
                {
                    text: '<i class="fas fa-file-upload me-1"></i> Import',
                    className: 'btn btn-primary', // blue small button
                    action: function () {
                        openImportPopup();
                    }
                }
            ],

            language: {
                search: "Search:",
                lengthMenu: "Show _MENU_ entries",
                info: "Showing _START_ to _END_ of _TOTAL_ entries"
            }
        });

    } else {
        table = $('#tblConversionData').DataTable();
    }

    // Show alert on page load (if Hindi mode)
    if (isHindiMode) showKrutiAlert();

    // ================================
    // LANGUAGE TOGGLE
    // ================================
    $('#langToggle').on('change', function () {
        isHindiMode = this.checked;
        var label = $('label[for="langToggle"]');

        if (isHindiMode) {
            label.text('Hindi');
            $('.lang-input[data-lang="hi"]').removeClass('lang-english');
            showKrutiAlert();  // Show alert when switched to Hindi
        } else {
            label.text('English');
            $('.lang-input[data-lang="hi"]').addClass('lang-english');
            hideKrutiAlert(); // Hide alert when switched to English
        }
    });

    // ================================
    // SAVE BUTTON
    // ================================
    $('#saveBtn').on('click', function () {
        var data = [];
        table.rows().every(function () {
            var row = $(this.node());
            var id = row.data('id');

            // Keyword
            var keyInput = row.find('.keyword-input');
            var keyOriginal = keyInput.data('original').trim();
            var keyCurrent = keyInput.val().trim();

            // Hindi
            var hiInput = row.find('.lang-input[data-lang="hi"]');
            var hiOriginal = hiInput.data('original').trim();
            var hiCurrent = hiInput.val().trim();

            // 🚀 If either keyword OR hindi changed
            if (keyOriginal !== keyCurrent || hiOriginal !== hiCurrent) {
                data.push({
                    id: id,
                    KeyWord: keyCurrent,
                    translations: {
                        hi: hiCurrent
                    }
                });
            }
        });

        if (!data.length) {
            Swal.fire("No Changes", "There are no changes to save.", "info");
            return;
        }

        Swal.fire({
            title: "Saving...",
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });

        $.ajax({
            url: '/Admin/MultiLangWord/Save',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (res) {
                Swal.fire("Success", res.message || "Saved successfully", "success");

                table.rows().every(function () {
                    $(this.node()).find('.lang-input[data-lang="hi"]').data('original', function () {
                        return $(this).val();
                    });
                });
            },
            error: function () {
                Swal.fire("Error", "Save failed", "error");
            }
        });
    });


    function openImportPopup() {
        Swal.fire({
            title: 'Import Excel',
            html: `
                    <input type="file"
                           id="excelFile"
                           accept=".xlsx,.xls"
                           class="swal2-file"
                           style="width:100%" />
                `,
            showCancelButton: true,
            confirmButtonText: 'Upload',
            cancelButtonText: 'Cancel',
            allowOutsideClick: false,
            allowEscapeKey: false,
            preConfirm: () => {
                const fileInput = document.getElementById('excelFile');
                if (!fileInput.files.length) {
                    Swal.showValidationMessage('Please select an Excel file');
                    return false;
                }
                return fileInput.files[0];
            }
        }).then(result => {
            if (result.isConfirmed) {
                importExcel(result.value);
            }
        });
    }
    // ✅ SAFE normalize function (Excel + Unicode proof)
    function normalize(str) {
        return str
            ?.toString()
            .trim()
            .replace(/\u00A0/g, ' ')                // Excel NBSP
            .replace(/[（）]/g, m => m === '（' ? '(' : ')') // full-width parentheses
            .toLowerCase();
    }

    function importExcel(file) {

        // 🔒 Lock popup + show uploading animation
        Swal.fire({
            title: 'Uploading...',
            html: 'Please wait while the file is being processed',
            allowOutsideClick: false,
            allowEscapeKey: false,
            showConfirmButton: false,   // 🔴 IMPORTANT: no close button
            didOpen: () => {
                Swal.showLoading();
            }
        });



        var reader = new FileReader();

        reader.onload = function (e) {
            console.log('📂 Excel file loaded');

            var data = new Uint8Array(e.target.result);
            var workbook = XLSX.read(data, { type: 'array' });

            var sheet = workbook.Sheets[workbook.SheetNames[0]];
            var rows = XLSX.utils.sheet_to_json(sheet, { defval: '' });

           
            var updatedCount = 0;

            table.rows().every(function (rowIdx) {

                var row = $(this.node());

                var keyInput = row.find('.keyword-input');
                var hiInput = row.find('.lang-input[data-lang="hi"]');

                var tableKeyRaw = keyInput.val();
                var tableKey = normalize(tableKeyRaw);

                // 🔑 Find matching Excel row
                var excelRow = rows.find(r => {
                    var excelKeyRaw = r['Keyword'];
                    var excelKey = normalize(excelKeyRaw);
                    return excelKey === tableKey;
                });

                if (excelRow) {
                    var excelValueRaw = excelRow['Value'];
                    var excelValueNorm = normalize(excelValueRaw);

                    if (excelValueNorm !== '') {
                        hiInput.val(excelValueRaw);
                        updatedCount++;
                       
                    } 
                } 
            });
            console.log(`🎯 TOTAL ROWS UPDATED: ${updatedCount}`);

            // ✅ Close loader and show result ONLY after processing
            Swal.fire({
                icon: 'success',
                title: 'Import Completed',
                text: updatedCount + ' rows updated'
            });

            // ❗ Optional: handle file read error
            reader.onerror = function () {
                Swal.fire('Error', 'Failed to read file', 'error');
            };

        };

        reader.readAsArrayBuffer(file);
    }

    
});