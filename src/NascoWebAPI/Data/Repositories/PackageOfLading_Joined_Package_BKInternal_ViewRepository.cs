using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class PackageOfLading_Joined_Package_BKInternal_ViewRepository : Repository<PackageOfLading_Joined_Package_BKInternal_View>, IPackageOfLading_Joined_Package_BKInternal_ViewRepository
    {

        public PackageOfLading_Joined_Package_BKInternal_ViewRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
