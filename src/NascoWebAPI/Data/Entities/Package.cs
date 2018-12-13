using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("Package")]
    public partial class Package
    {
        public int PackageID { get; set; }
        public string PackageCode { get; set; }
        public int? PostOfficeID { get; set; }
        public Nullable<bool> Packed { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? BKInternalID { get; set; }
        public int? State { get; set; }
        public double? Long { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double? Mass { get; set; }
        public int? TotalLading { get; set; }
        public int? TotalNumber { get; set; }
        public double? TotalWeight { get; set; }
        public string LadingIDs { get; set; }
        public string Note { get; set; }
        public string BKCode { get; set; }
        public string BKConfirmCode { get; set; }
        public int? Status { get; set; }
        public int? PoCurrent { get; set; }
        public int? POTo { get; set; }
        public int? TotalLadingReceived { get; set; }
        public double? TotalWeightToPrice { get; set; }
        public string PackageOfLadingIds { get; set; }
        public int? TotalNumberReceived { get; set; }
        public string Seal { get; set; }
        public string LadingReceivedIds { get; set; }
        public int? FlightId { get; set; }
    }
}