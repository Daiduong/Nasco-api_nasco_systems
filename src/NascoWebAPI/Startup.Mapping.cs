using Microsoft.Extensions.DependencyInjection;
using NascoWebAPI.Services;
using NascoWebAPI.Services.Interface;
using NascoWebAPI.Data;


namespace NascoWebAPI
{
    public partial class Startup
    {
        private void MappingScopeService(IServiceCollection services)
        {
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IOfficerRepository, OfficerRepository>();
            services.AddScoped<ILadingRepository, LadingRepository>();
            services.AddScoped<ILadingHistoryRepository, LadingHistoryRepository>();
            services.AddScoped<IBKInternalRepository, BKInternalRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IPostOfficeRepository, PostOfficeRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<ITypeReasonRepository, TypeReasonRepository>();
            services.AddScoped<IRecipientRepository, RecipientRepository>();
            services.AddScoped<IBKDeliveryRepository, BKDeliveryRepository>();
            services.AddScoped<IStatusRepository, StatusRepository>();
            services.AddScoped<ICODStatusRepository, CODStatusRepository>();
            services.AddScoped<ITransportRepository, TransportRepository>();
            services.AddScoped<ITypeOfPackRepository, TypeOfPackRepository>();
            services.AddScoped<IPriceRepository, PriceRepository>();
            services.AddScoped<IStructureRepository, StructureRepository>();
            services.AddScoped<ILadingMapPriceRepository, LadingMapPriceRepository>();
            services.AddScoped<IPriceListRepository, PriceListRepository>();
            services.AddScoped<IDeliveryReceiveRepository, DeliveryReceiveRepository>();
            services.AddScoped<ITypePackRepository, TypePackRepository>();
            services.AddScoped<ILadingTempHistoryRepository, LadingTempHistoryRepository>();
            services.AddScoped<ILadingTempRepository, LadingTempRepository>();
            services.AddScoped<IGroupServiceRepository, GroupServiceRepository>();
            services.AddScoped<IProvideLadingRepository, ProvideLadingRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IBKInternalHistoryRepository, BKInternalHistoryRepository>();
            services.AddScoped<IFlightRepository, FlightRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();

        }
    }
}
