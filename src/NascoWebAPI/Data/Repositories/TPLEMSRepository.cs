using NascoWebAPI.Data.Entities;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public List<Location> map()
        {
            return _context.Locations.Where(x => x.Type == 2).ToList();
        }
        public bool UpdateDistrictTemp(List<DistrictTemp> model)
        {
            _context.DistrictTemps.UpdateRange(model);
            _context.SaveChanges();
            return true;
        }
        public List<DistrictTemp> DistrictTemp()
        {
            return _context.DistrictTemps.Where(x => x.Id > 0).ToList();
        }

        public DistrictTemp GetEMSArea(int districtId)
        {
            return _context.DistrictTemps.Where(x => x.DistrictId.Value == districtId).FirstOrDefault();
        }

        public dynamic EMSCallBack(RequestEMS model)
        {
            var lading = _context.Ladings.Where(x => x.PartnerCode == model.tracking_code).SingleOrDefault();
            int status = (int)EMSHelper.StatusEMSMapping(model.status_code);
            lading.Status = status;
            if (model.status_code == 6)
            {
                lading.FinishDate = DateTime.ParseExact(model.datetime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            if (model.status_code == 8)
            {
                lading.Return = true;
            }
            LadingHistory ldHistory = new LadingHistory
            {
                LadingId = lading.Id,
                OfficerId = lading.OfficerId,
                Location = model.locate,
                //var office = iOfficerRepository.GetSingle(o => o.OfficerID == entity.OfficerId);
                //var postOffice = iDepartmentRepository.GetSingle(o => o.DeparmentID == office.DeparmentID);
                PostOfficeId = lading.POCurrent,
                Status = 1,
                DateTime = DateTime.Now,
                CreatedDate = DateTime.Now,
                TypeReason = null,
                Note = model.note
            };
            _context.LadingHistories.Add(ldHistory);
            _context.SaveChanges();
            return 1;
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
            var json = JsonConvert.SerializeObject(datapost);
            var result = await client.PostAsJsonAsync(url, datapost);
            var _response = result.Content;
            var respone = _response.ReadAsStringAsync().Result.ToString();
            string check = JsonConvert.DeserializeObject(respone).ToString();
            EMSLogInventoryTable log = new EMSLogInventoryTable();
            log.RequestUrl = url;
            log.RequestContent = json;
            log.Response = check;
            _context.EMSLogInventoryTables.Add(log);
            _context.SaveChanges();
            return check;
        }
        public async Task<ResultCreteShipment> CreateShipment(CreateShipment model)
        {

            string url = "http://staging.ws.ems.com.vn/api/v1/orders/create?merchant_token={0}";
            const string merchant_token = "f68bfe23861702b8b21bcba5568aa973";
            url = string.Format(url, merchant_token);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            var datapost = new
            {
                inventory = model.inventory,
                to_name = model.to_name,
                to_phone = model.to_phone,
                to_province = model.to_province,
                to_district = model.to_district,
                to_ward = model.to_ward,
                to_address = model.to_address,
                order_code = model.order_code,
                product_name = model.product_name,
                total_amount = model.total_amount,
                total_quantity = model.total_quantity,
                total_weight = model.total_weight,
                money_collect = model.money_collect,
                description = model.description,
                service = model.service,
                Checked = model.Checked,
                fragile = model.fragile,
            };

            //var stt = datapost.model;// ToString();
            var json = JsonConvert.SerializeObject(model);
            var result = await client.PostAsJsonAsync(url, datapost);
            var _response = result.Content;
            var respone = _response.ReadAsStringAsync().Result.ToString();
            var check = JsonConvert.DeserializeObject<ResultCreteShipment>(respone);
            EMSLogShipmentTable log = new EMSLogShipmentTable();
            log.RequestUrl = url;
            log.RequestContent = json;
            log.Response = respone;
            _context.EMSLogShipmentTables.Add(log);
            _context.SaveChanges();
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
}

public class RequestEMS
{
    public string tracking_code { get; set; }
    public string order_code { get; set; }
    public int status_code { get; set; }
    public string status_name { get; set; }
    public string note { get; set; }
    public string locate { get; set; }
    public string datetime { get; set; }
    public int total_weight { get; set; }
}

#region model shipment
public class ResultCreteShipment
{
    public string code { get; set; }
    public string message { get; set; }
    public data data { get; set; }


}
public class data
{
    public feedata fee { get; set; }
    public vasdata vas { get; set; }
    public double money_collect { get; set; }
    public int estimate { get; set; }
    public string tracking_code { get; set; }
    public string status_code { get; set; }
}
public class feedata
{
    public double fee { get; set; }
    public double remote_fee { get; set; }
}
public class vasdata
{
    public double total_vas { get; set; }
    public List<vas_detaildata> vas_detail { get; set; }
}
public class vas_detaildata
{
    public string code { get; set; }
    public double fee { get; set; }
}
#endregion

#region model inventory
public class InventoryModel
{
    public string code { get; set; }
    public string message { get; set; }
    public List<dataInventory> data { get; set; }
}
public class dataInventory
{
    public int id { get; set; }
    public string name { get; set; }
    public string username { get; set; }
    public string phone { get; set; }
    public string country_code { get; set; }
    public string province_code { get; set; }
    public string district_code { get; set; }
    public string ward_code { get; set; }
    public string address { get; set; }
    public int active { get; set; }
    public string created_at { get; set; }
    public string country_name { get; set; }
    public string province_name { get; set; }
    public string district_name { get; set; }
    public string ward_name { get; set; }
}
#endregion