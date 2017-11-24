using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("GroupService")]
    public partial class GroupService
    {
        public int Id { get; set; }
        public string GSCode { get; set; }
        public string GSName { get; set; }
    }
}
