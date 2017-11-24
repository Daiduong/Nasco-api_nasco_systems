using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{ 
    public class GroupServiceRepository : Repository<GroupService> , IGroupServiceRepository
    {
        public GroupServiceRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
