using NascoWebAPI.Helper.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper
{
    public class EMSHelper
    {
        public const int PARTNER_ID = 3;
#if DEBUG 
        public const string URI_BASE = "http://222.255.250.245:502/";
#else
        public const string URI_BASE = "http://222.255.250.245:550/";
#endif
        public const string KEY_API_POST = "nasco";
        public const string KEY_API_IMAGE_POST = "Nasco_Bill";
#if DEBUG 
        public const string KEY_API_GET = "011238f0dce720d3de6a4b7d787076ca";
#else
        public const string KEY_API_GET = "287e03db1d99e0ec2edb90d079e142f3";
#endif
        public const string STATUS_OK = "200";

        public const string LADING_STATUS_ONDELIVERY = "C";
        public const string LADING_STATUS_DELIVERYSUCESS = "I";
        public const string LADING_STATUS_DELIVERYNOTSUCCESS = "H";
        public const string LADING_STATUS_BEINGRETURNING = "T";
        public const string LADING_STATUS_RETURNINGSUCCESS = "G";

        public const string STRUCT_DOCUMENT = "0";
        public const string STRUCT_GOODS = "1";

        public static string ConvertToEMSStatus(int statusId, bool isReturn = false)
        {
            if (!isReturn && statusId == (int)StatusLading.DangPhat)
                return LADING_STATUS_ONDELIVERY;
            if (isReturn && statusId == (int)StatusLading.DangPhat)
                return LADING_STATUS_BEINGRETURNING;
            if (statusId == (int)StatusLading.PhatKhongTC)
                return LADING_STATUS_DELIVERYNOTSUCCESS;
            if (statusId == (int)StatusLading.ThanhCong)
                return LADING_STATUS_DELIVERYSUCESS;
            if (statusId == (int)StatusLading.DaChuyenHoan)
                return LADING_STATUS_RETURNINGSUCCESS;
            return "";
        }
        public static int ConvertToStructure(string _class)
        {
            if (_class == STRUCT_DOCUMENT)
                return (int)StructureType.DOCUMENT;
            return (int)StructureType.GOODS;
        }
    }
}
