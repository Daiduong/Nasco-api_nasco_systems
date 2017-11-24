using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class BKDeliveryRepository : Repository<BKDelivery> , IBKDeliveryRepository
    {
        public BKDeliveryRepository(ApplicationDbContext conText) : base(conText)
        {

        }
    }
}
