function initMap(Coords): void {
    const myLatLng = { lat: 0, lng: 78.75000000000001 };
    const map = new google.maps.Map(
        document.getElementById("map") as HTMLElement,
        {
            zoom: 3,
            center: myLatLng,
        }
    );
    for (var i = 0; i < Coords.length; i++) {
        new google.maps.Marker({
            position: { lat: Coords[i].Latitude, lng: Coords[i].Longitude },
            map,
            title: "Hello World!",
        });
    }


}



