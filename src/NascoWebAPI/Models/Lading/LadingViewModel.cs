using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class LadingViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } //Mã
        public string PartnerCode { get; set; } //Mã
        public DateTime? CreateDate { get; set; }
        public DateTime? FromDate { get; set; }//Search
        public DateTime? ToDate { get; set; }//Search
        public DateTime? FinishDate { get; set; }
        public int? OficerId { get; set; }//Id nhân viên tạo vận đơn
        public string OfficerName { get; set; }//Ten nhan vien tao van don
        public int? SenderId { get; set; }//Người gửi
                                          //public long? AddressID { get; set; }//Người gửi
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderPhone { get; set; }
        public string SenderCompany { get; set; }
        public int? RecipientId { get; set; }//Người nhận
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientCompany { get; set; }
        public int? CitySendId { get; set; }//Nơi gửi
        public string CitySendName { get; set; }
        public int? CityRecipientId { get; set; }//Nơi nhận
        public string CityRecipientName { get; set; }//Nơi nhận
                                                     //public int? PostOfficeId { get; set; }//Bưu cục
        public int? ServiceId { get; set; }//Dịch vụ
        public List<int> ListServiceId { get; set; }//Tìm kiếm
        public List<int> AnotherServiceIds
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(AnotherServiceId) ? new List<int>() : AnotherServiceId.Split(',').Select(Int32.Parse).ToList();
                }
                catch
                {
                    return new List<int>();
                }
            }
        }//Dịch vụ giá trị gia tăng
        public string AnotherServiceId { get; set; }//Dịch vụ giá trị gia tăng
        public int? PackId { get; set; } //Dịch vụ công thêm đóng gói
        public double? CODPrice { get; set; }//% COD đã tính
        public double? InsuredPrice { get; set; }//Bảo hiểm hàng hóa đã tính
        public double? THBBPercent { get; set; }//% thu hồi biên bản
        public double? HHKGPercent { get; set; }//% Hàng hóa khai giá
        public double? BPPercent { get; set; }//% báo phát
        public double? PPXDPercent { get; set; }//% thu hồi phụ phí xăng dầu
        public double? COD { get; set; }//Số tiền COD nhập
        public double? Insured { get; set; }//Số tiền bảo hiểm hàng hóa nhập
        public double? PackPrice { get; set; }//Số tiền đóng gói
        public int? PaymentId { get; set; }//Hình thức thanh toán
        public List<int> ListPaymentId { get; set; }//Tìm kiếm
        public List<int> ListStatus { get; set; }//Tìm kiếm theo status
        public double? Weight { get; set; }//Cân nặng
        public double? FromWeight { get; set; }//Cân nặng search
        public double? ToWeight { get; set; }//Cân nặng search
        public double? Length { get; set; }//Chiều dài
        public double? Height { get; set; }//Chiều cao
        public double? Width { get; set; }//Chiều rộng
        public double? Mass { get; set; }//khoi luong
        public double? Amount { get; set; }//Tổng tiền
        public double? TotalPriceDVGT { set; get; }
        public double? PriceOther { set; get; }
        public bool? IsPriceMain { get; set; }
        public double? PriceMain { get; set; }
        public double? PriceVAT { get; set; }
        public double? Number { get; set; }//Số kiện
        public string Noted { get; set; }//Ghi chú
        public string Description { get; set; }//Nội dung
        public double? OnSiteDeliveryPrice { get; set; }//% Cước phí phát tận nơi
                                                        //public int TypeAddlading { get; set; }// nếu điều hành tạo vận đơn thì trạng thái là chưa lấy hàng . quản lý vận đơn tạo vận đơn trạng thái đang lấy hàng
                                                        //public int AnotherPrice { get; set; } // nếu = 1, không tính theo bảng giá, giá do người dùng nhập vào
        public float Lat { get; set; }
        public float Lng { get; set; }
        public double? LatFrom { get; set; }
        public double? LngFrom { get; set; }
        public double? LatTo { get; set; }
        public double? LngTo { get; set; }
        public double? RouteLength { get; set; }
        public Nullable<int> DistrictTo { get; set; }
        public Nullable<int> DistrictFrom { get; set; }
        public Nullable<int> POFrom { get; set; }
        public Nullable<int> POTo { get; set; }
        public Nullable<int> CenterFrom { get; set; }
        public Nullable<int> CenterTo { get; set; }
        public Nullable<int> POCurrent { get; set; }
        public Nullable<int> OfficerPickup { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> CODKeepBy { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public string NameFrom { get; set; }
        public string NameTo { get; set; }
        public string PhoneFrom { get; set; }
        public string PhoneTo { get; set; }
        public string CompanyFrom { get; set; }
        public string CompanyTo { get; set; }
        public Nullable<int> TransportID { get; set; }
        public Nullable<int> StructureID { get; set; }
        public Nullable<int> RDFrom { get; set; }
        public Nullable<int> RDTo { get; set; }
        public Nullable<bool> IsGlobal { get; set; }
        public string AddressNoteFrom { get; set; }//Địa chỉ cụ thể
        public string AddressNoteTo { get; set; }//Địa chỉ cụ thể
        public double? InsuredPercent { get; set; }
        public int? KDNumber { get; set; }//Số lượng sản phẩm 
        public double? KDPrice { get; set; }//Phí dịch vụ kiểm đếm
        public string Number_L_W_H_DIM { get; set; }// Gồm danh sách thông tin theo thứ tự: Số kiện_Dài_Rộng_Cao_TLQuyDoi 
        public List<NumberDIM> Number_L_W_H_DIM_List { get; set; }
        public int? PriceListId { get; set; }
        public string PhoneFrom2 { get; set; }
        public string PhoneTo2 { get; set; }
        public string NoteTypePack { get; set; }
    }
    public class NumberDIM
    {
        public int Number { get; set; }
        public double Long { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double DIM { get; set; }
    }
}
