using IsraelRail.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsraelRail.Models.ViewModels
{
    public class StationData
    {
        public int StationId { get; set; }
        public string Name { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public IEnumerable<Gate> Gates { get; set; }

        public StationData(StationInformationResult stationInformation, StationLightData stationLightData)
        {
            StationId = stationInformation.StationDetails.StationId;
            Name = stationLightData.Name;
            Longitude = stationLightData.Longitude;
            Latitude = stationLightData.Latitude;
            Gates = stationInformation.GateInfo.Select(x => new Gate(x));
        }
    }

    public class Gate
    {
        public int GateId { get; set; }
        public string Name { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public IEnumerable<ActivityHours> ActivityHours { get; set; }
        public IEnumerable<string> Services { get; set; }

        public Gate(GateInfo gateInfo)
        {
            GateId = gateInfo.StationGateId;
            Name = gateInfo.GateName;
            Longitude = gateInfo.GateLontitude;
            Latitude = gateInfo.GateLatitude;
            ActivityHours = gateInfo.GateActivityHours.Select(x => new ActivityHours(x));
            Services = gateInfo.GateServices.Select(x => x.ServiceName);
        }

    }

    public class ActivityHours
    {
        public string StartHour { get; set; }
        public string EndHour { get; set; }
        public IEnumerable<DayOfWeek> DaysOfWeek { get; set; }
        public ActivityHours(GateActivityHour activityHour)
        {
            StartHour = activityHour.StartHour;
            EndHour = activityHour.EndHour;
            DaysOfWeek = activityHour.ActivityDaysNumbers.Split(',').Select(x => x.ParseToDayOfWeek());
        }
    }

    public class StationLightData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
    }
}
