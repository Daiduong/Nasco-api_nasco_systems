
using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper;
using NascoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Client
{
    public interface IFahamiClient
    {
        Task<ApiResponse<ProductTypeViewModel>> GetProductTypes();
    }

    public class FahamiClient : ClientBase, IFahamiClient
    {
        private const string URI = "http://api.fahami.com.vn/";
        private readonly KeyValuePair<string, string>[] HEADER_GETS;

        public FahamiClient(IApiClient apiClient) : base(apiClient)
        {
            HEADER_GETS = new KeyValuePair<string, string>[] {
                 new KeyValuePair<string, string>("Authorization","Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiY19oYXNoIjoiYzJlZjcwODcyMTI2YmJjYzNjMzQxNzVkYzM5MjZhMTJhY2NjNWY0NTU2M2MwNWRiZGFkYTJhOGIzM2ZhNzc5YSIsImp0aSI6IjExY2YyNzA0LWI4MDEtNDJhOC1hMzFhLTBmOGRhY2RiZDExMyIsImlhdCI6MTU0MDk4Njk2MywibmJmIjoxNTQwOTg2OTYzLCJleHAiOjE1NDM1Nzg5NjMsImlzcyI6Iklzc3VlciIsImF1ZCI6IkF1ZGllbmNlIn0.mL_pgvQkkE1-ONxMSAnNWmHVzXaD3IWvn7fQCudxV2k" )
            };
        }
        public async Task<ApiResponse<ProductTypeViewModel>> GetProductTypes()
        {
            return await GetJsonDecodedContent<ApiResponse<ProductTypeViewModel>, ProductTypeViewModel>(URI + "api/newspaper/getall", HEADER_GETS, null);
        }

    }
    public class ProductTypeViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public ProductTypeViewModel()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
