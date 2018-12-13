using Microsoft.Extensions.Logging;
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
        public PriceListRepository(ApplicationDbContext context) : base(context)
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
            var priceListByCustomers = Enumerable.Empty<PriceList>();
            var priceListCustomers = _context.PriceListCustomers.Where(o => o.PriceListId.HasValue && o.State == 0 && o.CustomerId == customerId);
            if (priceListCustomers.Count() > 0)
            {
                priceListByCustomers = this.Get(o => priceListCustomers.Any(p => p.PriceListId == o.PriceListID));
            }
            var priceListPublics = this.Get(o => (o.IsApply ?? false));
            if (union)
            {
                priceListPublics = priceListPublics.Union(_context.PriceLists.Where(o => o.PriceListTypeId.Value != 1));
            }
            return priceListByCustomers.Union(priceListPublics.OrderBy(o => o.Order));
        }
    }

}
