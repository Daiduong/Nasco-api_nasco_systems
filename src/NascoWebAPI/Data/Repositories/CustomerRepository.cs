using Microsoft.EntityFrameworkCore;
using NascoWebAPI.Data.Entities;
using NascoWebAPI.Models;
using NascoWebAPI.Models.Response;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext conText) : base(conText)
        {

        }
        public async Task<Customer> GetCustomerByPhone(string phone)
        {
            return await this.GetFirstAsync(o => ((o.Phone != null && o.Phone == phone) || (o.Phone2 != null && o.Phone2 == phone)) && o.State == 0);
        }
        public string GetCode(int id)
        {
            var model = "NAS";
            return model + id.ToString("D8");
        }
        public async Task<IEnumerable<int>> GetListIdByPartner(int partnerId)
        {
            return await _context.Customers.Where(x => x.State == 0 && x.PartnerId == partnerId).Select(x => x.CustomerID).ToListAsync();
        }
        public async Task<IEnumerable<int>> GetListIdHasParner()
        {
            return await _context.Customers.Where(x => x.State == 0 && x.PartnerId.HasValue).Select(x => x.CustomerID).ToListAsync();
        }
        public dynamic GetCustomerPromotionCode(int cusId, DateTime? fromDate = null, DateTime? toDate = null, string promotionCode = null, string codeOfPromotion = null,
                                                    bool? isActive = null, int? pageNumber = null, int? pageSize = null)
        {
            SqlParameter CusId = new SqlParameter("@CustomerId", cusId);


            SqlParameter PromotionCode = new SqlParameter("@PromotionCode", promotionCode);
            if (string.IsNullOrWhiteSpace(promotionCode)) PromotionCode.Value = DBNull.Value;

            SqlParameter CodeOfPromotion = new SqlParameter("@CodeOfPromotion", codeOfPromotion);
            if (string.IsNullOrWhiteSpace(codeOfPromotion)) CodeOfPromotion.Value = DBNull.Value;

            SqlParameter FromDate = new SqlParameter("@FromDate", fromDate);
            if (!fromDate.HasValue) FromDate.Value = DBNull.Value;

            SqlParameter ToDate = new SqlParameter("@ToDate", toDate);
            if (!toDate.HasValue) ToDate.Value = DBNull.Value;

            SqlParameter PageNumber = new SqlParameter("@PageNumber", pageNumber);
            if (!pageNumber.HasValue) PageNumber.Value = DBNull.Value;

            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            if (!pageSize.HasValue) PageSize.Value = DBNull.Value;

            SqlParameter IsActive = new SqlParameter("@IsActive", isActive);
            if (!isActive.HasValue) IsActive.Value = DBNull.Value;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(CusId);
            sqlParameters.Add(FromDate);
            sqlParameters.Add(ToDate);
            sqlParameters.Add(PromotionCode);
            sqlParameters.Add(CodeOfPromotion);
            sqlParameters.Add(IsActive);
            sqlParameters.Add(PageNumber);
            sqlParameters.Add(PageSize);

            return SqlHelper.ExecuteQuery<GetCustomerPromotionCodeResponseModel>(_context, "Proc_GetCustomerPromotionCode", sqlParameters);
            // return _context.ExecuteQuery<dynamic>("Proc_GetCustomerPromotionCode", sqlParameters);
        }
        public IEnumerable<bool> UsingPromotionCode(string promotionCode)
        {
            SqlParameter PromotionCode = new SqlParameter("@PromotionCode", promotionCode);
            if (string.IsNullOrWhiteSpace(promotionCode)) PromotionCode.Value = DBNull.Value;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(PromotionCode);
            return SqlHelper.ExecuteQuery<bool>(_context, "UsingPromotionCode", sqlParameters);
        }
        public dynamic GetCustomerMessage(int customerId)
        {
            var result = _context.CustomerMessages.Where(x => x.CustomerId == customerId).Join(_context.MarketingMessages, cm => cm.MarketingMessageId, mm => mm.Id,
                  (cm, mm) => new { cm.Id, cm.CustomerId, cm.IsPush, mm.IsEnabled, mm.Title, mm.Content }).Where(x => x.IsPush == true);
            return result.ToList();
        }
        public IEnumerable<CustomerPoint> GetCustomerPoint(int customerId)
        {
            return _context.CustomerPoints.Where(x => x.CustomerId == customerId);
        }
        public CustomerPoint InsertCustomerPoint(CustomerPoint customerPoint)
        {
            customerPoint.AllPoint = 0;
            customerPoint.currentpoint = 0;
            customerPoint.RankId = 1;
            customerPoint.IsEnabled = true;
            customerPoint.CreatedWhen = DateTime.Now;
            _context.CustomerPoints.Add(customerPoint);
            _context.SaveChanges();
            return customerPoint;
        }
        public dynamic CheckAndInsertCTKM(int customerId)
        {
            var getCustomer = _context.Customers.Single(x => x.CustomerID == customerId);
            if (getCustomer != null)
            {
                var checkProgramIndate = _context.ProgramPromotion.OrderByDescending(x=>x.Id).FirstOrDefault(x => x.PromotionTypeId == 1 && DateTime.Now > x.StartDate && DateTime.Now < x.EndDate && x.IsEnabled == true);
                if (checkProgramIndate != null)
                {
                    var now = DateTime.Now;
                    Random rd = new Random();
                    int.TryParse(checkProgramIndate.PercentMin.ToString(), out int min);
                    int.TryParse(checkProgramIndate.PercentMax.ToString(), out int max);
                    var cus = new CustomerProgramPromotion();
                    cus.CreatedBy = getCustomer.CustomerID;
                    cus.PromotionId = checkProgramIndate.Id;
                    cus.CreatedWhen = now;
                    cus.IsActive = true;
                    cus.IsEnabled = true;
                    cus.IsPush = false;
                    cus.CustomerId = getCustomer.CustomerID;
                    cus.IsPush = true;
                    cus.PromotionCode = "PNAS" + checkProgramIndate.Id + getCustomer.CustomerID;
                    cus.Value = rd.Next(min, max);
                    _context.CustomerProgramPromotion.Add(cus);
                    _context.SaveChanges();
                    return 3;
                }
                else
                {
                    return 2;
                }

            }
            else
            {
                return 1;
            }
        }
    }
}
