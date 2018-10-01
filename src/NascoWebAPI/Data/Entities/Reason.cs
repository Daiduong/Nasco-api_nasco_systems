using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("Reason")]
    public class Reason
    {
        public int Id { get; set; }
        public string ReasonName { get; set; }
        public bool? IsOther { get; set; }
        public string TableName { get; set; }
        public int? State { get; set; }
    }
}
