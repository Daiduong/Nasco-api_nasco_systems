using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("LadingTemp")]
    public partial class LadingTemp
    {
        public long Id { get; set; }
        public Nullable<int> State { get; set; }
        public string PartnerCode { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> StatusCus { get; set; }
        public Nullable<int> StatusCOD { get; set; }
        public Nullable<int> StatusPayee { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public string Recipient_reality { get; set; }
        public Nullable<int> OfficerId { get; set; }
        public Nullable<int> SenderId { get; set; }
        public Nullable<int> RecipientId { get; set; }
        public Nullable<int> CitySendId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> PackId { get; set; }
        public Nullable<double> CODPrice { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public Nullable<double> Weight { get; set; }
        public Nullable<double> Length { get; set; }
        public Nullable<double> Height { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> Mass { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<double> Number { get; set; }
        public string Noted { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public Nullable<int> CityRecipientId { get; set; }
        public Nullable<double> PackPrice { get; set; }
        public Nullable<double> InsuredPrice { get; set; }
        public Nullable<double> Insured { get; set; }
        public Nullable<double> COD { get; set; }
        public Nullable<double> OnSiteDeliveryPrice { get; set; }
        public Nullable<int> BkPaymentId { get; set; }
        public Nullable<double> THBBPrice { get; set; }
        public Nullable<double> HHKGPercent { get; set; }
        public Nullable<double> BPPrice { get; set; }
        public Nullable<double> PPXDPercent { get; set; }
        public Nullable<double> PriceMain { get; set; }
        public string ImageDelivery { get; set; }
        public Nullable<int> CustomerRate { get; set; }
        public Nullable<int> TypeLading { get; set; }
        public string KeyBk { get; set; }
        public Nullable<double> LatFrom { get; set; }
        public Nullable<double> LngFrom { get; set; }
        public Nullable<double> LatTo { get; set; }
        public Nullable<double> LngTo { get; set; }
        public Nullable<double> RouteLength { get; set; }
        public Nullable<int> DistrictFrom { get; set; }
        public Nullable<int> DistrictTo { get; set; }
        public Nullable<int> POFrom { get; set; }
        public Nullable<int> POTo { get; set; }
        public Nullable<int> CenterFrom { get; set; }
        public Nullable<int> CenterTo { get; set; }
        public Nullable<int> POCurrent { get; set; }
        public Nullable<int> OfficerPickup { get; set; }
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
        public Nullable<bool> IsPriceMain { get; set; }
        public Nullable<double> TotalPriceDVGT { get; set; }
        public Nullable<double> PriceOther { get; set; }
        public Nullable<double> PriceVAT { get; set; }
        public Nullable<System.DateTime> ExpectedDate { get; set; }
        public Nullable<int> TransportID { get; set; }
        public Nullable<int> StructureID { get; set; }
        public Nullable<int> RDFrom { get; set; }
        public Nullable<int> RDTo { get; set; }
        public Nullable<bool> IsGlobal { get; set; }
        public string AddressNoteFrom { get; set; }
        public string AddressNoteTo { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> PackageID { get; set; }
        public Nullable<int> RequestFrom { get; set; }
        public string AnotherServiceIds { get; set; }
        public Nullable<int> StatusRequestId { get; set; }
        public int? LadingId { get; set; }
        public int? POCreated { get; set; }
        public double? InsuredPercent { get; set; }
        public Nullable<double> KDPrice { get; set; }
        public Nullable<int> KDNumber { get; set; }
        public string PhoneFrom2 { get; set; }
        public string PhoneTo2 { get; set; }
        public string NoteTypePack { get; set; }
        public string Number_L_W_H_DIM { get; set; }
        public Nullable<int> PriceListId { get; set; }
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
        public virtual Status CurrenSttStatus { get; set; }
        [ForeignKey("DistrictFrom"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Location DistrictFromObj { get; set; }
        [ForeignKey("DistrictTo"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Location DistrictToObj { get; set; }
        [ForeignKey("TransportID"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Transport Transport { get; set; }
        [ForeignKey("PriceListId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual PriceList PriceList { get; set; }
    }
}
