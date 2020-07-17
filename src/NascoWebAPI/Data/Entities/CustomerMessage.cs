using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data.Entities
{
   [Table("CustomerMessage")]
    public class CustomerMessage
    {
        public int Id { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> MarketingMessageId { get; set; }
        public Nullable<DateTime> CreatedWhen { get; set; }
        public Nullable<int> MarketingMessageTypeId { get; set; }
        public Nullable<bool> IsPush { get; set; }
    }
}
