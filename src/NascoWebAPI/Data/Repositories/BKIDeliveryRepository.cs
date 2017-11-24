using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class BKInternalRepository : Repository<BKInternal>, IBKInternalRepository
    {
        public BKInternalRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task<IEnumerable<BKInternal>> GetListWaitingByOfficer(int officerID, params Expression<Func<BKInternal, object>>[] includeProperties)
        {
            return await this.GetAsync(o => o.OfficerId_sender.Value == officerID && o.Status.Value == 0 && !(o.IsConfirmByOfficer ?? false));
        }
    }
}
