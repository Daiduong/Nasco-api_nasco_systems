using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper
{
    public static class ApiCustomer
    {
        private static readonly HttpClient client = new HttpClient();
        public static bool LadingSendSMS(int ladingId, int poId, int sendType)
        {
            string postData = "{\"ladingId\": " + ladingId + ","
              + "\"postOfficeId\": " + poId + ","
               + "\"sendTypeId\": " + sendType + "}";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://capi.nascoexpress.com/api/Lading/SendSMS");
            Request.Method = "POST";
            Request.ContentType = "application/json";

            Task<Stream> taskDataStream = Request.GetRequestStreamAsync();
            taskDataStream.Wait();
            var dataStream = taskDataStream.Result;
            dataStream.Write(byteArray, 0, byteArray.Length);

            //
            //  SEND MESSAGE
            try
            {
                var taskResponse = Request.GetResponseAsync();
                taskResponse.Wait();
                WebResponse Response = taskResponse.Result;
                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = Reader.ReadToEnd();
            }
            catch (Exception e)
            {

            }
            return true;
        }
    }
}
