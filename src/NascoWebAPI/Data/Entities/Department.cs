using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("Deparment")]
    public partial class Deparment
    {
        public int DeparmentID { get; set; }
        public string DeparmentCode { get; set; }
        public Nullable<int> PostOfficeID { get; set; }
        public string DeparmentName { get; set; }
        public string DeparmentDescription { get; set; }
        public string Permition { get; set; }
        public Nullable<int> State { get; set; }
    }
}
