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
    [Table("TypeOfPack")]
    public partial class TypeOfPack
    {
        public int TypeOfPackID { get; set; }
        public Nullable<double> Width_Product { get; set; }
        public Nullable<double> Height_Product { get; set; }
        public Nullable<double> Length_Product { get; set; }
        public Nullable<double> Packing_Cartons { get; set; }
        public Nullable<double> Packing_Timber { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> TypePackID { get; set; }
    }
}