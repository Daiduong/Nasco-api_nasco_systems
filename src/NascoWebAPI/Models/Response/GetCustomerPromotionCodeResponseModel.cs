using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models.Response
{
    public class GetCustomerPromotionCodeResponseModel
    {
        public string PromotionCode { get; set; }
        public string CodeOfPromotion { get; set; }
        public double Value { get; set; }
        public string TypeName { get; set; }
        public DateTime? StartDateUseCode { get; set; }
        public DateTime? EndDateUseCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
