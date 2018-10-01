using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class FlightModel
    {
        public int Id { get; set; }
        public double WeightConfirm { get; set; }
        public DateTime? RealDateTime { get; set; }
        public DateTime? ReceivedRealDateTime { get; set; }
        public int? ModifiedBy { get; set; }
        public int? MAWBId { get; set; }
        public int? POTo { get; set; }
        public string Note { get; set; }
        public int[] BKInternalIds { get; set; }
        public FlightModel() { }
    }
}
