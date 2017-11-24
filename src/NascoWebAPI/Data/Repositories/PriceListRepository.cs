using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class PriceListRepository : Repository<PriceList>, IPriceListRepository
    {
        public PriceListRepository(ApplicationDbContext conText) : base(conText)
        {

        }
        public async Task<int> GetPriceApplyByPOID(int pOId)
        {
            var priceList = await this.GetFirstAsync(o => o.IsApply == true && o.POID == pOId);
            if (priceList != null)
            {
                return priceList.PriceListID;
            }
            return 0;
        }
        public IEnumerable<PriceList> GetListPriceListByCustomer(int? customerId = null, bool union = true)
        {
            var query = this.GetAll();

            if (customerId.HasValue && _context.Customers.Any(o => o.CustomerID == customerId.Value))
            {
                var customer = _context.Customers.Single(o => o.CustomerID == customerId.Value);
                if (!string.IsNullOrEmpty(customer.PriceListIds))
                {
                    var pricelistIds = customer.PriceListIds.Split(',').Select(int.Parse);
                    query = query.Where(o => pricelistIds.Contains(o.PriceListID));
                }
                else
                {
                    query = query.Where(o => (o.IsApply ?? false));
                }
            }
            else
            {
                query = query.Where(o => (o.IsApply ?? false));
            }
            if (union)
            {
                query = query.Union(_context.PriceLists.Where(o => o.PriceListTypeId.Value != 1));
            }
            return query;
        }
    }

}
