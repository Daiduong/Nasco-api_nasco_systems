using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class DeliveryReceiveRepository : Repository<DeliveryReceive> , IDeliveryReceiveRepository
    {
        public DeliveryReceiveRepository(ApplicationDbContext conText) : base(conText)
        {

        }
    }
}
