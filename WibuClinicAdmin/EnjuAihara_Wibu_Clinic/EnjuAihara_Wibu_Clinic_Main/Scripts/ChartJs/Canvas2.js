function CacBenhThuongGap() {
    let TenBenh = [];
    let SoLuongBenh = [];


    const data = {
        labels: TenBenh,
        datasets: [{
            label: "Các bệnh thường gặp",
            data: SoLuongBenh,
            backgroundColor: [
                '#fedcae',
                '#f2acae',
                '#9c97d3',
                '#da7771',
                '#f5e460',
                '#1f7575',
                '#cc8070',
                '#f7f4fb',
                '#e3d4eb',
                '#e0b9cc'
            ],
            hoverOffset: 4
        }]
    };

    const config = {
        type: 'pie',
        data: data,
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ thống kê các bệnh thường gặp tại phòng mạch',
                    fullSize: true,
                    fontSize: 25,
                    fontFamily: "Times new roman"
                },
                legend: {
                    labels: {
                        font: {
                            family: "Times new roman",
                            size: 15,
                            weight: "bold"
                        }
                    }
                }
            }
        },
        responsive: false,
        maintainAspectRatio: false
    };

    var canvas = $("#canvas2");
    const chart = new Chart(canvas, config);
    GetDataTopBenh(chart, TenBenh, SoLuongBenh);
    window.addEventListener('resize', function () { chart.resize() })
}

function GetDataTopBenh(chart, TenBenh, SoLuongBenh) {
    axios
        .post("/Home/CacBenhThuongGap")
        .then(res => {
            $.each(res.data, function (index, value) {
                TenBenh.push(value.TenBenh);
                SoLuongBenh.push(value.SLBenh);

            });
            chart.update();
        })
        .catch(error => console.log(error));
}