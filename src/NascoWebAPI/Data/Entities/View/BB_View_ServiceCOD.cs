
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("BB_View_ServiceCOD")]

    public partial class BB_View_ServiceCOD
    {
        public int ID { get; set; }
        public Nullable<double> Money { get; set; }
        public Nullable<double> Percent_COD { get; set; }
        public Nullable<double> Money_Percent_COD { get; set; }
        public string Description { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> Type_Service { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> PriceListID { get; set; }
        public Nullable<int> FormulaID { get; set; }
        public string PriceListName { get; set; }
        public string FormulaName { get; set; }
    }
}
