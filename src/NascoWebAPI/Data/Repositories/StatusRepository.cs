using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class StatusRepository : Repository<LadingStatus> , IStatusRepository
    {
        public StatusRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
