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
    [Table("SetupWeight")]
    public partial class SetupWeight
    {
        public int Id { get; set; }
        public string SWCode { get; set; }
        public Nullable<int> FormulaId { get; set; }
        public Nullable<double> SWPlus { get; set; }
        public Nullable<double> SWFrom { get; set; }
        public Nullable<double> SWTo { get; set; }
    }
}