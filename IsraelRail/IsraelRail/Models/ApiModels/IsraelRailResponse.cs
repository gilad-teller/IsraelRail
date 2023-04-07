using Newtonsoft.Json;
using System;

namespace IsraelRail.Models.ApiModels
{
    public abstract class IsraelRailResponse
    {
        public DateTime CreationDate { get; set; }
        public string Version { get; set; }
        public int SuccessStatus { get; set; }
        public int StatusCode { get; set; }
        public string[] ErrorMessages { get; set; }
    }
}
