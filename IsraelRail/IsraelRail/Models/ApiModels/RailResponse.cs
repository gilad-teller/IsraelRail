namespace IsraelRail.Models.ApiModels
{
    public abstract class RailResponse
    {
        public int MessageType { get; set; }
        public string Message { get; set; }
    }
}
