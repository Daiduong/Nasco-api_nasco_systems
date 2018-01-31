using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class ResultModel<T> where T : class
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public ResultModel()
        {
            Error = 1;
            Message = "";
            Data = null;
        }
        public ResultModel<T> Init(T data = null, string message = "", int error = 1)
        {
            return new ResultModel<T>()
            {
                Message = message,
                Error = error,
                Data = data
            };
        }
    }
}
