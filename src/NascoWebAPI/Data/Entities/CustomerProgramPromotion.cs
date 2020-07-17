using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data.Entities
{
    public partial class CustomerProgramPromotion
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> CreatedWhen { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<bool> IsEnabled { get; set; }
        public Nullable<int> PromotionId { get; set; }
        public Nullable<double> Value { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string PromotionCode { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsPush { get; set; }
        public Nullable<int> LadingId { get; set; }
    }
}
