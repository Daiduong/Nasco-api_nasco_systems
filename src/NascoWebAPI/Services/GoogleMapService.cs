using NascoWebAPI.Client;
using NascoWebAPI.Data;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using NascoWebAPI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NascoWebAPI.Services
{
    public interface IGoogleMapService
    {
        Task<ResultModel<LocationModel>> GetLocation(GeocoderRequest request, int index = 0);
        Task<KeyValuePair<int, double>> GetPostOfficeMinDistance(int type, int cityId, double lat, double lng, bool isBulky);
        Task<KeyValuePair<int, double>> GetDistancePostOffice(int postOfficeId, double lat, double lng);
    }
    public class GoogleMapService : IGoogleMapService
    {
        private readonly IGoogleClient _googleClient;
        private readonly ILocationRepository _locationRepository;
        private readonly IPostOfficeRepository _postOfficeRepository;
        public GoogleMapService()
        {
            var apiClient = new ApiClient();
            _googleClient = new GoogleClient(apiClient);
            var context = new ApplicationDbContext();
            _locationRepository = new LocationRepository(context);
            _postOfficeRepository = new PostOfficeRepository(context);
        }
        public GoogleMapService(
            IGoogleClient googleClient,
            ILocationRepository locationRepository,
            IPostOfficeRepository postOfficeRepository
            )
        {
            _googleClient = googleClient;
            _locationRepository = locationRepository;
            _postOfficeRepository = postOfficeRepository;
        }
      
        public async Task<ResultModel<LocationModel>> GetLocation(GeocoderRequest request, int index = 0)
        {
            var resultGetGeocoder = await _googleClient.GetGeocoder(request);
            var result = new ResultModel<LocationModel>();
            if (resultGetGeocoder.StatusIsSuccessful)
            {
                var geocoderResponse = resultGetGeocoder.Data;
                if (geocoderResponse.Status == GoogleMapHelper.STATUS_OK)
                {
                    var typeApplies = new string[] {
                        "street_address",
                        "political",
                        "locality",
                        "route",
                        "airport"
                    };
                    var location = new LocationModel();
                    foreach (var geocoderResult in geocoderResponse.Results.Reverse())
                    {
                        location = this.GetByType(geocoderResult.Address_components, index);
                        if (!string.IsNullOrEmpty(location.CityName) && !string.IsNullOrEmpty(location.DistrictName))
                        {
                            break;
                        };
                    };
                    location.Lat = request.Location?.Lat ?? 0;
                    location.Lng = request.Location?.Lng ?? 0;
                    if (location.Lat == 0 || location.Lng == 0)
                    {
                        foreach (var geocoderResult in geocoderResponse.Results)
                        {
                            if (geocoderResult.Types.Any(o => typeApplies.Contains(o)))
                            {
                                location.Lat = geocoderResult.Geometry.Location.Lat;
                                location.Lng = geocoderResult.Geometry.Location.Lng;
                                break;
                            }
                        }
                    }
                    if ((location.Lat == 0 || string.IsNullOrEmpty(location.CityName) || string.IsNullOrEmpty(location.DistrictName)) && index <= 1)
                    {
                        return await this.GetLocation(new GeocoderRequest() { Location = geocoderResponse.Results[0].Geometry.Location }, ++index);
                    }
                    if (!string.IsNullOrEmpty(location.CityName) && !string.IsNullOrEmpty(location.DistrictName))
                    {
                        var cities = await _locationRepository.GetCities();
                        var cityId = _locationRepository.GetIdBestMatches(cities, location.CityName);
                        var districts = await _locationRepository.GetDistrictsByCity(cityId);
                        var districtId = _locationRepository.GetIdBestMatches(districts, location.DistrictName, 50);

                        location.DistrictId = districtId;
                        location.CityId = cityId;
                        if (location.DistrictId == 0 && index < 1)
                        {
                            return await this.GetLocation(new GeocoderRequest() { Location = geocoderResponse.Results[0].Geometry.Location }, ++index);
                        }

                        result.Error = 0;
                        result.Data = location;
                    };
                }
                else
                {
                    result.Message = geocoderResponse.Status + " - " + geocoderResponse.Error_message;
                }

            }
            else
            {
                result.Message = resultGetGeocoder.Message;
            }
            return result;


        }
        private LocationModel GetByType(AddressComponent[] addressComponents, int index = 0)
        {
            var result = new LocationModel();
            var districtName = "";
            var cityName = "";
            if (addressComponents != null)
            {
                foreach (var addressComponent in addressComponents.Reverse())
                {
                    if (index == 0)
                    {
                        if (addressComponent.Types.Any(o => o == "administrative_area_level_1"))
                        {
                            cityName = addressComponent.Short_name;
                        }
                        else if (addressComponent.Types.Any(o => o == "administrative_area_level_2"))
                        {
                            if (string.IsNullOrEmpty(cityName)) cityName = addressComponent.Short_name;
                            else districtName = addressComponent.Short_name;
                        }
                        //else if (addressComponent.Types.Any(o => o == "locality"))
                        //{
                        //    if (string.IsNullOrEmpty(cityName)) cityName = addressComponent.Short_name;
                        //    else districtName = addressComponent.Short_name;
                        //}
                        else if (string.IsNullOrEmpty(districtName) && addressComponent.Types.Any(o => o == "sublocality"))
                        {
                            districtName = addressComponent.Short_name;
                        }
                        else if (string.IsNullOrEmpty(districtName) && addressComponent.Types.Any(o => o == "sublocality_level_1"))
                        {
                            districtName = addressComponent.Short_name;
                        }
                    }
                    else
                    {
                        if (addressComponent.Types.Any(o => o == "administrative_area_level_2") || addressComponent.Types.Any(o=> o == "locality"))
                        {
                            cityName = addressComponent.Short_name;
                        }
                        else if (addressComponent.Types.Any(o => o == "sublocality"))
                        {
                            districtName = addressComponent.Short_name;
                        }
                        else if (string.IsNullOrEmpty(districtName) && addressComponent.Types.Any(o => o == "sublocality_level_1"))
                        {
                            districtName = addressComponent.Short_name;
                        }
                    }
                }
            }
            result.CityName = cityName;
            result.DistrictName = districtName;
            return result;
        }

        #region Distance Matrix
        public async Task<ResultModel<DistanceMatrixResult[]>> GetDistanceMatrix(DistanceMatrixRequest request)
        {
            var resultGetGeocoder = await _googleClient.GetDistanceMatrix(request);
            var result = new ResultModel<DistanceMatrixResult[]>();
            if (resultGetGeocoder.StatusIsSuccessful)
            {
                var geocoderResponse = resultGetGeocoder.Data;
                if (geocoderResponse.Status == GoogleMapHelper.STATUS_OK)
                {
                    result.Error = 0;
                    result.Data = geocoderResponse.Rows;
                }
            }
            return result;
        }
        public async Task<KeyValuePair<int, double>> GetPostOfficeMinDistance(int type, int cityId, double lat, double lng, bool isBulky)
        {
            var postOfficeId = (_locationRepository.GetSingle(o => o.LocationId == cityId) ?? new Location()).PostOfficeId ?? 0;
            var postOffice = await _postOfficeRepository.GetFirstAsync(x => x.PostOfficeID == postOfficeId);
            if (lat != 0 && lng != 0 && postOffice != null && !(postOffice.IsPartner ?? false) && ((postOffice.IsFrom ?? false) || (postOffice.IsTo ?? false)))
            {
                var postOffices = await _postOfficeRepository.GetListFromCenter(postOfficeId, type);
                postOffices = postOffices.Where(x => x.Lat != null && x.Lng != null);
                if(postOffices.Count() == 1)
                {
                    isBulky = true;
                }
                double dMinFar = 30000;
                var result = await GetPostOfficeMinDistance(postOffices, lat, lng, isBulky);
                if (!isBulky && result.Value > dMinFar)
                {
                    result = await GetPostOfficeMinDistance(postOffices, lat, lng, !isBulky);
                }
                return result;
            }
            return new KeyValuePair<int, double>(postOffice.PostOfficeID, 0);
        }
        public async Task<KeyValuePair<int, double>> GetPostOfficeMinDistance(IEnumerable<PostOffice> postOffices, double lat, double lng, bool isBulky)
        {
            if (lat == 0 || lng == 0)
                return new KeyValuePair<int, double>(0, 0);
            postOffices = postOffices
                .Where(x => x.Lat != null && x.Lng != null);
            var origins = postOffices
                .Select(x => new KeyValuePair<int, string>(x.PostOfficeID, x.Lat + "," + x.Lng))
                .ToArray();
            var request = new DistanceMatrixRequest
            {
                Origins = origins.Select(x => x.Value).ToArray(),
                Destinations = new string[] { lat + "," + lng },
                Mode = isBulky ? GoogleMapHelper.TRAVEL_MODE_DRIVING : GoogleMapHelper.TRAVEL_MODE_WALKING
            };
            var result = await this.GetDistanceMatrix(request);
            if (result.Error != 0)
                return new KeyValuePair<int, double>(0, 0);
            var rows = result.Data;
            var elements = rows.Select(x => x.Elements[0]).ToArray();
            if (elements.Where(x => x.Distance != null).Count() == 0)
            {
                return new KeyValuePair<int, double>(0, 0);
            }
            double minDistance = elements[0].Distance.Value;
            int index = 0;
            if (origins.Count() > 1)
            {
                minDistance = elements.Where(x => x.Distance != null).Min(x => x.Distance.Value);
                var postOfficeHUBIndexs = postOffices.Select((po, i) => new { po, i }).Where(x => x.po.PostOfficeTypeId == (int)PostOfficeType.HUB && !(x.po.IsPartner ?? false));
                if (isBulky && postOfficeHUBIndexs.Count() != 0) // OTO
                {
                    index = postOfficeHUBIndexs.FirstOrDefault().i;
                    if (postOfficeHUBIndexs.Count() >= 2)
                    {
                        var poHUB = postOfficeHUBIndexs.OrderBy(x => (elements[x.i].Distance?.Value ?? 0)).First();
                        double dHUB = elements[poHUB.i].Distance?.Value ?? 0;
                        index = poHUB.i;
                        if (poHUB.po.SetsId != poHUB.po.PostOfficeID)
                        {
                            var poHUBSet = postOfficeHUBIndexs.FirstOrDefault(x => x.po.PostOfficeID == poHUB.po.SetsId);
                            if (poHUBSet != null)
                            {
                                double dBetween = 12000;
                                double dMaxHub = 20000;
                                var dHUBSet = elements[poHUBSet.i].Distance?.Value ?? 0;
                                if (dHUBSet - dBetween < dHUB && dHUBSet < dMaxHub)
                                {
                                    index = poHUBSet.i;
                                }
                            }
                        }

                    }
                    minDistance = elements[index].Distance?.Value ?? 0;
                }
                else
                {
                    index = Array.FindIndex(elements, x => x.Distance != null && x.Distance.Value == minDistance);
                }
            }
            return new KeyValuePair<int, double>(origins[index].Key, minDistance / 1000);
        }
        public async Task<KeyValuePair<int, double>> GetDistancePostOffice(int postOfficeId, double lat, double lng)
        {
            var postOffice = await _postOfficeRepository.GetFirstAsync(x => x.PostOfficeID == postOfficeId);
            double distance = 0;
            if (lat != 0 && lng != 0 && postOffice != null && postOffice.Lat.HasValue && postOffice.Lng.HasValue)
            {
                var request = new DistanceMatrixRequest
                {
                    Origins = new string[] { postOffice.Lat + "," + postOffice.Lng },
                    Destinations = new string[] { lat + "," + lng },
                };
                var result = await this.GetDistanceMatrix(request);
                if (result.Error == 0)
                {
                    distance = (result.Data[0].Elements[0].Distance?.Value ?? 0) / 1000;
                }
            }
            return new KeyValuePair<int, double>(postOfficeId, distance);
        }
        #endregion
    }
}
