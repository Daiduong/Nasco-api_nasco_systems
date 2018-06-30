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
        => await this.GetAsync(o => o.State == (int)StatusSystem.Enable && o.ParentId == cityId && o.Type == (int)LocationType.District);
        public int GetIdBestMatches(IEnumerable<Location> locations, string locationName, uint? percentlimitCost = null)
        {
            string[] replaces = { " ", "NUOC", "TINH", "THANHPHO", "THUATHIEN", "QUAN", "PHUONG", "XA", "HUYEN", "TP", "TT", "COUNTRY", "CITY", "STATE", "PROVINCE", "DISTRICT", "WARD" };
            locationName = locationName.ToAscii().ToUpper();
            locationName = replaces.Aggregate(locationName, (c1, c2) => c1.Replace(c2, ""));
            uint limitCost = (percentlimitCost ?? 0) != 0 ? (uint)(locationName.Length) * percentlimitCost.Value / 100 : 0;
            int index = 0;
            dynamic minDistance = new ExpandoObject();
            minDistance.Cost = 0;
            minDistance.Id = 0;
            var name = "";
            foreach (var location in locations)
            {
                string shortName = location.ShortName.ToAscii().ToUpper();
                shortName = replaces.Aggregate(shortName, (c1, c2) => c1.Replace(c2, ""));
                int cost = LevenshteinDistance.Compute(shortName, locationName);
                if (cost == 0)
                {
                    minDistance.Cost = cost;
                    minDistance.Id = location.LocationId;
                    break;
                }
                if (index == 0 || minDistance.Cost > cost)
                {
                    minDistance.Cost = cost;
                    minDistance.Id = location.LocationId;
                    name = location.Name;
                    index++;
                }
            }
            if (minDistance.Cost > limitCost && limitCost > 0)
            {
                return 0;
            }
            return minDistance.Id;
        }
    }
}
