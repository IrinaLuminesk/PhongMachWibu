function create() {
    const labels = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];
    const data = {
        labels: labels,
        datasets: [
            {
                label: 'My First Dataset',
                data: [65, 59, 80, 81, 56, 100],
                fill: false,
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            },
            {
                label: 'My First Dataset',
                data: [1, 78, 45, 11, 56, 99],
                fill: false,
                borderColor: 'yellow',
                tension: 0.1
            }]
    };
    const config = {
        type: 'line',
        data: data
    };

    var canvas = document.getElementById("canvas");
    const chart = new Chart(canvas, config);
}