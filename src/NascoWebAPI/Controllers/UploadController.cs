using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NascoWebAPI.Data;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Upload")]
    public class UploadController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IHostingEnvironment _env;
        public UploadController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IHostingEnvironment env
            ) : base(logger)
        {
            _officeRepository = officeRepository;
            _env = env;
        }
        [HttpGet("Image")]
        [AllowAnonymous]
        public JsonResult Image(string path)
        {
            //var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            //var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            //if (user != null)
            //{
            path = System.IO.Path.Combine(_env.ContentRootPath + path);
            if (!System.IO.File.Exists(path))
            {
                return JsonError("Not Found");
            }
            System.Drawing.Image img = System.Drawing.Image.FromFile(path);
            var result = ImageHelper.ImageToBase64(img, System.Drawing.Imaging.ImageFormat.Jpeg);
            return Json(new { base64String = result });
            //}
            //return JsonError("null");
        }
        [HttpPost("Image")]
        public async Task<JsonResult> Image([FromBody] JObject jsonData)
        {
            string base64String = jsonData["base64String"].ToString();
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var targetFolder = System.IO.Path.Combine(_env.ContentRootPath + ImageHelper._TARGET_FOLDER_ROOT, DateTime.Now.ToString("yyyyMMdd")) + "/";
                var result = ImageHelper.SaveImage(targetFolder, base64String);
                if (result.Length > 0)
                {
                    result = result.Substring(_env.ContentRootPath.Length, result.Length - _env.ContentRootPath.Length);
                }
                return Json(new { path = result });
            }
            return JsonError("null");
        }
    }
}