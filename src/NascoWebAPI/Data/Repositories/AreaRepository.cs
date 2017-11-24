using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class AreaRepository : Repository<Area> , IAreaRepository
    {
        public AreaRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
