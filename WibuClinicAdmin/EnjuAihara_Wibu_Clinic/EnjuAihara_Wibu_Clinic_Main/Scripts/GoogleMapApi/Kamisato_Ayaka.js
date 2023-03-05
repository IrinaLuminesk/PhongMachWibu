function initMap(Coords) {
    var myLatLng = { lat: 0, lng: 78.75000000000001 };
    var map = new google.maps.Map(document.getElementById("map"), {
        zoom: 3,
        center: myLatLng,
        mapTypeId: 'satellite'
    });
    for (var i = 0; i < Coords.data.length; i++) {
        var marker = new google.maps.Marker({
            position: { lat: Coords.data[i].Latitude, lng: Coords.data[i].Longitude },
            map: map,
            title: "Tên nhà cung cấp: " + Coords.data[i].Name + "\nĐịa chỉ: " + Coords.data[i].Address,
            optimized: false
        });
        marker.setMap(map);
    }
}
//# sourceMappingURL=Kamisato_Ayaka.js.map