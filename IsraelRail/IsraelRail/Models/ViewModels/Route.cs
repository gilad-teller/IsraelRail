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
        public string LineNumber { get; set; }
        public bool Midnight { get; set; }
        public bool Accesability { get; set; }
        public bool Direct { get; set; }
        public bool ReservedSeats { get; set; }
        public List<Stop> Stops { get; set; }
        public string CurrentStation { get; set; }
        public string CurrentStationName { get; set; }
        public string NextStation { get; set; }
        public string NextStationName { get; set; }
        public TimeSpan? Delay { get; set; }
        public int Carriages { get; set; }
        public string StopPoint { get; set; }
        public string Equipment { get; set; }
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
        public string Station { get; set; }
        public string StationName { get; set; }
        public IEnumerable<DateTime> StopTime { get; set; }
        public string Platform { get; set; }
        public TimeSpan? Delay { get; set; }
        public float? Congestion { get; set; }
        public bool IsCurrent { get; set; }
    }

    public interface IRailRouteBuilder
    {
        Task<IEnumerable<Route>> BuildRoutes(GetRoutesResponse routesResponse, IEnumerable<TrainAvailableChairsResponse> chairsResponses);
    }

    public class RailRoutesBuilder : IRailRouteBuilder
    {
        private readonly IStaticStations _staticStations;

        public RailRoutesBuilder(IStaticStations staticStations)
        {
            _staticStations = staticStations;
        }

        public async Task<IEnumerable<Route>> BuildRoutes(GetRoutesResponse routesResponse, IEnumerable<TrainAvailableChairsResponse> chairsResponses)
        {
            List<Route> routes = new List<Route>();
            int index = 0;
            foreach (ApiModels.Route r in routesResponse.Data.Routes)
            {
                TimeSpan.TryParseExact(r.EstTime, "g", null, out TimeSpan estimatedTime);
                Route route = new Route()
                {
                    Index = index++,
                    EstimatedTime = estimatedTime,
                    IsExchange = r.IsExchange,
                    Trains = new List<Train>()
                };
                foreach (ApiModels.Train t in r.Train)
                {
                    int.TryParse(t.Trainno, out int trainNumber);
                    Trainposition pos = routesResponse.Data.TrainPositions.FirstOrDefault(x => x.TrainNumber == trainNumber);
                    Omasim omasim = routesResponse.Data.Omasim.FirstOrDefault(x => x.TrainNumber == trainNumber);
                    int currentStation = pos != null ? pos.CurrentStation : 0;
                    int nextStation = pos != null ? pos.NextStation : 0;
                    Delay currentDelay = routesResponse.Data.Delays.FirstOrDefault(x => x.Train == t.Trainno);
                    
                    Train train = new Train()
                    {
                        TrainNumber = trainNumber,
                        LineNumber = t.LineNumber,
                        Midnight = t.Midnight,
                        Accesability = t.Handicap,
                        Direct = t.DirectTrain,
                        ReservedSeats = t.ReservedSeat,
                        Carriages = int.Parse(pos?.TotalKronot ?? "0"),
                        CurrentStation = currentStation.ToString(),
                        CurrentStationName = await _staticStations.GetStation(currentStation.ToString()),
                        NextStation = nextStation.ToString(),
                        NextStationName = await _staticStations.GetStation(nextStation.ToString()),
                        Equipment = pos?.SugNayad,
                        StopPoint = pos?.StopPoint,
                        Delay = Tools.StopDelay(pos, currentDelay),
                        Stops = new List<Stop>()
                    };
                    List<Stop> midwayStops = new List<Stop>();

                    int.TryParse(t.OrignStation, out int originStation);
                    DateTime.TryParseExact(t.DepartureTime, "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime departureTime);
                    Trainposition originPos = routesResponse.Data.TrainPositions.FirstOrDefault(x => x.TrainNumber == trainNumber && x.CurrentStation == originStation);
                    Delay originDelay = routesResponse.Data.Delays.FirstOrDefault(x => x.Train == t.Trainno && x.Station == t.OrignStation);
                    Stop firstStop = new Stop()
                    {
                        Station = t.OrignStation,
                        StationName = await _staticStations.GetStation(t.OrignStation),
                        StopTime = new List<DateTime>()
                        {
                            departureTime
                        },
                        Platform = t.Platform,
                        Congestion = omasim.Stations.FirstOrDefault(x => x.StationNumber == originStation && x.OmesPercent >= 0)?.OmesPercent,
                        Delay = Tools.StopDelay(originPos, originDelay),
                        IsCurrent = originStation == currentStation && nextStation == 0
                    };
                    train.OrigintStop = firstStop;
                    midwayStops.Add(firstStop);

                    foreach (Stopstation st in t.StopStations)
                    {
                        int.TryParse(st.StationId, out int stopStation);
                        DateTime.TryParseExact(st.ArrivalTime, "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime stopArrivalTime);
                        DateTime.TryParseExact(st.DepartureTime, "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime stopDepartureTime);
                        Trainposition stopPos = routesResponse.Data.TrainPositions.FirstOrDefault(x => x.TrainNumber == trainNumber && x.CurrentStation == stopStation);
                        Delay stopDelay = routesResponse.Data.Delays.FirstOrDefault(x => x.Train == t.Trainno && x.Station == st.StationId);
                        Stop stop = new Stop()
                        {
                            Station = st.StationId,
                            StationName = await _staticStations.GetStation(st.StationId),
                            StopTime = new List<DateTime>()
                            {
                                stopArrivalTime,
                                stopDepartureTime
                            },
                            Platform = st.Platform,
                            Congestion = omasim.Stations.FirstOrDefault(x => x.StationNumber == stopStation && x.OmesPercent >= 0)?.OmesPercent,
                            Delay = Tools.StopDelay(stopPos, stopDelay),
                            IsCurrent = stopStation == currentStation && nextStation == 0
                        };
                        midwayStops.Add(stop);
                    }

                    int.TryParse(t.DestinationStation, out int destinationStation);
                    DateTime.TryParseExact(t.ArrivalTime, "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime arrivalTime);
                    Trainposition destPos = routesResponse.Data.TrainPositions.FirstOrDefault(x => x.TrainNumber == trainNumber && x.CurrentStation == destinationStation);
                    Delay destDelay = routesResponse.Data.Delays.FirstOrDefault(x => x.Train == t.Trainno && x.Station == t.DestinationStation);
                    Stop lastStop = new Stop()
                    {
                        Station = t.DestinationStation,
                        StationName = await _staticStations.GetStation(t.DestinationStation),
                        StopTime = new List<DateTime>()
                        {
                            arrivalTime
                        },
                        Platform = t.DestPlatform,
                        Congestion = omasim.Stations.FirstOrDefault(x => x.StationNumber == destinationStation && x.OmesPercent >= 0)?.OmesPercent,
                        Delay = Tools.StopDelay(destPos, destDelay),
                        IsCurrent = destinationStation == currentStation && nextStation == 0
                    };
                    train.DestinationStop = lastStop;
                    midwayStops.Add(lastStop);

                    IEnumerable<Station> omasimUntilOrigin = omasim.Stations.TakeWhile(x => x.StationNumber != originStation);
                    List<Stop> stopsUntilOrigin = new List<Stop>();
                    foreach (Station s in omasimUntilOrigin)
                    {
                        Trainposition stopPos = routesResponse.Data.TrainPositions.FirstOrDefault(x => x.TrainNumber == trainNumber && x.CurrentStation == s.StationNumber);
                        Delay stopDelay = routesResponse.Data.Delays.FirstOrDefault(x => x.Train == t.Trainno && x.Station == s.StationNumber.ToString());
                        string stopTimeStr = $"{train.OrigintStop.StopTime.FirstOrDefault():dd/MM/yyyy} {s.Time}";
                        DateTime.TryParseExact(stopTimeStr, "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out DateTime stopTime);
                        Stop stop = new Stop()
                        {
                            Station = s.StationNumber.ToString(),
                            StationName = await _staticStations.GetStation(s.StationNumber.ToString()),
                            StopTime = new List<DateTime>()
                            {
                                stopTime
                            },
                            Congestion = s.OmesPercent >= 0 ? s.OmesPercent : (float?)null,
                            Delay = Tools.StopDelay(stopPos, stopDelay),
                            IsCurrent = s.StationNumber == currentStation && nextStation == 0
                        };
                        stopsUntilOrigin.Add(stop);
                    }
                    IEnumerable<Station> omasimAfterDestination = omasim.Stations.SkipWhile(x => x.StationNumber != destinationStation).Skip(1);
                    List<Stop> stopsAfterDestination = new List<Stop>();
                    foreach (Station s in omasimAfterDestination)
                    {
                        Trainposition stopPos = routesResponse.Data.TrainPositions.FirstOrDefault(x => x.TrainNumber == trainNumber && x.CurrentStation == s.StationNumber);
                        Delay stopDelay = routesResponse.Data.Delays.FirstOrDefault(x => x.Train == t.Trainno && x.Station == s.StationNumber.ToString());
                        string stopTimeStr = $"{train.DestinationStop.StopTime.FirstOrDefault():dd/MM/yyyy} {s.Time}";
                        DateTime.TryParseExact(stopTimeStr, "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out DateTime stopTime);
                        Stop stop = new Stop()
                        {
                            Station = s.StationNumber.ToString(),
                            StationName = await _staticStations.GetStation(s.StationNumber.ToString()),
                            StopTime = new List<DateTime>()
                            {
                                stopTime
                            },
                            Congestion = s.OmesPercent >= 0 ? s.OmesPercent : (float?)null,
                            Delay = Tools.StopDelay(stopPos, stopDelay),
                            IsCurrent = s.StationNumber == currentStation && nextStation == 0
                        };
                        stopsAfterDestination.Add(stop);
                    }
                    train.Stops.AddRange(stopsUntilOrigin);
                    train.Stops.AddRange(midwayStops);
                    train.Stops.AddRange(stopsAfterDestination);

                    if (chairsResponses != null && chairsResponses.Any(x => x.ListTrainAvailableChairs != null && x.ListTrainAvailableChairs.Length > 0))
                    {
                        TrainAvailableChairsResponse chairsResponse = chairsResponses.FirstOrDefault(x => x.ListTrainAvailableChairs.Any(y => y.TrainNumber == trainNumber && y.TrainDate.Date == departureTime.Date));
                        if (chairsResponse != null)
                        {
                            TrainAvailableChairsResponseItem chairsResponseItem = chairsResponse.ListTrainAvailableChairs.FirstOrDefault(x => x.TrainNumber == trainNumber && x.TrainDate.Date == departureTime.Date);
                            if (chairsResponseItem != null)
                            {
                                train.AvailableSeats = chairsResponseItem.SeatsAvailable;
                            }
                        }
                    }

                    route.Trains.Add(train);
                }
                routes.Add(route);
            }
            return routes;
        }
    }
}
