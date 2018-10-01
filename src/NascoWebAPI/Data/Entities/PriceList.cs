using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("PriceListCustomer")]
    public partial class PriceListCustomer
    {
        public int Id { get; set; }
        public Nullable<int> PriceListId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<double> PriceListPercent { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> State { get; set; }
        public string Note { get; set; }
        public int? DiscountTypeId { get; set; }
    }
}
