using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NascoWebAPI.Data
{
    [Table("PackageOfLading_Joined_Package_BKInternal_View")]
    public partial class PackageOfLading_Joined_Package_BKInternal_View
    {
        public long Id { get; set; }
        public Nullable<int> PackageOfLadingId { get; set; }
        public long LadingId { get; set; }
        public string Code { get; set; }
        public string LadingCode { get; set; }
        public Nullable<int> POCurrent { get; set; }
        public string POCurrentCode { get; set; }
        public string POCurrentName { get; set; }
        public string RecipientReality { get; set; }
        public Nullable<int> LadingPartnerId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> Number { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public bool Return { get; set; }
        public string Noted { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string NameFrom { get; set; }
        public string NameTo { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public string CompanyFrom { get; set; }
        public string CompanyTo { get; set; }
        public string PhoneFromJoined { get; set; }
        public string PhoneToJoined { get; set; }
        public string AddressNoteFrom { get; set; }
        public string AddressNoteTo { get; set; }
        public double Weight { get; set; }
        public double Mass { get; set; }
        public double WeightToPrice { get; set; }
        public double Amount { get; set; }
        public double DiscountAmount { get; set; }
        public double GrandTotal { get; set; }
        public double PriceMain { get; set; }
        public double PriceOther { get; set; }
        public double PriceVAT { get; set; }
        public double COD { get; set; }
        public double TotalPriceDVGT { get; set; }
        public double CODLocal { get; set; }
        public double PPXDPercent { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public Nullable<int> GroupServiceId { get; set; }
        public string GroupServiceCode { get; set; }
        public string GroupServiceName { get; set; }
        public Nullable<int> RDFrom { get; set; }
        public string RDFromCode { get; set; }
        public string RDFromName { get; set; }
        public Nullable<int> StructureId { get; set; }
        public string StructureName { get; set; }
        public Nullable<int> CityFromId { get; set; }
        public string CityFromCode { get; set; }
        public string CityFromName { get; set; }
        public Nullable<int> CityToId { get; set; }
        public string CityToCode { get; set; }
        public string CityToName { get; set; }
        public Nullable<int> POFrom { get; set; }
        public string POFromCode { get; set; }
        public string POFromName { get; set; }
        public Nullable<int> POTo { get; set; }
        public string POToCode { get; set; }
        public string POToName { get; set; }
        public Nullable<int> POCreated { get; set; }
        public string POCreatedCode { get; set; }
        public string POCreatedName { get; set; }
        public Nullable<int> CenterFrom { get; set; }
        public string CenterFromCode { get; set; }
        public string CenterFromName { get; set; }
        public Nullable<int> CenterTo { get; set; }
        public string CenterToCode { get; set; }
        public string CenterToName { get; set; }
        public Nullable<int> PackageId { get; set; }
        public string PackageCode { get; set; }
        public Nullable<int> BKInternalId { get; set; }
        public string BKInternalCode { get; set; }
        public Nullable<int> BKDeliveryId { get; set; }
        public string BKDeliveryCode { get; set; }
        public Nullable<int> OfficerDelivery { get; set; }
        public string OfficerDeliveryName { get; set; }
        public Nullable<int> OfficerPickup { get; set; }
        public string OfficerPickupName { get; set; }
        public string LadingPartnerCode { get; set; }
        public Nullable<int> PartnerId { get; set; }
        public string PartnerCode { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public string PaymentTypeName { get; set; }
        public Nullable<int> CouponLadingId { get; set; }
        public Nullable<int> CouponId { get; set; }
        public string CouponCode { get; set; }
        public bool IsLading { get; set; }
    }
}
