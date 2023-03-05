function initMap(Coords) {
    var myLatLng = { lat: 0, lng: 78.75000000000001 };
    var map = new google.maps.Map(document.getElementById("map"), {
        zoom: 3,
        center: myLatLng,
    });
    for (var i = 0; i < Coords.length; i++) {
        new google.maps.Marker({
            position: { lat: Coords[i].Latitude, lng: Coords[i].Longitude },
            map: map,
            title: "Hello World!",
        });
    }
}
//# sourceMappingURL=Kamisato_Ayaka.js.map