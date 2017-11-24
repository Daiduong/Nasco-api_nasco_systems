using Microsoft.EntityFrameworkCore;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class LadingHistoryRepository : Repository<LadingHistory>, ILadingHistoryRepository
    {
        public LadingHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<LadingHistory>>GetByUser(int officerID, DateTime? dateFrom, DateTime? dateTo, int? skip = null, int? take = null, params Expression<Func<LadingHistory, object>>[] includeProperties )
        {
            IQueryable<LadingHistory> query = _context.Set<LadingHistory>();
            Expression<Func<LadingHistory, bool>> predicate = r => officerID == r.OfficerId;

            if (!dateFrom.HasValue && !dateTo.HasValue)
            {
                dateFrom = dateTo = DateTime.Now;
            }
            if (dateFrom.HasValue && dateTo.HasValue)
            {
                predicate = r => dateFrom.Value.Date <= r.DateTime.Value.Date && r.DateTime.Value.Date <= dateTo.Value.Date && officerID == r.OfficerId;
            }
            else if (dateFrom.HasValue)
            {
                predicate = r => dateFrom.Value.Date <= r.DateTime.Value.Date && officerID == r.OfficerId;
            }
            else
            {
                predicate =  r => r.DateTime.Value.Date <= dateTo.Value.Date && officerID == r.OfficerId;
            }

            query = query.Where(predicate).OrderByDescending(o => o.Id);
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            query = query.GroupBy(g => g.LadingId).Select(g => g.FirstOrDefault());
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }
            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<LadingHistory>> GetDistinctLading(DateTime? dateFrom, DateTime? dateTo, int? skip = null, int? take = null, params Expression<Func<LadingHistory, object>>[] includeProperties)
        {
            IQueryable<LadingHistory> query = _context.Set<LadingHistory>();
            Expression<Func<LadingHistory, bool>> predicate = r => r.DateTime.Value.Date <= dateTo.Value.Date;

            if (!dateFrom.HasValue && !dateTo.HasValue)
            {
                dateFrom = dateTo = DateTime.Now;
            }
            if (dateFrom.HasValue && dateTo.HasValue)
            {
                predicate = r => dateFrom.Value.Date <= r.DateTime.Value.Date && r.DateTime.Value.Date <= dateTo.Value.Date;
            }
            else if (dateFrom.HasValue)
            {
                predicate = r => dateFrom.Value.Date <= r.DateTime.Value.Date;
            }

            query = query.Where(predicate).OrderByDescending(o => o.Id);

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            query = query.GroupBy(g => g.LadingId).Select(g => g.FirstOrDefault());
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }
            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }
            return await query.ToListAsync();
        }
    }
}
