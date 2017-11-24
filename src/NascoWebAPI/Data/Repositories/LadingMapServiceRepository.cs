using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class LadingMapPriceRepository : Repository<LadingMapService>, ILadingMapPriceRepository
    {
        public LadingMapPriceRepository(ApplicationDbContext conText) : base(conText)
        {

        }
     
    }

}
