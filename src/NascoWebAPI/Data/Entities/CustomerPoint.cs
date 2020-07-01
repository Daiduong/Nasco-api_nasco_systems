using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data.Entities
{
    [Table("CustomerPoint")]
    public class CustomerPoint
    {
        public int Id { get; set; }
        public int? CreatedBy { get; set; }
        public System.DateTime? CreatedWhen { get; set; }
        public bool IsEnabled { get; set; }
        public int? CustomerId { get; set; }
        public double AllPoint { get; set; }
        public double currentpoint { get; set; }
        public int? RankId { get; set; }
    }
}
