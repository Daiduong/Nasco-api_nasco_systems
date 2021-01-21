using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NascoWebAPI.Data;
using NascoWebAPI.Helper.JwtBearerAuthentication;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Services;
using NascoWebAPI.Models;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/PostOffice")]
    [Authorize]
    public class PostOfficeController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IPostOfficeRepository _postOfficeRepository;
        private readonly IGoogleMapService _iGoogleMapService;
        public PostOfficeController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IPostOfficeRepository postOfficeRepository,
              IGoogleMapService iGoogleMapService
            ) : base(logger)
        {
            _postOfficeRepository = postOfficeRepository;
            _officeRepository = officeRepository;
            _iGoogleMapService = iGoogleMapService;
        }
        #region [Get]

        [HttpGet("Get")]
        public async Task<JsonResult> Get(int? id)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                id = id ?? user.PostOfficeId;
                return Json(await _postOfficeRepository.GetFirstAsync(x => x.PostOfficeID == id));
            }
            return JsonError("User not found");
        }
        [HttpGet("GetListPostOfficeTo")]
        public async Task<JsonResult> GetListPostOfficeTo()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _postOfficeRepository.GetListFromRoot((int)PostOfficeMethod.TO);
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetListPostOfficeCenterTo")]
        public async Task<JsonResult> GetListPostOfficeCenterTo()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _postOfficeRepository.GetAsync(p => p.PostOfficeTypeId == (int)PostOfficeType.HUB && p.State == 0);
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetListPostOfficeAirport")]
        public async Task<JsonResult> GetListPostOfficeAirport(double lat = 0, double lng = 0)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _postOfficeRepository.GetListPostOfficeAirport(lat, lng);
                return Json(result);
            }
            return JsonError("null");
        }
        #endregion
        [AllowAnonymous]
        [HttpPost("GetPostOfficeMinDistance")]
        public async Task<JsonResult> GetPostOfficeMinDistance([FromBody] PostOfficeDistanceRequest request)
        {
            var isBulky = true;
            if (request.Weight < 30 && request.Mass < 30
                && (request.Number_L_W_H_DIMs == null
                   || !request.Number_L_W_H_DIMs.Any(x => x.Height >= 100 || x.Long >= 100 || x.Width >= 100)))
            {
                isBulky = false;
            }
            return JsonSuccess(await _iGoogleMapService.GetPostOfficeMinDistance(request.PostOfficeMethodId, request.CityId, request.Lat, request.Lng, isBulky, request.IsRecieveHub));
        }
        [AllowAnonymous]
        [HttpPost("GetDistancePostOffice")]
        public async Task<JsonResult> GetDistancePostOffice([FromBody] PostOfficeDistanceRequest request)
        {
            return JsonSuccess(await _iGoogleMapService.GetDistancePostOffice(request.PostOfficeId, request.Lat, request.Lng));
        }
        public class PostOfficeDistanceRequest
        {
            public int PostOfficeId { get; set; }
            public int PostOfficeMethodId { get; set; }
            public int CityId { get; set; }
            public double Lat { get; set; }
            public double Lng { get; set; }
            public List<NumberDIM> Number_L_W_H_DIMs { get; set; }
            public double Weight { get; set; }
            public double Mass { get; set; }
            public bool? IsRecieveHub { get; set; }
        }
    }
}