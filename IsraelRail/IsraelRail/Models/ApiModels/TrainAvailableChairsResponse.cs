using System;

namespace IsraelRail.Models.ApiModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class TrainAvailableChairsResponse
    {
        public TrainAvailableChairsResponseItem[] ListTrainAvailableChairs { get; set; }
        public ClsResult clsResult { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class TrainAvailableChairsResponseItem
    {
        public DateTime TrainDate { get; set; }
        public int TrainNumber { get; set; }
        public int SeatsAvailable { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class ClsResult
    {
        public int returnCode { get; set; }
        public string returnDescription { get; set; }
    }
}
