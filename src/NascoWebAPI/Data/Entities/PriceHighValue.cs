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
    [Table("PriceHighValue")]
    public partial class PriceHighValue
    {
        public int Id { get; set; }
        public Nullable<int> StructureId { get; set; }
        public Nullable<int> RDTo { get; set; }
        public Nullable<int> RDFrom { get; set; }
        public Nullable<double> PriceRange { get; set; }
        public Nullable<double> PriceFee { get; set; }
        public Nullable<int> FormulaId { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
}