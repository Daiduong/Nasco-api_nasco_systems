using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("Price")]
    public partial class Price
    {
        public int Id { get; set; }
        public Nullable<double> WeightMax { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> PostOfficeId { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> IsLast { get; set; }
        public Nullable<int> TypePrice { get; set; }
        public Nullable<int> Formula { get; set; }
        public Nullable<int> PriceListID { get; set; }
        public string ColumnName { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<bool> IsFly { get; set; }
        public Nullable<int> TransportID { get; set; }
        public Nullable<bool> IsPlus { get; set; }
    }
}
