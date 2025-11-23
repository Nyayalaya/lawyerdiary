$(document).ready(function () {
    var table = $("#tblRegister").DataTable({
        "language": {
            "emptyTable": "Record is not there!"
        },
        columnDefs: [
            { orderable: false, targets: [0, 1, 2, 3, 4, 6, 8] }
            
        ]
    });

    var lengthDiv = $("#tblRegister_wrapper .dataTables_length");
    var filterDiv = $("#tblRegister_wrapper .dataTables_filter");

    // Wrap Pagination Dropdown
    lengthDiv.wrap('<div class="d-flex align-items-center"></div>');

    // Define filter dropdowns (id, label)
    var filters = [
        { id: "ddlReferral", label: "Referral" },
        { id: "ddlClient", label: "Client" }
    ];

    // Function to create dropdown dynamically with dynamic width
    function createDropdown(id, label) {
        return $(`
            <div class="col-sm-3 d-flex align-items-center mb-2">
                <label class="fw-bold me-2">${label}:</label>
                <select class="form-control customFilter" id="${id}">
                    <option value="">Select</option>
                </select>
            </div>
        `);
    }

    // Function to create Radio buttons
    function createRadioButtons() {
        return $(`
            <div class="col-sm-3 d-flex align-items-center mb-2">
                <label class="fw-bold me-2">Status:</label>
                <div class="form-check me-3">
                    <input class="form-check-input" type="radio" name="status" id="statusAll" value="" checked>
                    <label class="form-check-label" for="statusAll">All</label>
                </div>
                <div class="form-check me-3">
                    <input class="form-check-input" type="radio" name="status" id="statusPending" value="Pending">
                    <label class="form-check-label" for="statusPending">Pending</label>
                </div>
                <div class="form-check">
                    <input class="form-check-input" type="radio" name="status" id="statusDisposal" value="Disposal">
                    <label class="form-check-label" for="statusDisposal">Disposal</label>
                </div>
            </div>
        `);
    }

    // Create Row Container for Filters and Radio buttons
    var filterContainer = $('<div class="row w-100 mt-2 flex-wrap"></div>');

    // Append Pagination
    filterContainer.append(lengthDiv.parent().addClass("col-sm-2"));

    // Append Radio Buttons (Status)
    filterContainer.append(createRadioButtons());

    // Append Filters (Referral, Client)
    filters.forEach(filter => {
        filterContainer.append(createDropdown(filter.id, filter.label));
    });

    // Append Search Box
    filterContainer.append(filterDiv.parent().addClass("col-sm-2"));

    // Insert into DataTable wrapper
    $("#tblRegister_wrapper").prepend(filterContainer);

    // Fetch Dropdown Data from API
    var urls = [];
    urls.push({ "Key": "ReferBy", "Value": "/Client/ManageClient/DdlReferBy" }
        , { "Key": "Client", "Value": "/Client/ManageClient/DdlClients" }
    );
    urls.forEach(function (item) {
        $.ajax({
            url: item.Value,
            method: "GET",
            success: function (data) {
                var ddl = "";
                if (item.Key === "ReferBy") ddl = "#ddlReferral";
                if (item.Key === "Client") ddl = "#ddlClient";
                populateDropdown(ddl, data);
            },
            error: function (error) {
                console.log("Error loading filter data:", error);
            }
        });
    });

    // Function to populate dropdown
    function populateDropdown(selector, options) {
        options.forEach(function (item) {
            $(selector).append(new Option(item.Name.toUpperCase(), item.Id));
        });
    }

    // Apply Select2 for better UI
    $(".customFilter").select2({
        width: '100%',
        placeholder: "Select",
        allowClear: true
    });

    // Apply Filter on change (Client, Referral, Status)
    $(".customFilter, input[name='status']").on("change", function () {
        debugger;
        let client = $("#ddlClient").val();
        let referral = $("#ddlReferral").val();
        let status = $("input[name='status']:checked").val();

        $.ajax({
            url: "/Report/Register/Search",
            type: "GET",
            data: { ClientId: client, ReferalBy: referral, Status: status },
            success: function (response) {
                table.clear().draw(); // Clear existing data and re-draw without destroying the table
                $("#tblRegister tbody").html(response); // Update only tbody with new rows
                table.rows.add($("#tblRegister tbody tr")).draw();
            },
            error: function () {
                alert("Error fetching filtered data.");
            }
        });
    });
});
