using System;

namespace IsraelRail.Models.ApiModels
{

    public class TimetableResponse : IsraelRailResponse
    {
        public TimetableResult Result { get; set; }
    }

    public class TimetableResult
    {
        public int NumOfResultsToShow { get; set; }
        public int StartFromIndex { get; set; }
        public int OnFocusIndex { get; set; }
        public int ClientMessageId { get; set; }
        public bool FreeSeatsError { get; set; }
        public Travel[] Travels { get; set; }
    }

    public class Travel
    {
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int FreeSeats { get; set; }
        public string[] TravelMessages { get; set; }
        public Train[] Trains { get; set; }
    }

    public class Train
    {
        public int TrainNumber { get; set; }
        public int OrignStation { get; set; }
        public int DestinationStation { get; set; }
        public int OriginPlatform { get; set; }
        public int DestPlatform { get; set; }
        public int FreeSeats { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public StopStation[] StopStations { get; set; }
        public int Handicap { get; set; }
        public float Crowded { get; set; }
        public TrainPosition TrainPosition { get; set; }
        public RouteStation[] RouteStations { get; set; }
    }

    public class TrainPosition
    {
        public int CurrentLastStation { get; set; }
        public int NextStation { get; set; }
        public int CalcDiffMinutes { get; set; }
    }

    public class StopStation
    {
        public int StationId { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public int Platform { get; set; }
        public float Crowded { get; set; }
    }

    public class RouteStation
    {
        public int StationId { get; set; }
        public string ArrivalTime { get; set; }
        public float Crowded { get; set; }
        public int Platform { get; set; }
    }

}
