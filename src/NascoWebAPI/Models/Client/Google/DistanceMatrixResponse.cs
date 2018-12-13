using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class DistanceMatrixResponse
    {
        public string Status { get; set; }
        public string Error_message { get; set; }
        public string[] Origin_addresses { get; set; }
        public string[] Destination_addresses { get; set; }
        public DistanceMatrixResult[] Rows { get; set; }
    }
}
