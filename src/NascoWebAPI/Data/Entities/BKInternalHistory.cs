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
    [Table("BK_internalHistory")]
    public partial class BKInternalHistory
    {
        public int Id { get; set; }
        public Nullable<int> BK_internalId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> Status { get; set; }
        public string LadingIds { get; set; }
        public Nullable<int> TotalLading { get; set; }
        public Nullable<double> TotalWeight { get; set; }
        public string Note { get; set; }
    }
}