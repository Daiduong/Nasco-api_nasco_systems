using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class TimeLineRepository : Repository<TimeLine> , ITimeLineRepository
    {
        public TimeLineRepository(ApplicationDbContext context) : base(context)
        {
         
        }
        public async Task<IEnumerable<ExpectedTimeModel>> GetListExpectedTime(int cityFromId, int cityToId, int serviceId, int poCurrentId, int? poToLevel = null, int? deliveryReceiveId = null)
        {
            var poCurrentLevel = (await _context.PostOffices.FirstOrDefaultAsync(x => x.PostOfficeID == poCurrentId))?.Level ?? 0;
            poToLevel = poToLevel ?? poCurrentLevel;
            int pickupProcess = (int)Helper.Common.Process.Process.PICKED;
            int pickupProcessNext = (int)Helper.Common.Process.Process.INSTOCK;
            int takeOfProcess = (int)Helper.Common.Process.Process.TAKEOFF;
            int takeOfProcessNext = (int)Helper.Common.Process.Process.LANDING;
            int deliveryProcess = (int)Helper.Common.Process.Process.DELIVERING;
            int deliveryProcessNext = (int)Helper.Common.Process.Process.DELIVERED;
            if (deliveryReceiveId.HasValue && !(await _context.DeliveryReceives.AnyAsync(x => x.DeliveryReceiveCode.EndsWith("-DC") && x.Id == deliveryReceiveId)))
            {
                deliveryProcess = (int)Helper.Common.Process.Process.TRANSPORT;
                deliveryProcessNext = (int)Helper.Common.Process.Process.INSTOCK;
            }
            var timeLines = _context.TimeLines
                                    .Where(x => x.State == 0 && x.CityFromId == cityFromId && x.CityToId == cityToId && x.ServiceId == serviceId)
                                    .Join(_context.TimeLinePostOfficeLevels
                                    .Where(x => x.State == 0 && x.TimeStart.HasValue && x.TimeEnd.HasValue),
                                    x => x.Id, y => y.TimeLineId, (x, y) => new
                                    {
                                        x.IsFromOrTo,
                                        x.OrderByService,
                                        y.PostOfficeLevel,
                                        x.ProcessId,
                                        x.ProcessNextId,
                                        y.TimeStart,
                                        y.TimeEnd,
                                        y.IntervalTime,
                                        y.IntervalDay,
                                    }).Distinct();
            var timePickup = (await timeLines.Where(x => (x.IsFromOrTo ?? true) && x.ProcessId == pickupProcess && x.ProcessNextId == pickupProcessNext && x.PostOfficeLevel == poCurrentLevel).ToListAsync())
                            .Select(x => new
                            {
                                OrderByService = x.OrderByService ?? 1,
                                TimeStart = x.TimeStart.Value,
                                IntervalDay = x.IntervalDay ?? 0
                            });
            var timeTakeOff = (await timeLines.Where(x => (x.IsFromOrTo ?? true) && x.ProcessId == takeOfProcess && x.ProcessNextId == takeOfProcessNext && x.PostOfficeLevel == ((int)PostOfficeType.HUB - 1)).ToListAsync())
                    .Select(x => new
                    {
                        OrderByService = x.OrderByService ?? 1,
                        TimeStart = x.TimeStart.Value,
                        IntervalDay = x.IntervalDay ?? 0
                    });
            var timeDelivery = (await timeLines.Where(x => !(x.IsFromOrTo ?? true) && x.ProcessId == deliveryProcess && x.ProcessNextId == deliveryProcessNext && x.PostOfficeLevel == poToLevel).ToListAsync())
                                .Select(x => new
                                {
                                    OrderByService = x.OrderByService ?? 1,
                                    TimeEnd = x.TimeEnd.Value,
                                    IntervalDay = x.IntervalDay ?? 0
                                });
            return timePickup.Join(timeDelivery, x => x.OrderByService, y => y.OrderByService, (x, y) => new { Start = x, End = y })
                             .Join(timeTakeOff, x => x.Start.OrderByService, y => y.OrderByService, (x, y) =>
                                new ExpectedTimeModel()
                                {
                                    Id = x.Start.OrderByService,
                                    TimeStart = x.Start.TimeStart,
                                    TimeEnd = x.End.TimeEnd,
                                    TimeTakeOff = y.TimeStart,
                                    IntervalDayStart = x.Start.IntervalDay + (x.Start.TimeStart.Subtract(DateTime.Now.TimeOfDay).TotalMinutes > 0 ? 0 : 1),
                                    IntervalDayEnd = x.End.IntervalDay + (x.Start.TimeStart.Subtract(DateTime.Now.TimeOfDay).TotalMinutes > 0 ? 0 : 1),
                                    IntervalDayTakeOff = y.IntervalDay + (x.Start.TimeStart.Subtract(DateTime.Now.TimeOfDay).TotalMinutes > 0 ? 0 : 1),
                                }).OrderBy(x => x.DateTimeStart);
        }
    }
}
