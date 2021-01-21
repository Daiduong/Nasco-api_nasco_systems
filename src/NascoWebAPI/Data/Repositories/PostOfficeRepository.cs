using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public async Task<PostOffice> GetByDistanceMin(IEnumerable<PostOffice> postOffices, double latDistance, double lngDistance)
        {
            var result = await GetListDistance(postOffices, latDistance, lngDistance);
            var min = result.Min(x => x.Value);
            return postOffices.First(x => x.PostOfficeID == result.First(y => y.Value == min).Key);
        }
        public async Task<IEnumerable<KeyValuePair<int, double>>> GetListDistance(IEnumerable<PostOffice> postOffices, double latDistance, double lngDistance)
        {
            dynamic minDistance = new ExpandoObject();
            minDistance.Distance = 0;
            minDistance.PostOffice = new PostOffice();
            minDistance.Id = 0;

            var tasks = new List<Task<KeyValuePair<int, double>>>();
            foreach (var postOffice in postOffices)
            {
                if (postOffice.Lat.HasValue && postOffice.Lng.HasValue)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        var distance = GeoHelper.distance(postOffice.Lat.Value, postOffice.Lng.Value, latDistance, lngDistance, 'K');
                        return new KeyValuePair<int, double>(postOffice.PostOfficeID, distance);
                    }));
                }
            }
            return await Task.WhenAll(tasks);
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

        public async Task<bool> SameCenter(int postOfficeId1, int postOfficeId2)
        {
            var postoffices = await this.GetListFromCenter(postOfficeId1);
            return postoffices.Any(o => o.PostOfficeID == postOfficeId2);
        }

        public async Task<PostOffice> GetDistanceMinByLocation(int cityId, double lat, double lng, int? type)
        {
            var postOfficeId = (await _context.Locations.SingleOrDefaultAsync(o => o.LocationId == cityId) ?? new Location()).PostOfficeId ?? 0;
            //var postOfficeId = (unitOfWork.AreaRepository._GetAreaById((areaId ?? 0)) ?? new Area()).CenterId ?? 0;
            var postOffice = this.GetSingle(o => o.State == 0 && o.PostOfficeID == postOfficeId);
            if (postOffice != null && !(postOffice.IsPartner ?? false) && ((postOffice.IsFrom ?? false) || (postOffice.IsTo ?? false)))
            {
                var postOffices = await this.GetListFromCenter(postOfficeId, type);
                if (postOffices.Count() == 1)
                    return postOffices.First();
                return await this.GetByDistanceMin(postOffices, lat, lng);
            }
            return postOffice;

        }
        public IEnumerable<PostOffice> GetByMethod(IEnumerable<PostOffice> postOffices, int? postOfficeMethodId = null)
        {
            if (postOfficeMethodId.HasValue && postOffices.Count() > 0)
            {
                switch (postOfficeMethodId.Value)
                {
                    case (int)PostOfficeMethod.FROM:
                        postOffices = postOffices.Where(x => (x.IsFrom ?? false));
                        break;
                    case (int)PostOfficeMethod.TO:
                        postOffices = postOffices.Where(x => (x.IsTo ?? false));
                        break;
                    case (int)PostOfficeMethod.PARTNER:
                        postOffices = postOffices.Where(x => (x.IsPartner ?? false));
                        break;
                }
            }
            return postOffices;
        }
        public async Task<IEnumerable<PostOffice>> GetListChild(PostOffice parent)
        {
            if (parent != null)
            {
                var children = parent.Traverse(x => _context.PostOffices.Where(y => x.PostOfficeID == y.ParentId && y.State == 0));
                _context.PostOffices.Where(o => (o.SetsId == parent.PostOfficeID || o.PostOfficeID == parent.SetsId) && o.State == 0 && parent.PostOfficeID != o.PostOfficeID)
                   .ForEach(o =>
                   {
                       children = children.Union(o.Traverse(x => _context.PostOffices.Where(y => x.PostOfficeID == y.ParentId && y.State == 0)));
                   });
                return children;
            }
            return Enumerable.Empty<PostOffice>();
        }
        public async Task<IEnumerable<PostOffice>> GetListChild(int parentId)
        {
            var parent = await _context.PostOffices.FirstOrDefaultAsync(x => x.PostOfficeID == parentId && x.State == 0);
            return (parent != null) ? await GetListChild(parent) : Enumerable.Empty<PostOffice>();
        }
        public async Task<IEnumerable<PostOffice>> GetListChild(int parentId, int? postOfficeMethodId = null)
        {
            var postOffices = await GetListChild(parentId);
            return this.GetByMethod(postOffices, postOfficeMethodId);
        }
        public async Task<IEnumerable<PostOffice>> GetListParent(PostOffice child, int? level = 0)
        {
            return await Task.Factory.StartNew(() => child.Traverse(x => _context.PostOffices.Where(y => x.ParentId == y.PostOfficeID && y.State == 0)));
        }
        public async Task<IEnumerable<PostOffice>> GetListParent(int childId, int? level = 0)
        {
            var child = await _context.PostOffices.FirstOrDefaultAsync(x => x.PostOfficeID == childId && x.State == 0);
            if (child != null) return await GetListParent(child, level);
            return Enumerable.Empty<PostOffice>();
        }
        public async Task<IEnumerable<PostOffice>> GetListParent(int childId, int? level = 0, int? postOfficeMethodId = null)
        {
            var postOffices = await GetListParent(childId, level);
            return GetByMethod(postOffices, postOfficeMethodId);
        }
        public async Task<IEnumerable<PostOffice>> GetListFromLevel(int id, int? level = 0)
        {
            var leaf = await _context.PostOffices.FirstOrDefaultAsync(x => x.PostOfficeID == id && x.State == 0);
            if (leaf != null && leaf.Level != ((level ?? leaf.Level) ?? 0))
            {
                leaf = (await GetListParent(leaf, level)).FirstOrDefault(x => x.Level == level);
            }
            return leaf != null ? await GetListChild(leaf) : Enumerable.Empty<PostOffice>();
        }
        public async Task<IEnumerable<PostOffice>> GetListFromLevel(int id, int? level = 0, int? postOfficeMethodId = null)
        {
            return GetByMethod(await GetListFromLevel(id, level), postOfficeMethodId);
        }
        public async Task<IEnumerable<PostOffice>> GetListFromBranch(int id, int? postOfficeTypeId = null)
        {
            return await this.GetListFromLevel(id, (int)PostOfficeType.BRANCH - 1, postOfficeTypeId);
        }
        public async Task<IEnumerable<PostOffice>> GetListFromCenter(int id, int? postOfficeTypeId = null)
        {
            return await this.GetListFromLevel(id, (int)PostOfficeType.HUB - 1, postOfficeTypeId);
        }
        public async Task<IEnumerable<PostOffice>> GetListFromRoot(int? postOfficeMethodId = null)
        {
            return GetByMethod(await GetAsync(o => o.State == 0), postOfficeMethodId);
        }
        public async Task<PostOffice> GetBranch(int id)
        {
            var po = _context.PostOffices.FirstOrDefault(o => o.PostOfficeID == id);
            if (po != null && po.PostOfficeTypeId != (int)PostOfficeType.HEADQUARTER)
            {
                if (po.PostOfficeTypeId == (int)PostOfficeType.BRANCH)
                    return po;
                return (await this.GetListParent(po)).FirstOrDefault(o => o.PostOfficeTypeId == (int)PostOfficeType.BRANCH);
            }
            return null;
        }
        #region Load Airport
        public async Task<IEnumerable<PostOffice>> GetListPostOfficeAirport(double lat, double lng)
        {
            var postOffices = await _context.PostOffices.Where(x => x.State == 0 && !(x.IsPartner ?? false) && (x.IsAirport ?? false)).ToListAsync();
            if (lat == 0 || lng == 0) return postOffices;
            var distances = await GetListDistance(postOffices, lat, lng);
            return postOffices
                .Join(distances, x => x.PostOfficeID, y => y.Key, (x, y) => new { x, y })
                .OrderBy(x => x.y.Value).Select(x => x.x);
        }
        #endregion
    }

}
