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
    [Table("TimeLine")]
    public partial class TimeLine
    {
        public int Id { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? State { get; set; }
        public int? ProcessId { get; set; }
        public int? ServiceId { get; set; }
        public int? CityFromId { get; set; }
        public int? CityToId { get; set; }
        public int? POCurrentId { get; set; }
        public int? ProcessNextId { get; set; }
        public Nullable<bool> IsFromOrTo { get; set; }
        public string Code { get; set; }
        public int? OrderByService { get; set; }
    }
}