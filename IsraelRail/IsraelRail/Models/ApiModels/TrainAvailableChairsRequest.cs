using System;

namespace IsraelRail.Models.ApiModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class TrainAvailableChairsRequest
    {
        public TrainAvailableChairsRequestItem[] lstTrainAvailableChairsQuery { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class TrainAvailableChairsRequestItem
    {
        public int trainNumber { get; set; }
        public DateTime trainDate { get; set; }
        public int fromStation { get; set; }
        public int destStation { get; set; }
    }
}
