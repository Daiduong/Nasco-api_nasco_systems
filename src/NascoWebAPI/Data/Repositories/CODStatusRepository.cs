using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class CODStatusRepository : Repository<CODStatus> , ICODStatusRepository
    {
        public CODStatusRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
