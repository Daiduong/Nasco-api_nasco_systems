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
    [Table("BB_View_Transport_Service_Joined")]

    public partial class BB_View_Transport_Service_Joined
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> Type { get; set; }
        public string Description { get; set; }
        public Nullable<int> WeightType { get; set; }
        public Nullable<int> PriceType { get; set; }
        public Nullable<bool> WeightThan100 { get; set; }
        public Nullable<bool> OnSiteDelivery { get; set; }
        public Nullable<double> WeightNorms { get; set; }
        public Nullable<int> TransportID { get; set; }
        public Nullable<int> ServiceID { get; set; }
        public string TransportCode { get; set; }
        public string TransportName { get; set; }
        public Nullable<bool> IsGlobal { get; set; }
    }
}
