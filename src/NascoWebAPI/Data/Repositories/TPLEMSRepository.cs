using NascoWebAPI.Data.Entities;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    public class TPLEMSRepository : Repository<Lading>, ITPLEMSRepository
    {
        public TPLEMSRepository(ApplicationDbContext context) : base(context)
        {

        }
        public dynamic GetInventory()
        {

            string url = "http://staging.ws.ems.com.vn/api/v1/inventory/list?merchant_token={0}";
            const string merchant_token = "f68bfe23861702b8b21bcba5568aa973";
            url = string.Format(url, merchant_token);


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            var result = client.GetAsync(url);

            var _response = result.Result.Content;
            var respone = _response.ReadAsStringAsync().Result.ToString();

            dynamic check = JsonConvert.DeserializeObject(respone).ToString();
            return check;
        }
        public async Task<string> InsertInventory(string name, string username,
                        string phone, string provinceCode, string districtCode, string wardCode, string address)
        {

            string url = "http://staging.ws.ems.com.vn/api/v1/inventory/create?merchant_token={0}";
            const string merchant_token = "f68bfe23861702b8b21bcba5568aa973";
            url = string.Format(url, merchant_token);



            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            var datapost = new
            {
                name = name,
                username = username,
                phone = phone,
                province_code = provinceCode,
                district_code = districtCode,
                ward_code = wardCode,
                address = address,

            };

            var result = await client.PostAsJsonAsync(url, datapost);

            var _response = result.Content;
            var respone = _response.ReadAsStringAsync().Result.ToString();
            string check = JsonConvert.DeserializeObject(respone).ToString();
            return check;
        }
        public async Task<string> CreateShipment(CreateShipment model)
        {

            string url = "http://staging.ws.ems.com.vn/api/v1/orders/create?merchant_token={0}";
            const string merchant_token = "f68bfe23861702b8b21bcba5568aa973";
            url = string.Format(url, merchant_token);



            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            var datapost = new
            {
                model

            };

            var result = await client.PostAsJsonAsync(url, datapost);

            var _response = result.Content;
            var respone = _response.ReadAsStringAsync().Result.ToString();

            dynamic check = JsonConvert.DeserializeObject(respone).ToString();
            return check;
        }
    }
}
public class CreateShipment
{
    public int inventory { get; set; }
    public string to_name { get; set; }
    public string to_phone { get; set; }
    public string to_province { get; set; }
    public string to_district { get; set; }
    public string to_ward { get; set; }
    public string to_address { get; set; }
    public string order_code { get; set; }
    public string product_name { get; set; }
    public int total_amount { get; set; }
    public int total_quantity { get; set; }
    public int total_weight { get; set; }
    public int money_collect { get; set; }
    public string description { get; set; }
    public int service { get; set; }
    public bool? Checked { get; set; }
    public bool? fragile { get; set; }
    public string from_province { get; set; }
}
