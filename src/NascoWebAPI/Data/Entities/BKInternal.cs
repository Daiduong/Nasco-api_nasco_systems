using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("BK_internal")]
    public partial class BKInternal
    {
        [Key]
        public int ID_BK_internal { get; set; }
        public string Code_BK_internal { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Create_date_ye { get; set; }
        public Nullable<int> Create_by { get; set; }
        public Nullable<int> OfficerId_sender { get; set; }
        public Nullable<int> PostOfficeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string ListLadingId { get; set; }
        public Nullable<int> TotalLading { get; set; }
        public Nullable<int> TotalLadingRecipient { get; set; }
        public string Notes { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> RouteID { get; set; }
        public Nullable<bool> IsCenter { get; set; }
        public Nullable<bool> IsFly { get; set; }
        public Nullable<double> Long { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> Height { get; set; }
        public Nullable<double> WeightExchange { get; set; }
        public Nullable<int> TotalBox { get; set; }
        public Nullable<double> TotalWeight { get; set; }
        public Nullable<int> POCreate { get; set; }
        public Nullable<int> PODeliveryId { get; set; }
        public Nullable<int> TransportTypeId { get; set; }
        public Nullable<int> GroupServiceId { get; set; }
        public Nullable<int> POAirportId { get; set; }
        public Nullable<bool> IsConfirmByOfficer { get; set; }
        public Nullable<int> FlightId { get; set; }
        public Nullable<int> DepartmentConfirmId { get; set; }
        public Nullable<int> JobConfirmId { get; set; }
        public Nullable<int> OfficerConfirmId { get; set; }
        public Nullable<bool> IsAirportToConfirm { get; set; }
        public string PackageOfLadingIds { get; set; }
        public Nullable<int> TotalNumberReceived { get; set; }
        public Nullable<int> TotalLadingRecipientAirport { get; set; }
        public string LadingConfirmIds { get; set; }
        public Nullable<int> TotalLadingConfirm { get; set; }
        public Nullable<double> TotalWeightToPrice { get; set; }
        public string PackageIds { get; set; }
        [ForeignKey("PostOfficeId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public PostOffice PostOfficeTo { get; set; }
    }
}
