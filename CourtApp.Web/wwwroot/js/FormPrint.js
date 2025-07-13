$(document).ready(function () {
    $('#CaseIds').multiselect({
        includeSelectAllOption: true,
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        filterPlaceholder: 'Search Cases',
        buttonWidth: '100%',
        dropUp: false, // force dropdown to open downward
        maxHeight: 300
    });

    $("#TitleIds").multiselect({
        includeSelectAllOption: true,
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        filterPlaceholder: 'Search Titles',
        buttonWidth: '100%',
        dropUp: false, // force dropdown to open downward
        maxHeight: 300
    });


    $("#FormTypeId").select2({
        placeholder: "Select form type",
        theme: "bootstrap4",
        allowClear: true,
        escapeMarkup: function (m) {
            return m;
        }
    });

    $("#reload").click(function () {
        $('#CaseIds').multiselect('deselectAll', false); 
        $('#CaseIds').multiselect('updateButtonText');   

        $('#TitleIds').multiselect('deselectAll', false); 
        $('#TitleIds').multiselect('updateButtonText');

        $('#FormTypeId').val(null).trigger('change');

        $('#viewAll').html("");
        $("#lblHeader").text("");
    });

    $("#FormTypeId").on("change", function () {
        $("#lblHeader").text($("#FormTypeId :selected").text())
    });

    $("#CaseIds").on("change", function () {
        const selectedIds = $("#CaseIds").val();

        $("#TitleIds").empty(); // Clear previous options

        $.getJSON("/Litigation/CaseInfoPrinting/GetCompTitlesByCases?caseIds=" + selectedIds, function (data) {
            $.each(data, function (i, item) {
                $("#TitleIds").append(`<option value="${item.Id}">${item.Name}</option>`);
            });

            // Refresh multiselect after updating options
            $("#TitleIds").multiselect('rebuild');
        });
    });

    $("#btnPrint").click(function () {
        var frmType = $("#FormTypeId :selected").text();
        if (frmType === "Envalop")
            printEnvalop();
        printData();
    });
    function printEnvalop() {
        var divToPrint = document.getElementById("printableArea");
        var newWin = window.open("", "_blank");

        newWin.document.write('<html><head><title>Print Envelope</title>');

        newWin.document.write(`
    <style>
    @media print {
        @page {
            size: 110mm 220mm; /* DL envelope in landscape */
            margin: 0;
        }

        body {
            margin: 0;
            padding: 0;
            font-family: 'Times New Roman', serif;
            font-size: 14pt;
        }

        .envelope-size {
            width: 220mm;
            height: 110mm;
            position: relative;
            box-sizing: border-box;
        }

        .address-box {
            width: 90mm;
            position: absolute;
            top: 40mm;
            left: 60mm;
            text-align: center;
            line-height: 1.5;
        }
    }

    /* Optional screen preview styles */
    body {
        margin: 0;
        font-family: 'Times New Roman', serif;
    }

    .envelope-size {
        width: 220mm;
        height: 110mm;
        border: 1px dashed #ccc;
        position: relative;
    }

    .address-box {
        width: 90mm;
        position: absolute;
        top: 40mm;
        left: 60mm;
        text-align: center;
        font-size: 14pt;
        line-height: 1.5;
    }
</style>

`);

        newWin.document.write('</head><body>');
        newWin.document.write('<div class="envelope-size">');
        newWin.document.write(divToPrint.innerHTML);
        newWin.document.write('</div>');
        newWin.document.write('</body></html>');

        newWin.document.close();
        newWin.focus();

        newWin.onload = function () {
            newWin.print();
            newWin.close();
        };

    }

    function printData1() {
        var divToPrint = document.getElementById("printableArea");
        newWin = window.open("");
        newWin.document.write(divToPrint.outerHTML);
        newWin.print();
        newWin.close();
    }

    function printData_() {
        var divToPrint = document.getElementById("printableArea");
        var newWin = window.open("", "_blank");

        newWin.document.write('<html><head><title>Print Form</title>');

        newWin.document.write(`
    <style>
        @media print {
            @page {
                size: A4 portrait;
                margin: 20mm 25mm 20mm 20mm;
            }

            body {
                font-size: 13px;
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
            }

            .a4-page {
                width: 100%;
                max-width: 170mm;
                margin: 0 auto;
                /*padding: 10mm;  */              
                page-break-after: always;
                /*background: white;*/               
            }

            table {
                width: 100%;
                table-layout: fixed;
                border-collapse: collapse;
            }

            td {
                word-wrap: break-word;
                vertical-align: top;
                padding: 4px;
            }

            * {
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
        }

        .a4-page {
            width: 100%;
            max-width: 170mm;
            margin: 0 auto;
            
           
            background: white;
        }

        table {
            width: 100%;
            table-layout: fixed;
        }

        td {
            word-wrap: break-word;
            vertical-align: top;
            padding: 4px;
        }
    </style>
    `);

        newWin.document.write('</head><body>');
        newWin.document.write(divToPrint.outerHTML);
        newWin.document.write('</body></html>');

        newWin.document.close();
        newWin.focus();
        newWin.print();
        newWin.close();
    }


    function printData() {
        var divToPrint = document.getElementById("printableArea");
        var newWin = window.open("", "_blank");

        newWin.document.write('<html><head><title>Print Form</title>');

        newWin.document.write(`
        <style>
            @media print {
                @page {
                    size: A4 portrait;
                    margin: 20mm 0mm 20mm 0mm;
                }

                body {
                    font-size: 13px;
                    font-family: Arial, sans-serif;
                    line-height: 1.5;
                    margin: 0;
                    padding: 0;
                    text-align: justify;
                }

                .a4-size {
                    width: 100%;
                    max-width: 170mm;
                    margin: 0 auto;
                    padding-left: 10mm;
                    box-sizing: border-box;
                    page-break-inside: avoid;
                }

                .row-flex {
                    display: flex;
                    justify-content: space-between;
                    margin: 0 0 10px 0;
                    font-size: 14pt;
                }

                .right-align {
                    text-align: right;
                }

                .center-text {
                    text-align: center;
                }

                hr {
                    border: none;
                    border-top: 1px solid #000;
                    margin: 20px 0;
                }

                * {
                    -webkit-print-color-adjust: exact;
                    print-color-adjust: exact;
                }
            }

            /* Screen styles (optional) */
            body {
                font-size: 12px;
                font-family: Arial, sans-serif;
                line-height: 1.4;
                text-align: justify;
            }

            .a4-size {
                width: 100%;
                max-width: 170mm;
                margin: 0 auto;
                padding-left: 10mm;
                box-sizing: border-box;
            }

            .row-flex {
                display: flex;
                justify-content: space-between;
                margin: 0 0 10px 0;
                font-size: 12pt;
            }

            .right-align {
                text-align: right;
            }

            .center-text {
                text-align: center;
            }

            table {
                width: 100%;
                table-layout: fixed;
                border-collapse: collapse;
                table-layout: fixed;           
                font-size: 12pt;
            }

            td {
                word-wrap: break-word;
                vertical-align: top;
                padding: 8px 20px;
                border: 1px solid black;
            }

            hr {
                border: none;
                border-top: 1px solid #000;
                margin: 20px 0;
                width:100%
            }
        </style>
    `);

        newWin.document.write('</head><body>');
        newWin.document.write('<div class="a4-size">');
        newWin.document.write(divToPrint.innerHTML);
        newWin.document.write('</div>');
        newWin.document.write('</body></html>');

        newWin.document.close();
        newWin.focus();

        // Wait for content to load before printing
        newWin.onload = function () {
            newWin.print();
            newWin.close();
        };
    }


    function printDatatoday() {
        var divToPrint = document.getElementById("printableArea");
        var newWin = window.open("", "_blank");

        newWin.document.write('<html><head><title>Print Form</title>');

        newWin.document.write(`
        <style>
            @media print {
                @page {
                    size: A4 portrait;
                    margin: 20mm 15mm 20mm 15mm;
                }

                body {
                    font-size: 14px;
                    font-family: Arial, sans-serif;
                    text-align: justify;
                    margin: 0;
                    padding: 0;
                }

                .a4-size {
                    width: 100%;
                    max-width: 180mm; /* Prevent content cutoff on right */
                    margin: 0 auto;
                    padding: 10mm;
                    box-sizing: border-box;
                    page-break-after: always;
                }

                table {
                    width: 100%;
                    table-layout: fixed;
                    border-collapse: collapse;
                }

                td {
                    word-wrap: break-word;
                    vertical-align: top;
                    padding: 4px;
                }

                * {
                    -webkit-print-color-adjust: exact;
                    print-color-adjust: exact;
                }
            }

            .a4-size {
                width: 100%;
                max-width: 180mm;
                margin: 0 auto;
                padding: 10mm;
                box-sizing: border-box;
            }

            table {
                width: 100%;
                table-layout: fixed;
            }

            td {
                word-wrap: break-word;
                vertical-align: top;
                padding: 4px;
            }
        </style>
    `);

        newWin.document.write('</head><body>');
        newWin.document.write(divToPrint.outerHTML);
        newWin.document.write('</body></html>');

        newWin.document.close();
        newWin.focus();
        newWin.print();
        newWin.close();
    }

    function printDataold() {
        var divToPrint = document.getElementById("printableArea");
        var newWin = window.open("", "_blank");

        newWin.document.write('<html><head><title>Print Form</title>');

        // Embedded CSS inside print function
        newWin.document.write(`
            <style>
                @media print {
                    @page {
                        size: A4 portrait; /* Ensures A4 print size */
                        margin: 20mm 10mm 20mm 10mm; /* Top, Right, Bottom, Left margins */
                    }

                    body {
                        font-size: 14px;
                        font-family: Arial, sans-serif;
                        text-align: justify; /* Ensures justified alignment */
                        margin: 0;
                        padding: 0;
                    }

                    /* Ensure the printable area is centered and does not cut content */
                    #printableArea {
                        width: 100%;
                        max-width: 210mm;
                        padding-left: 200px; /* Extra left margin as requested */
                        padding-right: 10px;
                        word-wrap: break-word;
                    }

                    /* Ensure no extra margin/padding for proper alignment */
                    .card-body {
                        margin: 0;
                        padding: 0;
                    }                    
                }
            </style>
        `);

        newWin.document.write('</head><body>');
        newWin.document.write(divToPrint.outerHTML);
        newWin.document.write('</body></html>');

        newWin.document.close();
        newWin.focus();
        newWin.print();
        newWin.close();
    }
    function printData2() {
        var divToPrint = document.getElementById("printableArea");
        var newWin = window.open("", "_blank");

        newWin.document.write(
            '<!DOCTYPE html>' +
            '<html lang="en">' +
            '<head>' +
            '<meta charset="UTF-8">' +
            '<meta name="viewport" content="width=device-width, initial-scale=1.0">' +
            '<title>Print</title>' +
            '<style>' +
            '@page { margin: 0; }' + // Removes browser header & footer
            'body { margin: 20px; padding: 0; font-family: Arial, sans-serif; }' +
            '.print-container { width: 750px; margin-left: 200px;margin-right: 50px; padding: 20px; text-align: justify; border: 1px solid #ddd; box-shadow: 2px 2px 10px rgba(0,0,0,0.1); page-break-after: always; }' +
            '.content { width: 100%; line-height: 1.6; font-size: 16px; }' +
            '.applicant-number { float: right; border: 1px solid black; height: 30px; width: 30px; text-align: center; font-weight: bold; }' +
            '.signature { text-align: right; font-weight: bold; margin-top: 40px; }' +
            '</style>' +
            '</head>' +
            '<body>' +
            divToPrint.outerHTML +
            '<script>' +
            'window.onload = function() {' +
            '  window.print();' +
            '  window.onafterprint = function() { window.close(); };' +
            '};' +
            '<\/script>' +
            '</body>' +
            '</html>'
        );

        newWin.document.close();
    }



    $("#btnSearch").on("click", function () {
        var t = $("#FormTypeId").val();
        var v = $("#CaseIds").val();
        var title = $("#TitleIds").val();
        if (t && v) {
            loadData(t, v, title);
        }
        else {
            Swal.fire({
                title: "Please select the form type or case for generating the report!",
                text: "error",
                icon: "error"
            });
        }
    });
});
function loadData(t, v, title) {
    $.ajax({
        url: '/Litigation/CaseInfoPrinting/LoadFormPrinting',
        traditional: true, // 👈 this fixes the array serialization!
        data: {
            type: t,
            Cases: v,
            AppNo: title
        },
        success: function (data) {
            $('#viewAll').html(data);
        }
    });
    //$('#viewAll').load('/Litigation/CaseInfoPrinting/LoadFormPrinting?type=' + t + "&Cases=" + v + "&AppNo=" + title);
}