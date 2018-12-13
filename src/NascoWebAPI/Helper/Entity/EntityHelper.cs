using NascoWebAPI.Data;
using NascoWebAPI.Helper.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper
{
    public class EntityHelper
    {
        public static void UpdateLading(Lading obj, int currentUserId, int currentPOId, int statusId)
        {
            obj.POCurrent = currentPOId;
            obj.ModifiedDate = DateTime.Now;
            obj.ModifiedBy = currentUserId;
            obj.Status = statusId;
        }
        public static LadingHistory GetLadingHistory(int currentUserId, int currentPOId, long ladingId, int statusId, string lat = null, string lng = null, string location = null, DateTime? dateTime = null, string note = null)
        {
            double.TryParse(lat, out double dLat);
            double.TryParse(lng, out double dLng);
            return new LadingHistory
            {
                CreatedDate = DateTime.Now,
                DateTime = dateTime ?? DateTime.Now,
                LadingId = ladingId,
                Location = location,
                OfficerId = currentUserId,
                PostOfficeId = currentPOId,
                Status = statusId,
                Note = note,
                Lat = dLat,
                Lng = dLng
            };
        }
        public static void UpdatePackageOfLading(PackageOfLading obj, int currentUserId, int currentPOId, int statusId)
        {
            obj.POCurrentId = currentPOId;
            obj.ModifiedDate = DateTime.Now;
            obj.ModifiedBy = currentUserId;
            obj.StatusId = statusId;
        }
        public static PackageOfLadingHistory GetPackageOfLadingHistory(int currentUserId, int currentPOId, int packageOfLadingId, int statusId, string lat = null, string lng = null, string location = "", string note = "")
        {
            double.TryParse(lat, out double dLat);
            double.TryParse(lng, out double dLng);
            return new PackageOfLadingHistory
            {
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ModifiedBy = currentUserId,
                CreatedBy = currentUserId,
                PackageOfLadingId = packageOfLadingId,
                Location = location,
                POCreated = currentPOId,
                StatusId = statusId,
                Note = note,
                Lat = dLat,
                Lng = dLng,
                State = 0
            };
        }
        public static void UpdatePackage(Package obj, int currentUserId, int currentPOId, int statusId)
        {
            obj.PoCurrent = currentPOId;
            obj.ModifiedDate = DateTime.Now;
            obj.ModifiedBy = currentUserId;
            obj.Status = statusId;
        }
        public static PackageHistory GetPackageHistory(int currentUserId, int currentPOId, int packageId, int statusId, string ladingIds, int? totalLading = null, int? totalNumber = null, double? totalWeight = null, string note = null)
        {
            return new PackageHistory
            {
                CreatedDate = DateTime.Now,
                LadingIds = ladingIds,
                PackageId = packageId,
                TotalLading = totalLading,
                TotalNumber = totalNumber,
                TotalWeight = totalWeight,
                CreatedBy = currentUserId,
                PostOfficeId = currentPOId,
                Status = statusId,
                Note = note
            };
        }
        public static int ConvertToPackageStatus(int ladingStatusId, bool isFrom = true)
        {
            if (ladingStatusId == (int)StatusLading.DenTrungTamSanBayNhan)
                return (int)StatusPackage.Landing;
            if (ladingStatusId == (int)StatusLading.RoiTrungTamBay)
                return (int)StatusPackage.TakeOff;
            if (ladingStatusId == (int)StatusLading.TaoChuyenBay)
                return (int)StatusPackage.AllowedToFlight;
            if (ladingStatusId == (int)StatusLading.SBXN && isFrom)
                return (int)StatusPackage.ConfirmByAirportDPF;
            if (ladingStatusId == (int)StatusLading.SBXN && !isFrom)
                return (int)StatusPackage.ConfirmByAirportDPT;
            if (ladingStatusId == (int)StatusLading.XuatKhoTrungChuyen)
                return (int)StatusPackage.WaitingDepartureFromPO;
            if (ladingStatusId == (int)StatusLading.DangTrungChuyen && isFrom)
                return (int)StatusPackage.DepartureFromPO;
            if (ladingStatusId == (int)StatusLading.DangTrungChuyen && !isFrom)
                return (int)StatusPackage.WaitingArrivalAtPO;
            if (ladingStatusId == (int)StatusLading.NhapKho)
                return (int)StatusPackage.ArrivalAtPO;
            return 0;
        }
        public static Customer ConvertToCustomer(CustomerContact contact)
        {
            var customer = new Customer()
            {
                CityId = contact.CityId,
                DistrictId = contact.DistrictId,
                Lat = contact.Lat +"",
                Lng = contact.Lng + "",
                CustomerID = contact.Id,
                CustomerName = contact.ContactName,
                CustomerCode = contact.Code,
                Address = contact.ContactAddress,
                Phone = contact.ContactPhone,
                Email = contact.ContactEmail
            };
            return customer;
        }
    }
}
