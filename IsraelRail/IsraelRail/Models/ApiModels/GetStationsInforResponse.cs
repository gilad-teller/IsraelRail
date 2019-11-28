using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsraelRail.Models.ApiModels
{
    public class GetStationsInforResponse : RailResponse
    {
        public GetStationsInforResponseData[] Data { get; set; }
    }

    public class GetStationsInforResponseData
    {
        public string StationCode { get; set; }
        public StationGeneral General { get; set; }
        public StationLanguage Hebrew { get; set; }
        public StationLanguage English { get; set; }
        public StationLanguage Russian { get; set; }
        public StationLanguage Arabic { get; set; }
        public Instation[] InStation { get; set; }
        public Trip[] Trips { get; set; }
        public Completeservicesapp[] CompleteServicesAPP { get; set; }
        public Stationtime[] StationTimes { get; set; }
        public object[] RavKavTimes { get; set; }
        public Parking[] Parking { get; set; }
        public Cashiertime[] CashierTimes { get; set; }
        public Completeservice[] CompleteServices { get; set; }
        public object[] Shatels { get; set; }
    }

    public class StationGeneral
    {
        public string StationCode { get; set; }
        public float Lat { get; set; }
        public float Long { get; set; }
        public string StationArea { get; set; }
        public bool ShowEasy { get; set; }
    }

    public class StationLanguage
    {
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public object CityName { get; set; }
        public string ParkingArrangments { get; set; }
        public string Accessability { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDes { get; set; }
        public string SeoKeyWords { get; set; }
        public string SocialTitle { get; set; }
        public string SocialDes { get; set; }
        public string TaxiDes { get; set; }
        public object StationMap { get; set; }
        public string Url { get; set; }
    }

    public class Instation
    {
        public string NameHeb { get; set; }
        public string NameArb { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string DessHeb { get; set; }
        public string DessArb { get; set; }
        public string DessEng { get; set; }
        public string DessRus { get; set; }
        public string img { get; set; }
        public int id { get; set; }
    }

    public class Trip
    {
        public string Link { get; set; }
        public string img { get; set; }
        public string lang { get; set; }
        public int order { get; set; }
        public string[] Codes { get; set; }
        public string title { get; set; }
    }

    public class Completeservicesapp
    {
        public string title { get; set; }
        public string NameHeb { get; set; }
        public string NameArb { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string DessHeb { get; set; }
        public string DessArb { get; set; }
        public string DessEng { get; set; }
        public string DessRus { get; set; }
        public string ImgRelSrc { get; set; }
        public int id { get; set; }
    }

    public class Stationtime
    {
        public string StationCode { get; set; }
        public string DayST { get; set; }
        public string DayF { get; set; }
        public string DaySNight { get; set; }
        public object Order { get; set; }
        public string Hebrew { get; set; }
        public string Arabic { get; set; }
        public string Russian { get; set; }
        public string English { get; set; }
    }

    public class Parking
    {
        public string StationCode { get; set; }
        public string NameHeb { get; set; }
        public string NameArb { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string AdressHeb { get; set; }
        public string AdressArb { get; set; }
        public string AdressEng { get; set; }
        public string AdressRus { get; set; }
        public string lat { get; set; }
        public string Long { get; set; }
        public string distance { get; set; }
        public object order { get; set; }
    }

    public class Cashiertime
    {
        public string StationCode { get; set; }
        public string DayS { get; set; }
        public string DayMW { get; set; }
        public string DayT { get; set; }
        public string DayF { get; set; }
        public string DaySNight { get; set; }
        public string Order { get; set; }
        public string Hebrew { get; set; }
        public string Arabic { get; set; }
        public string Russian { get; set; }
        public string English { get; set; }
    }

    public class Completeservice
    {
        public int id { get; set; }
        public string title { get; set; }
        public string Hebrew { get; set; }
        public string Arabic { get; set; }
        public string Russian { get; set; }
        public string English { get; set; }
        public string DessHeb { get; set; }
        public string DessArb { get; set; }
        public string DessEng { get; set; }
        public string DessRus { get; set; }
        public string ImgRelSrc { get; set; }
    }

}
