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
    [Table("SetupServiceArea")]
    public partial class SetupServiceArea
    {
        public int Id { get; set; }
        public string SSACode { get; set; }
        public Nullable<int> AreaId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> AreaIdFrom { get; set; }
        public Nullable<int> AreaIdTo { get; set; }
    }
}
