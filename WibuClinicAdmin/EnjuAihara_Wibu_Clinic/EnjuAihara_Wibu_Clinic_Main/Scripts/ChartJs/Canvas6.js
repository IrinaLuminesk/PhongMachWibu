function GetData(chart, SLNhap, SLXuat) {
    axios
        .post("/Home/WarehouseReport")
        .then(res => {
            $.each(res.data, function (index, value) {
                SLNhap.push(value.SLNhap);
                SLXuat.push(value.SLXuat);
            });
            chart.update();
        })
        .catch(error => console.log(error));
}
function BaoCaoKho() {
    let SLNhap = [];
    let SLXuat = [];


    const labels = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];
    const data = {
        labels: labels,
        datasets: [
            {
                label: 'Số lượng nhập',
                data: SLNhap,
                borderColor: '#feafbe',
                backgroundColor: '#feafbe',
                stack: 'single',
                type: 'bar',
                order: 1,
            },
            {
                label: 'Số lượng xuất',
                data: SLXuat,
                borderColor: '#877fae',
                backgroundColor: '#877fae',
                stack: 'single',
                order: 0,
            }
        ]
    };
    const config = {
        type: 'line',
        data: data,
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Biểu đồ thống kê chi tiêu trong năm'
                }
            },
            scales: {
                y: {
                    beginatzero: true,
                    stacked: false
                }
            }
        },
    };

    var canvas = $("#canvas6");
    const chart = new Chart(canvas, config);
    GetData(chart, SLNhap, SLXuat);
    window.addEventListener('resize', function () { chart.resize() })
}
