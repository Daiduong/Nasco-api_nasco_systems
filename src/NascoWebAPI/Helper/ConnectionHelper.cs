using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper
{
    public class ConnectionHelper
    {
        public static string CONNECTION_STRING = "NascoWebAPIConnection";
//        public const string CONNECTION_STRING =
//#if DEBUG
//         @"Data Source=192.168.68.43;Initial Catalog=test_core_nasco_db;Persist Security Info=True;User ID=core_nasco_user;Password=N@scoDev123;Connection Timeout=3600;;MultipleActiveResultSets=true;";
//#else
//         @"Data Source=192.168.68.43;Initial Catalog=core_nasco_db_new;Persist Security Info=True;User ID=core_nasco_user;Password=N@scoDev123;Connection Timeout=3600;;MultipleActiveResultSets=true;";
//#endif
    }
}
