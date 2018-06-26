using Microsoft.EntityFrameworkCore;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class LadingMapServiceRepository : Repository<LadingMapService>, ILadingMapServiceRepository
    {
        public LadingMapServiceRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task AddOrEdit(int ladingId, List<ServiceOtherModel> models)
        {
            var serviceIds = models.Select(x => x.Id);
            await _context.LadingMapServices.Where(x => x.State == 0 && x.LadingId == ladingId && !serviceIds.Contains(x.ServiceId.Value))
                .ForEachAsync(x => { x.State = 1; });
            await _context.SaveChangesAsync();
            await Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(models, (model) =>
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var ladingMapServiceRepository = new LadingMapServiceRepository(context); 
                        var insert = false;
                        var obj = ladingMapServiceRepository.GetSingle(x => x.ServiceId == model.Id && x.LadingId == ladingId);
                        if (obj == null)
                        {
                            obj = new LadingMapService();
                            insert = true;
                        }
                        obj.LadingId = ladingId;
                        obj.ServiceId = model.Id;
                        obj.TotalPrice = model.Charge;
                        obj.State = 0;
                        if (insert) ladingMapServiceRepository.Insert(obj);

                        ladingMapServiceRepository.SaveChanges();
                    }
                });
            });
        }
    }

}
