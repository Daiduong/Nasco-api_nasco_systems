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
    [Table("PriceDetail")]
    public partial class PriceDetail
    {
        public int Id { get; set; }
        public Nullable<int> PriceListId { get; set; }
        public string PDCode { get; set; }
        public Nullable<int> AreaIdFrom { get; set; }
        public Nullable<int> AreaIdTo { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> SWId { get; set; }
        public Nullable<double> SB_SB { get; set; }
        public Nullable<double> VP_VP { get; set; }
        public Nullable<double> VP_DC { get; set; }
        public Nullable<double> DC_VP { get; set; }
        public Nullable<double> DC_DC { get; set; }
        public Nullable<double> AON01From { get; set; }
        public Nullable<double> AON02From { get; set; }
        public Nullable<double> AON03From { get; set; }
        public Nullable<double> AON04From { get; set; }
        public Nullable<double> AON01To { get; set; }
        public Nullable<double> AON02To { get; set; }
        public Nullable<double> AON03To { get; set; }
        public Nullable<double> AON04To { get; set; }
        public Nullable<double> AON00From { get; set; }
        public Nullable<double> AON00To { get; set; }
    }
}
