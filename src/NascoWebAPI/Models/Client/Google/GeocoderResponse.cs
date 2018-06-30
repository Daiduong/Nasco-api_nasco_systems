using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class GeocoderResponse
    {
        public string Status { get; set; }
        public string Error_message { get; set; }
        public GeocoderResult[] Results { get; set; }
    }
}
