using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class LocationRepository : Repository<Location> , ILocationRepository
    {
        public LocationRepository(ApplicationDbContext conText) : base(conText)
        {

        }
        public async Task<IEnumerable<Location>> GetCities()
        => await this.GetAsync(o => o.State == (int)StatusSystem.Enable && o.Type == (int)LocationType.City);
        public async Task<IEnumerable<Location>> GetDistrictsByCity( int cityId)
        => await this.GetAsync(o => o.ParentId == cityId && o.Type == (int)LocationType.District);
        //Hàm Kiểm tra Location Trùng khớp nhất
        public int GetIdBestMatches(IEnumerable<Location> locations, string locationName)
        {
            locationName = locationName.ToAscii().Replace(" ", "");
            int index = 0;
            dynamic minDistance = new ExpandoObject();
            minDistance.Cost = 0;
            minDistance.Id = 0;
            foreach (var location in locations)
            {
                int cost = LevenshteinDistance.Compute(location.Name.ToAscii().Replace(" ", ""), locationName);
                if (index == 0 || minDistance.Cost > cost)
                {
                    minDistance.Cost = cost;
                    minDistance.Id = location.LocationId;
                    index++;
                }
            }
            return minDistance.Id;
        }
    }
}
