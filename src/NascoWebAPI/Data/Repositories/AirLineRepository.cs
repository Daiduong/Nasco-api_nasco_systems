using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class AirlineRepository : Repository<Airline>, IAirlineRepository
    {
        public AirlineRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
