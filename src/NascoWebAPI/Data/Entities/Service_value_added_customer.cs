using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("Service_value_added_customer")]
    public partial class Service_value_added_customer
    {
        public int ID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> ServiceID { get; set; }
        public Nullable<double> Money { get; set; }
        public string Description { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> PriceListID { get; set; }
        public Nullable<int> FormulaID { get; set; }
    }
}
