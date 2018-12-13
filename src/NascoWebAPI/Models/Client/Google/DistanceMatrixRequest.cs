using NascoWebAPI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class DistanceMatrixRequest
    {
        public string[] Origins { get; set; }
        public string[] Destinations { get; set; }
        public string Mode { get; set; } = GoogleMapHelper.TRAVEL_MODE_DRIVING;
    }
}
