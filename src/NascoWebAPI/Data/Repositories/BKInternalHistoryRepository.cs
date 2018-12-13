using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class BKInternalHistoryRepository : Repository<BKInternalHistory> , IBKInternalHistoryRepository
    {
        public BKInternalHistoryRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
