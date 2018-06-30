using NascoWebAPI.Client;
using NascoWebAPI.Data;
using NascoWebAPI.Models;
using NascoWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Services
{
    public interface IGoogleMapService
    {
        Task<ResultModel<LocationModel>> GetLocation(GeocoderRequest request, int index = 0);
    }
    public class GoogleMapService : IGoogleMapService
    {
        private readonly IGoogleClient _googleClient;
        private readonly ILocationRepository _locationRepository;
        public GoogleMapService(
            IGoogleClient googleClient,
            ILocationRepository locationRepository
            )
        {
            _googleClient = googleClient;
            _locationRepository = locationRepository;
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
                        "route"
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
                    foreach (var geocoderResult in geocoderResponse.Results.Reverse())
                    {
                        if (geocoderResult.Types.Any(o => typeApplies.Contains(o)))
                        {
                            location.Lat = geocoderResult.Geometry.Location.Lat;
                            location.Lng = geocoderResult.Geometry.Location.Lng;
                            break;
                        }
                    }
                    if (location.Lat == 0 || string.IsNullOrEmpty(location.CityName) || string.IsNullOrEmpty(location.DistrictName))
                    {
                        return await this.GetLocation(new GeocoderRequest() { Location = geocoderResponse.Results[0].Geometry.Location });
                    }
                    if (!string.IsNullOrEmpty(location.CityName) && !string.IsNullOrEmpty(location.DistrictName))
                    {
                        var cities = await _locationRepository.GetCities();
                        var cityId = _locationRepository.GetIdBestMatches(cities, location.CityName);
                        var districts = await _locationRepository.GetDistrictsByCity(cityId);
                        var districtId = _locationRepository.GetIdBestMatches(districts, location.DistrictName, 50);
                        location.DistrictId = districtId;
                        location.CityId = cityId;
                        if (location.DistrictId == 0)
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
                        if (addressComponent.Types.Any(o => o == "administrative_area_level_2"))
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
    }
}
