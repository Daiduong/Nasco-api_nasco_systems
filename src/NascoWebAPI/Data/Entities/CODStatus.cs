using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("CODStatus")]
    public partial class CODStatus
    {
        public int CODStatusID { get; set; }
        public string CODStatusName { get; set; }
    }
}
