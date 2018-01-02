using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("PostOffice")]
    public partial class PostOffice
    {
        public int PostOfficeID { get; set; }
        public string POName { get; set; }
        public string POCode { get; set; }
        public string POPhone { get; set; }
        public string POEmail { get; set; }
        public string POFaxNumber { get; set; }
        public string POAddress { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> LocationDistrictId { get; set; }
        public Nullable<double> Lat { get; set; }
        public Nullable<double> Lng { get; set; }
        public Nullable<int> Position { get; set; }
        public Nullable<int> LocationAreaID { get; set; }
        public Nullable<bool> IsCenter { get; set; }
        public int? POCenterID { get; set; }
        public bool? IsFrom { get; set; }
        public bool? IsTo { get; set; }
        public int? ParentId { get; set; }
    }
}
