using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class CouponRepository : Repository<Coupon> ,ICouponRepository
    {
        public CouponRepository(ApplicationDbContext context) : base(context)
        {

        }
        public Coupon GetById(int id)
        {
            return this.GetFirst(o => o.State == 0 && o.Id == id);
        }
        public Coupon GetByCode(string code)
        {
            return this.GetFirst(o => o.State == 0 && o.Code != null && o.Code.ToUpper() == code.ToUpper());
        }
        public ResultModel<object> GetDiscountAmount(LadingViewModel lading)
        {
            var result = new ResultModel<object>();
            try
            {
                var coupon = this.GetByCode(lading.CouponCode.Trim());
                if (coupon == null)
                {
                    result.Message = "Không tìm thấy thông tin mã giảm giá";
                }
                else
                {
                    var promotion = _context.Promotions.SingleOrDefault(o => o.Id == coupon.PromotionId && o.State == 0);
                    if (!(promotion.Enabled ?? false))
                    {
                        return result.Init(message: "Thông tin giảm giá này đã khóa ");
                    }
                    if (promotion.DateFrom.HasValue && DateTime.Now.Subtract(promotion.DateFrom.Value).Days < 0)
                    {
                        return result.Init(message: "Thông tin giảm giá chưa được áp dụng");
                    }
                    if (promotion.DateTo.HasValue && DateTime.Now.Subtract(promotion.DateTo.Value).Days > 0)
                    {
                        return result.Init(message: "Thông tin giảm giá đã hết hạn.");
                    }
                    var quota = (promotion.Quota ?? 0);
                    var usesPerCoupon = promotion.UsesPerCoupon ?? 0;
                    var usesPerUser = promotion.UsesPerUser ?? 0;
                    double totalAmountUses = 0;
                    if ((usesPerCoupon) > 0 && usesPerCoupon == (coupon.NumberUses ?? 0))
                    {
                        return result.Init(message: "Đã vượt quá số lần sử dụng cho một mã");
                    }
                    if (!(promotion.IsInternal ?? true))
                    {
                        if (!(promotion.IsPublic ?? false) && _context.CouponCustomers.Any(o => o.CouponId == coupon.Id && o.UserAssignedId.Value != (lading.SenderId ?? 0)))
                        {
                            return result.Init(message: "Chỉ áp dụng cho những khách hàng nhận được mã");
                        }
                        if (usesPerUser > 0 && lading.SenderId.HasValue)
                        {
                            var couponCustomer = _context.CouponCustomers.FirstOrDefault(o => o.CouponId == coupon.Id && o.State == 0 && lading.SenderId.Value == o.UserId.Value);
                            if (couponCustomer != null && (couponCustomer.NumberUses ?? 0) == usesPerUser)
                            {
                                return result.Init(message: "Đã vượt quá số lần sử dụng cho một khách hàng");
                            }
                        }
                    }
                    else if (quota > 0)
                    {
                        var couponOfficer = _context.CouponOfficers.FirstOrDefault(o => o.CouponId == coupon.Id && o.State == 0);
                        totalAmountUses = couponOfficer.TotalAmountUses ?? 0;
                        if (totalAmountUses > quota)
                        {
                            return result.Init(message: "Đã sử dụng hết hạn mức quy định");
                        }
                    }
                    var discountType = _context.DiscountTypes.SingleOrDefault(o => o.Id == promotion.DiscountTypeId.Value);
                    double discountAmount = promotion.DiscountAmount ?? 0;
                    if (discountType != null)
                    {
                        switch (discountType.Code)
                        {
                            case "AP":
                                discountAmount *= (lading.Amount) / 100;
                                break;
                            case "AF":
                                break;
                            case "PP":
                                discountAmount *= ((lading.PPXDPercent ?? 0) + (lading.PriceMain ?? 0)) / 100;
                                break;
                        }
                    }
                    if (discountAmount + totalAmountUses > quota && quota > 0)
                    {
                        return result.Init(message: $"Hạn mức quy định có thể sử dung {(quota - totalAmountUses)}");
                    }
                    result.Data = discountAmount;
                    result.Error = 0;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result.Init(message: ex.Message);
            }
            return result;
        }
        public ResultModel<Coupon> Discount(string code, int currentOffficerId, int ladingId, double discountAmount)
        {
            var result = new ResultModel<Coupon>();
            try
            {
                var lading = _context.Ladings.SingleOrDefault(o => o.Id == ladingId);
                if (lading == null) return result.Init(message: "Không tìm thấy thông tin vận đơn");
                var coupon = this.GetByCode(code.Trim());
                if (coupon == null) return result.Init(message: "Không tìm thấy thông tin khuyến mãi");
                coupon.NumberUses = (coupon.NumberUses ?? 0) + 1;

                var promotion = _context.Promotions.SingleOrDefault(o => o.Id == coupon.PromotionId && o.State == 0);
                if (promotion == null) return result.Init(message: "Không tìm thấy thông tin khuyến mãi");

                promotion.NumberUses = (promotion.NumberUses ?? 0) + 1;
                int? approvedBy = null;
                if ((promotion.IsInternal ?? true))
                {
                    var couponOfficer = _context.CouponOfficers.FirstOrDefault(o => o.CouponId == coupon.Id && o.State == 0);
                    couponOfficer.NumberUses = (couponOfficer.NumberUses ?? 0) + 1;
                    couponOfficer.ModifiedBy = currentOffficerId;
                    couponOfficer.ModifiedDate = DateTime.Now;
                    if ((couponOfficer.UserAssignedId ?? 0) == currentOffficerId) approvedBy = currentOffficerId;
                }
                else
                {
                    var userId = lading.SenderId;
                    var couponCustomer = _context.CouponCustomers.FirstOrDefault(o => o.CouponId == coupon.Id && o.State == 0 && (userId == o.UserAssignedId.Value || userId == o.UserId));
                    if (couponCustomer != null)
                    {
                        couponCustomer.NumberUses = (couponCustomer.NumberUses ?? 0) + 1;
                        couponCustomer.UserId = userId;
                        couponCustomer.ModifiedBy = currentOffficerId;
                        couponCustomer.ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        couponCustomer = new CouponCustomer()
                        {
                            CouponId = coupon.Id,
                            NumberUses = 1,
                            State = 0,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            CreatedBy = currentOffficerId,
                            ModifiedBy = currentOffficerId,
                            UserId = userId
                        };
                        _context.CouponCustomers.Add(couponCustomer);
                    }
                }
                var couponLading = new CouponLading()
                {
                    CouponId = coupon.Id,
                    State = 0,
                    LadingId = ladingId,
                    DiscountAmount = discountAmount,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CreatedBy = currentOffficerId,
                    ModifiedBy = currentOffficerId,
                    Status = (int)StatusCouponLading.WaitingForApproval,
                    IsApproved = false,
                    UnapprovedTimes = 0
                };
                _context.CouponLadings.Add(couponLading);
                this.SaveChanges();
                lading.CouponLadingId = couponLading.Id;
                result.Error = 0;
                return result;
            }
            catch (Exception ex)
            {
                return result.Init(message: ex.Message);
            }
        }
    }
}
