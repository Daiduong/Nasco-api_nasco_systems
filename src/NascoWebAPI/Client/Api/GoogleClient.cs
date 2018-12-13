
using NascoWebAPI.Models;
using NascoWebAPI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Client
{
    public interface IGoogleClient
    {
        Task<ApiResponse<GeocoderResponse>> GetGeocoder(GeocoderRequest request);
        Task<ApiResponse<DistanceMatrixResponse>> GetDistanceMatrix(DistanceMatrixRequest request);
    }

    public class GoogleClient : ClientBase, IGoogleClient
    {
        private const string GOOGLEMAPURI = "https://maps.googleapis.com/";
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
            return await GetJsonDecodedContent<ApiResponse<GeocoderResponse>, GeocoderResponse>(GOOGLEMAPURI + "maps/api/geocode/json", keyPair, parameterPair);
        }
        public async Task<ApiResponse<DistanceMatrixResponse>> GetDistanceMatrix(DistanceMatrixRequest request)
        {
            var originsPair = new KeyValuePair<string, string>("origins", string.Join("|", request.Origins));
            var destinationsPair = new KeyValuePair<string, string>("destinations", string.Join("|", request.Destinations));
            var modePair = new KeyValuePair<string, string>("mode", request.Mode);
            var keyPair = new KeyValuePair<string, string>("key", KEY_API);
            return await GetJsonDecodedContent<ApiResponse<DistanceMatrixResponse>, DistanceMatrixResponse>(GOOGLEMAPURI + "maps/api/distancematrix/json", keyPair, originsPair, destinationsPair, modePair);
        }
    }
}
