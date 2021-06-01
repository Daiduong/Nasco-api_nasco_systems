
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper
{
    public static class GoogleMapHelper
    {
        public static string KEY_API_LOCAL = "AIzaSyBWMPdmbQtORZxldsv5jJx5yQE_mxTEt3M";
        //public static string KEY_API_LOCAL = "AIzaSyBhE_29Hba5NX784TDpssjvWWUGkVbNxhA";
        public static string KEY_API_IP = "AIzaSyBWMPdmbQtORZxldsv5jJx5yQE_mxTEt3M";
        // public static string KEY_API_IP = "AIzaSyBhE_29Hba5NX784TDpssjvWWUGkVbNxhA";

        public static string STATUS_OK = "OK";
        public static string STATUS_ZERO_RESULTS = "ZERO_RESULTS";
        public static string STATUS_OVER_QUERY_LIMIT = "OVER_QUERY_LIMIT";
        public static string STATUS_REQUEST_DENIED = "REQUEST_DENIED";
        public static string STATUS_INVALID_REQUEST = "INVALID_REQUEST";
        public static string STATUS_UNKNOWN_ERROR = "UNKNOWN_ERROR";
        public static string STATUS_ERROR = "ERROR";


        public static string TRAVEL_MODE_DRIVING = "driving";
        public static string TRAVEL_MODE_WALKING = "walking";
    }
}
