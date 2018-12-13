
using Microsoft.Extensions.Logging;
using NascoWebAPI.Client;
using NascoWebAPI.Helper;
using NascoWebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace NascoWebAPI.Client
{
    public abstract class ClientBase
    {
        private readonly IApiClient _apiClient;

        protected ClientBase(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        protected async Task<TResponse> GetJsonDecodedContent<TResponse, TContentResponse>(string uri, params KeyValuePair<string, string>[] requestParameters) where TResponse : ApiResponse<TContentResponse>, new()
        {
            return await GetJsonDecodedContent<TResponse, TContentResponse>(uri, null, requestParameters);
        }
        protected async Task<TResponse> GetJsonDecodedContent<TResponse, TContentResponse>(string uri, KeyValuePair<string, string>[] headers, params KeyValuePair<string, string>[] requestParameters) where TResponse : ApiResponse<TContentResponse>, new()
        {
            var apiResponse = await _apiClient.GetFormEncodedContent(uri, headers, requestParameters);
            var response = await CreateJsonResponse<TResponse>(apiResponse);
            if (response.StatusIsSuccessful)
            {
                var token = JToken.Parse(response.ResponseResult);
                var jsonResponse = token.ToObject<TContentResponse>();
                response.Data = jsonResponse;
            }
            return response;
        }
        protected async Task<TResponse> PostEncodedContent<TResponse, TContentResponse, TModel>(string url, TModel model)
            where TModel : class
            where TContentResponse : class
            where TResponse : ApiResponse<TContentResponse>, new()
        {
            return await PostEncodedContent<TResponse, TContentResponse, TModel>(url, null, model);
        }
        protected async Task<TResponse> PostEncodedContent<TResponse, TContentResponse, TModel>(string url, KeyValuePair<string, string>[] headers, TModel model)
           where TModel : class
           where TContentResponse : class
           where TResponse : ApiResponse<TContentResponse>, new()
        {
            using (var apiResponse = await _apiClient.PostJsonEncodedContent(url, headers, model))
            {
                var response = await CreateJsonResponse<TResponse>(apiResponse);
                if (response.StatusIsSuccessful)
                {
                    var token = JToken.Parse(response.ResponseResult);
                    var jsonResponse = token.ToObject<TContentResponse>();
                    response.Data = jsonResponse;
                }
                return response;
            }
        }
        protected async Task<TResponse> PostEncodedContent<TResponse, TContentResponse>(string url, TContentResponse model)
          where TContentResponse : class
          where TResponse : ApiResponse<TContentResponse>, new()
        {
            return await PostEncodedContent<TResponse, TContentResponse>(url, null, model);
        }
        protected async Task<TResponse> PostEncodedContent<TResponse, TContentResponse>(string url, KeyValuePair<string, string>[] headers, TContentResponse model)
            where TContentResponse : class
            where TResponse : ApiResponse<TContentResponse>, new()
        {
            using (var apiResponse = await _apiClient.PostJsonEncodedContent(url, headers, model))
            {
                var response = await CreateJsonResponse<TResponse>(apiResponse);
                if (response.StatusIsSuccessful)
                {
                    var token = JToken.Parse(response.ResponseResult);
                    var jsonResponse = token.ToObject<TContentResponse>();
                    response.Data = jsonResponse;
                }
                return response;
            }
        }

        private static async Task<TResponse> CreateJsonResponse<TResponse>(HttpResponseMessage response) where TResponse : ApiResponse, new()
        {
            var clientResponse = new TResponse
            {
                StatusIsSuccessful = response.IsSuccessStatusCode,
                ResponseCode = response.StatusCode
            };
            if (response.Content != null)
            {
                clientResponse.ResponseResult = await response.Content.ReadAsStringAsync();
            }

            return clientResponse;
        }
    }
}
