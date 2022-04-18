using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Concurrent;
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
        public async Task<ResultModel<dynamic>> Transporting(int currentUserId, string code)
        {
            var result = new ResultModel<dynamic>();
            var bkInternal = await this.GetFirstAsync(i => i.Code_BK_internal.Equals(code));
            if (bkInternal == null || string.IsNullOrEmpty(bkInternal.ListLadingId) || bkInternal.ListLadingId.Replace("//", ";").Split(';').ToArray().Length == 0)
            {
                return result.Init(message: "Không tìm thấy bảng kê");
            }
            var statusRollback = (int)StatusLading.XuatKhoTrungChuyen;
            var statusUpdate = (int)StatusLading.DangTrungChuyen;
            var isFly = (bkInternal.IsFly ?? false);
            var poConfirm = bkInternal.POCreate.Value;
            var allowConfirm = true;//Được phép nhận hàng
            var confirmTo = false;//Xác nhận bởi trạm gửi
            var isDelivery = true;

            var postOfficeRepository = new PostOfficeRepository(_context);
            var locationRepository = new LocationRepository(_context);
            var officerRepository = new OfficerRepository(_context);
            var bkInternalHistoryRepository = new BKInternalHistoryRepository(_context);

            var officer = _context.Set<Officer>().Include(o => o.PostOffice).Single(o => o.OfficerID == currentUserId);
            int currentPOId = officer.PostOfficeId.Value;
            var ladingIds = bkInternal.ListLadingId.Replace("//", ";").Split(';').Select(long.Parse).ToArray();
            var packageOfLadingIds = new int[0];
            var packageIds = new int[0];

            if (!string.IsNullOrEmpty(bkInternal.PackageOfLadingIds))
            {
                packageOfLadingIds = bkInternal.PackageOfLadingIds.Replace("//", ";").Split(';').Select(int.Parse).ToArray();
            }
            if (!string.IsNullOrEmpty(bkInternal.PackageIds))
            {
                packageIds = bkInternal.PackageIds.Replace("//", ";").Split(';').Select(int.Parse).ToArray();
            }
            var queryPackageOfLading = _context.PackageOfLadings.Where(o => packageOfLadingIds.Contains(o.Id) && o.State == 0);
            var ladingIdExpecteds = queryPackageOfLading.Where(x => x.LadingId.HasValue).Select(x => x.LadingId.ToString()).Distinct().ToArray().Select(long.Parse) ?? new long[0];

            var queryLading = _context.Ladings.Where(o => ladingIds.Contains(o.Id) && !ladingIdExpecteds.Contains(o.Id) && o.State == 0);

            switch (bkInternal.Status)
            {
                case (int)StatusBK.DaTao:
                    bkInternal.Status = (int)StatusBK.TramGuiDangTrungChuyen;
                    break;
                case (int)StatusBK.TramGuiDangTrungChuyen:
                    if (!queryPackageOfLading.Any(o => o.StatusId == (int)StatusLading.XuatKhoTrungChuyen) &&
                        !queryLading.Any(o => o.Status == (int)StatusLading.XuatKhoTrungChuyen))
                    {
                        if (!isFly) allowConfirm = false;
                        else
                        {
                            isDelivery = false;
                            bkInternal.Status = (int)StatusBK.TramGuiBPSBXN;
                            statusRollback = (int)StatusLading.DangTrungChuyen;
                            statusUpdate = (int)StatusLading.SBXN;
                        }
                    }
                    break;
                case (int)StatusBK.DenSanBay:
                    confirmTo = true;
                    poConfirm = bkInternal.PostOfficeId.Value;
                    isDelivery = false;
                    bkInternal.Status = (int)StatusBK.TramNhanBPSBXN;
                    if (queryPackageOfLading.Any(o => o.StatusId == (int)StatusLading.DangTrungChuyen)
                        || queryLading.Any(o => o.Status == (int)StatusLading.DangTrungChuyen))
                    {
                        statusRollback = (int)StatusLading.DangTrungChuyen;
                        statusUpdate = (int)StatusLading.SBXN;
                    }
                    else
                    {
                        statusRollback = (int)StatusLading.DenTrungTamSanBayNhan;
                        statusUpdate = (int)StatusLading.SBXN;
                    }
                    break;
                case (int)StatusBK.TramNhanBPSBXN:
                    confirmTo = true;
                    poConfirm = bkInternal.PostOfficeId.Value;

                    bkInternal.Status = (int)StatusBK.TramNhanDangTrungChuyen;
                    if (queryPackageOfLading.Any(o => o.StatusId == (int)StatusLading.DenTrungTamSanBayNhan)
                      || queryLading.Any(o => o.Status == (int)StatusLading.DenTrungTamSanBayNhan))
                    {
                        statusRollback = (int)StatusLading.DenTrungTamSanBayNhan;
                        statusUpdate = (int)StatusLading.SBXN;
                    }
                    else
                    {
                        statusRollback = (int)StatusLading.SBXN;
                        statusUpdate = (int)StatusLading.DangTrungChuyen;
                    }
                    break;
                case (int)StatusBK.TramNhanDangTrungChuyen:
                    confirmTo = true;
                    poConfirm = bkInternal.PostOfficeId.Value;
                    bkInternal.Status = (int)StatusBK.TramNhanDangTrungChuyen;
                    if (queryPackageOfLading.Any(o => o.StatusId == (int)StatusLading.SBXN)
                      || queryLading.Any(o => o.Status == (int)StatusLading.SBXN))
                    {
                        statusRollback = (int)StatusLading.SBXN;
                        statusUpdate = (int)StatusLading.DangTrungChuyen;
                    }
                    else
                    {
                        allowConfirm = false;
                    }
                    break;
                default:
                    allowConfirm = false;
                    break;
            }
            if (!allowConfirm)
            {
                return result.Init(message: "Bảng kê không đúng trạng thái");
            }
            if (!(await postOfficeRepository.SameCenter(poConfirm, officer.PostOfficeId.Value)))
            {
                return result.Init(message: "Bảng kê này phải được xác nhận bởi nhân viên chịu sự quản lý của TRẠM " + (confirmTo ? "NHẬN" : "GỬI"));
            }
            if (!isDelivery)
            {
                var po = await postOfficeRepository.GetFirstAsync(o => o.PostOfficeID == officer.PostOfficeId);
                if (po == null || !(po.IsCenter ?? false)
                || !bkInternal.DepartmentConfirmId.HasValue || !bkInternal.JobConfirmId.HasValue || !officer.DeparmentID.HasValue || !officer.JobId.HasValue ||
                    (bkInternal.DepartmentConfirmId != officer.DeparmentID && bkInternal.JobConfirmId != officer.JobId))
                {
                    return result.Init(message: "Bảng kê này phải được xác nhận bởi BỘ PHẬN KHAI THÁC BAY của TRẠM " + (confirmTo ? "NHẬN" : "GỬI"));
                }
            }
            else if (!officerRepository.IsDelivery(currentUserId))
            {
                return result.Init(message: "Bảng kê này phải được xác nhận bởi BỘ PHẬN GIAO NHẬN của TRẠM " + (confirmTo ? "NHẬN" : "GỬI"));
            }

            try
            {
                int succeeded = queryPackageOfLading.Count(o => o.StatusId == statusRollback) + queryLading.Count(o => o.Status == statusRollback);
                int scanned = queryPackageOfLading.Count(o => o.StatusId == statusUpdate) + queryLading.Count(o => o.Status == statusUpdate);
                int failed = queryPackageOfLading.Count() + queryLading.Count() - succeeded - scanned;

                using (var packageOfLadingHistories = new BlockingCollection<PackageOfLadingHistory>())
                using (var ladingHistories = new BlockingCollection<LadingHistory>())
                using (var packageHistories = new BlockingCollection<PackageHistory>())
                using (var ladingIdsSuccess = new BlockingCollection<long>())
                {
                    #region Kiện
                    var pakageOfLadings = _context.PackageOfLadings.Where(x => x.State == 0 && packageOfLadingIds.Contains(x.Id) && statusRollback == x.StatusId);
                    pakageOfLadings.ForEach(packageOfLading =>
                    {
                        EntityHelper.UpdatePackageOfLading(packageOfLading, currentUserId, currentPOId, statusUpdate);
                        var packageOfLadingHistory = EntityHelper.GetPackageOfLadingHistory(currentUserId, currentPOId, packageOfLading.Id, packageOfLading.StatusId ?? 0);
                        packageOfLadingHistories.Add(packageOfLadingHistory);
                    });
                    #endregion
                    #region Vận đơn
                    var ladings = _context.Ladings.Where(x => x.State == 0 && ladingIds.Contains(x.Id) && statusRollback == x.Status);
                    ladings.ForEach(lading =>
                    {
                        lading.OfficerTransferId = currentUserId;
                        EntityHelper.UpdateLading(lading, currentUserId, currentPOId, statusUpdate);
                        var ladingHistory = EntityHelper.GetLadingHistory(currentUserId, currentPOId, lading.Id, lading.Status ?? 0, officer.LatDynamic, officer.LngDynamic);
                        ladingHistories.Add(ladingHistory);
                        ladingIdsSuccess.Add(lading.Id);
                    });
                    #endregion
                    #region Gói
                    var packages = _context.Packages.Where(o => packageIds.Contains(o.PackageID));
                    packages.ForEach(x =>
                    {
                        EntityHelper.UpdatePackage(x, currentUserId, currentPOId, EntityHelper.ConvertToPackageStatus(statusUpdate, !confirmTo));
                        var packageHistory = EntityHelper.GetPackageHistory(currentUserId, currentPOId, x.PackageID, x.Status ?? 0, x.LadingIDs, x.TotalLading, x.TotalNumber, x.TotalWeight);
                        packageHistories.Add(packageHistory);
                    });
                    #endregion
                    _context.LadingHistories.AddRange(ladingHistories);
                    _context.PackageOfLadingHistories.AddRange(packageOfLadingHistories);
                    _context.PackageHistories.AddRange(packageHistories);

                    this.SaveChanges();

                    if (succeeded > 0 || failed == 0)
                    {
                        bkInternal.IsConfirmByOfficer = true;
                        bkInternal.OfficerConfirmId = currentUserId;
                        #region Thêm lịch sử bảng kê
                        var bkhistory = new BKInternalHistory
                        {
                            BK_internalId = bkInternal.ID_BK_internal,
                            CreatedBy = currentUserId,
                            CreatedDate = DateTime.Now,
                            LadingIds = string.Join("//", ladingIdsSuccess),
                            PackageOfLadingIds = string.Join("//", packageOfLadingIds),
                            TotalLading = succeeded,
                            Status = bkInternal.Status,
                            Note = "Nhân viên xác nhận"
                        };
                        bkInternalHistoryRepository.Insert(bkhistory);
                        #endregion
                        this.SaveChanges();
                    }
                    Parallel.ForEach(ladingIdExpecteds, (id) => Task.Factory.StartNew(async () =>
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            var packageOfLadingRepository = new PackageOfLadingRepository(context);
                            await packageOfLadingRepository.UpdateIsPartStatusLading((int)id);
                        }
                    }));
                }
                result.Data = new { Success = succeeded, Fail = failed, Scanned = scanned };
                result.Error = 0;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
    }

}