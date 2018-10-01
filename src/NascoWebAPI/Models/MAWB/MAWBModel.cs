using NascoWebAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class MAWBModel
    {

        public int Id { get; set; }
        public string MAWBCode { get; set; }
        public Nullable<int> AirlineId { get; set; }
        public string AirlineName { get; set; }
        public Nullable<System.DateTime> ExpectedDateTime { get; set; }
        public Nullable<System.DateTime> RealDateTime { get; set; }
        public Nullable<int> MaxWeight { get; set; }
        public Nullable<bool> IsNew { get; set; }
        public Nullable<bool> IsFlew { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string StatusMAWB { get; set; }
        public string FlightNumber { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int? PoTo { get; set; }
        public int? PoFrom { get; set; }
        public string ReasonContent { get; set; }
        public string Note { get; set; }
        public MAWBModel() { }
        public MAWBModel(MAWB obj)
        {
            if (obj != null)
            {
                this.Id = obj.Id;
                this.MAWBCode = obj.MAWBCode;
                this.ExpectedDateTime = obj.ExpectedDateTime;
                this.RealDateTime = obj.RealDateTime;
                this.MaxWeight = obj.MaxWeight;
                this.IsNew = obj.IsNew;
                this.CreatedDate = obj.CreatedDate;
                this.AirlineId = obj.AirlineId;
                this.StatusMAWB = "Chưa bay";
                this.IsFlew = false;
                if (ExpectedDateTime.HasValue)
                {
                    if (DateTime.Now.Subtract(this.ExpectedDateTime.Value).TotalSeconds >= 0)
                    {
                        this.StatusMAWB = "Đã bay";
                        this.IsFlew = true;
                    }
                }
                this.RealDateTime = obj.RealDateTime;
                this.FlightNumber = obj.FlightNumber;
                this.PoTo = obj.PoTo; this.PoFrom = obj.PoFrom;
            }
        }
    }
}
