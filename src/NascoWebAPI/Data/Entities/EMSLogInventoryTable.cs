using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data.Entities
{
    [Table("EMSLogInventoryTable")]
    public class EMSLogInventoryTable
    {
        public int Id { get; set; }
        public string RequestUrl { get; set; }
        public string RequestContent { get; set; }
        public string Response { get; set; }
    }
}
