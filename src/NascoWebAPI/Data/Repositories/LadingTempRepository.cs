using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class LadingTempRepository : Repository<LadingTemp> , ILadingTempRepository
    {
        public LadingTempRepository(ApplicationDbContext context) : base(context)
        {

        }
        public override void Update(LadingTemp entity)
        {
            base.Update(entity);
            var ladingHistoryRepository = new LadingTempHistoryRepository(_context);
            var ladingHistoryOld =  ladingHistoryRepository.GetFirst(o => o.LadingId == entity.Id, or => or.OrderByDescending(o => o.Id));
            if (ladingHistoryRepository.Any(o => o.LadingId == entity.Id && o.Status.HasValue)
                && ladingHistoryRepository.GetFirst(o => o.LadingId == entity.Id && o.Status.HasValue, or => or.OrderByDescending(o => o.Id)).Status != entity.Status)
            {
                //Thêm hành trình cho lading
                var ldHistory = new LadingTempHistory();
                ldHistory.LadingId = entity.Id;
                ldHistory.OfficerId = entity.ModifiedBy;
                ldHistory.PostOfficeId = entity.POCurrent;
                ldHistory.Status = entity.Status;
                ldHistory.DateTime = DateTime.Now;
                ldHistory.Note = entity.Noted;
                ladingHistoryRepository.Insert(ldHistory);
                ladingHistoryRepository.SaveChanges();
            }
           
        }
    }
}
