using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("LadingMapService")]

    public partial class LadingMapService
    {
        public int Id { get; set; }
        public long? LadingId { get; set; }
        public int? ServiceId { get; set; }
        public double? TotalPrice { get; set; }
        public int? State { get; set; }
    }
}
