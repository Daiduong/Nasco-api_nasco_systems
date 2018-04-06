using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class PriceRepository : Repository<Price>, IPriceRepository
    {
        public PriceRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task<CalculatePriceModel> CalculatePrice(LadingViewModel model)
        {
            try
            {
                var message = "";
                var lading = new Lading() { };
                lading.SenderId = model.SenderId;
                lading.Insured = model.Insured;
                lading.COD = model.COD;
                lading.Number = model.Number;
                #region Giá
                var locationRepository = new LocationRepository(_context);
                var postOfficeRepository = new PostOfficeRepository(_context);
                var location = await locationRepository.GetFirstAsync(o => o.LocationId == int.Parse(model.CitySendId.ToString()));
                var postOffice = await postOfficeRepository.GetFirstAsync(o => o.PostOfficeID == (int)location.PostOfficeId);
                lading.InsuredPercent = model.InsuredPercent;
                lading.KDNumber = model.KDNumber;
                PriceList priceList;
                if ((model.PriceListId ?? 0) > 0 && _context.PriceLists.Any(r => r.PriceListID == model.PriceListId.Value))
                {
                    priceList = _context.PriceLists.Single(r => r.PriceListID == model.PriceListId.Value);
                }
                else
                {
                    priceList = _context.PriceLists.FirstOrDefault(r => (r.IsApply ?? false));
                }
                double weightD = 0;
                weightD = model.Weight ?? 0;
                if (model.Weight < model.Mass) weightD = model.Mass ?? 0;
                //cu?c chính
                if ((priceList.PriceListTypeId ?? 0) == 2)
                {
                    if (model.KDNumber.HasValue && model.Insured.HasValue && model.KDNumber.Value > 0 && model.Insured.Value > 0
                        && model.RDFrom.HasValue && model.StructureID.HasValue)
                    {
                        lading.PriceMain = this.GetPriceHightValue(model.KDNumber ?? 0, model.Insured ?? 0, model.RDFrom ?? 0, model.StructureID ?? 0);
                    }
                }
                else
                {
                    if (!(model.IsPriceMain ?? false))
                    {
                        lading.PriceMain = this.GetPrice(weightD, model.ServiceId ?? 0, priceList.PriceListID, model.CitySendId.Value, model.CityRecipientId ?? 0, model.RDFrom ?? 0);
                    }
                    else
                    {
                        lading.PriceMain = model.PriceMain;
                    }
                }

                if ((model.IsGlobal ?? false))
                {
                    lading.PPXDPercent = (lading.PriceMain * (priceList.PriceListFuelInternal ?? 0)) / 100;
                }
                else
                {
                    lading.PPXDPercent = (lading.PriceMain * (priceList.PriceListFuel ?? 0)) / 100;
                }
                //dich vụ gia tăng
                double totalDVGT = 0;
                if (model.AnotherServiceId != null)
                {
                    foreach (var otherId in model.AnotherServiceIds)
                    {
                        var otherRepository = new Repository<BB_View_Lading_Service>(_context);
                        var other = await otherRepository.GetFirstAsync(o => o.ServiceId == otherId);
                        if (other.ServiceCode.ToUpper() == "DGHH")//CUOC PHI DONG GOI
                        {
                            lading.PackPrice = model.PackPrice;
                            totalDVGT += lading.PackPrice ?? 0;
                        }
                        else if (other.ServiceCode.ToUpper() == "DBND")//CUOC PHI DONG GOI
                        {
                            //lading.PackPrice = Convert.ToDouble(unitOfWork.ServiceValueAddedCustomerRepository._GetPriceDGHH((int)model.PackId, (double)model.Length, (double)model.Width, (double)model.Height)?.Packing_Cartons ?? 0);
                            lading.DBNDPrice = model.DBNDPrice;
                            totalDVGT += lading.DBNDPrice ?? 0;
                        }
                        else
                        {
                            double priceDVGT = 0, priceDVGTAfter = 0;
                            int formula = 0;
                            bool formulaAbsolute = false;// L?y tuy?t d?i , ngo?c l?i tuong d?i %
                            if (other.ServiceCode.ToUpper() == "COD" && model.COD > 0)//tinh COD
                            {
                                if (_context.BB_View_ServiceCODs.Any(r => priceList.PriceListID == r.PriceListID && r.State == (int)StatusSystem.Enable && r.Money > model.COD))
                                {
                                    var serviceCOD = _context.BB_View_ServiceCODs.OrderBy(o => o.Money).First(r => priceList.PriceListID == r.PriceListID && r.State == (int)StatusSystem.Enable && r.Money > model.COD);
                                    priceDVGT = serviceCOD.Percent_COD ?? 0;
                                    formula = serviceCOD.FormulaID ?? 0;
                                }
                            }
                            else if (other.ServiceCode.ToUpper() == "NTCPN")
                            {
                                var service = _context.Services.SingleOrDefault(o => o.ServiceID == (model.ServiceId ?? 0));
                                if (service != null && service.GSId.HasValue)
                                {
                                    var priceSO = _context.PriceServiceOthers.OrderBy(o => o.GroupServiceId).ThenBy(o => o.WeightMax)
                                                .FirstOrDefault(o => o.ServiceOtherCode != null && o.GroupServiceId.Value == service.GSId && o.ServiceOtherCode.Equals("NTCPN") && o.WeightMax > weightD);
                                    if (priceSO != null)
                                    {
                                        priceDVGT = priceSO.Value ?? 0;
                                        formulaAbsolute = priceSO.FormulaAbsolute ?? false;
                                        formula = priceSO.FormulaId ?? 0;
                                    }
                                }
                            }
                            else
                            {
                                var serviceOther = _context.BB_View_ServiceOtherPriceLists.Where(r => r.ServiceID == other.ServiceId && priceList.PriceListID == r.PriceListID && r.State == (int)StatusSystem.Enable).FirstOrDefault();
                                if (serviceOther != null)
                                {
                                    priceDVGT = serviceOther.Money ?? 0;
                                    formula = serviceOther.FormulaID ?? 0;
                                }
                                if (other.ServiceCode.ToUpper() == "BHHH" && model.Insured > 0)
                                {
                                    priceDVGT = 0;
                                    priceDVGTAfter = (lading.Insured ?? 0) * (lading.InsuredPercent ?? 0) / 100;
                                }
                            }
                            if (priceDVGT != 0 && formula > 0)
                            {
                                priceDVGTAfter = this.GetPriceByFormula(lading, priceDVGT, formula, weightD);
                            }
                            if (other.ServiceCode.ToUpper() == "COD")
                            {
                                lading.CODPrice = priceDVGTAfter;
                            }
                            else if (other.ServiceCode.ToUpper() == "BHHH")
                            {
                                lading.InsuredPrice = priceDVGTAfter;
                            }
                            else if (other.ServiceCode.ToUpper() == "KDHH")
                            {
                                if (priceDVGTAfter < 30000) priceDVGTAfter = 30000;
                                lading.KDPrice = priceDVGTAfter;
                            }
                            totalDVGT += priceDVGTAfter;
                        }
                    }
                }
                lading.TotalPriceDVGT = totalDVGT;
                lading.PriceOther = model.PriceOther ?? 0;
                var priceListCustomer = _context.PriceListCustomers.FirstOrDefault(o => o.PriceListId == model.PriceListId.Value && o.CustomerId == model.SenderId && o.State == 0);
                var percent = (priceListCustomer != null) ? (priceListCustomer.PriceListPercent ?? 100) / 100 : 1;
                lading.PPXDPercent *= percent;
                lading.PriceMain *= percent;
                #endregion
                double totalPrice = lading.PPXDPercent + lading.PriceMain + lading.PriceOther + lading.TotalPriceDVGT ?? 0;
                double VATPrice = totalPrice * 10 / 100;
                totalPrice = totalPrice + VATPrice;
                model.Amount = totalPrice;
                model.PriceMain = lading.PriceMain;
                model.TotalPriceDVGT = lading.TotalPriceDVGT;
                model.PPXDPercent = lading.PPXDPercent;
                double discountAmount = 0;
                if (!string.IsNullOrEmpty(model.CouponCode))
                {
                    var couponRepository = new CouponRepository(_context);
                    var discountResult = couponRepository.GetDiscountAmount(model);
                    if (discountResult.Error == 0)
                    {
                        discountAmount = Math.Round((double)discountResult.Data);
                    }
                    else message = discountResult.Message;
                }
                var calculatePrice = new CalculatePriceModel
                {
                    Amount = Math.Round(totalPrice),
                    PriceMain = Math.Round(lading.PriceMain ?? 0),
                    PPXDPrice = Math.Round(lading.PPXDPercent ?? 0),
                    THBBPrice = Math.Round(lading.THBBPrice ?? 0),
                    BPPrice = Math.Round(lading.BPPrice ?? 0),
                    CODPrice = Math.Round(lading.CODPrice ?? 0),
                    InsuredPrice = Math.Round(lading.InsuredPrice ?? 0),
                    PackPrice = Math.Round(lading.PackPrice ?? 0),
                    PriceOther = Math.Round(lading.PriceOther ?? 0),
                    VAT = Math.Round(VATPrice, 0),
                    KDPrice = Math.Round(lading.KDPrice ?? 0),
                    WeightToPrice = weightD,
                    TotalDVGT = Math.Round(lading.TotalPriceDVGT ?? 0),
                    ExpectedDate = null,
                    StructureName = (lading.Weight ?? 0) > 0.5 ? "Hàng Hóa" : "Thư Từ",
                    GrandTotal = Math.Round(totalPrice) - discountAmount,
                    DiscountAmount = discountAmount,
                    Message = message
                };
                return calculatePrice;
            }
            catch (Exception ex)
            {
                return new CalculatePriceModel() { Message = ex.Message };
            }
        }

        public double GetPrice(double weight, int serviceId = 0, int priceListId = 0, int state_from = 0, int state_to = 0, int receive_delivery = 0)
        {
            double priceDefaul = 0;
            if (!_context.PriceLists.Any(o => o.PriceListID == priceListId))
            {
                priceListId = _context.PriceLists.First(r => r.IsApply == true).PriceListID;
            }
            var locationFrom = _context.Locations.FirstOrDefault(o => o.LocationId == state_from);
            var locationTo = _context.Locations.FirstOrDefault(o => o.LocationId == state_to);
            if (locationFrom == null || (locationFrom.Location_area_Id ?? 0) == 0
                || locationTo == null || (locationTo.Location_area_Id ?? 0) == 0)
            { }
            else if (_context.SetupServiceAreas
                                .Any(o => o.AreaId == locationFrom.Location_area_Id && o.ServiceId.Value == serviceId) &&
                      _context.SetupServiceAreas
                                .Any(o => o.AreaId == locationTo.Location_area_Id && o.ServiceId.Value == serviceId) &&
                      _context.DeliveryReceives.Any(o => o.Id == receive_delivery && o.DeliveryReceiveCode != null && o.DeliveryReceiveCode != string.Empty))
            {
                var areaFromId = _context.SetupServiceAreas
                                .FirstOrDefault(o => o.AreaId == locationFrom.Location_area_Id && o.ServiceId.Value == serviceId).AreaIdFrom;
                var areaToId = _context.SetupServiceAreas
                               .FirstOrDefault(o => o.AreaId == locationTo.Location_area_Id && o.ServiceId.Value == serviceId).AreaIdTo;

                var aOFromId = (_context.SetupAddOns.FirstOrDefault(o => o.LocationId == locationFrom.LocationId && o.ServiceId.Value == serviceId) ?? new SetupAddOn()).AOIdFrom;
                var aOToId = (_context.SetupAddOns.FirstOrDefault(o => o.LocationId == locationTo.LocationId && o.ServiceId.Value == serviceId) ?? new SetupAddOn()).AOIdTo;


                var priceListDetails = _context.PriceDetails.Where(o => o.AreaIdFrom.Value == areaFromId && o.AreaIdTo.Value == areaToId && o.ServiceId.Value == serviceId && o.SWId.HasValue && o.PriceListId.Value == priceListId);
                if (priceListDetails.Count() > 0)
                {
                    var weightIds = priceListDetails.Select(o => o.SWId);
                    var setupWeights = _context.SetupWeights.Where(o => weightIds.Contains(o.Id) && o.FormulaId.HasValue && o.SWFrom.HasValue && o.SWTo.HasValue);
                    if (setupWeights.Count() > 0)
                    {
                        var weightTemp = weight;
                        var aOFromCode = aOFromId != null && _context.AddOns.Any(o => o.Id == aOFromId.Value) ? _context.AddOns.Single(o => o.Id == aOFromId.Value).AOCode + "From" : string.Empty;
                        var aOToCode = aOToId != null && _context.AddOns.Any(o => o.Id == aOToId.Value) ? _context.AddOns.Single(o => o.Id == aOToId.Value).AOCode + "To" : string.Empty;
                        var deliveryCode = _context.DeliveryReceives.Single(o => o.Id == receive_delivery && o.DeliveryReceiveCode != null && o.DeliveryReceiveCode != string.Empty).DeliveryReceiveCode.Replace("-", "_");
                        while (weightTemp >= 0 && setupWeights.Count(o => o.SWFrom.Value <= weightTemp && o.SWTo.Value > weightTemp) > 0)
                        {
                            var setupWeight = setupWeights.OrderBy(o => o.SWTo).ThenByDescending(o => o.SWFrom).First(o => o.SWFrom.Value <= weightTemp && o.SWTo.Value > weightTemp);
                            var priceDetail = priceListDetails.First(o => o.SWId == setupWeight.Id);

                            double priceMain = 0;
                            double priceAOFrom = 0;
                            double priceAOTo = 0;

                            double.TryParse(priceDetail.GetType().GetProperty(deliveryCode).GetValue(priceDetail, null) + "", out priceMain);

                            if (!string.IsNullOrEmpty(aOFromCode))
                                double.TryParse(priceDetail.GetType().GetProperty(aOFromCode).GetValue(priceDetail, null) + "", out priceAOFrom);

                            if (!string.IsNullOrEmpty(aOToCode))
                                double.TryParse(priceDetail.GetType().GetProperty(aOToCode).GetValue(priceDetail, null) + "", out priceAOTo);
                            var totalPrice = (priceMain + priceAOFrom + priceAOTo);
                            switch (setupWeight.FormulaId)
                            {
                                case (int)StatusFormula.Formula1:
                                    priceDefaul += totalPrice;
                                    weightTemp = -1;
                                    break;
                                case (int)StatusFormula.Formula2:
                                    priceDefaul += totalPrice * Math.Round(weightTemp + 0.00000001, 7);
                                    weightTemp = -1;
                                    break;
                                case (int)StatusFormula.Formula3:
                                    var weightSubtract = weightTemp - setupWeight.SWFrom.Value;
                                    weightSubtract = weightSubtract == 0 ? (setupWeight.SWPlus ?? 0) : weightSubtract;
                                    priceDefaul += totalPrice * Math.Ceiling(weightSubtract / ((setupWeight.SWPlus ?? 0) > 0 ? setupWeight.SWPlus.Value : 1));
                                    weightTemp = setupWeight.SWFrom.Value - 0.00000001;
                                    break;
                            }

                        }
                    }
                }
            }
            return priceDefaul;
        }
        public Dictionary<int, double> GetListPrice(double weight, int customerId = 0, int state_from = 0, int state_to = 0, int receive_delivery = 0)
        {
            var priceDefaults = new Dictionary<int, double>();
            var priceListId = 0;
            if (_context.Customers.Any(o => o.CustomerID == customerId && o.PriceListID.Value > 0))
            {
                priceListId = _context.Customers.First(o => o.CustomerID == customerId && o.PriceListID.Value > 0).PriceListID.Value;
            }
            if (!_context.PriceLists.Any(o => o.PriceListID == priceListId))
            {
                priceListId = _context.PriceLists.First(r => (r.IsApply ?? false) == true).PriceListID;
            }
            var locationFrom = _context.Locations.FirstOrDefault(o => o.LocationId == state_from);
            var locationTo = _context.Locations.FirstOrDefault(o => o.LocationId == state_to);
            if (locationFrom == null || (locationFrom.Location_area_Id ?? 0) == 0
                || locationTo == null || (locationTo.Location_area_Id ?? 0) == 0)
            { }
            else if (_context.SetupServiceAreas
                                .Any(o => o.AreaId == locationFrom.Location_area_Id && o.ServiceId.HasValue) &&
                      _context.SetupServiceAreas
                                .Any(o => o.AreaId == locationTo.Location_area_Id && o.ServiceId.HasValue) &&
                      _context.DeliveryReceives.Any(o => o.Id == receive_delivery && o.DeliveryReceiveCode != null && o.DeliveryReceiveCode != string.Empty))
            {
                var areaFromId = _context.SetupServiceAreas
                                .FirstOrDefault(o => o.AreaId == locationFrom.Location_area_Id && o.ServiceId.HasValue).AreaIdFrom;
                var areaToId = _context.SetupServiceAreas
                               .FirstOrDefault(o => o.AreaId == locationTo.Location_area_Id && o.ServiceId.HasValue).AreaIdTo;

                var aOFromId = (_context.SetupAddOns.FirstOrDefault(o => o.LocationId == locationFrom.LocationId && o.ServiceId.HasValue) ?? new SetupAddOn()).AOIdFrom;
                var aOToId = (_context.SetupAddOns.FirstOrDefault(o => o.LocationId == locationTo.LocationId && o.ServiceId.HasValue) ?? new SetupAddOn()).AOIdTo;


                var priceListDetailWithoutServices = _context.PriceDetails.Where(o => o.AreaIdFrom.Value == areaFromId && o.AreaIdTo.Value == areaToId && o.ServiceId.HasValue && o.SWId.HasValue && o.PriceListId.Value == priceListId);
                var serviceByPriceDetails = priceListDetailWithoutServices.Select(o => o.ServiceId).GroupBy(o => o.GetType()).Select(o => o.First().Value);
                if (priceListDetailWithoutServices.Count() > 0)
                {
                    foreach (var serviceId in serviceByPriceDetails)
                    {
                        double priceDefault = 0;
                        var priceListDetails = priceListDetailWithoutServices.Where(o => o.ServiceId == serviceId);
                        var weightIds = priceListDetails.Select(o => o.SWId);
                        var setupWeights = _context.SetupWeights.Where(o => weightIds.Contains(o.Id) && o.FormulaId.HasValue && o.SWFrom.HasValue && o.SWTo.HasValue);
                        if (setupWeights.Count() > 0)
                        {
                            var weightTemp = weight;
                            var aOFromCode = _context.AddOns.Any(o => o.Id == aOFromId.Value) ? _context.AddOns.Single(o => o.Id == aOFromId.Value).AOCode + "From" : string.Empty;
                            var aOToCode = _context.AddOns.Any(o => o.Id == aOToId.Value) ? _context.AddOns.Single(o => o.Id == aOToId.Value).AOCode + "To" : string.Empty;
                            var deliveryCode = _context.DeliveryReceives.Single(o => o.Id == receive_delivery && o.DeliveryReceiveCode != null && o.DeliveryReceiveCode != string.Empty).DeliveryReceiveCode.Replace("-", "_");
                            while (weightTemp >= 0 && setupWeights.Count(o => o.SWFrom.Value <= weightTemp && o.SWTo.Value > weightTemp) > 0)
                            {
                                var setupWeight = setupWeights.First(o => o.SWFrom.Value <= weightTemp && o.SWTo.Value > weightTemp);
                                var priceDetail = priceListDetails.First(o => o.SWId == setupWeight.Id);

                                double priceMain = 0;
                                double priceAOFrom = 0;
                                double priceAOTo = 0;

                                double.TryParse(priceDetail.GetType().GetProperty(deliveryCode).GetValue(priceDetail, null) + "", out priceMain);

                                if (!string.IsNullOrEmpty(aOToCode))
                                    double.TryParse(priceDetail.GetType().GetProperty(aOFromCode).GetValue(priceDetail, null) + "", out priceAOFrom);

                                if (!string.IsNullOrEmpty(aOToCode))
                                    double.TryParse(priceDetail.GetType().GetProperty(aOToCode).GetValue(priceDetail, null) + "", out priceAOTo);
                                var totalPrice = (priceMain + priceAOFrom + priceAOTo);
                                switch (setupWeight.FormulaId)
                                {
                                    case (int)StatusFormula.Formula1:
                                        priceDefault += totalPrice;
                                        weightTemp = -1;
                                        break;
                                    case (int)StatusFormula.Formula2:
                                        priceDefault += totalPrice * Math.Round(weightTemp + 0.00000001, 7);
                                        weightTemp = -1;
                                        break;
                                    case (int)StatusFormula.Formula3:
                                        var weightSubtract = weightTemp - setupWeight.SWFrom.Value;
                                        priceDefault += totalPrice * Math.Ceiling(weightSubtract / ((setupWeight.SWPlus ?? 0) > 0 ? setupWeight.SWPlus.Value : 1));
                                        weightTemp = setupWeight.SWFrom.Value - 0.00000001;
                                        break;
                                }

                            }
                        }
                        priceDefaults.Add(serviceId, priceDefault);
                    }
                }
            }
            return priceDefaults;
        }
        public double GetPriceByFormula(Lading lading, double price, int formula, double weightD = 0)
        {
            double resultPrice = 0;
            if (price != 0 && formula > 0)
            {
                switch (formula)
                {
                    case 1: //GIÁ CHUẨN
                        resultPrice = price <= 100 ? (lading.PriceMain ?? 0) * price / 100 : price;
                        break;
                    case 2: //NHÂN TRỌNG LƯỢNG
                        resultPrice = (price <= 100 ? (lading.PriceMain ?? 0) * price / 100 : price) * weightD;
                        break;
                    case 4: //NHÂN % KHAI GIÁ
                        resultPrice = (price <= 100 ? (lading.Insured ?? 0) * price / 100 : price);
                        break;
                    case 5: //NHÂN % COD
                        resultPrice = (price <= 100 ? (lading.COD ?? 0) * price / 100 : price);
                        break;
                    case 6: //NHÂN SỐ KIỆN
                        resultPrice = (price <= 100 ? (lading.COD ?? 0) * price / 100 : price) * (lading.Number ?? 0);
                        break;
                    case 7: //GIÁ CHUẨN + PPXD
                        resultPrice = price <= 100 ? ((lading.PriceMain ?? 0) + (lading.PPXDPercent ?? 0)) * price / 100 : price;
                        break;
                    case 8: //NHÂN SỐ LƯỢNG SẢN PHẨM (CÁI)
                        resultPrice = (price <= 100 ? (lading.PriceMain ?? 0) * price / 100 : price) * (lading.KDNumber ?? 0);
                        break;
                }
            }
            return resultPrice;
        }

        public double GetPriceHightValue(int quantity, double price, int rdId, int structureId)
        {
            var priceRange = _context.PriceHighValues.Where(o => o.Quantity.Value <= quantity
                                   && o.PriceRange.Value >= price && o.RDFrom.Value == rdId
                                   && o.StructureId.Value == structureId).Min(o => o.PriceRange);

            var priceHighValues = _context.PriceHighValues.Where(o => o.Quantity.Value <= quantity
                                     && o.PriceRange.Value == priceRange && o.RDFrom.Value == rdId
                                     && o.StructureId.Value == structureId)
                                     .OrderByDescending(o => o.Quantity);
            double priceFree = 0;
            var quantityTemp = quantity;
            while (priceHighValues.Any(o => o.Quantity <= quantityTemp) && quantityTemp > 0)
            {
                var pricehighvalue = priceHighValues.First(o => o.Quantity <= quantityTemp);
                var quantityOfPHV = (pricehighvalue.Quantity ?? 0) - 1;
                priceFree += (quantityTemp - quantityOfPHV) * (pricehighvalue.PriceFee ?? 0);
                quantityTemp = quantityOfPHV;
            }
            return priceFree;
        }
    }

}

