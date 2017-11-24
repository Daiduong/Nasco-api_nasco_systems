using Microsoft.AspNetCore.Mvc;
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
        void UpdateAndInsertLadingHistory(Lading entity, int? typeReasonID = null, string location = null);
        Task<IEnumerable<Lading>> GetPickUpAsync(Officer officer);
        Task<LadingHistory> GetLastLadingHistoryAsync(long ladingId);
        Task<IEnumerable<Lading>> GetLadingReport(int officerID, int reportType);
        Task<double> GetSumLadingReport(int officerID, int reportType);
        string CodeGenerationByLocationCode(int locationId, int id);
        bool EqualsCode(string code);
    }
    public interface ILadingHistoryRepository : IRepository<LadingHistory>
    {
        Task<IEnumerable<LadingHistory>> GetByUser(int officerID, DateTime? dateFrom, DateTime? dateTo, int? skip = null, int? take = null, params Expression<Func<LadingHistory, object>>[] includeProperties);
        Task<IEnumerable<LadingHistory>> GetDistinctLading(DateTime? dateFrom, DateTime? dateTo, int? skip = null, int? take = null, params Expression<Func<LadingHistory, object>>[] includeProperties);
    }
    public interface IBKInternalRepository : IRepository<BKInternal>
    {
        Task<IEnumerable<BKInternal>> GetListWaitingByOfficer(int officerID, params Expression<Func<BKInternal, object>>[] includeProperties);
    }
    public interface IDepartmentRepository : IRepository<Deparment> { }
    public interface IPostOfficeRepository : IRepository<PostOffice>
    {
        Task<PostOffice> GetByOfficerId(int officeId);
        Task<IEnumerable<PostOffice>> GetByRootPO(int postOfficeId);
        Task<IEnumerable<PostOffice>> GetListPostOfficeInCenter(int postOfficeId);
        Task<bool> SameCenter(int postOfficeId1, int postOfficeId2);
        PostOffice GetByDistanceMin(IEnumerable<PostOffice> postOffices, double latDistance, double lngDistance);
        Task<PostOffice> GetDistanceMinByLocation(int cityId, double lat, double lng, int? type);
    }
    public interface ITypeReasonRepository : IRepository<TypeReason> { }
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetCustomerByPhone(string phone);
        string GetCode(int id);
    }
    public interface ILocationRepository : IRepository<Location>
    {

        Task<IEnumerable<Location>> GetCities();
        Task<IEnumerable<Location>> GetDistrictsByCity(int cityId);
        int GetIdBestMatches(IEnumerable<Location> locations, string locationName);

    }
    public interface IServiceRepository : IRepository<Service>
    {
        Task<IEnumerable<Service>> GetListSupportService();
        Task<IEnumerable<Service>> GetListMainService();
        Task<IEnumerable<Service>> GetListSupportServiceByTransport(int transportId);
        IEnumerable<Service> GetListMainServiceByLading(LadingViewModel model);
    }
    public interface IRecipientRepository : IRepository<Recipient> { }
    public interface IBKDeliveryRepository : IRepository<BKDelivery> { }
    public interface IStatusRepository : IRepository<Status> { }
    public interface ICODStatusRepository : IRepository<CODStatus> { }
    public interface ITransportRepository : IRepository<Transport> { }
    public interface IStructureRepository : IRepository<Structure>
    {
        Task<IEnumerable<Structure>> GetListByTransport(int transportId);
    }
    public interface IPriceRepository : IRepository<Price>
    {
        Task<CalculatePriceModel> CalculatePrice(LadingViewModel model);
        Dictionary<int, double> GetListPrice(double weight, int customerId = 0, int state_from = 0, int state_to = 0, int receive_delivery = 0);
        double GetPrice(double weight, int serviceId = 0, int priceListId = 0, int state_from = 0, int state_to = 0, int receive_delivery = 0);
    }
    public interface IPriceListRepository : IRepository<PriceList>
    {
        Task<int> GetPriceApplyByPOID(int pOId);
        IEnumerable<PriceList> GetListPriceListByCustomer(int? customerId = null, bool union = true);
    }
    public interface ITypeOfPackRepository : IRepository<TypeOfPack> { }
    public interface ILadingMapPriceRepository : IRepository<LadingMapService> { }
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


}
