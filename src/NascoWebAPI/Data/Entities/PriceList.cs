using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("PriceList")]
    public partial class PriceList
    {
        public int PriceListID { get; set; }
        public string PriceListName { get; set; }
        public string PriceListCode { get; set; }
        public Nullable<double> PriceListFuel { get; set; }
        public Nullable<int> POID { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<double> PriceListFuelInternal { get; set; }
        public Nullable<bool> IsApply { get; set; }
        public int? PriceListTypeId { get; set; }
    }
}
