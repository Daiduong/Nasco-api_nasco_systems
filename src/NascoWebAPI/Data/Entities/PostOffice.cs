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
        public int? State { get; set; }
        public int? LocationId { get; set; }
        public int? LocationDistrictId { get; set; }
        public Nullable<double> Lat { get; set; }
        public Nullable<double> Lng { get; set; }
        public int? Position { get; set; }
        public int? LocationAreaID { get; set; }
        public Nullable<bool> IsCenter { get; set; }
        public int? POCenterID { get; set; }
        public bool? IsFrom { get; set; }
        public bool? IsTo { get; set; }
        public int? ParentId { get; set; }
        public bool? IsPartner { get; set; }
        public int? Level { get; set; }
        public int? PostOfficeTypeId { get; set; }
        public int? SetsId { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? Order { get; set; }
        public string Note { get; set; }
        public bool? IsInternal { get; set; }
    }
}
