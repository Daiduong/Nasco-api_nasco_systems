using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class CalculatePriceModel
    {
        public double Amount { get; set; }
        public double PriceMain { get; set; }
        public double PPXDPrice { get; set; }
        public double THBBPrice { get; set; }
        public double BPPrice { get; set; }
        public double CODPrice { get; set; }
        public double InsuredPrice { get; set; }
        public double PackPrice { get; set; }
        public double PriceOther { get; set; }
        public double VAT { get; set; }
        public double KDPrice { get; set; }
        public double WeightToPrice { get; set; }
        public double TotalDVGT { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string StructureName { get; set; }
        public double DiscountAmount { get; set; }
        public double GrandTotal { get; set; }
        public string Message { get; set; }
        public CalculatePriceModel()
        {

        }
        public CalculatePriceModel(ComputedPriceModel computedPriceModel)
        {
            Amount = computedPriceModel.TotalCharge;
            TotalDVGT = computedPriceModel.ChargeAddition;
            PPXDPrice = computedPriceModel.ChargeFuel;
            PriceMain = computedPriceModel.ChargeMain;
            VAT = computedPriceModel.ChargeVAT;
            DiscountAmount = computedPriceModel.DiscountAmount;
            GrandTotal = computedPriceModel.GrandTotal;
            PriceOther = computedPriceModel.Surcharge;
        }
    }
    public class ComputedPriceModel
    {
        public double GrandTotal
        {
            get
            {
                return Math.Round(TotalCharge - DiscountAmount);
            }
        }
        public double TotalCharge
        {
            get
            {
                return Math.Round(ChargeMain + ChargeFuel + ChargeAddition + Surcharge + ChargeVAT);
            }
        }
        public double ChargeMain { get; set; }
        public double ChargeFuel { get; set; }
        public double Surcharge { get; set; }
        public double ChargeAddition
        {
            get
            {
                return this.ServiceOthers.Sum(x => x.Charge);
            }
        }
        public double ChargeVAT
        {
            get
            {
                return (this.ChargeMain + this.ChargeFuel + this.ChargeAddition + this.Surcharge) * 0.1;
            }
        }
        public double DiscountAmount { get; set; }
        public List<ServiceOtherModel> ServiceOthers { get; set; }
        public ComputedPriceModel()
        {
            this.ServiceOthers = new List<ServiceOtherModel>();
        }
    }
    public class ServiceOtherModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public double Charge { get; set; } = 0;
        public ServiceOtherModel() { }
        public ServiceOtherModel(int id, string code, double charge)
        {
            Code = code;
        }
    }
}
