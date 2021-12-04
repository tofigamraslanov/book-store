let dataTable;

$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tableData").DataTable({
        ajax: {
            url: "/Admin/User/GetAll"
        },
        columns: [
            { data: "name", width: "15%" },
            { data: "email", width: "15%" },
            { data: "phoneNumber", width: "15%" },
            { data: "company.name", width: "15%" },
            { data: "role", width: "15%" },
            {
                data: {
                    id: "id",
                    lockoutEnd: "lockoutEnd"
                },
                render: function(data) {
                    const today = new Date().getTime();
                    const lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        // user is currently locked
                        return `
                            <div class="text-center">
                                <a onclick=lockUnlock('${data.id
                            }') class="btn btn-danger text-white" style="cursor: pointer;width: 150px">
                                    <i class="fas fa-lock-open"></i> Unlock User
                                </a>
                            </div>
                        `;
                    } else {
                        return `
                            <div class="text-center">
                                <a onclick=lockUnlock('${data.id
                            }') class="btn btn-success text-white" style="cursor: pointer;width: 150px">
                                    <i class="fas fa-lock"></i> Lock User
                                </a>
                            </div>
                        `;
                    }
                },
                width: "25%"
            }
        ]
    });
}

function lockUnlock(id) {
    $.ajax({
        type: "POST",
        url: "/Admin/User/LockUnlock",
        data: JSON.stringify(id),
        contentType: "application/json",
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