using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data.Entities
{
    public partial class Proc_GetAllUnitGroup_Result
    {
        public int Id { get; set; }
        public string UnitGroupsCode { get; set; }
        public string UnitGroupsName { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> UnitCallId { get; set; }
        public Nullable<System.DateTime> CreatedWhen { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<bool> IsEnabled { get; set; }
        public string UnitCallName { get; set; }
        public string UnitCallCode { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
    }
}
