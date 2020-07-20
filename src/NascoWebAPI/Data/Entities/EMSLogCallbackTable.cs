using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data.Entities
{
    [Table("EMSLogCallbackTable")]
    public class EMSLogCallbackTable
    {
        public int Id { get; set; }
        public string RequestHeader { get; set; }
        public string RequestContent { get; set; }
        public string Response { get; set; }
    }
}
