﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("LadingStatus")]
    public partial class LadingStatus
    {
        [Key]
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusCustomerName { get; set; }
        public bool? IsHiddenCustomer { get; set; }

    }
}
