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
using System.Dynamic;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/PaymentType")]
    [Authorize]
    public class PaymentTypeController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        public PaymentTypeController(ILogger<UserController> logger,
            IOfficerRepository officeRepository
            ) : base(logger)
        {
            _officeRepository = officeRepository;
        }
        #region [Get]
        [AllowAnonymous]
        [HttpGet("GetAll")]

        public JsonResult GetAll()
        {
            ICollection<dynamic> paymentTypes = new HashSet<dynamic>();

            foreach (PaymentType eStatus in EnumHelper.GetValues<PaymentType>())
            {
                dynamic paymentType = new ExpandoObject();
                paymentType.Id = (int)eStatus;
                paymentType.PaymentTypeName = EnumHelper.GetEnumDescription(eStatus);
                paymentTypes.Add(paymentType);
            }

            return Json(paymentTypes);
        }
        #endregion
    }
}