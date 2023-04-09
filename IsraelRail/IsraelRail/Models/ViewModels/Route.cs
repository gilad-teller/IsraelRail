using IsraelRail.Models.ApiModels;
using IsraelRail.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IsraelRail.Models.ViewModels
{
    public class Route
    {
        public int Index { get; set; }
        public bool IsExchange { get; set; }
        public TimeSpan EstimatedTime { get; set; }
        public ICollection<Train> Trains { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Route route)
            {
                return route.Trains.Count == Trains.Count && route.Trains.SequenceEqual(Trains);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 19;
            foreach (Train t in Trains)
            {
                hashCode = hashCode * 31 + t.GetHashCode();
            }
            return hashCode;
        }
    }

    public class Train
    {
        public int TrainNumber { get; set; }
        public bool Accesability { get; set; }
        public bool Direct { get; set; }
        public List<Stop> Stops { get; set; }
        public int CurrentStation { get; set; }
        public string CurrentStationName { get; set; }
        public int NextStation { get; set; }
        public string NextStationName { get; set; }
        public TimeSpan? Delay { get; set; }
        public Stop OrigintStop { get; set; }
        public Stop DestinationStop { get; set; }
        public int AvailableSeats { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Train train)
            {
                return train.TrainNumber == TrainNumber;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TrainNumber);
        }
    }

    public class Stop
    {
        public int Station { get; set; }
        public string StationName { get; set; }
        public IEnumerable<DateTime> StopTime { get; set; }
        public string Platform { get; set; }
        public float? Congestion { get; set; }
        public bool IsCurrent { get; set; }
    }

    public interface IRailRouteBuilder
    {
        Task<IEnumerable<Route>> BuildRoutes(TimetableResult timetable);
    }

    public class RailRoutesBuilder : IRailRouteBuilder
    {
        private readonly IStaticStations _staticStations;

        public RailRoutesBuilder(IStaticStations staticStations)
        {
            _staticStations = staticStations;
        }

        public async Task<IEnumerable<Route>> BuildRoutes(TimetableResult timetable)
        {
            List<Route> routes = new List<Route>();
            int index = 0;
            foreach (Travel travel in timetable.Travels)
            {
                double delayMinitues = travel.Trains[0].TrainPosition != null ? (double)travel.Trains[0].TrainPosition.CalcDiffMinutes : 0.0;
                Route route = new Route()
                {
                    Index = index++,
                    EstimatedTime = TimeSpan.FromMinutes(delayMinitues * -1),
                    IsExchange = travel.Trains.Length > 1,
                    Trains = new List<Train>()
                };
                foreach (ApiModels.Train apiTrain in travel.Trains)
                {
                    int currentStation = apiTrain.TrainPosition != null ? apiTrain.TrainPosition.CurrentLastStation : 0;
                    int nextStation = apiTrain.TrainPosition != null ? apiTrain.TrainPosition.NextStation : 0;
                    Train train = new Train()
                    {
                        TrainNumber = apiTrain.TrainNumber,
                        Accesability = apiTrain.Handicap != 0,
                        Direct = travel.Trains.Length == 1,
                        CurrentStation = currentStation,
                        CurrentStationName = await _staticStations.GetStationName(currentStation),
                        NextStation = nextStation,
                        NextStationName = await _staticStations.GetStationName(nextStation),
                        AvailableSeats = apiTrain.FreeSeats,
                        OrigintStop = new Stop
                        {
                            Station = apiTrain.OrignStation,
                            StationName = await _staticStations.GetStationName(apiTrain.OrignStation),
                            Platform = apiTrain.OriginPlatform.ToString(),
                            StopTime = new List<DateTime> { apiTrain.DepartureTime },
                            IsCurrent = apiTrain.OrignStation == currentStation && nextStation != 0,
                            Congestion = apiTrain.Crowded
                        },
                        DestinationStop = new Stop 
                        {
                            Station = apiTrain.DestinationStation,
                            StationName = await _staticStations.GetStationName(apiTrain.DestinationStation),
                            Platform = apiTrain.DestPlatform.ToString(),
                            StopTime = new List<DateTime> { apiTrain.ArrivalTime },
                            IsCurrent = apiTrain.DestinationStation == currentStation && nextStation != 0,
                            Congestion = apiTrain.RouteStations.FirstOrDefault(x => x.StationId == apiTrain.DestinationStation).Crowded
                        },
                        Stops = new List<Stop>()
                    };
                    if (apiTrain.TrainPosition != null)
                    {
                        train.Delay = TimeSpan.FromMinutes((double)travel.Trains[0].TrainPosition.CalcDiffMinutes * -1);
                    }
                    foreach (RouteStation routeStation in apiTrain.RouteStations)
                    {
                        Stop stop = new Stop
                        {
                            Station = routeStation.StationId,
                            StationName = await _staticStations.GetStationName(routeStation.StationId),
                            Congestion = routeStation.Crowded,
                            Platform = routeStation.Platform.ToString(),
                            StopTime = new List<DateTime> { DateTime.Parse(routeStation.ArrivalTime)  },
                            IsCurrent = routeStation.StationId == currentStation && nextStation == 0,
                        };
                        train.Stops.Add(stop);
                    }
                    route.Trains.Add(train);
                }
                routes.Add(route);
            }
            return routes;
        }
    }
}
