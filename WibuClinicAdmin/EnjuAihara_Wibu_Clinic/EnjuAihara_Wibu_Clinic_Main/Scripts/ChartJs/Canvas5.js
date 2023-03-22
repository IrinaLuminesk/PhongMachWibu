function TopThuocTrongNam() {
	let TenThuoc = [];
	let SoLuongSuDung = [];


    const data = {
        labels: TenThuoc,
        datasets: [{
            data: SoLuongSuDung,
            backgroundColor: [
                '#f197a4',
                '#a1ebdb',
                '#e0cbd1',
                '#89d4e7',
                '#fff0d9',
                '#1f7575',
                '#cc8070',
                '#f7f4fb',
                '#e3d4eb',
                '#e0b9cc'
            ]
        }]
    };

    const config = {
        type: 'polarArea',
        data: data,
        options: {
            scales: {
                r: {
                    pointLabels: {
                        display: true,
                        font: {
                            size: 10
                        }
                    }
                }
            },
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ thống kê thuốc sử dụng nhiều trong năm',
                    fullSize: true,
                    fontSize: 25,
                    fontFamily: "Times new roman"
                },
                legend: {
                    labels: {
                        font: {
                            family: "Times new roman",
                            size: 10
                        }
                    }
                }
            }
        },
        responsive: false,
        maintainAspectRatio: false
    };

	var canvas = $("#canvas5");
    const chart = new Chart(canvas, config);
    GetDataTopThuoc(chart, TenThuoc, SoLuongSuDung);
	window.addEventListener('resize', function () { chart.resize() })
}

	let SoLuongSuDung = [];
function GetDataTopThuoc(chart, TenThuoc, SoLuongSuDung) {
    axios
        .post("/Home/TopThuocSuDungNhieuNhatTrongNam")
        .then(res => {
            $.each(res.data, function (index, value) {
                TenThuoc.push(value.TenThuoc);
                SoLuongSuDung.push(value.SoLuong);

            });
            chart.update();
        })
        .catch(error => console.log(error));
}