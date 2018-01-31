using Microsoft.EntityFrameworkCore;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static NascoWebAPI.Helper.Common.Constants;

namespace NascoWebAPI.Data
{
    public class FlightRepository : Repository<Flight>, IFlightRepository
    {
        public FlightRepository(ApplicationDbContext conText) : base(conText)
        {

        }
        public async Task<IEnumerable<Flight>> GetListFlight(int? poFrom, int? poTo, int[] statusIds = null, int? pageSize = null, int? pageNo = null, string cols = null)
        {
            Expression<Func<Flight, bool>> predicate = o => o.Id > 0;
            if (poFrom.HasValue)
            {
                predicate = predicate.And(o => o.POFrom.Value == poFrom);
            }
            if (poTo.HasValue)
            {
                predicate = predicate.And(o => o.POTo.Value == poTo);
            }
            if (statusIds != null && statusIds.Count() > 0)
            {
                predicate = predicate.And(o => o.FlightStatus.HasValue && statusIds.Contains(o.FlightStatus.Value));
            }
            return await this.GetAsync(predicate, or => or.OrderByDescending(o => o.Date_Create), pageSize, pageNo, getInclude(cols));
        }
        public Expression<Func<Flight, object>>[] getInclude(string cols)
        {
            List<Expression<Func<Flight, object>>> includeProperties = new List<Expression<Func<Flight, object>>>();
            if (!string.IsNullOrEmpty(cols))
            {
                foreach (var col in cols.Split(','))
                {
                    var colValue = col.Trim().ToLower();
                    if (colValue == "mawbid")
                    {
                        includeProperties.Add(inc => inc.MAWB);
                    }
                    if (colValue == "poto")
                    {
                        includeProperties.Add(inc => inc.PostOfficeTo);
                    }
                    if (colValue == "pofrom")
                    {
                        includeProperties.Add(inc => inc.PostOfficeFrom);
                    }
                    if (colValue == "status")
                    {
                        includeProperties.Add(inc => inc.Status);
                    }
                }
            }
            return includeProperties.ToArray();
        }
        public async Task<ResultModel<Flight>> TakeOff(FlightModel model)
        {
            var result = new ResultModel<Flight>();
            try
            {
                var flight = await this.GetSingleAsync(o => o.Id == model.Id, inc => inc.MAWB, inc => inc.Status);
                if (flight != null)
                {
                    flight.Status = flight.Status ?? new Status();
                    if (flight.MAWB != null)
                    {
                        if (flight.FlightStatus == (int)StatusFlight.Created)
                        {
                            if (model.WeightConfirm <= 0)
                            {
                                result.Message = "Trọng lượng chốt với nhà vận chuyển phải lớn hơn 0";
                            }
                            else if (flight.MAWB.MaxWeight < flight.TotalWeightToPrice)
                            {
                                result.Message = "Trọng lượng tính cước đã quá tải. Vui lòng book thêm trọng lượng";
                            }
                            else if ((flight.MAWB.MaxWeight ?? 0) < model.WeightConfirm)
                            {
                                result.Message = "Trọng lượng book phải lớn hơn trọng lượng chốt với nhà vận chuyển";
                            }
                            else
                            {
                                flight.MAWB.WeightConfirm = model.WeightConfirm;
                                flight.MAWB.RealDateTime = model.RealDateTime ?? new DateTime();
                                flight.FlightStatus = (int)StatusFlight.TakeOff;
                                var ladingStatus = (int)StatusLading.RoiTrungTamBay;
                                var officer = _context.Set<Officer>().Include(o => o.PostOffice).Single(o => o.OfficerID == model.ModifiedBy);
                                if (officer.PostOffice == null)
                                {
                                    result.Message = "Không tìm thấy thông tin bưu cục của nhân viên đang thao tác";
                                }
                                else
                                {
                           
                                    var bkinternals = _context.BKInternals.Where(o => o.FlightId == flight.Id).ToList();
                                    bkinternals.ForEach(bkinternal =>
                                    {
                                        var ladings = _context.Ladings.Where(o => o.BKInternalId == bkinternal.ID_BK_internal).ToList();

                                        ladings.ForEach(lading =>
                                        {
                                            if (lading.Status != ladingStatus)
                                            {
                                                lading.Status = ladingStatus;
                                                LadingHistory ladingHistory = new LadingHistory
                                                {
                                                    CreatedDate = DateTime.Now,
                                                    DateTime = model.RealDateTime ?? new DateTime(),
                                                    LadingId = lading.Id,
                                                    Location = officer.PostOffice.POName,
                                                    OfficerId = model.ModifiedBy,
                                                    PostOfficeId = officer.PostOfficeId,
                                                    Status = ladingStatus
                                                };
                                                _context.LadingHistories.Add(ladingHistory);
                                            }
                                        });
                                        bkinternal.Status = (int)StatusFlight.TakeOff;
                                    });
                                    this.SaveChanges();
                                    result.Error = 0;
                                    result.Message = "Rời sân bay thành công";
                                    result.Data = flight;
                                }
                            }
                        }
                        else result.Message = $"Không thể gửi hàng do chuyến bay hiện { flight.Status.StatusName }";
                    }
                    else
                        result.Message = "Không tìm thấy thông tin lịch bay";
                }
                else
                    result.Message = "Không tìm thấy thông tin chuyến bay";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<ResultModel<Flight>> Receive(FlightModel model)
        {
            var result = new ResultModel<Flight>();
            try
            {
                var flight = await this.GetSingleAsync(o => o.Id == model.Id, inc => inc.MAWB, inc => inc.Status);
                if (flight != null)
                {
                    flight.Status = flight.Status ?? new Status();
                    if (flight.MAWB != null)
                    {
                        if (flight.FlightStatus == (int)StatusFlight.TakeOff)
                        {
                            var ladingStatus = (int)StatusLading.DenTrungTamSanBayNhan;
                            var officer = _context.Set<Officer>().Include(o => o.PostOffice).Single(o => o.OfficerID == model.ModifiedBy);
                            if (officer.PostOffice == null)
                            {
                                result.Message = "Không tìm thấy thông tin bưu cục của nhân viên đang thao tác";
                            }
                            else
                            {
                                flight.MAWB.RealDateTime = model.ReceivedRealDateTime ?? new DateTime();
                                flight.FlightStatus = (int)StatusFlight.Landing;
                                var bkinternals = _context.BKInternals.Where(o => o.FlightId == flight.Id).ToList();
                                bkinternals.ForEach(bkinternal =>
                                {
                                    var ladings = _context.Ladings.Where(o => o.BKInternalId == bkinternal.ID_BK_internal).ToList();
                                    ladings.ForEach(lading =>
                                    {
                                        if (lading.Status != ladingStatus)
                                        {
                                            lading.Status = ladingStatus;
                                            LadingHistory ladingHistory = new LadingHistory
                                            {
                                                CreatedDate = DateTime.Now,
                                                DateTime = model.ReceivedRealDateTime ?? new DateTime(),
                                                LadingId = lading.Id,
                                                Location = officer.PostOffice.POName,
                                                OfficerId = model.ModifiedBy,
                                                PostOfficeId = officer.PostOfficeId,
                                                Status = ladingStatus
                                            };
                                            _context.LadingHistories.Add(ladingHistory);
                                        }
                                    });
                                    bkinternal.Status = (int)StatusFlight.Landing;
                                });
                                this.SaveChanges();
                                result.Error = 0;
                                result.Message = "Nhận hàng bay thành công";
                                result.Data = flight;
                            }
                        }
                        else result.Message = $"Không thể nhận hàng do chuyến bay hiện { flight.Status.StatusName }";
                    }
                    else
                        result.Message = "Không tìm thấy thông tin lịch bay";
                }
                else
                    result.Message = "Không tìm thấy thông tin chuyến bay";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
