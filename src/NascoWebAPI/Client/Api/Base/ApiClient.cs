using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper.Logger;
using NascoWebAPI.Helper.Logging;
using NascoWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NascoWebAPI.Client
{

    public interface IApiClient
    {
        Task<HttpResponseMessage> GetFormEncodedContent(string requestUri, params KeyValuePair<string, string>[] values);
        Task<HttpResponseMessage> GetFormEncodedContent(string requestUri, KeyValuePair<string, string>[] headers, params KeyValuePair<string, string>[] values);

        Task<HttpResponseMessage> PostJsonEncodedContent<T>(string requestUri, T content) where T : class;
        Task<HttpResponseMessage> PostJsonEncodedContent<T>(string requestUri, KeyValuePair<string, string>[] headers, T content) where T : class;
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
            return await GetFormEncodedContent(requestUri, null, values);
        }
        public async Task<HttpResponseMessage> GetFormEncodedContent(string requestUri, KeyValuePair<string, string>[] headers, params KeyValuePair<string, string>[] values)
        {
            using (var client = new HttpClient(new LoggingHandler(new HttpClientHandler())))
            {
                //client.BaseAddress = new Uri(BaseUri);
                using (var content = new FormUrlEncodedContent(values))
                {
                    var query = await content.ReadAsStringAsync();
                    var requestUriWithQuery = string.Concat(requestUri, "?", query);
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                    var response = await client.GetAsync(requestUriWithQuery);
                    return response;
                }
            }
        }
        public async Task<HttpResponseMessage> PostJsonEncodedContent<T>(string requestUri, T content) where T : class
        {
            return await PostJsonEncodedContent<T>(requestUri, null, content);
        }
        public async Task<HttpResponseMessage> PostJsonEncodedContent<T>(string requestUri, KeyValuePair<string, string>[] headers, T content) where T : class
        {
            using (var client = new HttpClient(new LoggingHandler(new HttpClientHandler())))
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var response = await client.PostAsJsonAsync(requestUri, content);
                return response;
            }
        }
    }

}

