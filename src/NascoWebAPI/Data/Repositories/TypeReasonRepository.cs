using Microsoft.EntityFrameworkCore;
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
 
        public async Task<bool> SameCenter(int postOfficeId1, int postOfficeId2)
        {
            var postoffices = this.GetListFromCenter(postOfficeId1);
            return postoffices.Any(o => o.PostOfficeID == postOfficeId2);
        }

        public async Task<PostOffice> GetDistanceMinByLocation(int cityId, double lat, double lng, int? type)
        {
            var postOfficeId = (await _context.Locations.SingleOrDefaultAsync(o => o.LocationId == cityId) ?? new Location()).PostOfficeId ?? 0;
            //var postOfficeId = (unitOfWork.AreaRepository._GetAreaById((areaId ?? 0)) ?? new Area()).CenterId ?? 0;
            var postOffice = this.GetSingle(o => o.State == 0 && o.PostOfficeID == postOfficeId);
            if (postOffice != null && !(postOffice.IsPartner ?? false) && ((postOffice.IsFrom ?? false) || (postOffice.IsTo ?? false)))
            {
                var postOffices = this.GetListFromCenter(postOfficeId, type);
                if (postOffices.Count() == 1)
                    return postOffices.First();
                return this.GetByDistanceMin(postOffices, lat, lng);
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
        public IEnumerable<PostOffice> GetListChild(PostOffice parent)
        {
            var children = parent.Recurse(x => _context.PostOffices.Where(y => x.PostOfficeID == y.ParentId && y.State == 0)); ;
            this.Get(o => (o.SetsId == parent.PostOfficeID || o.PostOfficeID == parent.SetsId) && o.State == 0 && parent.PostOfficeID != o.PostOfficeID)
                .ToList()
                .ForEach(o =>
                {
                    children = children.Union(o.Recurse(x => _context.PostOffices.Where(y => x.PostOfficeID == y.ParentId && y.State == 0)));
                });
            return children;
        }
        public IEnumerable<PostOffice> GetListChild(int parentId)
        {
            var parent = _context.PostOffices.FirstOrDefault(x => x.PostOfficeID == parentId && x.State == 0);
            if (parent != null) return this.GetListChild(parent);
            return Enumerable.Empty<PostOffice>();
        }
        public IEnumerable<PostOffice> GetListChild(int parentId, int? postOfficeMethodId = null)
        {
            var postOffices = this.GetListChild(parentId);
            return this.GetByMethod(postOffices, postOfficeMethodId);
        }
        public IEnumerable<PostOffice> GetListParent(PostOffice child)
        {
            return child.Recurse(x => _context.PostOffices.Where(y => x.ParentId == y.PostOfficeID && y.State == 0));
        }
        public IEnumerable<PostOffice> GetListParent(int childId)
        {
            var child = _context.PostOffices.FirstOrDefault(x => x.PostOfficeID == childId && x.State == 0);
            if (child != null) return this.GetListParent(child);
            return Enumerable.Empty<PostOffice>();
        }
        public IEnumerable<PostOffice> GetListParent(int childId, int? postOfficeMethodId = null)
        {
            var postOffices = this.GetListParent(childId);
            return this.GetByMethod(postOffices, postOfficeMethodId);
        }
        public IEnumerable<PostOffice> GetListFromLevel(int id, int? level = 0)
        {
            var leaf = _context.PostOffices.FirstOrDefault(x => x.PostOfficeID == id && x.State == 0);
            if (leaf != null && leaf.Level != ((level ?? leaf.Level) ?? 0))
            {
                leaf = this.GetListParent(leaf).FirstOrDefault(x => x.Level == level);
            }
            return leaf != null ? this.GetListChild(leaf) : Enumerable.Empty<PostOffice>();
        }
        public IEnumerable<PostOffice> GetListFromLevel(int id, int? level = 0, int? postOfficeTypeId = null)
        {
            return this.GetByMethod(GetListFromLevel(id, level), postOfficeTypeId);
        }
        public IEnumerable<PostOffice> GetListFromBranch(int id, int? postOfficeTypeId = null)
        {
            return this.GetListFromLevel(id, (int)PostOfficeType.BRANCH - 1, postOfficeTypeId);
        }
        public IEnumerable<PostOffice> GetListFromCenter(int id, int? postOfficeTypeId = null)
        {
            return this.GetListFromLevel(id, (int)PostOfficeType.HUB - 1, postOfficeTypeId);
        }
        public async Task<IEnumerable<PostOffice>> GetListFromRoot(int? postOfficeMethodId = null)
        {
            return GetByMethod(await GetAsync(o => o.State == 0), postOfficeMethodId);
        }
        public PostOffice GetBranch(int id)
        {
            var po = _context.PostOffices.FirstOrDefault(o => o.PostOfficeID == id);
            if (po != null && po.PostOfficeTypeId != (int)PostOfficeType.HEADQUARTER)
            {
                if (po.PostOfficeTypeId == (int)PostOfficeType.BRANCH)
                    return po;
                return this.GetListParent(po).FirstOrDefault(o => o.PostOfficeTypeId == (int)PostOfficeType.BRANCH);
            }
            return null;
        }
    }

}
