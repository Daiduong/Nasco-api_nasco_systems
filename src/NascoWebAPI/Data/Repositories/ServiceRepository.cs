using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        public ServiceRepository(ApplicationDbContext context) : base(context)
        {

        }
        public virtual async Task<IEnumerable<Service>> GetListMainService()
        {
            return await this.GetAsync(u => u.State == (int)StatusSystem.Enable && u.Type == (int)ServiceType.MainService);
        }
        public virtual async Task<IEnumerable<Service>> GetListSupportService()
        {
            return await this.GetAsync(u => u.State == (int)StatusSystem.Enable && u.Type == (int)ServiceType.SupportService);
        }
        public virtual async Task<IEnumerable<Service>> GetListSupportServiceByTransport(int transportId)
        {
            var transportServiceRespository = new Repository<Transport_Service>(_context);
            var transportService = await transportServiceRespository.GetAsync(o => o.TransportID == transportId);
            if (transportService != null && transportService.Count() > 0)
            {
                var serviceIds = transportService.Select(o => o.ServiceID);
                return await this.GetAsync(u => u.State == (int)StatusSystem.Enable && u.Type == (int)ServiceType.SupportService && serviceIds.Contains(u.ServiceID));
            }
            return Enumerable.Empty<Service>();
        }
        public IEnumerable<Service> GetListMainServiceByLading(LadingViewModel model)
        {
            if (model.CitySendId.HasValue && model.CityRecipientId.HasValue)
            {
                return _context.SetupAddOns.Join(_context.SetupAddOns, from => from.ServiceId, to => to.ServiceId, (from, to) => new { From = from, To = to })
                      .Where(o => o.From.LocationId == model.CitySendId && o.From.AOIdFrom.HasValue && o.To.LocationId == model.CityRecipientId && o.To.AOIdTo.HasValue)
                      .Join(_context.Services, p => p.From.ServiceId, s => s.ServiceID, (p, s) => new { Service = s })
                      .GroupBy(o => o.Service)
                      .Select(o => o.FirstOrDefault().Service);

            }
            return Enumerable.Empty<Service>();
        }

    }

}
