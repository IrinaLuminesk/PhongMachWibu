function GetDataSLKhachTheoThang(chart, SLKhach) {
    axios
        .post("Home/GetSLKhachTheoThang")
        .then(res => {
            $.each(res.data, function (index, value) {
                SLKhach.push(value.SLKhach);

            });
            chart.update();
        })
        .catch(error => console.log(error));
}
function ThongKeSLKhachTheoThang() {
    let SLKhach=[];

    const labels = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];
    const data = {
        labels: labels,
        datasets: [
            {
                label: 'Số lượng khách',
                data: SLKhach,
                fill: false,
                borderColor: '#feafbe',
                backgroundColor: '#feafbe',
                tension: 0.1
            },

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
                    /*text: (ctx) => 'Point Style: ' + ctx.chart.data.datasets[0].pointStyle,*/
                }
            }
        },
        responsive: false,
        maintainAspectRatio: false
    };
    var canvas = $("#canvas1");
    const chart = new Chart(canvas, config);
    GetDataSLKhachTheoThang(chart, SLKhach);
    window.addEventListener('resize', function () { chart.resize() })
}
    
