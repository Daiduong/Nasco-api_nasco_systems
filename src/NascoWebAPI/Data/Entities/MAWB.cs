//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("MAWB")]
    public partial class MAWB
    {
        public int Id { get; set; }
        public string MAWBCode { get; set; }
        public Nullable<int> AirlineId { get; set; }
        public Nullable<System.DateTime> ExpectedDateTime { get; set; }
        public Nullable<System.DateTime> RealDateTime { get; set; }
        public Nullable<int> MaxWeight { get; set; }
        public Nullable<bool> IsNew { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string FlightNumber { get; set; }
        public Nullable<double> RealWeight { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> PoFrom { get; set; }
        public Nullable<int> PoTo { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<double> WeightConfirm { get; set; }
        public Nullable<System.DateTime> ReceivedRealDateTime { get; set; }
        public string Note { get; set; }
        public string ReasonContent { get; set; }
        [ForeignKey("AirlineId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Airline Airline { get; set; }
        [ForeignKey("PoTo"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public PostOffice PoToObj { get; set; }
        [ForeignKey("PoFrom"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public PostOffice PoFromObj { get; set; }
    }
}