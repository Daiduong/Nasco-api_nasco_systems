using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("Location_Area")]

    public partial class LocationArea
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> RootPostOffice { get; set; }
    }
}
