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
    [Table("BB_View_Lading_Service")]

    public partial class BB_View_Lading_Service
    {
        public int Id { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<long> LadingId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public Nullable<int> Type { get; set; }
    }
}
