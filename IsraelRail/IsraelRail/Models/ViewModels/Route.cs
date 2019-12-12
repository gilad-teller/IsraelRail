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
        public bool IsExchange { get; set; }
        public TimeSpan EstimatedTime { get; set; }
        public ICollection<Train> Trains { get; set; }
    }

    public class Train
    {
        public int TrainNumber { get; set; }
        public string LineNumber { get; set; }
        public bool Midnight { get; set; }
        public bool Accesability { get; set; }
        public bool Direct { get; set; }
        public bool ReservedSeats { get; set; }
        public ICollection<Stop> Stops { get; set; }
        public E_Station CurrentStation { get; set; }
        public string CurrentStationName { get; set; }
        public E_Station NextStation { get; set; }
        public string NextStationName { get; set; }
        public TimeSpan? Delay { get; set; }
        public int Carriages { get; set; }
        public string StopPoint { get; set; }
        public string Equipment { get; set; }
    }

    public class Stop
    {
        public E_Station Station { get; set; }
        public string StationName { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Departure { get; set; }
        public string Platform { get; set; }
        public TimeSpan? Delay { get; set; }
        public float? Congestion { get; set; }
    }

    public interface IRailRouteBuilder
    {
        IEnumerable<Route> BuildRoutes(GetRoutesResponse routesResponse);
    }

    public class RailRoutesBuilder : IRailRouteBuilder
    {
        private readonly IStaticStations _staticStations;

        public RailRoutesBuilder(IStaticStations staticStations)
        {
            _staticStations = staticStations;
        }

        public IEnumerable<Route> BuildRoutes(GetRoutesResponse routesResponse)
        {
            List<Route> routes = new List<Route>();
            foreach (ApiModels.Route r in routesResponse.Data.Routes)
            {
                TimeSpan.TryParseExact(r.EstTime, "g", null, out TimeSpan estimatedTime);
                Route route = new Route()
                {
                    EstimatedTime = estimatedTime,
                    IsExchange = r.IsExchange,
                    Trains = new List<Train>()
                };
                foreach (ApiModels.Train t in r.Train)
                {
                    int.TryParse(t.Trainno, out int trainNumber);
                    Trainposition pos = routesResponse.Data.TrainPositions.FirstOrDefault(x => x.TrainNumber == trainNumber);
                    Omasim omasim = routesResponse.Data.Omasim.FirstOrDefault(x => x.TrainNumber == trainNumber);
                    E_Station currentStation = pos != null ? (E_Station)pos.CurrentStation : E_Station.None;
                    E_Station nextStation = pos != null ? (E_Station)pos.NextStation : E_Station.None;
                    Train train = new Train()
                    {
                        TrainNumber = trainNumber,
                        LineNumber = t.LineNumber,
                        Midnight = t.Midnight,
                        Accesability = t.Handicap,
                        Direct = t.DirectTrain,
                        ReservedSeats = t.ReservedSeat,
                        Carriages = int.Parse(pos?.TotalKronot ?? "0"),
                        CurrentStation = currentStation,
                        CurrentStationName = _staticStations.GetStation(currentStation),
                        NextStation = nextStation,
                        NextStationName = _staticStations.GetStation(nextStation),
                        Equipment = pos?.SugNayad,
                        StopPoint = pos?.StopPoint,
                        Delay = Tools.DelayFromPosition(pos),
                        Stops = new List<Stop>()
                    };

                    int.TryParse(t.OrignStation, out int originStation);
                    DateTime.TryParseExact(t.DepartureTime, "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime departureTime);
                    Trainposition originPos = routesResponse.Data.TrainPositions.FirstOrDefault(x => x.TrainNumber == trainNumber && x.CurrentStation == originStation);
                    Delay originDelay = routesResponse.Data.Delays.FirstOrDefault(x => x.Train == t.Trainno && x.Station == t.OrignStation);
                    Stop firstStop = new Stop()
                    {
                        Station = (E_Station)originStation,
                        StationName = _staticStations.GetStation((E_Station)originStation),
                        Arrival = null,
                        Departure = departureTime,
                        Platform = t.Platform,
                        Congestion = omasim.Stations.FirstOrDefault(x => x.StationNumber == originStation)?.OmesPercent,
                        Delay = Tools.StopDelay(originPos, originDelay)
                    };
                    train.Stops.Add(firstStop);

                    foreach (Stopstation st in t.StopStations)
                    {
                        int.TryParse(st.StationId, out int stopStation);
                        DateTime.TryParseExact(st.DepartureTime, "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime stopDepartureTime);
                        DateTime.TryParseExact(st.DepartureTime, "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime stopArrivalTime);
                        Trainposition stopPos = routesResponse.Data.TrainPositions.FirstOrDefault(x => x.TrainNumber == trainNumber && x.CurrentStation == stopStation);
                        Delay stopDelay = routesResponse.Data.Delays.FirstOrDefault(x => x.Train == t.Trainno && x.Station == st.StationId);
                        Stop stop = new Stop()
                        {
                            Station = (E_Station)stopStation,
                            StationName = _staticStations.GetStation((E_Station)stopStation),
                            Arrival = stopArrivalTime,
                            Departure = stopDepartureTime,
                            Platform = st.Platform,
                            Congestion = omasim.Stations.FirstOrDefault(x => x.StationNumber == stopStation)?.OmesPercent,
                            Delay = Tools.StopDelay(stopPos, stopDelay)
                        };
                        train.Stops.Add(stop);
                    }

                    int.TryParse(t.DestinationStation, out int destinationStation);
                    DateTime.TryParseExact(t.ArrivalTime, "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime arrivalTime);
                    Trainposition destPos = routesResponse.Data.TrainPositions.FirstOrDefault(x => x.TrainNumber == trainNumber && x.CurrentStation == destinationStation);
                    Delay destDelay = routesResponse.Data.Delays.FirstOrDefault(x => x.Train == t.Trainno && x.Station == t.DestinationStation);
                    Stop lastStop = new Stop()
                    {
                        Station = (E_Station)destinationStation,
                        StationName = _staticStations.GetStation((E_Station)destinationStation),
                        Arrival = arrivalTime,
                        Departure = null,
                        Platform = t.DestPlatform,
                        Congestion = omasim.Stations.FirstOrDefault(x => x.StationNumber == destinationStation)?.OmesPercent,
                        Delay = Tools.StopDelay(destPos, destDelay)
                    };
                    train.Stops.Add(lastStop);
                    route.Trains.Add(train);
                }
                routes.Add(route);
            }
            return routes;
        }
    }
}
