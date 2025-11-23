$(document).ready(function () {
    $('.form-image').click(function () { $('#customFile').trigger('click'); });
    $(function () {
        $('.selectpicker').selectpicker();
    });
    setTimeout(function () {
        $('body').addClass('loaded');
    }, 200);

    jQueryModalGet = (url, title) => {
        try {
            $.ajax({
                type: 'GET',
                url: url,
                contentType: false,
                processData: false,
                success: function (res) {
                    $('#form-modal .modal-body').html(res.html);
                    $('#form-modal .modal-title').html(title);
                    $('#form-modal').modal('show');
                    console.log(res);
                },
                error: function (err) {
                    console.log(err)
                }
            })
            //to prevent default form submit event
            return false;
        } catch (ex) {
            console.log(ex)
        }
    }

    jQueryModalPost = form => {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {                  
                    if (res.isValid) {
                        $('#viewAll').html(res.html)
                        $('#form-modal').modal('hide');
                    }                   
                },
                error: function (err) {
                    console.log(err)
                }
            })
            return false;
        } catch (ex) {
            console.log(ex)
        }
    }
    jQueryModalDelete = form => {
        if (confirm('Are you sure to delete this record ?')) {
            try {
                $.ajax({
                    type: 'POST',
                    url: form.action,
                    data: new FormData(form),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.isValid) {
                            $('#viewAll').html(res.html)
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                })
            } catch (ex) {
                console.log(ex)
            }
        }

        //prevent default form submit event
        return false;
    }

    //JqueryDataTable = function (tableId, ptitle, ecolumns) {
    //    $('#' + tableId).DataTable({
    //        dom:
    //            "<'row align-items-center custom-header-row'<'col-md-2'l><'col-md-8 text-center'f><'col-md-2 text-end dt-export-buttons'B>>" +
    //            "<'row'<'col-12'tr>>" +
    //            "<'row'<'col-md-5'i><'col-md-7'p>>",

    //        buttons: [
    //            {
    //                extend: 'excelHtml5',
    //                text: '<i class="fas fa-file-excel"></i> Export',
    //                className: 'btn btn-outline-success me-2',
    //                title: ptitle,
    //                filename: function () {
    //                    var date = new Date();
    //                    return ptitle + "_" + date.toISOString().split('T')[0]; // e.g. CaseReport_2025-06-21
    //                },
    //                exportOptions: {
    //                    columns: ecolumns
    //                }
    //            }
    //            // Add more buttons dynamically if needed
    //        ],

    //        paging: true,
    //        lengthChange: true,
    //        pageLength: 10,
    //        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
    //        processing: false,
    //        info: true,
    //        searching: true
    //    });
    //}

    JqueryDataTable = function (tableId, config) {
        let defaultConfig = {
            tableId: tableId,
            ajaxUrl: null,
            addUrl: null,
            columns: [],
            exportColumns: ':not(:last-child)',
            serverSide: false,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
            buttons: null, // ← Allow user to override this
            dom:
                "<'row align-items-center custom-header-row'<'col-md-2'l><'col-md-2 text-center'f><'col-md-8 text-end dt-export-buttons'B>>" +
                "<'row'<'col-12'tr>>" +
                "<'row'<'col-md-5'i><'col-md-7'p>>"
        };

        const dt = Object.assign({}, defaultConfig, config);

        // Fallback to default button set only if user did not provide custom buttons
        const buttons = dt.buttons || [
            {
                extend: 'excelHtml5',
                text: '<i class="fas fa-file-excel"></i> Export',
                className: 'btn btn-outline-warning me-2',
                exportOptions: {
                    modifier: { page: 'all' },
                    columns: dt.exportColumns
                }
            }
        ];

        const options = {
            dom: dt.dom,
            paging: true,
            pageLength: dt.pageLength,
            lengthMenu: dt.lengthMenu,
            processing: true,
            serverSide: dt.serverSide,
            columns: dt.columns,
            searching: true,
            info: true,
            lengthChange: true,
            buttons: buttons
        };

        if (dt.ajaxUrl) {
            options.ajax = {
                url: dt.ajaxUrl,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                dataSrc: function (json) {
                    console.log(json);
                    return json.data;
                }
            };
        }

        const table = $('#' + dt.tableId).DataTable(options);

        // External reload support
        $('#reload').on('click', function () {
            table.ajax.reload(null, false);
        });

        return table;
    };




});


// ---Debounced Session Expiry Check on Interaction ---

function checkSession() {
    fetch('/Litigation/CaseManage/IsSessionActive', { cache: 'no-store' })
        .then(response => response.json())
        .then(data => {
            if (!data.isActive) {
                alert("Session expired. Redirecting to login.");
                window.location.href = "/Identity/Account/Login";
            }
        })
        .catch(error => {
            console.error("Session check failed:", error);
        });
}

// Debounce function to prevent too many requests
function debounce(func, delay) {
    let timeout;
    return function () {
        clearTimeout(timeout);
        timeout = setTimeout(func, delay);
    };
}

// Use debounced version
const checkSessionDebounced = debounce(checkSession, 2000);

// Attach to user interaction events
document.addEventListener("keydown", checkSessionDebounced);
document.addEventListener("mousedown", checkSessionDebounced);



$(document).ready(function () {
    $(document).on('submit', 'form', function (e) {
   
       // Prevent double submit
        if ($(this).data("submitted") === true) {
            e.preventDefault();
            return false;
        }

        // Mark as submitted
        $(this).data("submitted", true);

        // Disable all submit buttons inside this form
        $(this).find('button[type="submit"]').prop('disabled', true);

        // Show loader
        $('#loader-wrapper').show();
    });
});



