using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class CustomerContactRepository : Repository<CustomerContact>, ICustomerContactRepository
    {
        public CustomerContactRepository(ApplicationDbContext conText) : base(conText)
        {

        }
        public IQueryable<CustomerContact> GetListContactByCustomer(int id)
        {
            return _context.CustomerContacts.Where(x => x.CustomerId == id && x.State == 0);
        }
        public IQueryable<CustomerContact> GetListContactByCustomer(int[] ids)
        {
            return _context.CustomerContacts.Where(x => ids.Contains(x.CustomerId.Value) && x.State == 0);
        }
        public IQueryable<CustomerContact> GetListContactByPartnerId(int id)
        {
            return _context.CustomerContacts
                .Join(_context.Customers.Where(x => x.State == 0 && x.PartnerId == id), x => x.CustomerId, y => y.CustomerID, (x, y) => x)
                .Where(x => x.State == 0);
        }
    }
}
