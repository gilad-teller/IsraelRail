function GetRoutes() {
    $("#getRoutesBtn").prop('disabled', true);
    $("#getRoutesSpinner").show();
    $("#getRoutesLoad").show();
    $("#getRoutesTxt").hide();
    var oId = $("#oId option:selected").val();
    var dId = $("#dId option:selected").val();
    var dateTime = $("#dateTime").val();
    var isOut = $("input[name=isOut]:checked").val();
    var url = 'Routes/Routes?origin=' + oId + '&destination=' + dId + '&dateTime=' + dateTime + '&isOut=' + isOut;
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {
            $("#content").html(data);
            $("#getRoutesBtn").prop('disabled', false);
            $("#getRoutesSpinner").hide();
            $("#getRoutesLoad").hide();
            $("#getRoutesTxt").show();debugger
            var toShow = $("#toShow").val();
            var elementToShow = document.getElementById("route-" + toShow);
            elementToShow.scrollIntoView();
        },
        error: function (data) {
            console.log(data);
            $("#content").html(data.responseText);
            $("#getRoutesBtn").prop('disabled', false);
            $("#getRoutesSpinner").hide();
            $("#getRoutesLoad").hide();
            $("#getRoutesTxt").show();
        }
    });
}