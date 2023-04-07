namespace IsraelRail.Models.ApiModels
{
    public class StationsResponse : IsraelRailResponse
    {
        public StationsResult[] Result { get; set; }
    }

    public class StationsResult
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
        public bool Parking { get; set; }
        public bool Handicap { get; set; }
        public Location Location { get; set; }
        public string[] Synonyms { get; set; }
    }

    public class Location
    {
        public float Latitude { get; set; }
        public float Lontitude { get; set; }
    }

}
