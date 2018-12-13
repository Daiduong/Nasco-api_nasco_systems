using NascoWebAPI.Helper.VNPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models.Client.VNPost
{
    public class VNPostRequest
    {
        public string SoDonHang { get; set; }
        public string HoTenNguoiGui { get; set; }
        public string DiaChiNguoiGui { get; set; }
        public string DienThoaiNguoiGui { get; set; }
        public string DiaChiKhoHang { get; set; }
        public string TenKhoHang { get; set; }
        public string DienThoaiLienHeKhoHang { get; set; }
        public string HoTenNguoiNhan { get; set; }
        public string DienThoaiNguoiNhan { get; set; }
        public string DiaChiNguoiNhan { get; set; }
        public double TongTrongLuong { get; set; }
        public double TongCuoc { get; set; }
        public double TongTienPhaiThu { get; set; }
        public string NgayGiao { get; set; }
        public string TinhThanh { get; set; }
        public string QuanHuyen { get; set; }
        public int PhuongThuc { get; set; }
        public string NoiDungHang { get; set; }
        public string MaPhien { get; set; }
        public bool DonHangDoiTra { get; set; }
        public string MaHuyenPhat { get; set; }
        public int iddichvu { get; set; }
        public VNPostRequest() { }
        public VNPostRequest(string code,
            string nameFrom, string addressFrom, string phoneFrom,
            string nameTo, string addressTo, string phoneTo,
            int cityPOFromId, string cityToName, string districtToCode, int vNPostServiceId, string description,
                 double weight, double cod, bool isReturn)
        {
            this.SoDonHang = code;
            this.HoTenNguoiGui = nameFrom;
            this.DiaChiNguoiGui = addressFrom;
            this.DienThoaiNguoiGui = phoneFrom;
            this.TenKhoHang = (cityPOFromId == 1 ? "129090" : (cityPOFromId == 50 ? "BDHCM" : "BDDN"));
            this.DiaChiKhoHang = "";
            this.DienThoaiLienHeKhoHang = "";
            this.HoTenNguoiNhan = nameTo;
            this.DiaChiNguoiNhan = addressTo;
            this.DienThoaiNguoiNhan = phoneTo;
            this.TongTrongLuong = weight;
            this.TongCuoc = 0;
            this.TongTienPhaiThu = cod;
            this.NgayGiao = DateTime.Now.ToString("MM/dd/yyyy");

            this.TinhThanh = cityToName;
            this.QuanHuyen = districtToCode;
            if (this.TongTienPhaiThu > 0)
            {
                this.PhuongThuc = (int)VNPostHelper.VNPostMethod.DeliveryCOD;
            }
            else
            {
                this.PhuongThuc = (int)VNPostHelper.VNPostMethod.Delivery;
            }
            this.NoiDungHang = description;
            this.MaPhien = "";
            this.DonHangDoiTra = isReturn;
            this.MaHuyenPhat = districtToCode + "-" + cityToName;
            this.iddichvu = vNPostServiceId;
        }
    }
    public class VNPostHistory
    {
        public string STT { get; set; }
        public string NgayPhat { get; set; }
        public string GioPhat { get; set; }
        public string TrangThai { get; set; }
        public string TaiBuuCuc { get; set; }
    }
    public class VNPostDelivery
    {
        public string STTBaoPhat { get; set; }
        public string NgayBaoPhat { get; set; }
        public string GioBaoPhat { get; set; }
        public string BuuCucPhat { get; set; }
        public string TrangThaiPhat { get; set; }
    }
    public class VNPostLading
    {
        public string MaSo { get; set; }
        public string SoDonHang { get; set; }
        public double SoTienCOD { get; set; }
        public string MaBuuGui { get; set; }
        public double TrongLuong { get; set; }
        public double ChieuDai { get; set; }
        public double ChieuRong { get; set; }
        public double ChieuCao { get; set; }
        public double TongCuoc { get; set; }
        public int MaTinhPhat { get; set; }
        public double CuocCOD { get; set; }
        public bool VungSauVungXa { get; set; }
        public bool PhatDongKiem { get; set; }
        public bool VUN { get; set; }
        public string TenNguoiNhan { get; set; }
        public string DiaChiNguoiNhan { get; set; }
        public string DienThoaiNguoiNhan { get; set; }
        public string GhiChuThuGom { get; set; }
        public string TrangThaiXuLy { get; set; }
        public DateTime? NgayCapNhatThongTinXuLy { get; set; }
        public DateTime? NgayDongBCCP { get; set; }
        public int IDTrangThai { get; set; }
        public int IDTrangThaiPhat { get; set; }
        public string Ngay { get; set; }
        public string Gio { get; set; }
        public string TaiBuuCuc { get; set; }
        public string GhiChuPhat { get; set; }
        public string TrangThaiBuuGui { get; set; }
    }
}
