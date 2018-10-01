using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper.Common
{
    public class Constants
    {

        public const string PassPhrase = "OriBTech@2014";
        public const string SystemAdmin = "oribtechadmin";
        public const string DefaultLat = "21.02894860978742";
        public const string DefaultLng = "105.85244722590335";

        public enum FilterDefault
        {
            PageSize = 10,
            PageNo = 1
        }
        public enum Default
        {
            Interger = 1
        }
        public enum ParentType
        {
            Video = 1
        }
        public enum DeviceType
        {
            Ios = 1,
            Android = 2,
            Web = 4
        }

        public enum Role
        {
            Administrator = 1,
            Officer = 2,
            Manager = 3,
            Shipper = 4,
            TransportRoute = 5
        }
        public enum MasterData
        {
            Reason = 1,
            Status = 2
        }
        public static class Headers
        {
            public static string Authorization = "Authorization";
        }
        public enum ReportType
        {
            [Description("Tổng số vận đơn phải đi lấy")]
            PhaiDiLay = 1,
            [Description("Tổng số vận đơn đã đi lấy")]
            DaDiLay = 2,
            [Description("Tổng vận đợn đã được phân đi giao")]
            DaPhanDiGiao = 3,
            [Description("Tổng số vận đơn đang giữ")]
            DangGiu = 4,
            [Description("Tổng số vận đơn phát thành công trong ngày")]
            PhatTCNgay = 5,
            [Description("Tổng số vận đơn phát thành công trong tháng")]
            PhatTCThang = 6,
            [Description("Tổng số vận đơn chuyển hoàn trong ngày")]
            ChuyenHoanNgay = 7,
            [Description("Tổng số vận đơn chuyển hoàn trong tháng")]
            ChuyenHoanThang = 8,
            [Description("Tổng COD đang giữ trong ngày")]
            CODDangGiuNgay = 9,
            [Description("Tổng COD đang giữ trong tháng")]
            CODDangGiuThang = 10
        }
        public enum StatusFlight
        {
            Created = 100,
            TakeOff,
            Landing,
            Cancel,
            Confirmed = 105,
        }
        public enum JobType
        {
            KTBAY = 6
        }

    }

}

