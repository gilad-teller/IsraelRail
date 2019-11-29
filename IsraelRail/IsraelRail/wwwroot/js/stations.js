function GetStationData() {
    $("#getDataBtn").prop('disabled', true);
    $("#getDataSpinner").show();
    $("#getDataLoad").show();
    $("#getDataTxt").hide();
    var station = $("#station option:selected").val();
    var url = 'Stations/Station?id=' + station;
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {
            $("#content").html(data);
            $("#getDataBtn").prop('disabled', false);
            $("#getDataSpinner").hide();
            $("#getDataLoad").hide();
            $("#getDataTxt").show();
        },
        error: function (data) {
            console.log(data);
            $("#content").html(data.responseText);
            $("#getDataBtn").prop('disabled', false);
            $("#getDataSpinner").hide();
            $("#getDataLoad").hide();
            $("#getDataTxt").show();
        }
    });
}