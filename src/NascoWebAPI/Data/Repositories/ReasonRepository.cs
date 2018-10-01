using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class ReasonRepository : Repository<Reason> , IReasonRepository
    {
        public ReasonRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
