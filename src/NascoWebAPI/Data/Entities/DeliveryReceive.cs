using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("DeliveryReceive")]

    public partial class DeliveryReceive
    {
        public int Id { get; set; }
        public string DeliveryReceiveCode { get; set; }
        public string DeliveryReceiveName { get; set; }
        public Nullable<int> Index { get; set; }
    }
}
