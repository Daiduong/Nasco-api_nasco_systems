using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Client;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using NascoWebAPI.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class BKDeliveryRepository : Repository<BKDelivery>, IBKDeliveryRepository
    {
        public BKDeliveryRepository(ApplicationDbContext conText) : base(conText)
        {
        }
        public async Task<ResultModel<BKDelivery>> Delivering(int currentUserId, string code)
        {
            var result = new ResultModel<BKDelivery>();
            var postOfficeRepository = new PostOfficeRepository(_context);
            var locationRepository = new LocationRepository(_context);
            var EMSService = new EMSService();

            var officer = _context.Set<Officer>().Include(o => o.PostOffice).Single(o => o.OfficerID == currentUserId);
            int currentPOId = officer.PostOfficeId ?? 0;
            var bkDelivery = await this.GetFirstAsync(i => i.Code_BK_Delivery.Equals(code) && i.State == 0);
            if (bkDelivery == null || bkDelivery.ListLadingId.Replace("//", ";").Split(';').ToArray().Length == 0)
            {
                return result.Init(message: "Không tìm thấy bảng kê phát");
            }
            var poes = await postOfficeRepository.GetListFromCenter(bkDelivery.POCreate.Value);
            if (!poes.Any(o => o.PostOfficeID == currentPOId))
            {
                return result.Init(message: "Bảng kê phát không thuộc chi nhánh của bạn");
            }
            if (bkDelivery.Status == 0 || bkDelivery.Status == 700)
            {
                bkDelivery.Status = 701;//Xác nhận - đang phát
                bkDelivery.ModifiedDate = DateTime.Now;
                //bkDelivery.ModifiedBy = user.OfficerID;
                bkDelivery.OfficerConfirmId = currentUserId;
                this.SaveChanges();
            }

            var statusUpdate = (int)StatusLading.DangPhat;
            var statusRollbacks = new int[] { (int)StatusLading.XuatKho };

            var ladingIds = bkDelivery.ListLadingId.Replace("//", ";").Split(';').Select(long.Parse).ToArray();
            var packageOfLadingIds = new int[0];
            if (!string.IsNullOrEmpty(bkDelivery.PackageOfLadingIds))
            {
                packageOfLadingIds = bkDelivery.PackageOfLadingIds.Replace("//", ";").Split(';').Select(int.Parse).ToArray();
            }
            try
            {
                using (var ladingIdExpecteds = new BlockingCollection<int>())
                using (var packageOfLadingHistories = new BlockingCollection<PackageOfLadingHistory>())
                using (var ladingHistories = new BlockingCollection<LadingHistory>())
                using (var EMSDeliveries = new BlockingCollection<EMSLadingDeliveryRequest>())
                {
                    var pakageOfLadings = _context.PackageOfLadings.Where(x => x.State == 0 && packageOfLadingIds.Contains(x.Id) && statusRollbacks.Contains(x.StatusId ?? 0));
                    await pakageOfLadings.ForEachAsync(packageOfLading =>
                    {
                        packageOfLading.DeliveryBy = currentUserId;
                        EntityHelper.UpdatePackageOfLading(packageOfLading, currentUserId, currentPOId, statusUpdate);
                        var packageOfLadingHistory = EntityHelper.GetPackageOfLadingHistory(currentUserId, currentPOId, packageOfLading.Id, packageOfLading.StatusId ?? 0);
                        packageOfLadingHistories.Add(packageOfLadingHistory);
                        ladingIdExpecteds.Add(packageOfLading.LadingId.Value);
                    });
                    var ladings = _context.Ladings.Where(x => x.State == 0 && ladingIds.Contains(x.Id) && statusRollbacks.Contains(x.Status ?? 0)).Include(x => x.Sender);
                    await ladings.ForEachAsync(lading =>
                    {
                        lading.OfficerDelivery = officer.OfficerID;
                        EntityHelper.UpdateLading(lading, currentUserId, currentPOId, statusUpdate);
                        var ladingHistory = EntityHelper.GetLadingHistory(currentUserId, currentPOId, lading.Id, lading.Status ?? 0, officer.LatDynamic, officer.LngDynamic);
                        ladingHistories.Add(ladingHistory);
                        if (lading.Sender != null && lading.Sender.PartnerId == (int)EMSHelper.PARTNER_ID)
                        {
                            int cityId = ((lading.Return ?? false) ? lading.CitySendId : lading.CityRecipientId) ?? 0;
                            var city = locationRepository.GetFirst(x => x.LocationId == cityId);
                            EMSDeliveries.Add(new EMSLadingDeliveryRequest(lading.Code, EMSHelper.ConvertToEMSStatus(lading.Status ?? 0, lading.Return ?? false),
                               city?.LocationEMSId + "", city?.PostOffficeEMSCode, DateTime.Now, ""));
                        }
                    });

                    _context.LadingHistories.AddRange(ladingHistories);
                    _context.PackageOfLadingHistories.AddRange(packageOfLadingHistories);

                    this.SaveChanges();
                    Parallel.ForEach(ladingIdExpecteds, (id) => Task.Factory.StartNew(async () =>
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            var packageOfLadingRepository = new PackageOfLadingRepository(context);
                            await packageOfLadingRepository.UpdateIsPartStatusLading((int)id);
                        }
                    }));
                    Parallel.ForEach(EMSDeliveries, (obj) => Task.Factory.StartNew(async () =>
                    {
                        await EMSService.Delivery(obj);
                    }));
                }

                result.Data = bkDelivery;
                result.Message = $"Đã quét thành công bảng kê { bkDelivery.Code_BK_Delivery }. Tổng số vận đơn: {bkDelivery.TotalLading} . Tổng kiện: {bkDelivery.TotalNumber}";
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
