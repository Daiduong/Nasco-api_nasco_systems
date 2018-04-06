using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper.Common
{
    public enum StatusSystem
    {
        Delete = 1,
        Disable = 2,
        Enable = 0 // la trang thai mac dinh trong database.
    }
    public enum StatusCheckPrice
    {
        [Description("Không tính theo bảng giá")]
        GiaTuNhap = 1,
        [Description("Sử dụng bảng giá chuẩn")]
        SdBangGia = 0,
    }
    //Trạng Thái Vận Đơn
    public enum StatusLading
    {
        [Description("Khách Hàng Tạo Bill")] // bước đầu tiên khách hàng tạo bill từ thương mại điện tử
        KHTaoBill = 9,
        [Description("Đang Lấy Hàng")]
        DangLayHang = 1,
        [Description("Đã Lấy Hàng")]
        DaLayHang = 2,
        [Description("Đang Trung Chuyển")]
        DangTrungChuyen = 3,
        [Description("Đã Nhận Lại Chi Nhánh Phát")]
        DaNhanLai = 4,
        [Description("Đang Phát")]
        DangPhat = 5,
        [Description("Đã Phát Thành Công")]
        ThanhCong = 6,
        [Description("Khai Thác Lại Thông Tin Gốc")]
        KhaiThacThongTin = 7,
        [Description("Hoàn Gốc")]
        HoanGoc = 8,
        [Description("Nhân Viên Không Nhận")]
        NVKhongNhan = 10,
        [Description("Bill Chưa Đủ Thông Tin")]
        BillTrang = 11,
        [Description("Chưa nhập kho")]
        ChuaNhapKho = 12,
        [Description("Đã chuyển hoàn")]
        DaChuyenHoan = 13,
        [Description("Lấy hàng không thành công")]
        LayHangKhongTC = 14,
        [Description("Nhập kho")]
        NhapKho = 15,
        [Description("Xuất kho")]
        XuatKho = 16,
        [Description("Phát không thành công")]
        PhatKhongTC = 17,
        [Description("Chờ lấy hàng")]
        ChoLayHang = 18,
        [Description("Xuat kho trung chuyển")]
        XuatKhoTrungChuyen = 20,
        [Description("Đã hủy")]
        DaHuy = 21,
        [Description("Đóng gói")]
        DongGoi = 22,
        [Description("Tạo chuyến bay")]
        TaoChuyenByay = 23,
        [Description("Xác nhận chuyển bay hợp lệ")]
        XacNhanChuyenBayHopLe = 24,
        [Description("Rời trung tâm bay")]
        RoiTrungTamBay = 25,
        [Description("Đến trung tâm sân bay nhận")]
        DenTrungTamSanBayNhan = 26,
        [Description("Hủy chuyến bay")]
        HuyChuyenBay = 27,
        [Description("Mở gói khai thác lại")]
        MoGoiKHaiThacLai = 28,
        [Description("Bị gỡ ra khỏi chuyến bay")]
        BiGoKhoiChuyenBay = 29,
        [Description("Bộ phận sân bay xác nhận")]
        SBXN = 30
    }

    public enum StatusInCome //Trạng thái bảng kê thanh toan cua khach hang
    {
        [Description("Chưa Tạo Bảng Kê")]
        ChuaTao = 0,
        [Description("Đã Tạo Bảng Kê")]
        DaTao = 1,
        [Description("Đã Chốt Bảng Kê")]
        ChoTT = 2,
        [Description("Đã Thanh Toán")]
        DaTT = 3
    }

    public enum StatusCOD //Trạng thái COD trên bảng kê thanh toán khách hàng, quản lý xem đã thanh toán COD cho khách hàng hay chưa
    {
        [Description("Chưa Tạo Phiên GD")]
        ChuaTao = 0,
        [Description("KhongCOD")]
        KhongCOD = 1,
        [Description("ChuaThu")]
        ChuaThu = 2,
        [Description("DaThu")]
        DaThu = 3,
        [Description("NVDangGiu")]
        NVDangGiu = 4,
        [Description("ThuQuyGiu")]
        ThuQuyGiu = 5,
        [Description("DaTra")]
        DaTra = 6
    }
    //Loại địa điểm
    public enum LocationType
    {
        [Description("Quốc gia")]
        National = 0,
        [Description("Thành phố - Tỉnh")]
        City = 1,
        [Description("Quận - Huyện")]
        District = 2,
        [Description("Phường - Xã")]
        Phoenix = 3
    }
    public enum StatusPermition
    {
        [Description("Sysadmin")]
        SystemManager = 0,
    }

    //Phân loại dịch vụ
    public enum ServiceType
    {
        [Description("Dịch vụ chính")]
        MainService = 0,
        [Description("Dịch vụ phụ")]
        SupportService = 1
    }

    //Phân loại COD hay phí xăng dầu
    public enum CodOrGasOil
    {
        [Description("COD")]
        COD = 0,
        [Description("Xăng dầu")]
        GasOil = 1
    }

    //Các dịch vụ giá trị gia tăng
    public enum ServiceGTGT
    {
        [Description("Thu Hồi Biên Bản")]
        THBB = 1,
        [Description("Báo Phát")]
        BP = 2,
        [Description("H.Hóa Khai Giá")]
        HHKG = 3,
        [Description("Phát Hàng Thu Tiền")]
        COD = 4,
        [Description("Phát T7,CN")]
        T7CN = 5,
        [Description("Đóng gói")]
        DG = 6
    }

    public enum ServiceMainEnum
    {
        [Description("Chuyển phát hàng giá trị cao, hàng lạnh, hồ sơ thầu, vắc xin")]
        HST = 1,
        [Description("Chuyển phát nhanh trước 16h")]
        PT16H = 2,
        [Description("Chuyển phát nhanh trước 12h")]
        PT12H = 3,
        [Description(" Chuyển phát nhanh trước 9h")]
        PT9H = 4,
        [Description(" Chuyển phát nhanh hỏa tốc")]
        PTN = 5,
        [Description(" Chuyển phát hàng hóa bằng đường bộ ")]
        TK = 6,
        [Description("Chuyển phát nhanh bưu phẩm - bưu kiện")]
        CPN = 7,
        [Description("Khác")]
        KHAC = 8,
    }

    //Các loại trọng lượng
    public enum WeightTypeEnum
    {
        [Description("Kg")]
        Kg = 0,
        [Description("Gram")]
        Gram = 1
    }

    //Các loại khoảng cách
    public enum LocationDistanceEnum
    {
        [Description("Nội thành")]
        Local = 0,
        [Description("< 300 km")]
        LT300 = 1,
        [Description(">= 300 km")]
        GTE300 = 2,
        [Description("Khác")]
        Another = 3
    }

    public enum TypePrice //hinh thuc tinh cua bang gia
    {
        [Description("Theo Trọng Lượng")]
        TL = 0,
        [Description("Theo Khối Lượng")]
        KL = 1,
        [Description("Theo Số Kiện - Lô")]
        SLK = 2
    }

    public enum TypeFormula //cong thuc tinh gia
    {
        [Description("VAT, PPXD / Tổng Tiền Cước")]
        CT0 = 0,
        [Description("VAT, PPXD / Tổng Cước Trắng")]
        CT1 = 1,
        [Description("VAT, PPXD / Tổng Cước Giảm")]
        CT2 = 2
    }

    public enum IsLast
    {
        [Description("Giá tính bình thường theo đơn vị tính")]
        GiaTinh = 0,
        [Description("Giá tính vượt định mức tiếp theo")]
        GiaVuot = 1,
        [Description("Giá áp trực tiếp theo đơn vị tính")]
        GiaAp = 2
    }

    //Các kiểu thanh toán
    public enum PaymentType
    {
        [Description("Thanh toán cuối tháng")]
        Month = 1,
        [Description("Người nhận thanh toán")]
        Recipient = 2,
        [Description("Đã thanh toán")]
        Done = 3
    }
    //Loại đóng gói
    public enum PackType
    {
        [Description("Không đóng")]
        None = 0,
        [Description("Đóng carton")]
        Carton = 1,
        [Description("Đóng gỗ")]
        Timber = 2,
        [Description("Đóng mút - xốp")]
        Foam = 3,
    }
    public enum DiscountPriceEnum
    {
        [Description("Theo Trọng Lượng")]//Giảm theo trọng lượng của từng vận đơn
        Weight = 0,
        [Description("Theo Số Tiền")]//Giảm theo số tiền của từng vận đơn
        Money = 1,
        [Description("Theo Tổng Tiền")]//Giảm theo tổng tiền của bảng kê
        Totalprice = 2
    }
    public enum StatusFormula //cong thuc tinh gia
    {
        [Description("Giá chuẩn")]
        Formula1 = 1,
        [Description("Nhân với trọng lượng")]
        Formula2 = 2,
        [Description("Nhân số vượt định mức")]
        Formula3 = 3
    }
    public enum StatusBK
    {
        [Description("Đã đóng gói")]
        DongGoi = -1,
        [Description("Đã Tạo - Đã chuyển")]
        DaTao = 0,
        [Description("Đã Giao")] //(Không dùng cho bảng kê phát)
        DaGiao = 2,
        [Description("Đã Xác Nhận")]
        DaNhan = 1,
        [Description("Đang treo")]
        Dangxuly = 3,
        [Description("Trạm gửi,Đang trung chuyển")]
        TramGuiDangTrungChuyen = 5,
        [Description("Trạm gửi Bộ phận sân bay xác nhận")]
        TramGuiBPSBXN = 105,
        [Description("Trạm nhận Bộ phận sân bay xác nhận")]
        TramNhanBPSBXN = 106,
        [Description("Trạm nhận, Ðang trung chuyển")]
        TramNhanDangTrungChuyen = 6,
        ChuyenBayMoiTao = 100,
        RoiSanBay,
        DenSanBay,
        HuyChuyenBay
    }
    public enum StatusCouponLading
    {
        WaitingForApproval = 800,
        Approved,
        Unapproved,
        Responded,
        Accepted
    }
    public enum StatusLadingTemp
    {
        Created = 1,//Mới tạo
        WaitingPickUp , //Đang chờ lấy hàng
        PickingUp, //Đang  lấy hàng,
        PickedUp, //Đã lấy hàng
        InStock , //Nhập kho
        Cancel, //Hủy
    }
    public enum PostOfficeMethod
    {
        FROM = 1,
        TO,
        PARTNER
    }
    public enum PostOfficeType
    {
        HEADQUARTER = 1,
        BRANCH,
        HUB,
        STATION
    }
    public enum TransportType
    {
        MOTORBIKE = 1,
        TRUCK,
        PLANE,
        PARTNER
    }
}
