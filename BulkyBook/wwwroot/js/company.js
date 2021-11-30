let dataTable;

$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tableData").dataTable({
        ajax: {
            url: "/Admin/Company/GetAll"
        },
        columns: [
            { data: "name", width: "15%" },
            { data: "streetAddress", width: "15%" },
            { data: "city", width: "10%" },
            { data: "state", width: "10%" },
            { data: "phoneNumber", width: "15%" },
            {
                data: "IsAuthorizedCompany",
                render: function(data) {
                    if (data) {
                        return `<input type="checkbox" disabled checked />`;
                    } else {
                        return `<input type="checkbox" disabled />`;
                    }
                },
                width: "10%"
            },
            {
                data: "id",
                render: function(data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Company/Upsert/${data
                        }" class="btn btn-success text-white" style="cursor: pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a onclick=deleteCompany("/Admin/Company/Delete/${data
                        }") class="btn btn-danger text-white" style="cursor: pointer">
                                <i class="fas fa-trash-alt"></i>
                            </a>
                        </div>
                    `;
                },
                width: "25%"
            }
        ]
    });
}

function deleteCompany(url) {
    window.swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function(data) {
                    if (data.success) {
                        window.toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        window.toastr.error(data.message);
                    }
                }
            });
        }
    });
}