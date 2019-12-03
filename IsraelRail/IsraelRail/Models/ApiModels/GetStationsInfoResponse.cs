using System;
namespace IsraelRail.Models.ApiModels
{

    public class GetStationsInfoResponse : RailResponse
    {
        public GetStationsInfoResponseData[] Data { get; set; }
    }

    public class GetStationsInfoResponseData
    {
        public string UpdateContentArb { get; set; }
        public string UpdateContentEng { get; set; }
        public string UpdateContentHeb { get; set; }
        public string UpdateContentRus { get; set; }
        public string UpdateLinkArb { get; set; }
        public string UpdateLinkEng { get; set; }
        public string UpdateLinkHeb { get; set; }
        public string UpdateLinkRus { get; set; }
        public object[] Station { get; set; }
        public string ReportType { get; set; }
        public string ReportImage { get; set; }
        public DateTime StartValidationOfReport { get; set; }
        public DateTime EndValidationOfReport { get; set; }
        public string NameArb { get; set; }
        public string NameEng { get; set; }
        public string NameHeb { get; set; }
        public string NameRus { get; set; }
        public bool Float { get; set; }
        public int Order { get; set; }
    }

}
