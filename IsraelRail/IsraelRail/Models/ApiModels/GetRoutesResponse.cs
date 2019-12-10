using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsraelRail.Models.ApiModels
{
    public class GetRoutesResponse : RailResponse
    {
        public GetRoutesResponseData Data { get; set; }
    }

    public class GetRoutesResponseData
    {
        public object Error { get; set; }
        public Details Details { get; set; }
        public int StartIndex { get; set; }
        public Route[] Routes { get; set; }
        public object BeforeRoutes { get; set; }
        public object[] Rates { get; set; }
        public Delay[] Delays { get; set; }
        public Trainposition[] TrainPositions { get; set; }
        public Omasim[] Omasim { get; set; }
    }

    public class Details
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Date { get; set; }
        public string SugKav { get; set; }
        public string Hour { get; set; }
    }

    public class Route
    {
        public Train[] Train { get; set; }
        public bool IsExchange { get; set; }
        public string EstTime { get; set; }
    }

    public class Train
    {
        public string Trainno { get; set; }
        public string OrignStation { get; set; }
        public string DestinationStation { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureTime { get; set; }
        public Stopstation[] StopStations { get; set; }
        public string LineNumber { get; set; }
        public string Route { get; set; }
        public bool Midnight { get; set; }
        public bool Handicap { get; set; }
        public bool DirectTrain { get; set; }
        public object TrainParvariBenironi { get; set; }
        public bool ReservedSeat { get; set; }
        public string Platform { get; set; }
        public string DestPlatform { get; set; }
        public bool IsFullTrain { get; set; }
    }

    public class Stopstation
    {
        public string StationId { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureTime { get; set; }
        public string Platform { get; set; }
    }

    public class Delay
    {
        public string Station { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Train { get; set; }
        public string Min { get; set; }
    }

    public class Trainposition
    {
        public int CurrentStation { get; set; }
        public int DifMin { get; set; }
        public int NextStation { get; set; }
        public int TrainNumber { get; set; }
        public string DifType { get; set; }
        public string TotalKronot { get; set; }
        public string SugNayad { get; set; }
        public string StopPoint { get; set; }
    }

    public class Omasim
    {
        public int TrainNumber { get; set; }
        public Station[] Stations { get; set; }
    }

    public class Station
    {
        public int StationNumber { get; set; }
        public float OmesPercent { get; set; }
        public string Shmurim { get; set; }
        public string Time { get; set; }
        public int Platform { get; set; }
    }
}
