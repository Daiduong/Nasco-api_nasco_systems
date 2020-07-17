using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Data;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/TPL")]
    [Authorize]
    public class TPLController : BaseController
    {
        private readonly ITPLEMSRepository _tplemsRepository;
        public TPLController(ILogger<UserController> logger,
           ITPLEMSRepository TPLEMSRepository
           ) : base(logger)
        {
            _tplemsRepository = TPLEMSRepository;
        }

        [AllowAnonymous]
        [HttpGet("GetEMSArea")]
        public JsonResult GetInventory(int districtId)
        {
            var json = _tplemsRepository.GetEMSArea(districtId);
            return JsonSuccess(json);
        }

        [AllowAnonymous]
        [HttpGet("GetInventory")]
        public async Task<JsonResult> GetInventory()
        {
            var json = await _tplemsRepository.GetInventory();
            return JsonSuccess(json);
        }
        [AllowAnonymous]
        [HttpGet("InsertInventory")]
        public async Task<JsonResult> InsertInventory(string name, string username,
                        string phone, string provinceCode, string districtCode, string wardCode, string address)
        {
            var json = _tplemsRepository.InsertInventory(name, username, phone, provinceCode, districtCode, wardCode, address);
            return JsonSuccess(json);
        }
        [AllowAnonymous]
        [HttpGet("CreateShipment")]
        public async Task<JsonResult> CreateShipment([FromBody] CreateShipment model)
        {
            var json = _tplemsRepository.CreateShipment(model);
            return JsonSuccess(json.Result);
        }
        [AllowAnonymous]
        [HttpPost("EMSCallBack")]
        public EMSResponseCallBack EMSCallBack([FromBody] RequestEMS model)
        {
            EMSResponseCallBack response = new EMSResponseCallBack();
            var transection = Request.Headers["ems-transaction"];
            if(string.IsNullOrWhiteSpace(transection))
            {
                response.code = "error/transectionisnull";
                return response;
            }
            try
            {
                response.transection = transection;
                var json = _tplemsRepository.EMSCallBack(model);
                response.code = "success";
                response.transection = transection;
                return response;
            }
            catch( Exception ex)
            {
                response.code = "error";
                response.transection = transection;
                return response;
            }
        }

        [AllowAnonymous]
        [HttpGet("map")]
        public int Mapping()
        {
            var districttemp = _tplemsRepository.DistrictTemp();

            List<string> vs = new List<string>();
            var data = _tplemsRepository.map();
            foreach (var item in data)
            {

                string str = item.Name.Replace(" ", "-");
                char[] str1 = str.ToCharArray();
                var _locdau = locDau(str.ToLower());
                foreach (var value in districttemp)
                {
                    if (value.EMSDistrictCode.Contains(_locdau) && value.ProvinceId == item.ParentId)
                    {
                        value.DistrictId = item.LocationId;
                    }
                }
            }
            _tplemsRepository.UpdateDistrictTemp(districttemp);
            return 1;
        }

        #region Supported
        public class EMSResponseCallBack
        {
            public string code { get; set; }
            public string transection { get; set; }
        }
        public static string locDau(string str)
        {

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

            }

            return str;
        }
        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
         };
        #endregion
    }
}