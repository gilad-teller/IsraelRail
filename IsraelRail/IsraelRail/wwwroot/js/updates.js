function GetUpdates() {
    $("#getDataBtn").prop('disabled', true);
    $("#getDataSpinner").show();
    $("#getDataLoad").show();
    $("#getDataTxt").hide();
    var oId = $("#oId option:selected").val();
    var dId = $("#dId option:selected").val();
    var url = 'Updates/StationUpdates?oId=' + oId + '&dId=' + dId;
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