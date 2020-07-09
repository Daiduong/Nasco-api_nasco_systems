using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models.Response
{
    public class ReportAccumulateResponseModel
    {
        
        public Int64? Id { get; set; }
        public string Code { get; set; }
        public double? Point { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public string FlightCode { get; set; }
        public double? Weight { get; set; }
        public string DeliveryReceiveCode { get; set; }
        public double? Number { get; set; }
        public double? COD { get; set; }
        public string StatusName { get; set; }
        public double? PriceVAT { get; set; }
        public double? WeightToPrice { get; set; }
        public int? TotalCount { get; set; }
        public string TypePayName { get; set; }
    }
}
