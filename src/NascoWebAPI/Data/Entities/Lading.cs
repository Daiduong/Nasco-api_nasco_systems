using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("Lading")]
    public partial class Lading
    {
        public long Id { get; set; }
        public int? State { get; set; }
        public string PartnerCode { get; set; }
        public int? Status { get; set; }
        public int? StatusCus { get; set; }
        public int? StatusCOD { get; set; }
        public int? StatusPayee { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string Recipient_reality { get; set; }
        public int? OfficerId { get; set; }
        public int? SenderId { get; set; }
        public int? RecipientId { get; set; }
        public int? CitySendId { get; set; }
        public int? ServiceId { get; set; }
        public int? PackId { get; set; }
        public double? CODPrice { get; set; }
        public int? PaymentType { get; set; }
        public double? Weight { get; set; }
        public double? Length { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Mass { get; set; }
        public double? Amount { get; set; }
        public double? Number { get; set; }
        public string Noted { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int? CityRecipientId { get; set; }
        public double? PackPrice { get; set; }
        public double? InsuredPrice { get; set; }
        public double? Insured { get; set; }
        public double? COD { get; set; }
        public double? OnSiteDeliveryPrice { get; set; }
        public int? BkPaymentId { get; set; }
        public double? THBBPrice { get; set; }
        public double? HHKGPercent { get; set; }
        public double? BPPrice { get; set; }
        public double? PPXDPercent { get; set; }
        public double? PriceMain { get; set; }
        public string ImageDelivery { get; set; }
        public int? CustomerRate { get; set; }
        public int? TypeLading { get; set; }
        public string KeyBk { get; set; }
        public double? LatFrom { get; set; }
        public double? LngFrom { get; set; }
        public double? LatTo { get; set; }
        public double? LngTo { get; set; }
        public double? RouteLength { get; set; }
        public int? DistrictFrom { get; set; }
        public int? DistrictTo { get; set; }
        public int? POFrom { get; set; }
        public int? POTo { get; set; }
        public int? CenterFrom { get; set; }
        public int? CenterTo { get; set; }
        public int? POCurrent { get; set; }
        public int? OfficerPickup { get; set; }
        public int? OfficerDelivery { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public int? CODKeepBy { get; set; }
        public string NameFrom { get; set; }
        public string NameTo { get; set; }
        public string PhoneFrom { get; set; }
        public string PhoneTo { get; set; }
        public string CompanyFrom { get; set; }
        public string CompanyTo { get; set; }
        public bool? IsPriceMain { get; set; }
        public double? TotalPriceDVGT { get; set; }
        public double? PriceOther { get; set; }
        public double? PriceVAT { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public int? TransportID { get; set; }
        public int? StructureID { get; set; }
        public int? RDFrom { get; set; }
        public int? RDTo { get; set; }
        public bool? IsGlobal { get; set; }
        public string AddressNoteFrom { get; set; }
        public string AddressNoteTo { get; set; }
        public int? ModifiedBy { get; set; }
        public int? POCreated { get; set; }
        public bool? PaymentAmount { get; set; }
        public bool? PaymentCOD { get; set; }
        public bool? Return { get; set; }
        public double? InsuredPercent { get; set; }
        public double? KDPrice { get; set; }
        public int? KDNumber { get; set; }
        public string PhoneFrom2 { get; set; }
        public string PhoneTo2 { get; set; }
        public int? PriceListId { get; set; }
        public string NoteTypePack { get; set; }
        public string Number_L_W_H_DIM { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? OfficerTransferId { get; set; }
        public int? AmountKeepBy { get; set; }
        public int? BKDeliveryId { get; set; }
        public int? BKInternalId { get; set; }
        public int? FlightId { get; set; }
        public double? DBNDPrice { get; set; }
        public string DBNDFrom { get; set; }
        public string DBNDTo { get; set; }
        public string DBNDNote { get; set; }
        public int? PostOfficeKeepAmount { get; set; }
        public int? PostOfficeKeepCOD { get; set; }
        public double? DiscountAmount { get; set; }
        public bool? Locked { get; set; }
        public int? CouponLadingId { get; set; }
        public bool? IsPartStatus { get; set; }
        public bool? IsConfirmByLading { get; set; }
        public string ShopCode { get; set; }
        public DateTime? ExpectedTimePickUp { get; set; }
        public DateTime? ExpectedTimeDelivery { get; set; }
        public int? TimesPickUp { get; set; }
        public int? TimesDelivery { get; set; }
        public int? TimesDeliveryReturn { get; set; }
        public int? POMediateId { get; set; }
        public bool? IsInternal { get; set; }
        public int? CountryFromId { get; set; }
        public int? CountryToId { get; set; }
        public string EmailFrom { get; set; }
        public string IdentityFrom { get; set; }
        public string EmailTo { get; set; }
        public string IdentityTo { get; set; }
        public string PostCodeTo { get; set; }
        public int? OrderByService { get; set; }
        public DateTime? ExpectedTimeTransfer { get; set; }
        public DateTime? ExpectedTimeTakeOff { get; set; }
        public double? DistanceFrom { get; set; }
        public double? DistanceTo { get; set; }
        public bool? isCaculatePerPackage { get; set; }
        public double? Point { get; set; }

        [ForeignKey("RecipientId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Recipient Recipient { get; set; }
        [ForeignKey("SenderId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        private Customer _sender;
        public virtual Customer Sender
        {
            get; set;
        }
        [ForeignKey("CitySendId"), Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Location CitySend { get; set; }
        [ForeignKey("CityRecipientId"), Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]

        public virtual Location CityRecipient { get; set; }
        [ForeignKey("ServiceId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Service Service { get; set; }
        [ForeignKey("Status"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual LadingStatus CurrenSttStatus { get; set; }
        [ForeignKey("DistrictFrom"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Location DistrictFromObj { get; set; }
        [ForeignKey("DistrictTo"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Location DistrictToObj { get; set; }
        [ForeignKey("TransportID"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Transport Transport { get; set; }
        [ForeignKey("PriceListId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual PriceList PriceList { get; set; }

        public Lading()
        {

        }
        public Lading(LadingTemp ladingTemp)
        {

        }
    }
}
