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
    [Table("PackageOfLading")]
    public partial class PackageOfLading
    {
        public int Id { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<int> State { get; set; }
        public string Code { get; set; }
        public Nullable<int> Order { get; set; }
        public Nullable<double> Long { get; set; }
        public Nullable<double> Weight { get; set; }
        public Nullable<double> Height { get; set; }
        public Nullable<double> Mass { get; set; }
        public Nullable<int> LadingId { get; set; }
        public Nullable<int> POCurrentId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> TotalNumber { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public string RecipientReality { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> BKInternalId { get; set; }
        public Nullable<int> BKDeliveryId { get; set; }
        public Nullable<int> PackageId { get; set; }
        public Nullable<int> FlightId { get; set; }
        public Nullable<bool> Return { get; set; }
        public int? DeliveryBy { get; set; }
        public int? UnitGroupsId { get; set; }
    }
}