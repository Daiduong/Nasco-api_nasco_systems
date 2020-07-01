using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("Rank")]
    public partial class Rank
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string Code { get; set; }
        public double MinPoint { get; set; }
        public double MaxPoint { get; set; }
        public DateTime? CreatedWhen { get; set; }
        public int? Createdby { get; set; }
        public bool? IsEnabled { get; set; }
    }
}
