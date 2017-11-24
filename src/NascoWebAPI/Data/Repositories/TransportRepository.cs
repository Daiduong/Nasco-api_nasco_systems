using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class TransportRepository : Repository<Transport> , ITransportRepository
    {
        public TransportRepository(ApplicationDbContext conText) : base(conText)
        {

        }
    }
}
