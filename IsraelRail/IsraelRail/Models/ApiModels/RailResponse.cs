using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsraelRail.Models.ApiModels
{
    public abstract class RailResponse
    {
        public int MessageType { get; set; }
        public string Message { get; set; }
    }
}
