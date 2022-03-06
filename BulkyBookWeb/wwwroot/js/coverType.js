let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tableData").DataTable({
        ajax: {
            url: "/Admin/CoverType/GetAll"
        },
        columns: [
            { data: "name", width: "60%" },
            {
                data: "id",
                render: function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/CoverType/Upsert/${data
                        }" class="btn btn-success text-white" style="cursor: pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a onclick=deleteCoverType("/Admin/CoverType/Delete/${data
                        }") class="btn btn-danger text-white" style="cursor: pointer">
                                <i class="fas fa-trash-alt"></i>
                            </a>
                        </div>
                    `;
                },
                width: "40%"
            }
        ]
    });
}

function deleteCoverType(url) {
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
                success: function (data) {
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