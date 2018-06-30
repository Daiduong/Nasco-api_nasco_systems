
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
        private readonly IApiClient apiClient;

        protected ClientBase(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected async Task<TResponse> GetJsonDecodedContent<TResponse, TContentResponse>(string uri, params KeyValuePair<string, string>[] requestParameters) where TResponse : ApiResponse<TContentResponse>, new()
        {
            var apiResponse = await apiClient.GetFormEncodedContent(uri, requestParameters);
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
            where TModel : ApiModel
            where TContentResponse : ApiModel
            where TResponse : ApiResponse<TContentResponse>, new()
        {
            using (var apiResponse = await apiClient.PostJsonEncodedContent(url, model))
            {
                var response = await CreateJsonResponse<TResponse>(apiResponse);
                if (response.StatusIsSuccessful)
                {
                    var token = JToken.Parse(response.ResponseResult);

                    if (token is JArray)
                    {
                        var jsonResponse = token.ToObject<TContentResponse>();
                        response.Data = jsonResponse;
                    }
                    else if (token is JObject)
                    {
                        var jsonResponse = JObject.Parse(response.ResponseResult);
                        response.CopyFromJOject(jsonResponse);
                    }
                }
                return response;
            }
        }
        protected async Task<TResponse> PostEncodedContent<TResponse, TContentResponse>(string url, TContentResponse model)
            where TContentResponse : ApiModel
            where TResponse : ApiResponse<TContentResponse>, new()
        {
            using (var apiResponse = await apiClient.PostJsonEncodedContent(url, model))
            {
                var response = await CreateJsonResponse<TResponse>(apiResponse);
                if (response.StatusIsSuccessful)
                {
                    var token = JToken.Parse(response.ResponseResult);
                    if (token is JArray)
                    {
                        var jsonResponse = token.ToObject<TContentResponse>();
                        response.Data = jsonResponse;
                    }
                    else if (token is JObject)
                    {
                        var jsonResponse = JObject.Parse(response.ResponseResult);
                        response.CopyFromJOject(jsonResponse);
                    }
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
