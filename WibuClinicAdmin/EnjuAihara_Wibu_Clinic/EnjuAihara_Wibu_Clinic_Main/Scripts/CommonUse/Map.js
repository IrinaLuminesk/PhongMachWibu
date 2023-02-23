function initMap() {

    defaultZoom = 12; // Zoom to
    defaultLatLng = { lat: 14.0583, lng: 108.2772 }; // Việt Nam

    // new Maps
    map = new google.maps.Map(document.getElementById('googleMaps'), {
        zoom: defaultZoom,
        center: defaultLatLng,
        draggableCursor: 'default',
        streetViewControl: false,
        clickableIcons: false
    });


    //new google.maps.Marker({
    //    position: myLatLng,
    //    map,
    //    title: "Hello World!",
    //});

} // end initMap


function CreateMarker(Id) {
    var formData = new FormData();
    formData.append("Id", Id);
    $.ajax({
        type: "POST",
        url: "/MasterData/Medicine/GetCoord",
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function () {
            $("#loading").show();
        },
        success: function (data) {
            $.each(data, function (key, val) {
                console.log(val);
            });
        },
        error: function (data) {
            AlertPopup(2, "Lỗi", data.message);
        },
        complete: function () {
            $("#loading").hide();
        }
    });
}