﻿using Microsoft.EntityFrameworkCore;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Concurrent;
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

                                var officer = _context.Set<Officer>().Include(o => o.PostOffice).Single(o => o.OfficerID == model.ModifiedBy);
                                if (officer.PostOffice == null)
                                {
                                    result.Message = "Không tìm thấy thông tin bưu cục của nhân viên đang thao tác";
                                }
                                else
                                {
                                    var currentUserId = officer.OfficerID;
                                    var currentPOId = officer.PostOfficeId ?? 0;
                                    var statusUpdate = (int)StatusLading.RoiTrungTamBay;
                                    var statusRollbacks = new int[] { (int)StatusLading.TaoChuyenBay };

                                    var bkinternals = _context.BKInternals.Where(o => o.FlightId == flight.Id).ToList();
                                    bkinternals.ForEach(bkinternal =>
                                    {
                                        bkinternal.Status = (int)StatusFlight.TakeOff;
                                        var ladingIds = new int[0];
                                        var packageOfLadingIds = new int[0];
                                        if (!string.IsNullOrEmpty(bkinternal.ListLadingId))
                                        {
                                            ladingIds = bkinternal.ListLadingId.Replace("//", ",").Split(',').Select(int.Parse).ToArray();
                                        }
                                        if (!string.IsNullOrEmpty(bkinternal.PackageOfLadingIds))
                                        {
                                            packageOfLadingIds = bkinternal.PackageOfLadingIds.Replace("//", ",").Split(',').Select(int.Parse).ToArray();
                                        }
                                        #region Kiện
                                        Parallel.ForEach(packageOfLadingIds, (_id) =>
                                        {
                                            using (var context = new ApplicationDbContext())
                                            {
                                                var packageOfLadingRepository = new PackageOfLadingRepository(context);
                                                var packageOfLading = packageOfLadingRepository.GetFirst(o => _id == o.Id && o.State == 0);
                                                if (packageOfLading != null)
                                                {
                                                    packageOfLading.StatusId = statusUpdate;
                                                    packageOfLadingRepository.Update(currentUserId, currentPOId, packageOfLading).Wait();
                                                }
                                            }
                                        });
                                        #endregion
                                        #region Vận đơn
                                        Parallel.ForEach(ladingIds, (_id) =>
                                        {
                                            using (var context = new ApplicationDbContext())
                                            {
                                                var ladingRepository = new LadingRepository(context);
                                                var lading = ladingRepository.GetFirst(o => _id == o.Id && statusRollbacks.Contains(o.Status.Value));
                                                if (lading != null)
                                                {
                                                    lading.Status = statusUpdate;
                                                    lading.ModifiedBy = currentUserId;
                                                    lading.ModifiedDate = DateTime.Now;
                                                    lading.POCurrent = currentPOId;
                                                    ladingRepository.SaveChanges();
                                                    LadingHistory ladingHistory = new LadingHistory
                                                    {
                                                        CreatedDate = DateTime.Now,
                                                        DateTime = model.RealDateTime ?? new DateTime(),
                                                        LadingId = lading.Id,
                                                        Location = officer.PostOffice.POName,
                                                        OfficerId = currentUserId,
                                                        PostOfficeId = currentPOId,
                                                        Status = statusUpdate
                                                    };
                                                    context.LadingHistories.Add(ladingHistory);
                                                    context.SaveChanges();
                                                }
                                            }
                                        });
                                        #endregion
                                        Parallel.ForEach(ladingIds, (_id) => Task.Factory.StartNew(async () =>
                                        {
                                            using (var context = new ApplicationDbContext())
                                            {
                                                var packageOfLadingRepository = new PackageOfLadingRepository(context);
                                                await packageOfLadingRepository.UpdateIsPartStatusLading((int)_id);
                                            }
                                        }));
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
                            var officer = _context.Set<Officer>().Include(o => o.PostOffice).Single(o => o.OfficerID == model.ModifiedBy);
                            if (officer.PostOffice == null)
                            {
                                result.Message = "Không tìm thấy thông tin bưu cục của nhân viên đang thao tác";
                            }
                            else
                            {
                                flight.MAWB.ReceivedRealDateTime = model.ReceivedRealDateTime ?? new DateTime();
                                flight.FlightStatus = (int)StatusFlight.Landing;
                                var currentUserId = officer.OfficerID;
                                var currentPOId = officer.PostOfficeId ?? 0;
                                var statusUpdate = (int)StatusLading.DenTrungTamSanBayNhan;
                                var statusRollbacks = new int[] { (int)StatusLading.RoiTrungTamBay };

                                var bkinternals = _context.BKInternals.Where(o => o.FlightId == flight.Id).ToList();
                                bkinternals.ForEach(bkinternal =>
                                {
                                    bkinternal.Status = (int)StatusFlight.Landing;
                                    var ladingIds = new int[0];
                                    var packageOfLadingIds = new int[0];
                                    if (!string.IsNullOrEmpty(bkinternal.ListLadingId))
                                    {
                                        ladingIds = bkinternal.ListLadingId.Replace("//", ",").Split(',').Select(int.Parse).ToArray();
                                    }
                                    if (!string.IsNullOrEmpty(bkinternal.PackageOfLadingIds))
                                    {
                                        packageOfLadingIds = bkinternal.PackageOfLadingIds.Replace("//", ",").Split(',').Select(int.Parse).ToArray();
                                    }
                                    #region Kiện
                                    Parallel.ForEach(packageOfLadingIds, (_id) =>
                                    {
                                        using (var context = new ApplicationDbContext())
                                        {
                                            var packageOfLadingRepository = new PackageOfLadingRepository(context);
                                            var packageOfLading = packageOfLadingRepository.GetFirst(o => _id == o.Id && o.State == 0);
                                            if (packageOfLading != null)
                                            {
                                                packageOfLading.StatusId = statusUpdate;
                                                packageOfLadingRepository.Update(currentUserId, currentPOId, packageOfLading).Wait();
                                            }
                                        }
                                    });
                                    #endregion
                                    #region Vận đơn
                                    Parallel.ForEach(ladingIds, (_id) =>
                                    {
                                        using (var context = new ApplicationDbContext())
                                        {
                                            var ladingRepository = new LadingRepository(context);
                                            var lading = ladingRepository.GetFirst(o => _id == o.Id && statusRollbacks.Contains(o.Status.Value));
                                            if (lading != null)
                                            {
                                                lading.Status = statusUpdate;
                                                lading.ModifiedBy = currentUserId;
                                                lading.ModifiedDate = DateTime.Now;
                                                lading.POCurrent = currentPOId;
                                                ladingRepository.SaveChanges();
                                                LadingHistory ladingHistory = new LadingHistory
                                                {
                                                    CreatedDate = DateTime.Now,
                                                    DateTime = model.ReceivedRealDateTime ?? new DateTime(),
                                                    LadingId = lading.Id,
                                                    Location = officer.PostOffice.POName,
                                                    OfficerId = currentUserId,
                                                    PostOfficeId = currentPOId,
                                                    Status = statusUpdate
                                                };
                                                context.LadingHistories.Add(ladingHistory);
                                                context.SaveChanges();
                                            }
                                        }
                                    });
                                    #endregion
                                    Parallel.ForEach(ladingIds, (_id) => Task.Factory.StartNew(async () =>
                                    {
                                        using (var context = new ApplicationDbContext())
                                        {
                                            var packageOfLadingRepository = new PackageOfLadingRepository(context);
                                            await packageOfLadingRepository.UpdateIsPartStatusLading((int)_id);
                                        }
                                    }));
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
        public async Task<ResultModel<Flight>> AddOrEdit(FlightModel model)
        {
            var result = new ResultModel<Flight>();
            try
            {
                var officer = _context.Set<Officer>().Include(o => o.PostOffice).Single(o => o.OfficerID == model.ModifiedBy);
                var currentPOId = officer.PostOfficeId;
                var currentPO = officer.PostOffice;
                var currentUserId = officer.OfficerID;
                var now = DateTime.Now;
                bool insert = true;
                var statusUpdate = (int)StatusLading.TaoChuyenBay;
                var statusRollbacks = new int[] { (int)StatusLading.SBXN };

                var bkInternalIds = model.BKInternalIds ?? new int[0];
                var bkInternalIdNews = bkInternalIds;
                var bkInternalIdDels = new int[0];

                var mawb = _context.MAWBs.SingleOrDefault(o => o.Id == model.MAWBId);
                if (mawb == null)
                {
                    return result.Init(message: "Không tìm thấy AWB.");
                }
                if (bkInternalIds.Count() == 0)
                {
                    return result.Init(message: "Không tìm thấy danh sách bảng kê cần trung chuyển.");
                }
                if (_context.BKInternals.Any(o => bkInternalIds.Contains(o.ID_BK_internal) && !o.IsFly.Value && o.Status != (int)StatusFlight.Confirmed))
                {
                    return result.Init(message: "Danh sách bảng kê bay không nằm trong trạng thái Bộ phận sân bay xác nhận");
                }
                model.POTo = mawb.PoTo;
                if (model.POTo == 0)
                {
                    return result.Init(message: "Không tìm trung tâm (sân bay) nhận.");
                }
                var flight = new Flight();
                if (model.Id > 0)
                {
                    insert = false;
                    flight = await this.GetSingleAsync(o => o.Id == model.Id, inc => inc.MAWB, inc => inc.Status);
                    if (flight == null)
                    {
                        return result.Init(message: "Không tìm thấy thông tin chuyến bay");
                    }
                    if ((flight.FlightStatus ?? 0) != (int)StatusFlight.Created)
                    {
                        return result.Init(message: "Chuyến bay không được phép sửa.");
                    }
                    if (!string.IsNullOrEmpty(flight.ListPackage))
                    {
                        var bkInternalIdOlds = flight.ListPackage.Replace("//", ",").Split(',').Select(int.Parse).ToArray();
                        bkInternalIdDels = bkInternalIdOlds.Except(bkInternalIds).ToArray();
                        bkInternalIdNews = bkInternalIds.Except(bkInternalIdOlds).ToArray();
                    }
                    if (flight.MAWBId.HasValue && flight.MAWBId != mawb.Id)
                    {
                        if (!(mawb.IsNew ?? true))
                            return result.Init(message: "Lịch bay đã được sử dụng");
                        var mAWBOld = _context.MAWBs.SingleOrDefault(o => o.Id == flight.MAWBId.Value);
                        if (mAWBOld != null)
                        {
                            mAWBOld.IsNew = true;
                            mAWBOld.RealWeight = 0;
                        }
                    }
                }
                flight.State = 1;
                flight.TotalPackage = bkInternalIds.Count();
                flight.TotalWeight = Math.Round(_context.BKInternals.Where(x => bkInternalIds.Contains(x.ID_BK_internal)).Select(x => x.TotalWeight).Sum() ?? 0, 1);
                flight.TotalWeightToPrice = Math.Round(_context.BKInternals.Where(x => bkInternalIds.Contains(x.ID_BK_internal)).Select(x => x.TotalWeightToPrice).Sum() ?? 0, 1);
                flight.FlightStatus = (int)StatusFlight.Created;
                flight.ModifiedBy = currentUserId;
                flight.ModifiedDate = now;
                flight.ListPackage = string.Join("//", bkInternalIds);
                flight.Note = model.Note;
                flight.POTo = model.POTo;
                flight.FlightMAWB = mawb.MAWBCode;
                flight.MAWBId = model.MAWBId;
                mawb.IsNew = false;
                mawb.RealWeight = flight.TotalWeight;
                var flightNumber = mawb.FlightNumber;
                if (insert)
                {
                    flight.Create_By = currentUserId;
                    flight.Date_Create = now;
                    flight.POFrom = currentPOId;
                    this.Insert(flight);
                    this.SaveChanges();
                    flight.FlightCode = "AIR" + Helper.Helper.GetCodeWithMinLegth(flight.Id, 6);
                }
                var ladingIdJoineds = new List<int>();
                #region Bảng kê mới
                var bkInternalNews = _context.BKInternals.Where(o => o.State == 0 && bkInternalIdNews.Contains(o.ID_BK_internal));
                using (var packageOfLadingHistories = new BlockingCollection<PackageOfLadingHistory>())
                using (var ladingHistories = new BlockingCollection<LadingHistory>())
                {
                    foreach (var bkInternal in bkInternalNews)
                    {
                        bkInternal.Status = (int)StatusBK.ChuyenBayMoiTao;
                        bkInternal.FlightId = flight.Id;

                        var ladingIds = new int[0];
                        var packageOfLadingIds = new int[0];
                        if (!string.IsNullOrEmpty(bkInternal.ListLadingId))
                        {
                            ladingIds = bkInternal.ListLadingId.Replace("//", ",").Split(',').Select(int.Parse).ToArray();
                            ladingIdJoineds.AddRange(ladingIds);
                        }
                        if (!string.IsNullOrEmpty(bkInternal.PackageOfLadingIds))
                        {
                            packageOfLadingIds = bkInternal.PackageOfLadingIds.Replace("//", ",").Split(',').Select(int.Parse).ToArray();
                        }
                        #region Kiện
                        var pakageOfLadings = _context.PackageOfLadings.Where(x => x.State == 0 && packageOfLadingIds.Contains(x.Id));
                        await pakageOfLadings.ForEachAsync(packageOfLading =>
                        {
                            packageOfLading.StatusId = statusUpdate;
                            packageOfLading.FlightId = flight.Id;
                            packageOfLading.POCurrentId = currentPOId;
                            packageOfLading.ModifiedDate = now;
                            packageOfLading.ModifiedBy = currentUserId;
                            PackageOfLadingHistory packageOfLadingHistory = new PackageOfLadingHistory
                            {
                                CreatedDate = now,
                                ModifiedDate = now,
                                ModifiedBy = currentUserId,
                                CreatedBy = currentUserId,
                                PackageOfLadingId = packageOfLading.Id,
                                Location = officer.PostOffice.POName,
                                POCreated = currentPOId,
                                StatusId = statusUpdate,
                                Note = "Số hiệu chuyến bay: " + flightNumber
                            };
                            packageOfLadingHistories.Add(packageOfLadingHistory);
                        });
                        #endregion
                        #region Vận đơn
                        var ladings = _context.Ladings.Where(x => x.State == 0 && ladingIds.Contains((int)x.Id));
                        await ladings.ForEachAsync(lading =>
                        {
                            lading.Status = statusUpdate;
                            lading.FlightId = flight.Id;
                            lading.ModifiedBy = currentUserId;
                            lading.ModifiedDate = now;
                            lading.POCurrent = currentPOId;
                            LadingHistory ladingHistory = new LadingHistory
                            {
                                CreatedDate = now,
                                DateTime = now,
                                LadingId = lading.Id,
                                Location = officer.PostOffice.POName,
                                OfficerId = currentUserId,
                                PostOfficeId = currentPOId,
                                Status = statusUpdate,
                                Note = "Số hiệu chuyến bay: " + flightNumber
                            };
                            ladingHistories.Add(ladingHistory);
                        });
                        #endregion
                    }
                    #endregion
                    #region Bảng kê cũ
                    var bkInternalDels = _context.BKInternals.Where(o => o.State == 0 && bkInternalIdDels.Contains(o.ID_BK_internal));
                    foreach (var bkInternal in bkInternalDels)
                    {
                        bkInternal.Status = (int)StatusBK.TramGuiBPSBXN;
                        bkInternal.FlightId = null;
                        //update lading
                        var ladingIds = new int[0];
                        var packageOfLadingIds = new int[0];
                        if (!string.IsNullOrEmpty(bkInternal.ListLadingId))
                        {
                            ladingIds = bkInternal.ListLadingId.Replace("//", ",").Split(',').Select(int.Parse).ToArray();
                            ladingIdJoineds.AddRange(ladingIds);
                        }
                        if (!string.IsNullOrEmpty(bkInternal.PackageOfLadingIds))
                        {
                            packageOfLadingIds = bkInternal.PackageOfLadingIds.Replace("//", ",").Split(',').Select(int.Parse).ToArray();
                        }
                        #region Kiện
                        var pakageOfLadings = _context.PackageOfLadings.Where(x => x.State == 0 && packageOfLadingIds.Contains(x.Id));
                        await pakageOfLadings.ForEachAsync(packageOfLading =>
                        {
                            packageOfLading.StatusId = statusRollbacks[0];
                            packageOfLading.FlightId = null;
                            packageOfLading.POCurrentId = currentPOId;
                            packageOfLading.ModifiedDate = now;
                            packageOfLading.ModifiedBy = currentUserId;
                            PackageOfLadingHistory packageOfLadingHistory = new PackageOfLadingHistory
                            {
                                CreatedDate = now,
                                ModifiedDate = now,
                                ModifiedBy = currentUserId,
                                CreatedBy = currentUserId,
                                PackageOfLadingId = packageOfLading.Id,
                                Location = officer.PostOffice.POName,
                                POCreated = currentPOId,
                                StatusId = statusUpdate,
                                Note = "Cập nhật gửi hàng bay: " + flightNumber
                            };
                            packageOfLadingHistories.Add(packageOfLadingHistory);
                        });
                        #endregion
                        #region Vận đơn
                        var ladings = _context.Ladings.Where(x => x.State == 0 && ladingIds.Contains((int)x.Id));
                        await ladings.ForEachAsync(lading =>
                        {
                            lading.Status = statusRollbacks[0];
                            lading.FlightId = null;
                            lading.ModifiedBy = currentUserId;
                            lading.ModifiedDate = now;
                            lading.POCurrent = currentPOId;
                            LadingHistory ladingHistory = new LadingHistory
                            {
                                CreatedDate = now,
                                DateTime = now,
                                LadingId = lading.Id,
                                Location = officer.PostOffice.POName,
                                OfficerId = currentUserId,
                                PostOfficeId = currentPOId,
                                Status = statusUpdate,
                                Note = "Cập nhật gửi hàng bay: " + flightNumber
                            };
                            ladingHistories.Add(ladingHistory);
                        });
                        #endregion
                    }
                    #endregion
                    flight.State = 0;
                    this.Update(flight);
                    _context.LadingHistories.AddRange(ladingHistories);
                    _context.PackageOfLadingHistories.AddRange(packageOfLadingHistories);
                    this.SaveChanges();
                    Parallel.ForEach(ladingIdJoineds.Distinct(), (_id) => Task.Factory.StartNew(async () =>
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            var packageOfLadingRepository = new PackageOfLadingRepository(context);
                            await packageOfLadingRepository.UpdateIsPartStatusLading((int)_id);
                        }
                    }));
                }
                result.Data = flight;
                result.Error = 0;
                result.Message = "Chốt bảng kê bay thành công. Mã bảng kê : " + flight.FlightCode;
                // Task.Factory.StartNew(() => InsertFightHistory(flight, currentUserId, currentPOId));
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                var msg = String.Format("Exception: {0} - {1}",
                           ex.InnerException, ex.Message);
            }
            return result;
        }
    }
}
