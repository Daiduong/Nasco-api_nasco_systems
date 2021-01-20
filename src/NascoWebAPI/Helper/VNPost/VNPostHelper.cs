using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper.VNPost
{
    public static class VNPostHelper
    {
        public enum VNPostMethod : int
        {
            DeliveryCOD = 1,
            Delivery
        }
        public enum VNPostService : int
        {
            [Description("Bưu kiện")]
            Kien = 1,
            [Description("EMS")]
            EMS,
            [Description("Thư")]
            BPBD
        }
        public enum VNPostPickupStatus : int
        {
            [Description("Khóa")]
            Khoa = 1,
            [Description("Nhập")]
            Nhap = 4,
            [Description("Điều hành")]
            DieuHanh = 6,
            [Description("Báo hủy")]
            BaoHuy = 8,
            [Description("Chuyển Tiếp")]
            ChuyenTiep = 10,
            [Description("Lạc hướng")]
            LacHuong = 12,
            [Description("Đến lấy nhưng chưa có hàng lần 1")]
            LayHangKhongTC1 = 13,
            [Description("Đến lấy nhưng chưa có hàng lần 2")]
            LayHangKhongTC2 = 14,
            [Description("Đến lấy nhưng chưa có hàng lần 3")]
            LayHangKhongTC3 = 15,
            [Description("Đến lấy nhưng chưa có hàng >= 3 lần")]
            LayHangKhongTC4 = 16,
        }
        public enum VNPostDeliveryStatus : int
        {
            [Description("Chưa có thông tin")]
            ChuaCoThongTin = 0,
            [Description("Chấp nhận gửi")]
            ChapNhanGui,
            [Description("Đang trên tuyến vận chuyển")]
            DangTrenTuyen,
            [Description("Đã đến bưu cục phát")]
            DenBuuCucPhat,
            [Description("4	Chưa phát được")]
            ChuaPhatDuoc,
            [Description("Phát thành công")]
            PhatThanhCong,
            [Description("[COD]Đã thu tiền")]
            DaThuTien,
            [Description("Chuyển hoàn")]
            ChuyenHoan,
            [Description("[COD]Trả tiền cho người gửi")]
            DaTraTienNguoiGui,
            [Description("Phát hoàn thành công")]
            DaHoanThanhCong,
            [Description("Đã đến bưu cục (Đến bưu cục tỉnh)")]
            DaDenBuuCuc,
            [Description("Giao hàng thành công")]
            GiaoHangThanhCong,
        }
    }
}
