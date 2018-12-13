using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("Coupon")]
    public partial class Coupon
    {
        public int Id { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? State { get; set; }
        public int? PromotionId { get; set; }
        public int? NumberUses { get; set; }
        public string Code { get; set; }
        [ForeignKey("PromotionId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Promotion Promotion { get; set; }
    }
}