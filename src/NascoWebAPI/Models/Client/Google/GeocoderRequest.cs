using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class GeocoderRequest
    {
        public string Address { get; set; }
        public LatLng Location { get; set; }
        public string PlaceId { get; set; }
        public string Region { get; set; }
        //
        //
    }
}
