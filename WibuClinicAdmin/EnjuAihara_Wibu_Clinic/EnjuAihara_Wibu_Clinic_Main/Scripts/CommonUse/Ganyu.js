function Select2Init(Id) {
    $(Id).select2({
        width: '100%',
        scroll: true
    });
};

function Select2MultipleInit(Id) {
    $(Id).select2({
        width: '100%',
        scroll: true,
        tags: true
    });
};

function Select2_AutoComplete(url, id) {
    $(id).select2({
        width: '100%',
        scroll: true,
        ajax: {
            url: url,
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    searchTerm: params.term, // search term
                    page: params.page
                };
            },
            processResults: function (data) {
                return {
                    results: $.map(data, function (obj) {
                        return { id: obj.value, text: obj.text };
                    })
                };
            },
            minimumInputLength: 0

        }
    });
}
function SearchInitialWithClick(controller) {
    $("#btn-search").click(function () {
        SearchInit(controller);
    });
    $("#btn-search").trigger("click");
}


function PaggingServerSideSearchInitialWithClick(controller, columns, dropdown) {
    $("#btn-search").click(function () {
        PaggingServerSide(controller, columns, dropdown);
    });
    $("#btn-search").trigger("click");
}


function SearchInit(controller) {
    var $btn = $("#btn-search");
    $btn.button('loading');

    /*  var load = AjaxLoaderRan();*/
    $.ajax({
        type: "POST",
        url: "/" + controller + "/_Search",
        data: $("#frmSearch").serializeArray(),
        beforeSend: function () {
            $("#loading").show();
        },
        success: function (data) {
            $("#divSearchResult").html("");
            $("#divSearchResult").html(data);
            Pagging();
        },
        error: function (error) {
            $btn.button('reset');
            AlertPopup(2, "Đã có lỗi xảy ra", error);
        },
        complete: function () {
            $("#loading").hide();
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
            $("table.dataTable td").css('white-space', 'nowrap')
            $("table.dataTable th").css('white-space', 'nowrap')
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

//Nếu có dropdown thì cho dropdown = true, còn không thì = false
function PaggingServerSide(controller, columns, dropdown) {
    /*var load = AjaxLoaderRan();*/
    $("#tableRes").DataTable().clear().destroy();
    $("#tableRes").on('processing.dt', function (e, settings, processing) {
        LoadingDataTable(processing, '.dataTableServerSide');
        $("table.dataTable td").css('white-space', 'nowrap')
        $("table.dataTable th").css('white-space', 'nowrap')
    }).DataTable({
        proccessing: true,
        serverSide: true,
        paging: true,
        scrollX: true,
        pageLength: 10,
        autoWidth: true,
        searching: false,
        bPaginate: true,
        scrollCollapse: true,
        ajax: {
            type: 'POST',
            url: "/" + controller + "/_PaggingServerSide",
            contentType: 'application/json',
            data: function (d) {
                //var arr = {};
                ////data search
                //var data = $("#frmSearch").serializeArray();
                //$.each(data, function (index, val) {
                //    var obj = {};
                //    obj[val.name] = val.value;
                //    $.extend(true, arr, obj);
                //});
                ////data datatable (draw, start, length,...)
                ///*$.extend(true, arr, data);*/
                //$.extend(true, arr, d);
                //console.log(arr);
                //return JSON.stringify(arr);
                var form = {};
                $.each($("#frmSearch").serializeArray(), function (i, field) {
                    form[field.name] = field.value || '';
                });
                $.extend(true, d, form);
                return JSON.stringify(d);

            },
            beforeSend: function () {
                $("#loading").show();
            },
            complete: function () {
                $("#loading").hide();
            }
        },
        columns: columns,
        destroy: true,
        initComplete: function (settings) {
            $(window).resize();

        },
        drawCallback: function (settings) {
            $(window).trigger('resize');
            if (dropdown == true) {
                Select2Init(".dropdown");
            }
        },
        language: {
            sProcessing: "Vui lòng đợi xíu.....(๑´•ε •`๑)",
            sLengthMenu: "Xem _MENU_ mục",
            sZeroRecords: "Không tìm thấy dòng nào phù hợp",
            sInfo: "Đang xem _START_ đến _END_ trong tổng số _TOTAL_ mục",
            sInfoEmpty: "Đang xem 0 đến 0 trong tổng số 0 mục",
            sInfoFiltered: "(được lọc từ _MAX_ mục)",
            sInfoPostFix: "",
            sSearch: "Tìm nội dung:",
            sUrl: "",
            oPaginate: {
                sFirst: "Đầu",
                sPrevious: "&laquo;",
                sNext: "&raquo;",
                sLast: "Cuối"
            }
        },
        columnDefs: [
            { targets: [0, 1], visible: true },
            { targets: 'no-sort', visible: false },
        ],
        /* "sDom": '<"top"flp>rt<"bottom"ip><"clear">',*/
    });
}

function LoadingDataTable(processing, element) {
    var height = $(element + " tbody").height();
    var width = $(element).width();

    var ele = $(element).parent(".dataTables_scrollBody");
    var processingElement = ele.find('.dataTables_processing');
    if (processingElement.length == 0) {
        $(element).parent(".dataTables_scrollBody").append('<div class="dataTables_processing"></div>');
    }
    $(processingElement).css('width', width + 10);
    $(processingElement).css('padding-top', height / 2);
    $(processingElement).css('display', processing ? 'block' : 'none');

    if ($($("ul.nav-tabs li.active a").attr('href')).find('.dataTables_processing').length > 0 || $('.nav-tabs-custom').length == 0) {
        //Loading content
        var contentWidth = $(element).parent(".dataTables_scrollBody").width();
        if (contentWidth > 0) {
            $('.loading-content').css($(element).parent(".dataTables_scrollBody").position());
            $('.loading-content').css('left', contentWidth / 2);
            $('.loading-content').css('padding-top', height / 2);
            $('.loading-content').css('display', $(element + ' tbody tr').length > 0 && processing ? 'block' : 'none');
        }
    }
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
    setTimeout(function () {
        $(alertId).removeClass("show");
        $(alertId).addClass("hide");
    }, 3000);
}


function SaveData(controller, frmCreate) {
    var frm = $(frmCreate);
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
    /*  var load = AjaxLoaderRan();*/
    $.ajax({
        type: "POST",
        url: "/" + controller + "/Create",
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function () {
            $("#loading").show();
        },
        success: function (data) {
            if (data.isSucess) {
                if (data.title && data.message)
                    AlertPopup(1, data.title, data.message);
                if (data.redirect) {
                    setTimeout(function () {
                        window.location.href = data.redirect;
                    }, 1500);
                }
            }
            else {
                AlertPopup(3, data.title, data.message);
            }
        },
        error: function (data) {
            AlertPopup(2, "Lỗi", data.message);
        },
        complete: function () {
            $("#loading").hide();
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

    /* var load = AjaxLoaderRan();*/
    $.ajax({
        type: "POST",
        url: "/" + controller + "/Edit",
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function () {
            $("#loading").show();
        },
        success: function (data) {
            if (data.isSucess) {
                if (data.title && data.message)
                    AlertPopup(1, data.title, data.message);
                if (data.redirect) {
                    setTimeout(function () {
                        window.location.href = data.redirect;
                    }, 1500);
                }
            }
            else {
                AlertPopup(3, data.title, data.message);
            }
        },
        error: function (data) {
            AlertPopup(2, "Lỗi", data.message);
        },
        complete: function () {
            $("#loading").hide();
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


function Logout() {
    $.ajax({
        type: "POST",
        url: "/Permission/Auth/Logout",
        success: function (data) {
            window.location.replace("/Permission/Auth/Login");
        },
        error: function (data) {
            AlertPopup(2, "Lỗi", data.message);
        }
    });
}


function AjaxLoaderRan() {
    var i = Math.floor(Math.random() * 2) + 1;
    if (i == 1)
        return "#loading";
    return "#loading2";
}

function PreviewImg(input, id) {
    if (input.files && input.files[0]) {

        var filePath = input.value;
        var allowedExtensions =
            /(\.jpg|\.png|\.jpeg|\.webp)$/i;

        if (allowedExtensions.exec(filePath)) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $(id).attr('src', e.target.result);
            };

            reader.readAsDataURL(input.files[0]);
        }
        else {
            input.value = "";
            AlertPopup(2, "Lỗi định dạng", "Vui lòng chọn file ảnh đúng định dạng");
        }
    }
}


function Delete(controller, id) {
    $.ajax({
        url: "/" + controller + "/Delete/" + id,
        type: "POST",
        dataType: "json",
        success: function (data) {
            if (data.isSucess == true) {
                if (data.title && data.message) {
                    AlertPopup(1, data.title, data.message);
                }
                if (data.redirect) {
                    setTimeout(function () {
                        window.location.href = data.redirect;
                    }, 1500);
                }
            }

        },
        error: function (data) {
            AlertPopup(2, "Lỗi", data.message);
        }

    });
}
$(document).on("click", ".btn-delete", function (e) {
    var itemName = $(this).data("item-name");
    var id = $(this).data("id");
    var controller = $(this).data("controller");
    $("#deleteConfirmModal .modal-title .item-name").html(itemName);
    $("#deleteConfirmModal .modal-question .controller").html(controller);
    $("#deleteConfirmModal").modal("show");

    $("#deleteBtn").on("click", function () {
        Delete(controller, id);
    });
});
$(document).on("click", "#cancel", function (e) {
    $("#deleteConfirmModal").modal("hide");
});
$(document).on("click", ".close", function (e) {
    $("#deleteConfirmModal").modal("hide");
});


function CloseSideBar() {
    $("#bodytag").addClass("sidebar-mini sidebar-collapse")
}

