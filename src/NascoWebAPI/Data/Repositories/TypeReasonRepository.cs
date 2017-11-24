using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class PostOfficeRepository : Repository<PostOffice>, IPostOfficeRepository
    {
        public PostOfficeRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task<IEnumerable<PostOffice>> GetByRootPO(int postOfficeId)
        {
            var postOffice = await this.GetFirstAsync(o => o.PostOfficeID == postOfficeId);
            if (postOffice != null)
            {
                var rootPostOfficeId = await this.GetIdByLocationArea(postOffice.LocationAreaID.Value);
                var locationAreaRepository = new Repository<LocationArea>(_context);
                var locationAreaIds = (await locationAreaRepository
                                .GetAsync(l => l.State == (int)StatusSystem.Enable &&
                                            l.RootPostOffice == rootPostOfficeId)).Select(i => i.ID);
                return await this.GetAsync(p => p.State == (int)StatusSystem.Enable && locationAreaIds.Contains(p.LocationAreaID.Value));
            }
            return Enumerable.Empty<PostOffice>();

        }
        public async Task<int?> GetIdByLocationArea(int locationAreaId)
        {
            var locationAreaRepository = new Repository<LocationArea>(_context);
            var locationArea = await locationAreaRepository
                                 .GetFirstAsync(o => o.ID == locationAreaId);
            return locationArea != null ? locationArea.RootPostOffice : null;
        }
        public PostOffice GetByDistanceMin(IEnumerable<PostOffice> postOffices, double latDistance, double lngDistance)
        {
            int index = 0;
            dynamic minDistance = new ExpandoObject();
            minDistance.Distance = 0;
            minDistance.PostOffice = new PostOffice();
            minDistance.Id = 0;

            foreach (var postOffice in postOffices)
            {
                if (postOffice.Lat.HasValue && postOffice.Lng.HasValue)
                {
                    var distance = GeoHelper.distance(postOffice.Lat.Value, postOffice.Lng.Value, latDistance, lngDistance, 'K');
                    if (index == 0 || minDistance.Distance > distance)
                    {
                        minDistance.Distance = distance;
                        minDistance.PostOffice = postOffice;
                        minDistance.Id = postOffice.PostOfficeID;
                        index++;
                    }
                }
            }
            return minDistance.PostOffice;
        }
        public async Task<PostOffice> GetByOfficerId(int officeId)
        {
            var office = _context.Officers.FirstOrDefault(o => o.OfficerID == officeId);
            if (office != null)
            {
                var department = _context.Deparments.FirstOrDefault(o => o.DeparmentID == office.DeparmentID);
                if (department != null) return await this.GetFirstAsync(o => o.PostOfficeID == department.PostOfficeID);
            }
            return null;
        }
        public async Task<IEnumerable<PostOffice>> GetListPostOfficeInCenter(int postOfficeId)
        {
            if (_context.PostOffices.Any(o => o.PostOfficeID == postOfficeId))
            {
                var po = await this.GetSingleAsync(o => o.PostOfficeID == postOfficeId);
                var poCenterId = !po.POCenterID.HasValue ? postOfficeId : po.POCenterID;
                return await this.GetAsync(o => o.State == 0 && (o.POCenterID.Value == poCenterId || o.PostOfficeID == poCenterId));
            }
            else
            {
                return Enumerable.Empty<PostOffice>();
            }
        }
        public async Task<bool> SameCenter(int postOfficeId1, int postOfficeId2)
        {
            var postoffices = await this.GetListPostOfficeInCenter(postOfficeId1);
            return postoffices.Any(o => o.PostOfficeID == postOfficeId2);

        }

        public async Task<PostOffice> GetDistanceMinByLocation(int cityId, double lat, double lng, int? type)
        {
            var postOfficeId = (_context.Locations.SingleOrDefault( o => o.LocationId == cityId) ?? new Location()).PostOfficeId ?? 0;
            //var postOfficeId = (unitOfWork.AreaRepository._GetAreaById((areaId ?? 0)) ?? new Area()).CenterId ?? 0;
            var postOffices = await this.GetListPostOfficeInCenter(postOfficeId);
            if (type.HasValue)
            {
                switch (type.Value)
                {
                    case 1:
                        postOffices = postOffices.Where(o => (o.IsFrom ?? false)).ToList();
                        break;
                    case 2:
                        postOffices = postOffices.Where(o => (o.IsTo ?? false)).ToList();
                        break;
                }
            }
            if (postOffices.Count() == 1)
                return postOffices.First();
            else
            {
                var postOffice = this.GetByDistanceMin(postOffices, lat, lng);
                return postOffice;
            }

        }
    }

}
