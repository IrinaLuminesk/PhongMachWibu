function GetDataTop10KH(chart, SLDH, RealName) {
    axios
        .post("/Home/Top10KHTT")
        .then(res => {
            $.each(res.data, function (index, value) {
                SLDH.push(value.SoLanDat);
                RealName.push(value.RealName);
            });
            chart.update();
        })
        .catch(error => console.log(error));
}
function Top10KH() {
    let RealName = [];
    let SoLanDat = [];


    const labels = RealName;
    const data = {
        labels: labels,
        datasets: [{
            label: 'Top 10 Khách Hàng Thân Thiết',
            data: SoLanDat,
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(255, 159, 64, 0.2)',
                'rgba(255, 205, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(54, 162, 235, 0.2)',
                'rgba(153, 102, 255, 0.2)',
                'rgba(201, 203, 207, 0.2)'
            ],
            borderColor: [
                'rgb(255, 99, 132)',
                'rgb(255, 159, 64)',
                'rgb(255, 205, 86)',
                'rgb(75, 192, 192)',
                'rgb(54, 162, 235)',
                'rgb(153, 102, 255)',
                'rgb(201, 203, 207)'
            ],
            borderWidth: 1
        }]
    };
    const config = {
        type: 'bar',
        data: data,
        options: {
            scales: {
                y: {
                    ticks: {
                        precision: 0,
                        beginAtZero: true,
                    },
                }
            }
        },
        responsive: false,
        maintainAspectRatio: false
    };

    var canvas = $("#canvas4");
    const chart = new Chart(canvas, config);
    GetDataTop10KH(chart, SoLanDat, RealName);
    window.addEventListener('resize', function () { chart.resize() })
}