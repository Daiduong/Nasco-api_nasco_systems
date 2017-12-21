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
            var priceListCustomers = _context.PriceListCustomers.Where(o => o.PriceListId.HasValue && o.State == 0 && o.CustomerId == customerId);
            if (priceListCustomers.Count() > 0)
            {
                query = query.Where(o => priceListCustomers.Any(p => p.PriceListId == o.PriceListID));
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
