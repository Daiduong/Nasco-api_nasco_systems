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
            return await this.GetFirstAsync(o => o.Phone == phone);
        }
        public string GetCode(int id)
        {
            var model = "NAS";
            return model + id.ToString("D8");
        }
    }
}
