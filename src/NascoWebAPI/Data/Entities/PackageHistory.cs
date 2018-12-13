using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("PackageHistory")]
    public partial class PackageHistory
    {
        public int Id { get; set; }
        public int? PackageId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? TotalLading { get; set; }
        public int? TotalNumber { get; set; }
        public string LadingIds { get; set; }
        public int? Status { get; set; }
        public string Note { get; set; }
        public string BKCode { get; set; }
        public double? TotalWeight { get; set; }
        public int? PostOfficeId { get; set; }
    }
}