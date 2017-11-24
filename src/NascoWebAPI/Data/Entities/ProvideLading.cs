using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("ProvideLading")]
    public partial class ProvideLading
    {
        public int Id { get; set; }
        public Nullable<int> OfficerProvide { get; set; }
        public Nullable<int> OfficerReceive { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string LadingCode { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> POID { get; set; }
        public string Note { get; set; }
    }
}
