
using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Client
{
    public interface IEMSClient
    {
        Task<ApiResponse<EMSLadingResponse>> GetLading(string code);
        Task<ApiResponse<EMSLadingDeliveryResponse>> Delivery(EMSLadingDeliveryRequest request);
        Task<ApiResponse<EMSLadingDeliveryResponse>> DeliveryImage(EMSLadingDeliveryImageRequest request);

    }

    public class EMSClient : ClientBase, IEMSClient
    {
        private const string URI = EMSHelper.URI_BASE;
        private readonly KeyValuePair<string, string>[] HEADER_GETS;
        private readonly KeyValuePair<string, string>[] HEADER_POSTS;
        private readonly KeyValuePair<string, string>[] HEADER_IMAGE_POSTS;


        public EMSClient(IApiClient apiClient) : base(apiClient)
        {
            HEADER_GETS = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("key", EMSHelper.KEY_API_GET)
            };
            HEADER_POSTS = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("key", EMSHelper.KEY_API_POST)
            };
            HEADER_IMAGE_POSTS = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("key", EMSHelper.KEY_API_IMAGE_POST)
            };
        }
        public async Task<ApiResponse<EMSLadingResponse>> GetLading(string code)
        {
            var codePair = new KeyValuePair<string, string>("code", code);
            return await GetJsonDecodedContent<ApiResponse<EMSLadingResponse>, EMSLadingResponse>(URI + "E1InfomationV2", HEADER_GETS, codePair);
        }
        public async Task<ApiResponse<EMSLadingDeliveryResponse>> Delivery(EMSLadingDeliveryRequest request)
        {
            return await PostEncodedContent<ApiResponse<EMSLadingDeliveryResponse>, EMSLadingDeliveryResponse, EMSLadingDeliveryRequest>(URI + "api/Journey/Create", HEADER_POSTS, request);
        }
        public async Task<ApiResponse<EMSLadingDeliveryResponse>> DeliveryImage(EMSLadingDeliveryImageRequest request)
        {
            return await PostEncodedContent<ApiResponse<EMSLadingDeliveryResponse>, EMSLadingDeliveryResponse, EMSLadingDeliveryImageRequest>(URI + "api/Journey/Create", HEADER_IMAGE_POSTS, request);
        }
    }
}
