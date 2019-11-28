function GetStationData() {
    var station = $("#station option:selected").val();
    var url = '~/Stations/GetStationData?id=' + station;
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {

        },
        error: function (data) {

        }
    });
}