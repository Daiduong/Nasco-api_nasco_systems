using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("LadingTempView")]
    public partial class LadingTempView
    {
        public long Id { get; set; }
        public int State { get; set; }
        public string PartnerCode { get; set; }
        public int Status { get; set; }
        public int StatusCOD { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public string Recipient_reality { get; set; }
        public Nullable<int> PackId { get; set; }
        public double CODPrice { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public Nullable<double> Mass { get; set; }
        public double Amount { get; set; }
        public double Number { get; set; }
        public string Noted { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public Nullable<double> PackPrice { get; set; }
        public Nullable<double> InsuredPrice { get; set; }
        public Nullable<double> Insured { get; set; }
        public double COD { get; set; }
        public Nullable<double> OnSiteDeliveryPrice { get; set; }
        public int BkPaymentCustomerId { get; set; }
        public Nullable<double> THBBPrice { get; set; }
        public Nullable<double> HHKGPercent { get; set; }
        public Nullable<double> BPPrice { get; set; }
        public Nullable<double> PPXDPercent { get; set; }
        public double PriceMain { get; set; }
        public string KeyBk { get; set; }
        public string OfficerName { get; set; }
        public Nullable<int> OfficerDeparmentID { get; set; }
        public string OfficerUserName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public Nullable<int> ServiceWeightType { get; set; }
        public Nullable<int> RecipientId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientCompany { get; set; }
        public Nullable<int> SenderId { get; set; }
        public string SenderCode { get; set; }
        public string SenderName { get; set; }
        public string SenderCompany { get; set; }
        public string SenderPhone { get; set; }
        public string SenderAddress { get; set; }
        public Nullable<int> Officer_Liabilities { get; set; }
        public Nullable<int> TypeCustomer { get; set; }
        public Nullable<int> PostOffice_Id { get; set; }
        public Nullable<int> CitySendId { get; set; }
        public string CitySendCode { get; set; }
        public string CitySendName { get; set; }
        public Nullable<int> CityRecipientId { get; set; }
        public string CityRecipientCode { get; set; }
        public string CityRecipientName { get; set; }
        public Nullable<double> RouteLength { get; set; }
        public Nullable<double> LngTo { get; set; }
        public Nullable<double> LngFrom { get; set; }
        public Nullable<double> LatFrom { get; set; }
        public Nullable<double> LatTo { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> OfficerId { get; set; }
        public Nullable<int> DistrictTo { get; set; }
        public Nullable<int> DistrictFrom { get; set; }
        public Nullable<int> POFrom { get; set; }
        public Nullable<int> POTo { get; set; }
        public Nullable<int> CenterFrom { get; set; }
        public Nullable<int> CenterTo { get; set; }
        public Nullable<int> POCurrent { get; set; }
        public Nullable<int> OfficerPickup { get; set; }
        public string DistrictFromName { get; set; }
        public string DistrictToName { get; set; }
        public string POFromName { get; set; }
        public string POToName { get; set; }
        public string CenterFromName { get; set; }
        public string CenterToName { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> OfficerDelivery { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public Nullable<int> CODKeepBy { get; set; }
        public string NameFrom { get; set; }
        public string NameTo { get; set; }
        public string PhoneFrom { get; set; }
        public string PhoneTo { get; set; }
        public string CompanyFrom { get; set; }
        public string CompanyTo { get; set; }
        public string CODStatusName { get; set; }
        public string OfficerPickupName { get; set; }
        public Nullable<int> TransportID { get; set; }
        public Nullable<int> StructureID { get; set; }
        public Nullable<int> RDFrom { get; set; }
        public Nullable<int> RDTo { get; set; }
        public string StructureName { get; set; }
        public string AddressNoteTo { get; set; }
        public string AddressNoteFrom { get; set; }
        public Nullable<bool> IsGlobal { get; set; }
        public Nullable<int> PackageID { get; set; }
        public string PackageCode { get; set; }
        public Nullable<int> StatusRequestId { get; set; }
        public string StatusRequestName { get; set; }
        public Nullable<int> RequestFrom { get; set; }
        public string RequestFromName { get; set; }
        public string AnotherServiceIds { get; set; }
    }
}
