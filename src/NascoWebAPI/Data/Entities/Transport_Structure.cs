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
    [Table("Transport_Structure")]
    public partial class Transport_Structure
    {
        public int Id { get; set; }
        public Nullable<int> TransportID { get; set; }
        public Nullable<int> StructureID { get; set; }
    }
}
