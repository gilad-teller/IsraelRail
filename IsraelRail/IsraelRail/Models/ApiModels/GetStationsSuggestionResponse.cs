namespace IsraelRail.Models.ApiModels
{

    public class GetStationsSuggestionResponse : RailResponse
    {
        public GetStationsSuggestionResponseData Data { get; set; }
    }

    public class GetStationsSuggestionResponseData
    {
        public CustomProperty[] CustomPropertys { get; set; }
    }

    public class CustomProperty
    {
        public string Id { get; set; }
        public string[] Heb { get; set; }
        public string[] Eng { get; set; }
        public string[] Rus { get; set; }
        public string[] Arb { get; set; }
    }

}
