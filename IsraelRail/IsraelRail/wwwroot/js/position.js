function SetPosition(position) {
    const latitude = position.coords.latitude;
    const longitude = position.coords.longitude;
    var min = 100;
    var minStation = "0";
    for (const s of stationsData) {
        var dis = GetDistance(latitude, longitude, s);
        if (dis < min) {
            min = dis;
            minStation = s.id;
        }
    }
    if (minStation != "0") {
        $(".userStation").val(minStation);
    }
    GetStations();
}

function FailedPosition(err) {
    console.error('Failed to get user position');
    console.error(err);
    GetStations();
}

function GetDistance(lat, lon, station) {
    var latDiff = Math.abs(lat - station.latitude);
    var lonDiff = Math.abs(lon - station.longitude);
    var latSqr = latDiff * latDiff;
    var lonSqr = lonDiff * lonDiff;
    return Math.sqrt(latSqr + lonSqr);
}

function GetStations() {
    var oId = Cookies.get('oId');
    var dId = Cookies.get('dId');
    var oIdElement = $("#oId");
    var dIdElement = $("#dId");
    if (oId != undefined && dId != undefined) {
        if (oId == oIdElement.val()) {
            dIdElement.val(dId);
        }
        else {
            dIdElement.val(oId);
        }
    }
}