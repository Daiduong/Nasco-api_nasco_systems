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
    public class LadingTempHistoryRepository : Repository<LadingTempHistory>, ILadingTempHistoryRepository
    {
        private const int _STATUS_PickUp = (int)StatusLading.ChoLayHang;

        private static int[] _STATUS_LATLNG_USER = { (int)StatusLading.DaLayHang, (int)StatusLading.DangTrungChuyen,
                                                        (int)StatusLading.DangPhat};
        private static int[] _STATUS_LATLNG_POCURRENT = { (int)StatusLading.KhaiThacThongTin, (int)StatusLading.ChuaNhapKho,
                                                        (int)StatusLading.NhapKho, (int)StatusLading.XuatKho, (int)StatusLading.XuatKhoTrungChuyen ,(int)StatusLading.PhatKhongTC };
        private static int[] _STATUS_LATLNG_TO = { (int)StatusLading.DaChuyenHoan, (int)StatusLading.ThanhCong };
        public LadingTempHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<LadingTempHistory>>GetByUser(int officerID, DateTime? dateFrom, DateTime? dateTo, int? skip = null, int? take = null, params Expression<Func<LadingTempHistory, object>>[] includeProperties )
        {
            IQueryable<LadingTempHistory> query = _context.Set<LadingTempHistory>();
            Expression<Func<LadingTempHistory, bool>> predicate = r => officerID == r.OfficerId;

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
        public async Task<IEnumerable<LadingTempHistory>> GetDistinctLading(DateTime? dateFrom, DateTime? dateTo, int? skip = null, int? take = null, params Expression<Func<LadingTempHistory, object>>[] includeProperties)
        {
            IQueryable<LadingTempHistory> query = _context.Set<LadingTempHistory>();
            Expression<Func<LadingTempHistory, bool>> predicate = r => r.DateTime.Value.Date <= dateTo.Value.Date;

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
        public override void Insert(LadingTempHistory ldHistory)
        {
            var office = new Officer();
            var postOfficeRepository = new PostOfficeRepository(_context);
            var officerRepository = new OfficerRepository(_context);
            var ladingRepository = new LadingTempRepository(_context);
            var postOffice = postOfficeRepository.GetFirst(o => o.PostOfficeID == ldHistory.PostOfficeId);
            var lading = ladingRepository.GetFirst(l => l.Id == ldHistory.LadingId);

            if (ldHistory.Status.HasValue && lading != null)
            {
                if (_STATUS_LATLNG_USER.Contains(ldHistory.Status.Value))
                {
                    if (ldHistory.Status.Value == _STATUS_LATLNG_USER[0])
                    {
                        office = officerRepository.GetFirst(o => o.OfficerID == lading.OfficerPickup);
                    }
                    else
                    {
                        office = officerRepository.GetFirst(o => o.OfficerID == lading.OfficerDelivery);
                    }
                    if (office != null)
                    {
                        if (office.LatDynamic.IsNumeric())
                        {
                            ldHistory.Lat = Convert.ToDouble(office.LatDynamic);
                        }
                        if (office.LngDynamic.IsNumeric())
                        {
                            ldHistory.Lng = Convert.ToDouble(office.LngDynamic);
                        }
                    }
                }
                else if (_STATUS_LATLNG_POCURRENT.Contains(ldHistory.Status.Value) && postOffice != null)
                {
                    ldHistory.Lat = postOffice.Lat;
                    ldHistory.Lng = postOffice.Lng;
                }
                else if (lading != null)
                {
                    if (_STATUS_LATLNG_TO.Contains(ldHistory.Status.Value))
                    {
                        ldHistory.Lat = lading.LatTo;
                        ldHistory.Lng = lading.LngTo;
                    }
                    else
                    {
                        ldHistory.Lat = lading.LatFrom;
                        ldHistory.Lng = lading.LngFrom;
                    }
                }
            }
            base.Insert(ldHistory);
        }
    }
}
