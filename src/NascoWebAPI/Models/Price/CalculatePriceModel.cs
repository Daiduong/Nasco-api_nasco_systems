using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class CalculatePriceModel
    {
        public double Amount { get; set; }
        public double? PriceMain { get; set; }
        public double? PPXDPrice { get; set; }
        public double? THBBPrice { get; set; }
        public double? BPPrice { get; set; }
        public double? CODPrice { get; set; }
        public double? InsuredPrice { get; set; }
        public double? PackPrice { get; set; }
        public double? PriceOther { get; set; }
        public double? VAT { get; set; }
        public double? KDPrice { get; set; }
        public double? WeightToPrice { get; set; }
        public double? TotalDVGT { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string StructureName { get; set; }
        public double DiscountAmount { get; set; }
        public double GrandTotal { get; set; }
        public string Message { get; set; }
    }
}
