function SetPosition(position) {
    const latitude = position.coords.latitude;
    const longitude = position.coords.longitude;
    var min = 100;
    var minStation = 0;
    for (const s of stationsData) {
        var dis = GetDistance(latitude, longitude, s);
        if (dis < min) {
            min = dis;
            minStation = s.station;
        }
    }
    $(".userStation").val(minStation);
}

function FailedPosition() {
    console.error('Failed to get user position');
}

function GetDistance(lat, lon, station) {
    var latDiff = Math.abs(lat - station.latitude);
    var lonDiff = Math.abs(lon - station.longitude);
    var latSqr = latDiff * latDiff;
    var lonSqr = lonDiff * lonDiff;
    return Math.sqrt(latSqr + lonSqr);
}