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
    [Table("DistrictTypeMapping")]
    public partial class DistrictTypeMapping
    {
        public int Id { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> State { get; set; }
        public string Code { get; set; }
        public string DistrictTypeMappingName { get; set; }
        public Nullable<int> DistrictTypeFromId { get; set; }
        public Nullable<int> DistrictTypeToId { get; set; }
        public string Description { get; set; }
        public Nullable<int> DistrictTypeId { get; set; }
    }
}