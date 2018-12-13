using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("TypeReason")]
    public partial class TypeReason
    {
        public int TypeReasonID { get; set; }
        public string TypeReasonName { get; set; }
        public string Code { get; set; }
        public bool IsText { get; set; }
        public bool IsDeny { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDelivery { get; set; }
    }
}
