using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NascoWebAPI.Data;
using NascoWebAPI.Data.Entities;

namespace NascoWebAPI
{
    public class ApplicationDbContext : DbContext
    {
        public static IConfiguration Configuration { get; set; }
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Helper.ConnectionHelper.CONNECTION_STRING);
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<Lading>()
            //   .HasOne(l => l.CityRecipient)
            //   .WithMany(c => c.LadingRecipients)
            //   .HasForeignKey(m => m.CityRecipientId);

            //builder.Entity<Lading>()
            //    .HasOne(s => s.CitySend)
            //    .WithMany(c => c.LadingSends)
            //    .HasForeignKey(m => m.CitySendId);

            //builder.Entity<Location>().Ignore(p => p.LadingSends).Ignore(p=> p.LadingRecipients);
        }
        public virtual DbSet<Officer> Officers { get; set; }
        public virtual DbSet<Lading> Ladings { get; set; }
        public virtual DbSet<LadingHistory> LadingHistories { get; set; }
        public virtual DbSet<BKInternal> BKInternals { get; set; }
        public virtual DbSet<Deparment> Deparments { get; set; }
        public virtual DbSet<PostOffice> PostOffices { get; set; }
        public virtual DbSet<TypeReason> TypeReasons { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Recipient> Recipients { get; set; }
        public virtual DbSet<BKDelivery> BKDeliveries { get; set; }
        public virtual DbSet<LadingStatus> LadingStatuses { get; set; }
        public virtual DbSet<CODStatus> CODStatuses { get; set; }
        public virtual DbSet<Transport> Transports { get; set; }
        public virtual DbSet<Transport_Service> Transport_Services { get; set; }
        public virtual DbSet<Transport_Structure> Transport_Structures { get; set; }
        public virtual DbSet<Structure> Structures { get; set; }
        public virtual DbSet<DeliveryReceive> DeliveryReceives { get; set; }
        public virtual DbSet<PriceList> PriceLists { get; set; }
        public virtual DbSet<Service_value_added_customer> Service_Value_Added_Customer { get; set; }
        public virtual DbSet<Price> Prices { get; set; }
        public virtual DbSet<TypeOfPack> TypeOfPacks { get; set; }
        public virtual DbSet<LocationArea> LocationAreas { get; set; }
        public virtual DbSet<LadingMapService> LadingMapServices { get; set; }
        public virtual DbSet<TypePack> TypePacks { get; set; }
        public virtual DbSet<AddOn> AddOns { get; set; }
        public virtual DbSet<SetupAddOn> SetupAddOns { get; set; }
        public virtual DbSet<PriceDetail> PriceDetails { get; set; }
        public virtual DbSet<SetupWeight> SetupWeights { get; set; }
        public virtual DbSet<PriceHighValue> PriceHighValues { get; set; }
        public virtual DbSet<LadingTemp> LadingTemps { get; set; }
        public virtual DbSet<GroupService> GroupServices { get; set; }
        public virtual DbSet<LadingTempHistory> LadingTempHistorys { get; set; }
        public virtual DbSet<ProvideLading> ProvideLadingS { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<SetupServiceArea> SetupServiceAreas { get; set; }
        public virtual DbSet<BKInternalHistory> BKInternalHistorys { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<PriceServiceOther> PriceServiceOthers { get; set; }
        public virtual DbSet<DividendOfVolumetricWeight> DividendOfVolumetricWeights { get; set; }
        public virtual DbSet<PriceListCustomer> PriceListCustomers { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<MAWB> MAWBs { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<CouponCustomer> CouponCustomers { get; set; }
        public virtual DbSet<CouponOfficer> CouponOfficers { get; set; }
        public virtual DbSet<CouponLading> CouponLadings { get; set; }
        public virtual DbSet<DiscountType> DiscountTypes { get; set; }
        public virtual DbSet<PackageOfLading> PackageOfLadings { get; set; }
        public virtual DbSet<PackageOfLadingHistory> PackageOfLadingHistories { get; set; }
        public virtual DbSet<ServiceOtherFormula> ServiceOtherFormulas { get; set; }
        public virtual DbSet<PriceDetailCustomer> PriceDetailCustomers { get; set; }
        public virtual DbSet<DistrictTypeMapping> DistrictTypeMappings { get; set; }
        public virtual DbSet<PriceDetailDistrictTypeAddOn> PriceDetailDistrictTypeAddOns { get; set; }
        public virtual DbSet<TimeLine> TimeLines { get; set; }
        public virtual DbSet<TimeLinePostOfficeLevel> TimeLinePostOfficeLevels { get; set; }
        public virtual DbSet<Airline> Airlines { get; set; }
        public virtual DbSet<Reason> Reasons { get; set; }
        public virtual DbSet<CustomerContact> CustomerContacts { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<PackageHistory> PackageHistories { get; set; }
        public virtual DbSet<CustomerPoint> CustomerPoints { get; set; }
        public virtual DbSet<Rank> Ranks { get; set; }
        public virtual DbSet<UnitGroups> UnitGroups { get; set; }
        public virtual DbSet<CustomerMessage> CustomerMessages { get; set; }
        public virtual DbSet<MarketingMessage> MarketingMessages { get; set; }
        public virtual DbSet<DistrictTemp> DistrictTemps { get; set; }
        public virtual DbSet<ProgramPromotion> ProgramPromotion { get; set; }
        public virtual DbSet<CustomerProgramPromotion> CustomerProgramPromotion { get; set; }
        public virtual DbSet<EMSLogShipmentTable> EMSLogShipmentTables { get; set; }
        public virtual DbSet<EMSLogInventoryTable> EMSLogInventoryTables { get; set; }
        public virtual DbSet<EMSLogCallbackTable> EMSLogCallbackTables { get; set; }

        #region View
        public virtual DbSet<PackageOfLading_Joined_Package_BKInternal_View> PackageOfLading_Joined_Package_BKInternal_Views { get; set; }
        public virtual DbSet<BB_View_Calculator> BB_View_Calculators { get; set; }
        public virtual DbSet<BB_View_Lading_Service> BB_View_Lading_Services { get; set; }
        public virtual DbSet<BB_View_Transport_Service_Joined> BB_View_Transport_Service_Joineds { get; set; }
        public virtual DbSet<BB_View_ServiceOtherPriceList> BB_View_ServiceOtherPriceLists { get; set; }
        public virtual DbSet<BB_View_ServiceCOD> BB_View_ServiceCODs { get; set; }
        public virtual DbSet<LadingTempView> LadingTempViews { get; set; }



        #endregion
    }
}
