
using NascoWebAPI.Models;
using NascoWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Client
{
    public interface IGoogleClient
    {
        Task<ApiResponse<GeocoderResponse>> GetGeocoder(GeocoderRequest request);
    }

    public class GoogleClient : ClientBase, IGoogleClient
    {
        private const string GEOCODEURI = "https://maps.googleapis.com/maps/api/geocode/json";
        private readonly string KEY_API;
        public GoogleClient(IApiClient apiClient) : base(apiClient)
        {
#if DEBUG
            KEY_API = GoogleMapHelper.KEY_API_LOCAL;
#else
            KEY_API = GoogleMapHelper.KEY_API_IP;
#endif
        }
        public async Task<ApiResponse<GeocoderResponse>> GetGeocoder(GeocoderRequest request)
        {
            var parameterPair = new KeyValuePair<string, string>();
            if (!string.IsNullOrEmpty(request.Address))
            {
                parameterPair = new KeyValuePair<string, string>("address", request.Address);
            }
            else if (request.Location != null && request.Location.Lat != 0)
            {
                parameterPair = new KeyValuePair<string, string>("latlng", request.Location.Lat + "," + request.Location.Lng);
            }
            var keyPair = new KeyValuePair<string, string>("key", KEY_API);
            return await GetJsonDecodedContent<ApiResponse<GeocoderResponse>, GeocoderResponse>(GEOCODEURI, keyPair, parameterPair);
        }
    }
}
