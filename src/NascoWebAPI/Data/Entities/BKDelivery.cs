using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("BK_Delivery")]
    public partial class BKDelivery
    {
        [Key]
        public int ID_BK_Delivery { get; set; }
        public string Code_BK_Delivery { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Create_date_ye { get; set; }
        public Nullable<int> Create_by { get; set; }
        public Nullable<int> OfficerId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string ListLadingId { get; set; }
        public Nullable<int> TotalLading { get; set; }
        public Nullable<int> TotalLadingDeliveried { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> State { get; set; }
        public int? ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int? OfficerConfirmId { get; set; }
        public int? POCreate { get; set; }
        public int? PostOfficeId { get; set; }
        public int? PODeliveryId { get; set; }
        public string PackageOfLadingIds { get; set; }
        public Nullable<int> TotalNumberDelivery { get; set; }
        public Nullable<int> TotalNumber { get; set; }
        public Nullable<double> TotalWeight { get; set; }
        public Nullable<double> TotalWeightToPrice { get; set; }
    }
}
