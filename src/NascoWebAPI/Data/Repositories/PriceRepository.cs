using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static NascoWebAPI.Helper.Common.Constants;

namespace NascoWebAPI.Data
{
    public class PriceRepository : Repository<Price>, IPriceRepository
    {
        public PriceRepository(ApplicationDbContext context) : base(context)
        {

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
                        resultPrice = Math.Abs(price) <= 100 ? (lading.PriceMain ?? 0) * price / 100 : price;
                        break;
                    case 2: //NHÂN TRỌNG LƯỢNG
                        resultPrice = (Math.Abs(price) <= 100 ? (lading.PriceMain ?? 0) * price / 100 : price) * weightD;
                        break;
                    case 4: //NHÂN % KHAI GIÁ
                        resultPrice = (Math.Abs(price) <= 100 ? (lading.Insured ?? 0) * price / 100 : price);
                        break;
                    case 5: //NHÂN % COD
                        resultPrice = (Math.Abs(price) <= 100 ? (lading.COD ?? 0) * price / 100 : price);
                        break;
                    case 6: //NHÂN SỐ KIỆN
                        resultPrice = (Math.Abs(price) <= 100 ? (lading.COD ?? 0) * price / 100 : price) * (lading.Number ?? 0);
                        break;
                    case 7: //GIÁ CHUẨN + PPXD
                        resultPrice = Math.Abs(price) <= 100 ? ((lading.PriceMain ?? 0) + (lading.PPXDPercent ?? 0)) * price / 100 : price;
                        break;
                    case 8: //NHÂN SỐ LƯỢNG SẢN PHẨM (CÁI)
                        resultPrice = (Math.Abs(price) <= 100 ? (lading.PriceMain ?? 0) * price / 100 : price) * (lading.KDNumber ?? 0);
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
        public async Task<double> Computed(double weight, int serviceId, int priceListId, int cityFromId, int cityToId, int districtFromId, int districtToId, int deliveryReceiveId, int? customerId = null)
        {
            double priceDefaul = 0;
            if (!(await _context.PriceLists.AnyAsync(o => o.PriceListID == priceListId)))
            {
                priceListId = (await _context.PriceLists.FirstAsync(r => r.IsApply == true)).PriceListID;
            }
            var locationFrom = await _context.Locations.FirstOrDefaultAsync(o => o.LocationId == cityFromId);
            var locationTo = await _context.Locations.FirstOrDefaultAsync(o => o.LocationId == cityToId);
            if (locationFrom == null || (locationFrom.Location_area_Id ?? 0) == 0
                || locationTo == null || (locationTo.Location_area_Id ?? 0) == 0)
            { }
            else if ((await _context.SetupServiceAreas
                                .AnyAsync(o => o.AreaId == locationFrom.Location_area_Id && o.ServiceId.Value == serviceId)) &&
                      (await _context.SetupServiceAreas
                                .AnyAsync(o => o.AreaId == locationTo.Location_area_Id && o.ServiceId.Value == serviceId)) &&
                     (await _context.DeliveryReceives.AnyAsync(o => o.Id == deliveryReceiveId && o.DeliveryReceiveCode != null && o.DeliveryReceiveCode != string.Empty)))
            {
                var areaFromId = (await _context.SetupServiceAreas
                                .FirstOrDefaultAsync(o => o.AreaId == locationFrom.Location_area_Id && o.ServiceId.Value == serviceId)).AreaIdFrom;
                var areaToId = (await _context.SetupServiceAreas
                               .FirstOrDefaultAsync(o => o.AreaId == locationTo.Location_area_Id && o.ServiceId.Value == serviceId)).AreaIdTo;

                var aOFromId = (await _context.SetupAddOns.FirstOrDefaultAsync(o => o.LocationId == locationFrom.LocationId && o.ServiceId.Value == serviceId))?.AOIdFrom;
                var aOToId = (await _context.SetupAddOns.FirstOrDefaultAsync(o => o.LocationId == locationTo.LocationId && o.ServiceId.Value == serviceId))?.AOIdTo;

                if (aOFromId.HasValue && aOToId.HasValue)
                {
                    var priceListDetails = _context.PriceDetails.Where(o => o.AreaIdFrom.Value == areaFromId && o.AreaIdTo.Value == areaToId && o.ServiceId.Value == serviceId && o.SWId.HasValue && o.PriceListId.Value == priceListId);
                    if (priceListDetails.Count() > 0)
                    {
                        var weightIds = priceListDetails.Select(o => o.SWId);
                        var setupWeights = await _context.SetupWeights.Where(o => weightIds.Contains(o.Id) && o.FormulaId.HasValue && o.SWFrom.HasValue && o.SWTo.HasValue).ToListAsync();
                        if (setupWeights.Count() > 0)
                        {
                            var weightTemp = weight;
                            var aOFromCode = await _context.AddOns.AnyAsync(o => o.Id == aOFromId.Value) ? _context.AddOns.Single(o => o.Id == aOFromId.Value).AOCode + "From" : string.Empty;
                            var aOToCode = await _context.AddOns.AnyAsync(o => o.Id == aOToId.Value) ? _context.AddOns.Single(o => o.Id == aOToId.Value).AOCode + "To" : string.Empty;
                            var deliveryCode = (await _context.DeliveryReceives.SingleAsync(o => o.Id == deliveryReceiveId && o.DeliveryReceiveCode != null && o.DeliveryReceiveCode != string.Empty)).DeliveryReceiveCode.Replace("-", "_");
                            int? districtTypeId = null;
                            //
                            if (cityFromId == cityToId)
                            {
                                var districtTypeFromId = _context.Locations.FirstOrDefault(o => o.LocationId == districtFromId)?.DistrictTypeId;
                                var districtTypeToId = _context.Locations.FirstOrDefault(o => o.LocationId == districtToId)?.DistrictTypeId;
                                districtTypeId = _context.DistrictTypeMappings.FirstOrDefault(o => o.DistrictTypeFromId == districtTypeFromId && o.DistrictTypeToId == districtTypeToId)?.DistrictTypeId;
                            }

                            var priceDetail = new PriceDetail();
                            while (weightTemp >= 0 && setupWeights.Count(o => o.SWFrom.Value <= weightTemp && o.SWTo.Value > weightTemp) > 0)
                            {
                                var setupWeight = setupWeights.OrderBy(o => o.SWTo).ThenByDescending(o => o.SWFrom).First(o => o.SWFrom.Value <= weightTemp && o.SWTo.Value > weightTemp);
                                priceDetail = priceListDetails.First(o => o.SWId == setupWeight.Id);

                                double priceAOFrom = 0;
                                double priceAOTo = 0;
                                double priceDTAddOn = 0;
                                double.TryParse(priceDetail.GetType().GetProperty(deliveryCode).GetValue(priceDetail, null) + "", out double priceMain);

                                if (!string.IsNullOrEmpty(aOFromCode))
                                    double.TryParse(priceDetail.GetType().GetProperty(aOFromCode).GetValue(priceDetail, null) + "", out priceAOFrom);

                                if (!string.IsNullOrEmpty(aOToCode))
                                    double.TryParse(priceDetail.GetType().GetProperty(aOToCode).GetValue(priceDetail, null) + "", out priceAOTo);
                                if (districtTypeId.HasValue)
                                    priceDTAddOn = (_context.PriceDetailDistrictTypeAddOns.FirstOrDefault(o => o.PriceDetailId == priceDetail.Id && o.DistrictTypeId == districtTypeId)?.PriceAddOn ?? 0);
                                var totalPrice = (priceMain + priceAOFrom + priceAOTo + priceDTAddOn);
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

                            if (customerId.HasValue && priceDetail != null)
                            {
                                var priceDetailCustomer = await _context.PriceDetailCustomers.FirstOrDefaultAsync(o => o.State == 0 && o.CustomerId == customerId && o.PriceDetailId == priceDetail.Id);
                                if (priceDetailCustomer != null)
                                {
                                    double.TryParse(priceDetailCustomer.GetValue(deliveryCode + "_DiscountAmount") + "", out double discountAmount);
                                    priceDefaul = !(priceDetailCustomer.Fixed ?? false) ? priceDefaul * (1 + discountAmount) : discountAmount;
                                }
                            }
                        }
                    }
                }
            }
            return Math.Round(priceDefaul < 0 ? 0 : priceDefaul);
        }
        public async Task<ResultModel<ComputedPriceModel>> Computed(LadingViewModel lading)
        {
            var result = new ResultModel<ComputedPriceModel>();
            var couponCode = lading.CouponCode;
            var serviceOtherIds = lading.AnotherServiceIds;
            var discountType = new DiscountType();
            var priceList = await _context.PriceLists.FirstOrDefaultAsync(x => x.PriceListID == (lading.PriceListId ?? 0));
            if (priceList != null)
            {
                double weightToPrice = (lading.Weight ?? 0) > (lading.Mass ?? 0) ? (lading.Weight ?? 0) : (lading.Mass ?? 0);
                double chargeDefault = 0;
                double percent = 0;
                var tasks = new List<Task>
                    {
                        await Task.Factory.StartNew(async () =>
                        {
                            using (var context = new ApplicationDbContext())
                            {

                                var priceListCustomer = await context.PriceListCustomers.FirstOrDefaultAsync(o => o.PriceListId ==(lading.PriceListId ?? 0) && o.CustomerId == (lading.SenderId ?? 0) && o.State == 0);
                                if(priceListCustomer != null){
                                    discountType = await context.DiscountTypes.FirstOrDefaultAsync(o => o.Id  ==  (priceListCustomer.DiscountTypeId ?? 0));
                                    percent = (priceListCustomer.PriceListPercent ?? 100) / 100 ;
                                    if (discountType!= null && (discountType.Fixed ?? false))
                                    {
                                        percent = priceListCustomer.PriceListPercent ?? 0;
                                    }
                                }
                            }
                        }),
                        await Task.Factory.StartNew(async () =>
                        {
                            using (var context = new ApplicationDbContext())
                            {
                                using (var priceRepository = new PriceRepository(context))
                                {
                                    if ((priceList.PriceListTypeId ?? 0) == 2)
                                    {
                                        if (lading.KDNumber.HasValue && lading.Insured.HasValue && lading.KDNumber.Value > 0 && lading.Insured.Value > 0
                                            && lading.RDFrom.HasValue && lading.StructureID.HasValue)
                                        {
                                            chargeDefault = priceRepository.GetPriceHightValue(lading.KDNumber ?? 0, lading.Insured ?? 0, lading.RDFrom ?? 0, lading.StructureID ?? 0);
                                        }
                                    }
                                    else {
                                        var cityRecipientId = lading.CityRecipientId ?? 0;
                                        if (lading.POMediateId.HasValue && lading.POTo.HasValue)
                                        {
                                            var poMediate = context.PostOffices.SingleOrDefault(x => x.PostOfficeID == lading.POMediateId.Value);
                                            if(poMediate != null && poMediate.CityId.HasValue )
                                            {
                                                cityRecipientId = poMediate.CityId ?? 0;
                                            }
                                        }
                                        chargeDefault = await priceRepository.Computed(weightToPrice, lading.ServiceId ?? 0, lading.PriceListId ?? 0, lading.CitySendId ?? 0, cityRecipientId, lading.DistrictFrom ?? 0, lading.DistrictTo ?? 0, lading.RDFrom ?? 0, lading.SenderId);
                                    }
                                }
                            }
                        })
                    };
                await Task.WhenAll(tasks);
                var computedPrice = new ComputedPriceModel
                {
                    ChargeMain = chargeDefault,
                    Surcharge = lading.PriceOther ?? 0,
                    ChargeFuel = Math.Round(chargeDefault * (priceList.PriceListFuel ?? 0) / 100)
                };
                if (discountType != null)
                {
                    switch (discountType.Code)
                    {
                        case "PP":
                            computedPrice.ChargeMain = percent * computedPrice.ChargeMain;
                            computedPrice.ChargeFuel = percent * computedPrice.ChargeFuel;
                            break;
                        case "PPM":
                            computedPrice.ChargeMain = percent * computedPrice.ChargeMain;
                            break;
                    }
                }
                lading.PPXDPercent = computedPrice.ChargeFuel;
                lading.PriceOther = computedPrice.Surcharge;
                lading.PriceMain = computedPrice.ChargeMain;
                if (serviceOtherIds != null)
                {
                    foreach (var serviceOtherId in serviceOtherIds)
                    {
                        var serviceOther = _context.Services.FirstOrDefault(x => x.ServiceID == serviceOtherId);
                        var serviceOtherModel = new ServiceOtherModel()
                        {
                            Id = serviceOtherId,
                            Name = serviceOther?.ServiceName,
                            Code = serviceOther?.ServiceCode,
                        };
                        if (serviceOtherId == (int)ServiceOther.PACK)// DBND 
                        {
                            serviceOtherModel.Charge = lading.PackPrice ?? 0;
                        }
                        else
                        {
                            serviceOtherModel.Charge = await ComputedServiceOther(serviceOtherId, lading.PriceListId, lading.StructureID, lading.CitySendId,
                                 lading.CityRecipientId, lading.DistrictFrom, lading.DistrictTo, lading);
                        }
                        if (serviceOtherModel.Charge > 0 || (serviceOtherModel.Charge == 0 && serviceOtherId != (int)ServiceOther.DBND_PH && serviceOtherId != (int)ServiceOther.DBND_LH))
                        {
                            computedPrice.ServiceOthers.Add(serviceOtherModel);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(couponCode))
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var couponRepository = new CouponRepository(context);
                        var discountResult = couponRepository.GetDiscountAmount(couponCode, computedPrice, lading.SenderId);
                        if (discountResult.Error == 0)
                        {
                            computedPrice.DiscountAmount = Math.Round((double)discountResult.Data);
                        }
                        else result.Message = discountResult.Message;
                    }
                }
                result.Data = computedPrice;
                result.Error = 0;
            }
            return result;
        }
        public async Task<double> ComputedServiceOther(int serviceOtherId, int? priceListId, int? structureId = null, int? cityFromId = null, int? cityToId = null, int? districtFromId = null, int? districtToId = null, object lading = null)
        {
            var serviceOtherFomulas = _context.ServiceOtherFormulas.Where(o => o.State == 0 && o.ServiceOtherId == serviceOtherId);
            //
            if (await (serviceOtherFomulas.AnyAsync(o => o.PriceListId == priceListId)))
                serviceOtherFomulas = serviceOtherFomulas.Where(o => o.PriceListId == priceListId);
            else
                serviceOtherFomulas = serviceOtherFomulas.Where(o => !o.PriceListId.HasValue);
            //
            if ((await serviceOtherFomulas.AnyAsync(o => o.DistrictTypeId.HasValue)))
            {
                int? districtTypeFromId = null;
                int? districtTypeToId = null;

                if (districtFromId.HasValue)
                {
                    districtTypeFromId = _context.Locations.FirstOrDefault(o => o.LocationId == districtFromId.Value)?.DistrictTypeId;
                }
                if (districtToId.HasValue)
                {
                    districtTypeToId = _context.Locations.FirstOrDefault(o => o.LocationId == districtToId.Value)?.DistrictTypeId;
                }
                int? districtTypeId = _context.DistrictTypeMappings.FirstOrDefault(o => o.DistrictTypeFromId == districtTypeFromId && o.DistrictTypeToId == districtTypeToId && o.State == 0)?.DistrictTypeToId;
                if (districtTypeId.HasValue)
                {
                    if (serviceOtherFomulas.Any(o => o.DistrictTypeId == districtTypeId && o.CityFromId == cityFromId && o.CityToId == cityToId))
                        serviceOtherFomulas = serviceOtherFomulas.Where(o => o.DistrictTypeId == districtTypeId);
                    else
                        serviceOtherFomulas = serviceOtherFomulas.Where(o => !o.DistrictTypeId.HasValue);
                }
            }
            //if (await (serviceOtherFomulas.AnyAsync(o => o.CityFromId == cityFromId)))
            //    serviceOtherFomulas = serviceOtherFomulas.Where(o => o.CityFromId == cityFromId);
            //else
            //    serviceOtherFomulas = serviceOtherFomulas.Where(o => !o.CityFromId.HasValue);
            ////
            //if (await (serviceOtherFomulas.AnyAsync(o => o.CityToId == cityToId)))
            //    serviceOtherFomulas = serviceOtherFomulas.Where(o => o.CityToId == cityToId);
            //else
            //    serviceOtherFomulas = serviceOtherFomulas.Where(o => !o.CityToId.HasValue);

            //
            if (await (serviceOtherFomulas.AnyAsync(o => o.StructureId == structureId)))
                serviceOtherFomulas = serviceOtherFomulas.Where(o => o.StructureId == structureId);
            else
                serviceOtherFomulas = serviceOtherFomulas.Where(o => !o.StructureId.HasValue);

            var serviceOtherFomula = await serviceOtherFomulas.FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(serviceOtherFomula?.ColumnNameMoneyRange))
            {
                var moneyRange = GetMoneyRange(serviceOtherFomula?.ColumnNameMoneyRange, lading);
                serviceOtherFomula = await serviceOtherFomulas.Where(o => o.MoneyRange.Value >= moneyRange)
                                            .OrderBy(o => o.MoneyRange)
                                            .FirstOrDefaultAsync();
            }
            if (serviceOtherFomula == null)
                return 0;
            if (string.IsNullOrEmpty(serviceOtherFomula.Formula))
                return serviceOtherFomula.ChargeMin ?? 0;

            var formula = ConvertFormula(serviceOtherFomula.Formula, lading);
            double result = 0;
            var connection = _context.Database.GetDbConnection();

            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT CAST(" + formula + " AS decimal) ";
                double.TryParse(command.ExecuteScalar().ToString(), out result);
            }
            connection.Close();
            var min = (serviceOtherFomula.ChargeMin ?? 0);
            return result > min ? result : min;
        }
        public string ConvertFormula(string formula, object obj)
        {
            var str = formula.Substring(formula.IndexOf("[") + 1)
                .Split('[')
                .Select(x => x.Split(']').FirstOrDefault())
                .Distinct();
            foreach (var s in str)
            {
                var value = obj.GetValue(s) + "";
                formula = formula.Replace($"[{s}]", value);
            }
            return formula;
        }
        public double GetMoneyRange(string columnName, object obj)
        {
            double.TryParse(obj.GetValue(columnName) + "", out double d);
            return d;
        }

    }
}

