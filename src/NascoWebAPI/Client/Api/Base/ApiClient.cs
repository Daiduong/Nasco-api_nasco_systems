using NascoWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NascoWebAPI.Client
{

    public interface IApiClient
    {
        Task<HttpResponseMessage> GetFormEncodedContent(string requestUri, params KeyValuePair<string, string>[] values);
        Task<HttpResponseMessage> PostJsonEncodedContent<T>(string requestUri, T content) where T : ApiModel;
    }
    public class ApiClient : IApiClient
    {
        private readonly HttpClient httpClient;
        public ApiClient()
        {
            this.httpClient = new HttpClient();
        }
        public ApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<HttpResponseMessage> GetFormEncodedContent(string requestUri, params KeyValuePair<string, string>[] values)
        {
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(BaseUri);
                using (var content = new FormUrlEncodedContent(values))
                {
                    var query = await content.ReadAsStringAsync();
                    var requestUriWithQuery = string.Concat(requestUri, "?", query);
                    var response = await client.GetAsync(requestUriWithQuery);
                    return response;
                }
            }
        }
        public async Task<HttpResponseMessage> PostJsonEncodedContent<T>(string requestUri, T content) where T : ApiModel
        {
            //httpClient.BaseAddress = new Uri(BaseUri);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.PostAsJsonAsync(requestUri, content);
            return response;
        }
    }
}

