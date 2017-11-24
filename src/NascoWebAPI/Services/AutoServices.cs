using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Helper;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using NascoWebAPI.Models;

namespace NascoWebAPI.Data
{
    public class AutoServices
    {
        private readonly ILadingRepository _iLadingRepository;
        private readonly ILadingHistoryRepository _iLadingHistoryRepository;
        private readonly IDepartmentRepository _iDepartmentRepository;
        private readonly IOfficerRepository _iOfficerRepository;
        private readonly IPostOfficeRepository _iPostOfficeRepository;


        private const int _STATUS_PickUp = (int)StatusLading.ChoLayHang;

        private static int[] _STATUS_LATLNG_USER = { (int)StatusLading.DaLayHang, (int)StatusLading.DangTrungChuyen,
                                                        (int)StatusLading.DangPhat};
        private static int[] _STATUS_LATLNG_POCURRENT = { (int)StatusLading.KhaiThacThongTin, (int)StatusLading.ChuaNhapKho,
                                                        (int)StatusLading.NhapKho, (int)StatusLading.XuatKho, (int)StatusLading.PhatKhongTC };
        private static int[] _STATUS_LATLNG_TO = { (int)StatusLading.DaChuyenHoan, (int)StatusLading.ThanhCong };

        private static int[] _STATUS_LATLNG_AUTO_PICKUP = { (int)StatusLading.NVKhongNhan };

        public AutoServices(ApplicationDbContext context)
        {
            _iLadingRepository = new LadingRepository(context);
            _iOfficerRepository = new OfficerRepository(context);
            _iDepartmentRepository = new DepartmentRepository(context);
            _iPostOfficeRepository = new PostOfficeRepository(context);
            _iLadingHistoryRepository = new LadingHistoryRepository(context);
        }

        public AutoServices(ApplicationDbContext context,
            ILadingRepository iLadingRepository,
            IOfficerRepository iOfficerRepository,
            ILadingHistoryRepository iLadingHistoryRepository,
            IDepartmentRepository iDepartmentRepository,
            IPostOfficeRepository iPostOfficerRepository
            ) 
        {
            _iLadingRepository = iLadingRepository;
            _iLadingHistoryRepository = iLadingHistoryRepository;
            _iOfficerRepository = iOfficerRepository;
            _iDepartmentRepository = iDepartmentRepository;
            _iPostOfficeRepository = iPostOfficerRepository;
        }

        public async Task<JsonResult> GetPickUpAuto(int poid)
        {
            var ladings = await _iLadingRepository.GetAsync(r => _STATUS_LATLNG_AUTO_PICKUP.Contains((int)r.Status));
            if (poid != 0)
            {
                ladings = ladings.Where(r => r.POCurrent == poid);
            }
            List<Notification> notifications = new List<Notification>();
            foreach (var lading in ladings)
            {
                var ladinghistory = await _iLadingHistoryRepository.GetAsync(r => r.LadingId == lading.Id);
                var departments = await _iDepartmentRepository.GetAsync(d => d.PostOfficeID == lading.POCurrent);
                if (departments != null)
                {
                    var officers = await _iOfficerRepository.GetAsync(r => ladinghistory.Any(h => h.OfficerId != r.OfficerID && h.Status == (int)StatusLading.NVKhongNhan) && departments.Any(s => s.DeparmentID == r.DeparmentID) && r.State == (int)StatusSystem.Enable && r.Types == 1);
                    if (officers != null)
                    {
                        dynamic minDistance = new ExpandoObject();
                        minDistance.Distance = 10000;
                        minDistance.Id = 0;
                        string token = "";
                        foreach (var officer in officers)
                        {
                            if (officer.LatDynamic.IsNumeric() && officer.LngDynamic.IsNumeric())
                            {
                                var distance = GeoHelper.distance(lading.LatFrom.Value, lading.LngFrom.Value, Convert.ToDouble(officer.LatDynamic), Convert.ToDouble(officer.LngDynamic), 'K');
                                if (minDistance.Distance > distance)
                                {
                                    minDistance.Distance = distance;
                                    minDistance.Id = officer.OfficerID;
                                    token = officer.InstanceIDToken;
                                }
                            }
                        }
                        // KO CÓ NHÂN VIÊN NÀO ĐI LẤY HÀNG
                        if (minDistance.Id == 0)
                        {
                            lading.Status = 19;//Ko có nhân viên đi lấy hàng cần phải xử lý
                            _iLadingRepository.UpdateAndInsertLadingHistory(lading, null);
                        }
                        else
                        {
                            lading.Status = (int)StatusLading.ChoLayHang;
                            lading.OfficerPickup = minDistance.Id;
                            _iLadingRepository.UpdateAndInsertLadingHistory(lading, null);
                            Notification gNoti = notifications.Where(r => r.OfficerID == minDistance.Id).FirstOrDefault();
                            if (gNoti != null)
                            {
                                gNoti.Badger = (gNoti.Badger + 1);
                            }
                            else
                            {
                                Notification notification = new Notification();
                                notification.OfficerID = minDistance.Id;
                                notification.Badger = 1;
                                notification.Token = token;
                                notifications.Add(notification);
                            }
                        }
                    }
                }
            }
            foreach (var noti in notifications)
            {
                GCM.SendNotification(noti.Token, string.Format("Bạn đang có '{0}' đơn hàng mới cần đi lấy.", noti.Badger), noti.Badger);
            }
            return null;
        }

        public async Task<JsonResult> RePickupLadingAuto(int poid)
        {
            var ladings = await _iLadingRepository.GetAsync(r => r.Status == (int)StatusLading.ChoLayHang);
            if (poid != 0)
            {
                ladings = ladings.Where(r => r.POCurrent == poid);
            }
            foreach (var lading in ladings)
            {
                var ladinghistorys = await _iLadingHistoryRepository.GetAsync(r => r.LadingId == lading.Id);
                var lastid = ladinghistorys.Max(m => m.Id);
                var ladthistory = ladinghistorys.Where(r => r.Id == lastid).FirstOrDefault();
                DateTime lastTime = Convert.ToDateTime(ladthistory.DateTime);
                DateTime dNow = DateTime.Now;
                if (lastTime.AddMinutes(2) < dNow)
                {
                    lading.Status = (int)StatusLading.NVKhongNhan;//Ko có nhân viên đi lấy hàng cần phải xử lý
                    _iLadingRepository.UpdateAndInsertLadingHistory(lading, null);
                }
                //                
            }
            return null;
        }
    }
}