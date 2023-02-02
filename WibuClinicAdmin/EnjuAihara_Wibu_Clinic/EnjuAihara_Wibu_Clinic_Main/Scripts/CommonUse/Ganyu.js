function Select2Init(Id) {
	$(Id).select2();
};
function SearchInitialWithClick(controller) {
    $("#btn-search").click(function () {
        SearchInit(controller);
    });
    $("#btn-search").trigger("click");
}


function SearchInit(controller) {
    var $btn = $("#btn-search");
    $btn.button('loading');

    $.ajax({
        type: "POST",
        url: "/" + controller + "/_Search",
        data: $("#frmSearch").serializeArray(),
        success: function (data) {
            $("#divSearchResult").html("");
            $("#divSearchResult").html(data);
            Pagging();
        },
        error: function (error) {
            $btn.button('reset');
            AlertPopup(2, "Đã có lỗi xảy ra", error);
        }
    });
}

function Pagging() {
    $('#tableRes').dataTable({
        pageLength: 10,
        paging: true,
        autoWidth: true,
        scrollX: true,
        initComplete: function (settings) {
            $(window).resize();

        },
        drawCallback: function (settings) {
            $(window).trigger('resize');
        },
        destroy: true,
        language: {
            sProcessing: "Đang xử lý...",
            sLengthMenu: "Xem _MENU_ mục",
            sZeroRecords: "Không tìm thấy dòng nào phù hợp",
            sInfo: "Đang xem _START_ đến _END_ trong tổng số _TOTAL_ mục",
            sInfoEmpty: "Đang xem 0 đến 0 trong tổng số 0 mục",
            sInfoFiltered: "(được lọc từ _MAX_ mục)",
            sInfoPostFix: "",
            sSearch: "Tìm nội dung:",
            sUrl: "",
            columns: "adjust",
            oPaginate: {
                sFirst: "Đầu",
                sPrevious: "&laquo;",
                sNext: "&raquo;",
                sLast: "Cuối"
            },
            columnDefs: [
                { targets: [0, 1], visible: true },
                { targets: 'no-sort', visible: false },
            ]
        },
        searching: false,
    });
}

//id = 1 Là thành công
//id = 2 Là cảnh báo
//id = 3 Là thất bại
function AlertPopup(id, title, message) {
    var alertId = "";
    switch (id) {
        case 1:
            alertId = "#SucessToast";
            break;
        case 2:
            alertId = "#WarningToast";
            break;
        case 3:
            alertId = "#FailToast";
            break;
        default:
            break
    }
    $(alertId + " p:first-child").text(message);
    $(alertId + " p:nth-child(2)").text(title);
    $(alertId).toast({
        animation: true
    });
    $(alertId).removeClass("hide");
    $(alertId).addClass("show");
    setTimeout(function(){
        $(alertId).removeClass("show");
        $(alertId).addClass("hide");
    }, 5000);
}


function SaveData(controller, frmCreate) {
 
    var frm = $(frmCreate),
        formData = new FormData(),
        formParams = frm.serializeArray();
        $.each(frm.find('input[type="file"]'), function (i, tag) {
            isHasFile = true;
            $.each($(tag)[0].files, function (i, file) {
                formData.append(tag.name, file);
            });
        });

        $.each(formParams, function (i, val) {
            formData.append(val.name, val.value);
        });

        $.ajax({
            type: "POST",
            url: "/" + controller + "/Create",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.isSucess) {
                    if (data.title && data.message)
                        AlertPopup(1, data.title, data.message);
                    if (data.redirect) {
                        setTimeout(function () {
                            window.location.href = data.redirect;
                        }, 3000);
                    }
                }
                else {
                    AlertPopup(3, data.title, data.message);
                }
            },
            error: function (data) {
                AlertPopup(2, "Lỗi", data.message);
            }
        });
}

function Edit(controller, frmEdit) {

    var frm = $(frmEdit),
        formData = new FormData(),
        formParams = frm.serializeArray();
    $.each(frm.find('input[type="file"]'), function (i, tag) {
        isHasFile = true;
        $.each($(tag)[0].files, function (i, file) {
            formData.append(tag.name, file);
        });
    });

    $.each(formParams, function (i, val) {
        formData.append(val.name, val.value);
    });

    $.ajax({
        type: "POST",
        url: "/" + controller + "/Edit",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.isSucess) {
                if (data.title && data.message)
                    AlertPopup(1, data.title, data.message);
                if (data.redirect) {
                    setTimeout(function () {
                        window.location.href = data.redirect;
                    }, 3000);
                }
            }
            else {
                AlertPopup(3, data.title, data.message);
            }
        },
        error: function (data) {
            AlertPopup(2, "Lỗi", data.message);
        }
    });
}

function PreventEnterButton() {
    $(document).keypress(
        function (event) {
            if (event.which == '13') {
                event.preventDefault();
            }
        });
}