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
        public Nullable<long> LadingId { get; set; }
        public Nullable<int> ServiceId { get; set; }
    }
}
