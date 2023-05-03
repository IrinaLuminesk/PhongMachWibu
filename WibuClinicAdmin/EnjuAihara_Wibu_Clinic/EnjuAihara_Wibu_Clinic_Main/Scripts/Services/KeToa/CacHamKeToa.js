function PaggingServerSideCustom(url, columns) {
	/*var load = AjaxLoaderRan();*/
	$("#tableRes").DataTable().clear().destroy();
	$("#tableRes").on('processing.dt', function (e, settings, processing) {
		LoadingDataTable(processing, '.dataTableServerSide');
	}).DataTable({
		proccessing: true,
		serverSide: true,
		paging: true,
		scrollX: true,
		pageLength: 5,
		autoWidth: true,
		searching: false,
		bPaginate: true,
		scrollCollapse: true,
		lengthChange: false,
		ajax: {
			type: 'POST',
			url: "/" + url,
			contentType: 'application/json',
			data: function (d) {
				var form = {};
				$.each($("#MediSearching :input").serializeArray(), function (i, field) {
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
			$("table.dataTable td").css('white-space', 'nowrap');
			$("table.dataTable th").css('white-space', 'nowrap');
			$(window).trigger('resize');
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


function AddToPrescription(id, quantity, index) {
	var formdata = new FormData();
	formdata.append("WarehouseDetailId", id);
	formdata.append("InstockQuantity", quantity);
	formdata.append("Index", index);
	$.ajax({
		type: "POST",
		url: "/Services/Prescription/AddNewMedi",
		data: formdata,
		processData: false,
		contentType: false,
		beforeSend: function () {
			$("#loading").show();
		},
		success: function (response, status, xhr) {
			var ct = xhr.getResponseHeader("content-type") || "";
			if (ct.indexOf('html') > -1) {
				$("#DanhMuchKeToa tbody").append(response)
			}
			if (ct.indexOf('json') > -1) {
				AlertPopup(3, "Lỗi", response);
			}
		},
		error: function (event, xhr, options, exc) {
			AlertPopup(2, "Lỗi", event.message);
		},
		complete: function () {
			$("#loading").hide();
		}
	});
}


function ShowClientInfo(id) {
	var formdata = new FormData();
	formdata.append("ClientId", id);
	$.ajax({
		type: "POST",
		url: "/Services/Prescription/GetInfo",
		data: formdata,
		processData: false,
		contentType: false,
		beforeSend: function () {
			$("#loading").show();
		},
		success: function (response, status, xhr) {
			var data = JSON.parse(JSON.stringify(response));
			ClearInfo();
			$("#PatientFirstName").val(data.FirstName);
			$("#PatientLastName").val(data.LastName);
			$("#PatientPhone").val(data.Phone);
			$("#PatientBirthday").val(data.Birthday);
			$("#PatientAddress").val(data.Address);
			$("#Email").val(data.Email);

		},
		error: function (xhr, status, error) {
			var err = eval("(" + xhr.responseText + ")");
			AlertPopup(2, "Lỗi", err);
		},
		complete: function () {
			$("#loading").hide();
		}
	});
}

function ClearInfo() {
	var id = ["#PatientFirstName", "#PatientLastName", "#PatientPhone", "#PatientBirthday", "#PatientAddress", "#Email"];
	$.each(id, function (index, value) {
		$(value).val("");
	});
}

function LoopThrough(obj, WarehouseDetailId) {
	var flag = true;
	$("#DanhMuchKeToa").find('tbody>tr').each(function () {
		if ($(this).find('input[class^="WarehouseDetailId"]').val() == WarehouseDetailId)
			flag = false;
	});
	if (flag == true) {
		$("#DanhMuchKeToa").find('tbody>tr').each(function () {
			var temp = obj.index + 1;
			$(this).find('input[class^="WarehouseDetailId"]').attr('name', 'Prescription[' + obj.index + '].WarehouseDetailId');
			$(this).find('input[class^="SoluongKe"]').attr('name', 'Prescription[' + obj.index + '].PrescriptionNumber');
			$(this).find('textarea[class^="CachDung"]').attr('name', 'Prescription[' + obj.index + '].HowToUse');
			/*	$(this).find('td[class^="STT"').text(temp);*/
			$(this).find('label[class^="STT"]').text(temp);
			obj.index++;
		});
		return true;
	}
	else {
		return false;
	}
}