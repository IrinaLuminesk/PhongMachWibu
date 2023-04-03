function InitCalendar() {

	var PersonalEvent = [];
	var calendarEl = document.getElementById('calendar');
	var calendar = new FullCalendar.Calendar(calendarEl, {
		themeSystem: 'bootstrap5',
		locale: 'vi',
		timeZone: 'Asia/Ho_Chi_Minh',
		events: {
			url: '/Personal/Calendar/GetPersonalEvent',
			method: 'POST',
			failure: function () {
				alert('there was an error while fetching events!');
			},
			color: '#9c97d3',   // a non-ajax option
			textColor: 'white' // a non-ajax option
		}
	});
	calendar.setOption('locale', 'vi');
	calendar.setOption('themeSystem', 'bootstrap5');
	calendar.render();
	calendar.render();

}

function GetDataEvent(PersonalEvent) {
	axios
		.post("/Personal/Calendar/GetPersonalEvent")
		.then(res => {
			$.each(res.data, function (index, value) {
				PersonalEvent.push({
					id: value.id,
					title: value.title,
					start: value.start
				});
			});
		})
		.catch(error => console.log(error));
}
