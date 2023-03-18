
function BaoCaoChiTieuTrongNam() {
    let SoTienChi = [];
    let SoTienThu = [];
 

    const labels = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];
    const data = {
        labels: labels,
        datasets: [
            {
                label: 'Số tiền chi',
                data: SoTienChi,
                fill: false,
                borderColor: '#feafbe',
                backgroundColor: '#feafbe',
                tension: 0.1
            },
            {
                label: 'Số tiền thu',
                data: SoTienThu,
                fill: false,
                borderColor: '#877fae',
                backgroundColor: '#877fae',
                tension: 0.1
            }
        ]
    };
    const config = {
        type: 'line',
        data: data,
        options: {
            tooltips: {
                callbacks: {
                    label: function (tooltipItem, data) {
                        return tooltipItem.yLabel.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                    }
                }
            },
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ thống kê chi tiêu trong năm',
                    fullSize: true,
                    size: 20
                }
            }
        },
        responsive: false,
        maintainAspectRatio: false
    };

    var canvas = $("#canvas7");
    const chart = new Chart(canvas, config);
    GetData(chart, SoTienChi, SoTienThu);
    window.addEventListener('resize', function () { chart.resize() })
}


function GetData(chart, SoTienChi, SoTienThu) {
    axios
        .post("/Home/GetTongTienTienChiTrongNam")
        .then(res => {
            $.each(res.data, function (index, value) {
                SoTienChi.push(value.Money);

            });
            chart.update();
        })
        .catch(error => console.log(error));


    axios
        .post("/Home/GetTongTienTienThuTrongNam")
        .then(res => {
            $.each(res.data, function (index, value) {
                SoTienThu.push(value.Money);

            });
            chart.update();
        })
        .catch(error => console.log(error));

    chart.update();

}