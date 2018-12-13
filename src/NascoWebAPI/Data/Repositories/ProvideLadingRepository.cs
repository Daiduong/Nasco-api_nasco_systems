using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{ 
    public class ProvideLadingRepository : Repository<ProvideLading> , IProvideLadingRepository
    {
        public ProvideLadingRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
