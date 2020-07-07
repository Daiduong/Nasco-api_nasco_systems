using Microsoft.AspNetCore.Mvc;
using NascoWebAPI.Data.Entities;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public interface IOfficerRepository : IRepository<Officer>
    {
        Task<int?> GetPOIdByOffice(int officeId);
        bool IsDelivery(int officeId);
        IEnumerable<Menu> GetListMenuByOfficer(int officerId);
    }
    public interface ILadingRepository : IRepository<Lading>
    {
        void InsertAndInsertLadingHistory(Lading entity);
        void UpdateAndInsertLadingHistory(Lading entity, Officer currentUser, int? typeReasonID = null, string location = null, string note = "");
        Task<LadingHistory> GetLastLadingHistoryAsync(long ladingId);
        Task<IEnumerable<Lading>> GetLadingReport(int officerID, int reportType);
        Task<double> GetSumLadingReport(int officerID, int reportType);
        string CodeGenerationByLocationCode(int locationId, int id);
        bool EqualsCode(string code);

        Task<ResultModel<Lading>> InsertEMS(int currentUserId, int customerContactId, int serviceId, string code, int statusId = (int)StatusLading.DaLayHang);
        Task<ResultModel<dynamic>> UpdateEMS();
        Task<ResultModel<dynamic>> UpdateToPartner(int partnerId, int ladingId, int poCurrentId, int statusId, DateTime? datetime);
    }
    public interface ILadingHistoryRepository : IRepository<LadingHistory>
    {
        Task<IEnumerable<LadingHistory>> GetByUser(int officerID, DateTime? dateFrom, DateTime? dateTo, int? skip = null, int? take = null, params Expression<Func<LadingHistory, object>>[] includeProperties);
        Task<IEnumerable<LadingHistory>> GetDistinctLading(DateTime? dateFrom, DateTime? dateTo, int? skip = null, int? take = null, params Expression<Func<LadingHistory, object>>[] includeProperties);
    }
    public interface IBKInternalRepository : IRepository<BKInternal>
    {
        Task<IEnumerable<BKInternal>> GetListWaitingByOfficer(int officerID, params Expression<Func<BKInternal, object>>[] includeProperties);
        Task<ResultModel<dynamic>> Transporting(int currentUserId, string code);
    }
    public interface IDepartmentRepository : IRepository<Deparment> { }
    public interface IPostOfficeRepository : IRepository<PostOffice>
    {
        Task<PostOffice> GetByOfficerId(int officeId);
        Task<IEnumerable<PostOffice>> GetByRootPO(int postOfficeId);
        Task<bool> SameCenter(int postOfficeId1, int postOfficeId2);
        Task<PostOffice> GetByDistanceMin(IEnumerable<PostOffice> postOffices, double latDistance, double lngDistance);
        Task<PostOffice> GetDistanceMinByLocation(int cityId, double lat, double lng, int? type);
        Task<IEnumerable<PostOffice>> GetListChild(int parentId);
        Task<IEnumerable<PostOffice>> GetListChild(PostOffice parent);
        Task<IEnumerable<PostOffice>> GetListChild(int parentId, int? postOfficeMethodId = null);
        Task<IEnumerable<PostOffice>> GetListParent(PostOffice child, int? level = 0);
        Task<IEnumerable<PostOffice>> GetListParent(int childId, int? level = 0);
        Task<IEnumerable<PostOffice>> GetListParent(int childId, int? level = 0, int? postOfficeMethodId = null);
        Task<IEnumerable<PostOffice>> GetListFromLevel(int id, int? level = 0);
        Task<IEnumerable<PostOffice>> GetListFromLevel(int id, int? level = 0, int? postOfficeTypeId = null);
        Task<IEnumerable<PostOffice>> GetListFromBranch(int id, int? postOfficeTypeId = null);
        Task<IEnumerable<PostOffice>> GetListFromCenter(int id, int? postOfficeTypeId = null);
        Task<IEnumerable<PostOffice>> GetListFromRoot(int? postOfficeMethodId = null);
        Task<PostOffice> GetBranch(int id);
        Task<IEnumerable<PostOffice>> GetListPostOfficeAirport(double lat, double lng);
    }
    public interface ITypeReasonRepository : IRepository<TypeReason> { }
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetCustomerByPhone(string phone);
        string GetCode(int id);
        Task<IEnumerable<int>> GetListIdByPartner(int partnerId);
        Task<IEnumerable<int>> GetListIdHasParner();
    }
    public interface ILocationRepository : IRepository<Location>
    {
        Task<IEnumerable<Location>> GetCities();
        Task<IEnumerable<Location>> GetDistrictsByCity(int cityId);
        int GetIdBestMatches(IEnumerable<Location> locations, string locationName, uint? percentlimitCost = null);
    }
    public interface IServiceRepository : IRepository<Service>
    {
        Task<IEnumerable<Service>> GetListSupportService();
        Task<IEnumerable<Service>> GetListMainService();
        Task<IEnumerable<Service>> GetListSupportServiceByTransport(int transportId);
        IEnumerable<Service> GetListMainServiceByLading(LadingViewModel model);
    }
    public interface IRecipientRepository : IRepository<Recipient> { }
    public interface IBKDeliveryRepository : IRepository<BKDelivery>
    {
        Task<ResultModel<BKDelivery>> Delivering(int currentUserId, string code);
    }
    public interface IStatusRepository : IRepository<LadingStatus> { }
    public interface ICODStatusRepository : IRepository<CODStatus> { }
    public interface ITransportRepository : IRepository<Transport> { }
    public interface IStructureRepository : IRepository<Structure>
    {
        Task<IEnumerable<Structure>> GetListByTransport(int transportId);
    }
    public interface IPriceRepository : IRepository<Price>
    {
        Task<ResultModel<ComputedPriceModel>> Computed(LadingViewModel lading);
        Task<ResultModel<ComputedPriceModel>> ComputedBox(LadingViewModel lading);
        Dictionary<int, double> GetListPrice(double weight, int customerId = 0, int state_from = 0, int state_to = 0, int receive_delivery = 0);
        double GetPrice(double weight, int serviceId = 0, int priceListId = 0, int state_from = 0, int state_to = 0, int receive_delivery = 0);
        Task<double> ComputedBox(double weight, int serviceId, int priceListId, int cityFromId, int cityToId, int districtFromId, int districtToId, int deliveryReceiveId, int? customerId = null, int? unitGroupId = null);
    }
    public interface IPriceListRepository : IRepository<PriceList>
    {
        Task<int> GetPriceApplyByPOID(int pOId);
        IEnumerable<PriceList> GetListPriceListByCustomer(int? customerId = null, bool union = true);
    }
    public interface ITypeOfPackRepository : IRepository<TypeOfPack> { }
    public interface ILadingMapServiceRepository : IRepository<LadingMapService>
    {
        Task AddOrEdit(int ladingId, List<ServiceOtherModel> models);
    }
    public interface IDeliveryReceiveRepository : IRepository<DeliveryReceive> { }
    public interface ITypePackRepository : IRepository<TypePack> { }
    public interface ILadingTempRepository : IRepository<LadingTemp> { }
    public interface ILadingTempHistoryRepository : IRepository<LadingTempHistory>
    {
        Task<IEnumerable<LadingTempHistory>> GetByUser(int officerID, DateTime? dateFrom, DateTime? dateTo, int? skip = null, int? take = null, params Expression<Func<LadingTempHistory, object>>[] includeProperties);
        Task<IEnumerable<LadingTempHistory>> GetDistinctLading(DateTime? dateFrom, DateTime? dateTo, int? skip = null, int? take = null, params Expression<Func<LadingTempHistory, object>>[] includeProperties);
    }
    public interface IProvideLadingRepository : IRepository<ProvideLading> { }
    public interface IGroupServiceRepository : IRepository<GroupService> { }
    public interface IAreaRepository : IRepository<Area> { }
    public interface IJobRepository : IRepository<Job> { }
    public interface IBKInternalHistoryRepository : IRepository<BKInternalHistory> { }
    public interface IFlightRepository : IRepository<Flight>
    {
        Task<IEnumerable<Flight>> GetListFlight(int? poFrom, int? poTo, int[] statusIds = null, int? pageSize = null, int? pageNo = null, string cols = null);
        Task<ResultModel<Flight>> TakeOff(FlightModel model);
        Task<ResultModel<Flight>> Receive(FlightModel model);
        Task<ResultModel<Flight>> AddOrEdit(FlightModel model);
    }
    public interface ICouponRepository : IRepository<Coupon>
    {
        ResultModel<object> GetDiscountAmount(string couponCode, ComputedPriceModel computedPriceModel, int? customerId);
        ResultModel<Coupon> Discount(string code, int currentOffficerId, int ladingId, double discountAmount);
    }
    public interface IPackageOfLadingRepository : IRepository<PackageOfLading>
    {
        Task Insert(List<NumberDIM> model, int ladingId, int currentUserId, int currentPOId);
        Task Update(int currentUserId, int currentPOId, int packageOfLadingId, int statusId, int? packageId = null, int? bkInternalId = null, int? bkDeliveryId = null, string note = "");
        Task Update(int currentUserId, int currentPOId, PackageOfLading packageOfLading, string note = "");
        Task UpdateIsPartStatusLading(int ladingId);
        Task InsertHistory(int currentUserId, int currentPOId, int packageOfLadingId, int? statusId, string note = "");
    }
    public interface ITimeLineRepository : IRepository<TimeLine>
    {
        Task<IEnumerable<ExpectedTimeModel>> GetListExpectedTime(int cityFromId, int cityToId, int serviceId, int poCurrentId, int? poToLevel = null, int? deliveryReceiveId = null);
    }
    public interface IMAWBRepository : IRepository<MAWB>
    {
        Task<IEnumerable<MAWBModel>> GetListMAWBNew(int poId);
        Task<ResultModel<MAWB>> AddOrEdit(MAWBModel model);
    }
    public interface IAirlineRepository : IRepository<Airline> { }
    public interface IReasonRepository : IRepository<Reason> { }
    public interface ICustomerContactRepository : IRepository<CustomerContact>
    {
        IQueryable<CustomerContact> GetListContactByCustomer(int id);
        IQueryable<CustomerContact> GetListContactByCustomer(int[] ids);
        IQueryable<CustomerContact> GetListContactByPartnerId(int id);
    }
    public interface IUnitGroupRepository : IRepository<UnitGroups>
    {
        List<Proc_GetAllUnitGroup_Result> _GetAllUnitGroup();
    }
}
