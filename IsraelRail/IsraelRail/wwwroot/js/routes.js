function GetRoutes() {
    $("#getRoutesBtn").prop('disabled', true);
    $("#getRoutesSpinner").show();
    $("#getRoutesLoad").show();
    $("#getRoutesTxt").hide();
    var oId = $("#oId option:selected").val();
    var dId = $("#dId option:selected").val();
    var dateTime = $("#dateTime").val();
    var isDepart = $("input[name=isDepart]:checked").val();
    var url = 'Routes/Routes?origin=' + oId + '&destination=' + dId + '&dateTime=' + dateTime + '&isDepart=' + isDepart;
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {
            $("#content").html(data);
            $("#getRoutesBtn").prop('disabled', false);
            $("#getRoutesSpinner").hide();
            $("#getRoutesLoad").hide();
            $("#getRoutesTxt").show();
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

function GetAdvanced() {
    $("#getAdvancedBtn").prop('disabled', true);
    $("#getAdvancedSpinner").show();
    $("#getAdvancedLoad").show();
    $("#getAdvancedTxt").hide();
    var oId = $("#oId option:selected").val();
    var dId = $("#dId option:selected").val();
    var dateTime = $("#dateTime").val();
    var isDepart = $("input[name=isDepart]:checked").val();
    var url = 'Routes/Advanced?origin=' + oId + '&destination=' + dId + '&dateTime=' + dateTime + '&isDepart=' + isDepart;
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {
            $("#content").html(data);
            $("#getAdvancedBtn").prop('disabled', false);
            $("#getAdvancedSpinner").hide();
            $("#getAdvancedLoad").hide();
            $("#getAdvancedTxt").show();
            var toShow = $("#toShow").val();
            var elementToShow = document.getElementById("route-" + toShow);
            elementToShow.scrollIntoView();
        },
        error: function (data) {
            console.log(data);
            $("#content").html(data.responseText);
            $("#getAdvancedBtn").prop('disabled', false);
            $("#getAdvancedSpinner").hide();
            $("#getAdvancedLoad").hide();
            $("#getAdvancedTxt").show();
        }
    });
}

function ToggleShortLong(routeIndex, trainNumber) {
    var short = $("#short-" + routeIndex + "-" + trainNumber);
    var long = $("#long-" + routeIndex + "-" + trainNumber);
    short.toggle();
    long.toggle();
}