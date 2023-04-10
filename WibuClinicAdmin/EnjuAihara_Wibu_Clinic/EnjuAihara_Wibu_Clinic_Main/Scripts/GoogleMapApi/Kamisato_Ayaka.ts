function initMap(Coords): void {
    const myLatLng = { lat: 0, lng: 78.75000000000001 };
    const map = new google.maps.Map(
        document.getElementById("map") as HTMLElement,
        {
            zoom: 3,
            center: myLatLng,
            mapTypeId: 'satellite'
        }
    );
    for (var i = 0; i < Coords.data.length; i++) {
        const marker = new google.maps.Marker({
            position: { lat: Coords.data[i].Latitude, lng: Coords.data[i].Longitude },
            map,
            title: "Tên nhà cung cấp: " + Coords.data[i].Name + "\nĐịa chỉ: " + Coords.data[i].Address,
            optimized: false
        });
        marker.setMap(map);
    }
}



