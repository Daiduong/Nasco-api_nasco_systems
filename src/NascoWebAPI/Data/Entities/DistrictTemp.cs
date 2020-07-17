using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("DistrictTemp")]
    public partial class DistrictTemp
    {
        public int Id { get; set; }
        public int? EMSDistrictId { get; set; }
        public string EMSDistrictCode { get; set; }
        public int? EMSProvinceId { get; set; }
        public string EMSProvinceCode { get; set; }
        public int? DistrictId { get; set; }
        public string Districtname { get; set; }
        public string DistrictCode { get; set; }
        public int? ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceCode { get; set; }

    }
}