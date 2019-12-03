using IsraelRail.Models.ApiModels;
using System.Collections.Generic;
using System.Linq;

namespace IsraelRail.Models.ViewModels
{
    public class StationData
    {
        public E_Language Language { get; set; }
        public string StationCode { get; set; }
        public string Name { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string Area { get; set; }
        public IEnumerable<OpeningHours> OpeningHours { get; set; }
        public IEnumerable<StationFacility> Facilities { get; set; }
        public IEnumerable<StationParking> Parking { get; set; }
        public IEnumerable<Cashier> Cashiers { get; set; }

        public StationData(GetStationsInforResponseData getStationsInforResponseData, E_Language language)
        {
            Language = language;
            StationCode = getStationsInforResponseData.StationCode;
            Longitude = getStationsInforResponseData.General.Long;
            Latitude = getStationsInforResponseData.General.Lat;
            Area = getStationsInforResponseData.General.StationArea;
            switch (language)
            {
                case E_Language.Hebrew:
                    Name = getStationsInforResponseData.Hebrew.StationName;
                    break;
                case E_Language.English:
                    Name = getStationsInforResponseData.English.StationName;
                    break;
                case E_Language.Russian:
                    Name = getStationsInforResponseData.Russian.StationName;
                    break;
                case E_Language.Arabic:
                    Name = getStationsInforResponseData.Arabic.StationName;
                    break;
            }
            OpeningHours = getStationsInforResponseData.StationTimes.Select(x => new OpeningHours(x));
            Facilities = getStationsInforResponseData.InStation.Select(x => new StationFacility(x, language));
            Parking = getStationsInforResponseData.Parking.Select(x => new StationParking(x, language));
            Cashiers = getStationsInforResponseData.CashierTimes.Select(x => new Cashier(x, language));
        }
    }

    public class OpeningHours
    {
        public string Weekday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }

        public OpeningHours(Stationtime stationtime)
        {
            Weekday = stationtime.DayST;
            Friday = stationtime.DayF;
            Saturday = stationtime.DaySNight;
        }
    }

    public class StationFacility
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public StationFacility(Instation instation, E_Language language)
        {
            switch (language)
            {
                case E_Language.Hebrew:
                    Name = instation.NameHeb;
                    Description = instation.DessHeb;
                    break;
                case E_Language.English:
                    Name = instation.NameEng;
                    Description = instation.DessEng;
                    break;
                case E_Language.Russian:
                    Name = instation.NameRus;
                    Description = instation.DessRus;
                    break;
                case E_Language.Arabic:
                    Name = instation.NameArb;
                    Description = instation.DessArb;
                    break;
            }
        }
    }

    public class StationParking
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string Distance { get; set; }

        public StationParking(Parking parking, E_Language language)
        {
            switch (language)
            {
                case E_Language.Hebrew:
                    Name = parking.NameHeb;
                    Address = parking.AdressHeb;
                    break;
                case E_Language.English:
                    Name = parking.NameEng;
                    Address = parking.AdressEng;
                    break;
                case E_Language.Russian:
                    Name = parking.NameRus;
                    Address = parking.AdressRus;
                    break;
                case E_Language.Arabic:
                    Name = parking.NameArb;
                    Address = parking.AdressArb;
                    break;
            }
            float.TryParse(parking.lat, out float lat);
            float.TryParse(parking.Long, out float lon);
            Latitude = lat;
            Longitude = lon;
            Distance = parking.distance;
        }
    }

    public class Cashier
    {
        public string Name { get; set; }
        public string Sunday { get; set; }
        public string Weekday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }

        public Cashier(Cashiertime cashier, E_Language language)
        {
            switch (language)
            {
                case E_Language.Hebrew:
                    Name = cashier.Hebrew;
                    break;
                case E_Language.English:
                    Name = cashier.English;
                    break;
                case E_Language.Russian:
                    Name = cashier.Russian;
                    break;
                case E_Language.Arabic:
                    Name = cashier.Arabic;
                    break;
            }
            Sunday = cashier.DayS;
            Weekday = cashier.DayMW;
            Thursday = cashier.DayT;
            Friday = cashier.DayF;
            Saturday = cashier.DaySNight;
        }
    }
}
