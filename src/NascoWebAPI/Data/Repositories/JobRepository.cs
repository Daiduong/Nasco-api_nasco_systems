using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class JobRepository : Repository<Job> , IJobRepository
    {
        public JobRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
