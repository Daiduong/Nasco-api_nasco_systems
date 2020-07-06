using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data.Entities
{
    public class UnitGroups
    {
        public int Id { get; set; }
        public string UnitGroupsCode { get; set; }
        public string UnitGroupsName { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> UnitCallId { get; set; }
        public Nullable<System.DateTime> CreatedWhen { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<bool> IsEnabled { get; set; }
    }
}
