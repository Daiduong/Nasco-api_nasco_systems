using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data.Entities
{
    public partial class ProgramPromotion
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> CreatedWhen { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<bool> IsEnabled { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> PercentMin { get; set; }
        public Nullable<int> PercentMax { get; set; }
        public Nullable<int> PromotionTypeId { get; set; }
        public Nullable<System.DateTime> StartDateUseCode { get; set; }
        public Nullable<System.DateTime> EndDateUseCode { get; set; }
        public string Content { get; set; }
    }
}
