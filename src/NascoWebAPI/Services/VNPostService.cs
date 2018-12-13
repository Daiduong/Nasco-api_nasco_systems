using NascoWebAPI.Helper;
using NascoWebAPI.Models;
using NascoWebAPI.Models.Client.VNPost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace NascoWebAPI.Services
{
    public class VNPostService
    {
        //#if DEBUG
        //        public static string KEY = "Test@Vnpost";
        //#else
        //        public static string KEY = "FNC#BDHN";
        //#endif
        //#if DEBUG
        //        public static string KEY_AUTH = "695D2323-7E34-4FC1-999C-932B4B5AFD35";
        //#else
        //        public static string KEY_AUTH = "DB867972-445F-4181-B0E9-B1B3BED210CB";
        //#endif

        //        public static string KEY_ID = "87";
        //        public static string BASE_URL = "http://buudienhanoi.com.vn";
        //        public static async Task<string> GetKey()
        //        {
        //            var input = string.Format(@"<KetNoi xmlns=""http://buudienhanoi.com.vn/""><Ma>{0}</Ma></KetNoi>",
        //                        KEY);
        //            var xElement = await XMLHelper.PostEncodedContent<string>($"{BASE_URL}/Nhanh/BDHNNhanh.asmx", input);
        //            return xElement;
        //        }
        //        public static async Task<ResultModel<string>> SendRequest(VNPostRequest vNPostLading)
        //        {
        //            vNPostLading.MaPhien = KEY_AUTH;
        //            var xElement = await XMLHelper.PostEncodedContent<string>($"{BASE_URL}/Nhanh/BDHNNhanh.asmx", vNPostLading);
        //            var result = new ResultModel<string>();
        //            if (xElement.Length < 2)
        //            {
        //                return result.Init(message: xElement);
        //            }
        //            switch (xElement.Substring(0, 2))
        //            {
        //                case "00":
        //                    return result.Init(message: "Phiên làm việc không tồn tại");
        //                case "10":
        //                    return result.Init(message: "Quá thời gian kết nối của phiên làm việc");
        //                case "20":
        //                    return result.Init(message: "Đã có đơn hàng này trong dữ liệu");
        //                case "99":
        //                    return result.Init(xElement.Substring(3), null, 0);
        //                default:
        //                    return result.Init(message: xElement);
        //            }

        //        }
        //        public static async Task<ResultModel<string>> CancelRequest(string ladingCode)
        //        {
        //            var input = string.Format(@"<HuyYeuCauThuGom xmlns=""http://buudienhanoi.com.vn/""><sodonhang>{0}</sodonhang>
        //                        <maxacthuc>{1}</maxacthuc></HuyYeuCauThuGom>", ladingCode, KEY);
        //            var xElement = await XMLHelper.PostEncodedContent<string>($"{BASE_URL}/Nhanh/BDHNNhanh.asmx", input);
        //            var result = new ResultModel<string>();
        //            if (xElement.Length < 1)
        //            {
        //                return result.Init(message: xElement);
        //            }
        //            switch (xElement.Substring(0, 1))
        //            {
        //                case "0":
        //                    return result.Init("0", "Cập nhật thành công", 0);
        //                case "1":
        //                    return result.Init("1", "Lỗi đơn hàng đã được lưu hành vào hệ thống vận chuyển");
        //                case "2":
        //                    return result.Init("2", "Lỗi không tìm thấy đơn hàng trên hệ thống");
        //                case "3":
        //                    return result.Init("3", "Lỗi trong quá trình hủy tin");
        //                default:
        //                    return result.Init(message: xElement);
        //            }
        //        }
        //        public static async Task<List<VNPostLading>> GetLadingPickup(string ladingCode)
        //        {
        //            var input = string.Format(@"<KetQuaThuGom  xmlns=""http://buudienhanoi.com.vn/""><ItemID>{0}:{1}</ItemID>
        //                        <OrderNumber>{1}</OrderNumber></KetQuaThuGom >", KEY_ID, ladingCode);
        //            var xElement = await XMLHelper.PostEncodedContent($"{BASE_URL}/Nhanh/BDHNNhanh.asmx", input);
        //            var dataSet = new DataSet();
        //            dataSet.Tables.Add(Helper.CreateDatatable(typeof(VNPostLading), "DanhSach"));
        //            using (StringReader s = new StringReader(xElement.ToXmlNode().OuterXml))
        //            {
        //                dataSet.ReadXml(s, XmlReadMode.DiffGram);
        //                return Helper.ConvertDataTable<VNPostLading>(dataSet.Tables[0]);
        //            }
        //        }
        //        public static async Task<List<VNPostLading>> GetLadingDelivery(string ladingCode, string vNPostCode)
        //        {
        //            var input = string.Format(@"<KetQuaPhat xmlns=""http://buudienhanoi.com.vn/""><ItemID>{0}:{1}</ItemID>
        //                        <OrderNumber>{1}</OrderNumber><CodeVnpost>{2}</CodeVnpost></KetQuaPhat>", KEY_ID, ladingCode, vNPostCode);
        //            var xElement = await XMLHelper.PostEncodedContent($"{BASE_URL}/Nhanh/BDHNNhanh.asmx", input);
        //            var dataSet = new DataSet();
        //            dataSet.Tables.Add(Helper.CreateDatatable(typeof(VNPostLading), "DanhSach"));
        //            using (StringReader s = new StringReader(xElement.ToXmlNode().OuterXml))
        //            {
        //                dataSet.ReadXml(s, XmlReadMode.DiffGram);
        //                return Helper.ConvertDataTable<VNPostLading>(dataSet.Tables[0]);
        //            }
        //        }
        //        public static async Task<List<VNPostHistory>> GetHistories(string ladingPartnerCode)
        //        {
        //            var input = string.Format(@"<TraCuuThongTinTrangThai  xmlns=""http://tempuri.org/""><mabuugui>{0}</mabuugui>
        //                        </TraCuuThongTinTrangThai>", ladingPartnerCode);
        //            var xElement = await XMLHelper.PostEncodedContent($"{BASE_URL}/htkh2015/ws_dinhvi.asmx", input);
        //            var dataSet = new DataSet();
        //            dataSet.Tables.Add(Helper.CreateDatatable(typeof(VNPostHistory), "Table1"));
        //            using (StringReader s = new StringReader(xElement.ToXmlNode().OuterXml))
        //            {
        //                dataSet.ReadXml(s, XmlReadMode.DiffGram);
        //                return Helper.ConvertDataTable<VNPostHistory>(dataSet.Tables[0]);
        //            }
        //        }
        //        public static async Task<List<VNPostDelivery>> GetDeliveries(string ladingPartnerCode)
        //        {
        //            var input = string.Format(@"<TraCuuThongTinPhat  xmlns=""http://tempuri.org/""><mabuugui>{0}</mabuugui>
        //                        </TraCuuThongTinPhat>", ladingPartnerCode);
        //            var xElement = await XMLHelper.PostEncodedContent($"{BASE_URL}/htkh2015/ws_dinhvi.asmx", input);
        //            var dataSet = new DataSet();
        //            dataSet.Tables.Add(Helper.CreateDatatable(typeof(VNPostDelivery), "Table1"));
        //            using (StringReader s = new StringReader(xElement.ToXmlNode().OuterXml))
        //            {
        //                dataSet.ReadXml(s, XmlReadMode.DiffGram);
        //                return Helper.ConvertDataTable<VNPostDelivery>(dataSet.Tables[0]);
        //            }
        //        }
        //        public static bool IsRequestSuccessful(int statusId)
        //        {
        //            return false;
        //        }
        //        public static bool IsPickedUp(int statusId)
        //        {
        //            return (statusId == (int)VNPostPickupStatus.Khoa);
        //        }
        //        public static bool IsTransferring(int statusId)
        //        {
        //            return (statusId == (int)VNPostDeliveryStatus.ChuaCoThongTin
        //                || statusId == (int)VNPostDeliveryStatus.ChapNhanGui
        //                || statusId == (int)VNPostDeliveryStatus.DangTrenTuyen);
        //        }
        //        public static bool IsTransferring(string statusName)
        //        {
        //            return statusName.IndexOf("Đã đến bưu cục", StringComparison.OrdinalIgnoreCase) >= 0
        //                || statusName.IndexOf("Arrival at PO", StringComparison.OrdinalIgnoreCase) >= 0;
        //        }
        //        public static bool IsDelivering(int statusId)
        //        {
        //            return statusId == (int)VNPostDeliveryStatus.DenBuuCucPhat;
        //        }
        //        public static bool IsDelivering(string statusName)
        //        {
        //            return statusName.IndexOf("Đã giao cho bưu tá", StringComparison.OrdinalIgnoreCase) >= 0;
        //        }
        //        public static bool IsDeliverySuccessful(int statusId)
        //        {
        //            return (statusId == (int)VNPostDeliveryStatus.PhatThanhCong
        //                || statusId == (int)VNPostDeliveryStatus.DaHoanThanhCong
        //                || statusId == (int)VNPostDeliveryStatus.DaThuTien
        //                || statusId == (int)VNPostDeliveryStatus.DaTraTienNguoiGui);
        //        }
        //        public static bool IsDeliverySuccessful(string statusName)
        //        {
        //            statusName = statusName.ToAscii();
        //            return statusName.EqualsIgnoreCase(Helper.GetEnumDescription(VNPostDeliveryStatus.PhatThanhCong).ToAscii())
        //                || statusName.EqualsIgnoreCase(Helper.GetEnumDescription(VNPostDeliveryStatus.DaHoanThanhCong).ToAscii())
        //                || statusName.EqualsIgnoreCase(Helper.GetEnumDescription(VNPostDeliveryStatus.DaThuTien).ToAscii())
        //                || statusName.EqualsIgnoreCase(Helper.GetEnumDescription(VNPostDeliveryStatus.DaTraTienNguoiGui).ToAscii());
        //        }
        //        public static bool IsDeliveryFail(int statusId)
        //        {
        //            return (statusId == (int)VNPostDeliveryStatus.ChuaPhatDuoc);
        //        }
        //        public static bool IsDeliveryFail(string statusName)
        //        {
        //            return statusName.ToAscii().EqualsIgnoreCase(Helper.GetEnumDescription(VNPostDeliveryStatus.ChuaPhatDuoc).ToAscii());
        //        }
        //        public static bool IsReturn(int statusId)
        //        {
        //            return (statusId == (int)VNPostDeliveryStatus.ChuyenHoan);
        //        }
        //        public static bool IsReturn(string statusName)
        //        {
        //            return statusName.ToAscii().EqualsIgnoreCase(Helper.GetEnumDescription(VNPostDeliveryStatus.ChuyenHoan).ToAscii());
        //        }
    }
}
