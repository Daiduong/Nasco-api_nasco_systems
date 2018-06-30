
using NascoWebAPI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NascoWebAPI.Client
{
    public abstract class ApiResponse
    {
        private string _message;
        private int _error;

        public bool StatusIsSuccessful { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
        public string ResponseResult { get; set; }

        public string Message
        {
            get
            {
                if (!this.StatusIsSuccessful)
                {
                    return EnumHelper.GetEnumDescription(ResponseCode);
                }
                else
                {
                    return _message;
                }
            }
            set
            {
                this._message = value;
            }
        }
        public int Error
        {
            get
            {
                if (!this.StatusIsSuccessful)
                {
                    return 0;
                }
                else
                {
                    return _error;
                }
            }
            set
            {
                this._error = value;
            }
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }

}
