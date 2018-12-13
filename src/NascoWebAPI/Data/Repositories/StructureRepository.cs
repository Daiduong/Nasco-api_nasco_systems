using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class StructureRepository : Repository<Structure>, IStructureRepository
    {
        public StructureRepository(ApplicationDbContext context) : base(context)
        {

        }
        public virtual async Task<IEnumerable<Structure>> GetListByTransport(int transportId)
        {
            var transportStructureRespository = new Repository<Transport_Structure>(_context);
            var transportStructure = await transportStructureRespository.GetAsync(o => o.TransportID == transportId);
            if (transportStructure != null && transportStructure.Count() > 0)
            {
                var structureIds = transportStructure.Select(o => o.StructureID);
                return await this.GetAsync(u => structureIds.Contains(u.Id));
            }
            return Enumerable.Empty<Structure>();
        }
    }

}
