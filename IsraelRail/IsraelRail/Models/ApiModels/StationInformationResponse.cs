using System;

namespace IsraelRail.Models.ApiModels
{
    public class StationInformationResponse : IsraelRailResponse
    {
        public StationInformationResult Result { get; set; }
    }

    public class StationInformationResult : IsraelRailResponse
    {
        public object[] StationUpdates { get; set; }
        public StationDetails StationDetails { get; set; }
        public GateInfo[] GateInfo { get; set; }
        public string[] EasyCategories { get; set; }
        public string[] SafetyInfos { get; set; }
    }

    public class StationDetails
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
        public string CarParking { get; set; }
        public string ParkingCosts { get; set; }
        public string BikeParking { get; set; }
        public string BikeParkingCosts { get; set; }
        public object AirPollutionIcon { get; set; }
        public object StationMap { get; set; }
        public object NonActiveElevators { get; set; }
        public object NonActiveElevatorsLink { get; set; }
        public bool StationIsClosed { get; set; }
        public DateTime StationIsClosedUntill { get; set; }
        public object StationIsClosedText { get; set; }
        public string StationInfoTitle { get; set; }
        public string StationInfo { get; set; }
        public string AboutStationTitle { get; set; }
        public string AboutStationContent { get; set; }
    }

    public class GateInfo
    {
        public int StationGateId { get; set; }
        public string GateName { get; set; }
        public object GateAddress { get; set; }
        public float GateLatitude { get; set; }
        public float GateLontitude { get; set; }
        public GateActivityHour[] GateActivityHours { get; set; }
        public GateService[] GateServices { get; set; }
    }

    public class GateActivityHour
    {
        public int ActivityHoursType { get; set; }
        public string IsClosedShortText { get; set; }
        public string IsClosedLongText { get; set; }
        public string ActivityDaysNumbers { get; set; }
        public object StartHourTextKey { get; set; }
        public string StartHour { get; set; }
        public object StartHourReplaceTextKey { get; set; }
        public object EndHourPrefixTextKey { get; set; }
        public string EndHour { get; set; }
        public object EndHourReplaceTextKey { get; set; }
        public object EndHourPostfixTextKey { get; set; }
        public object ActivityHoursReplaceTextKey { get; set; }
    }

    public class GateService
    {
        public string ServiceName { get; set; }
        public string ServiceIcon { get; set; }
        public string ServiceIconLink { get; set; }
    }

}
