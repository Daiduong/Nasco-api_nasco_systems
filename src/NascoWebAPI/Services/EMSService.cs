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
    public interface IEMSService
    {
        Task<ResultModel<EMSLading>> GetLading(string code);
        Task<ResultModel<EMSLadingDeliveryResponse>> Delivery(EMSLadingDeliveryRequest request);
        Task<ResultModel<EMSLadingDeliveryResponse>> DeliveryImage(EMSLadingDeliveryImageRequest request);
    }
    public class EMSService : IEMSService
    {
        private readonly IEMSClient _iEMSClient;

        public EMSService(
            IEMSClient eMSClient
            )
        {
            _iEMSClient = eMSClient;
        }
        public EMSService()
        {
            var apiClient = new ApiClient();
            _iEMSClient = new EMSClient(apiClient);
        }
        public async Task<ResultModel<EMSLading>> GetLading(string code)
        {
            var result = new ResultModel<EMSLading>();
            var response = await _iEMSClient.GetLading(code);
            if(response.Error == 0)
            {
                if (response.Data.Code == EMSHelper.STATUS_OK)
                {
                    return result.Init(response.Data.Data, "", 0);
                }
                else result.Message = response.Data.Message;
            }
            result.Message = response.Message;
            return result;
        }
        public async Task<ResultModel<EMSLadingDeliveryResponse>> Delivery(EMSLadingDeliveryRequest request)
        {
            var result = new ResultModel<EMSLadingDeliveryResponse>();
            var response = await _iEMSClient.Delivery(request);
            if (response.Error == 0)
            {
                if (response.Data.Code == EMSHelper.STATUS_OK)
                {
                    return result.Init(response.Data, "", 0);
                }
                else result.Message = response.Data.Message;
            }
            result.Message = response.Message;
            return result;
        }
        public async Task<ResultModel<EMSLadingDeliveryResponse>> DeliveryImage(EMSLadingDeliveryImageRequest request)
        {
            var result = new ResultModel<EMSLadingDeliveryResponse>();
            var response = await _iEMSClient.DeliveryImage(request);
            if (response.Error == 0)
            {
                if (response.Data.Code == EMSHelper.STATUS_OK)
                {
                    return result.Init(response.Data, "", 0);
                }
                else result.Message = response.Data.Message;
            }
            result.Message = response.Message;
            return result;
        }
    }
}
