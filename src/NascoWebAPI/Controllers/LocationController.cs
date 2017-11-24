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
using System.Dynamic;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Location")]
    [AllowAnonymous]
    public class LocationController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly ILocationRepository _locationRepository;
        public LocationController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            ILocationRepository locationRepository
            ) : base(logger)
        {
            _locationRepository = locationRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [HttpGet("GetSingle")]
        public async Task<JsonResult> GetSingle(int id)
        {

            var result = await _locationRepository.GetSingleAsync(l => l.LocationId == id);
            if (result == null)
            {
                return JsonError("Not Found");
            }
            return Json(result);

        }
        [HttpGet("GetCities")]
        public async Task<JsonResult> GetCities()
        {

            var result = await _locationRepository.GetAsync(o => o.State == (int)StatusSystem.Enable && o.Type == (int)LocationType.City);
            return Json(result);
        }
        [HttpGet("GetDistrictsByCity")]
        public async Task<JsonResult> GetDistrictsByCity(int cityId)
        {
            var result = await _locationRepository.GetAsync(o => o.ParentId == cityId && o.Type == (int)LocationType.District);
            return Json(result);
        }
        #endregion
        [HttpGet("GetLocaitonMapping")]
        public async Task<JsonResult> GetLocaitonMapping(string cityName, string districtName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
            {
                return JsonError("CityName is empty");
            }
            //Khai bao doi tuong tra ve
            dynamic location = new ExpandoObject();
            location.CityId = 0;
            location.DistrictId = null;

            var cities = await _locationRepository.GetCities();
            var cityId = _locationRepository.GetIdBestMatches(cities, cityName);
            if (cityId != 0)
            {
                location.CityId = cityId;
                location.CityName = (await _locationRepository.GetFirstAsync(o => o.LocationId == cityId)).Name;
            }

            if (!string.IsNullOrWhiteSpace(districtName) && cityId != 0)
            {
                var districts = await _locationRepository.GetDistrictsByCity(cityId);
                var districtId = _locationRepository.GetIdBestMatches(districts, districtName);
                location.DistrictId = districtId;
                if (districtId != 0)
                {
                    location.DistrictName = (await _locationRepository.GetFirstAsync(o => o.LocationId == districtId)).Name;
                }
            }
            return Json(location);
        }
    }
}