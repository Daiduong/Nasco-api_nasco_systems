using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class CustomerRepository : Repository<Customer> , ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext conText) : base(conText)
        {

        }
        public async  Task<Customer> GetCustomerByPhone( string phone)
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
    }
}
