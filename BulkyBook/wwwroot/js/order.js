let dataTable;

$(document).ready(function() {
    const url = window.location.search;

    if (url.includes("pending")) {
        loadDataTable("GetOrders?status=pending");
    } else {
        if (url.includes("inProcess")) {
            loadDataTable("GetOrders?status=inProcess");
        } else {
            if (url.includes("completed")) {
                loadDataTable("GetOrders?status=completed");
            } else {
                if (url.includes("rejected")) {
                    loadDataTable("GetOrders?status=rejected");
                } else {
                    loadDataTable("GetOrders?status=all");
                }
            }
        }
    }
});

function loadDataTable(url) {
    dataTable = window.$("#tableData").DataTable({
        ajax: {
            url: "/Admin/Order/" + url
        },
        columns: [
            { data: "id", width: "5%" },
            { data: "name", width: "15%" },
            { data: "phoneNumber", width: "15%" },
            { data: "applicationUser.email", width: "15%" },
            { data: "orderStatus", width: "15%" },
            { data: "orderTotal", width: "15%" },
            {
                data: "id",
                render: function(data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Order/Details/${data
                        }" class="btn btn-success text-white" style="cursor: pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                        </div>
                    `;
                },
                width: "5%"
            }
        ]
    });
}