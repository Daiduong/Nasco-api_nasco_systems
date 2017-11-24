using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Summary description for GCM
/// </summary>
public class GCM
{
    public const string BrowserAPIKey = "AIzaSyA2kpDn2wkhoNwnRlSUHWoTNvXsWNhamk8";
    public const string CONTENT_TYPE_JSON = "application/json";
    //public GCM()
    //{
    //    //
    //    // TODO: Add constructor logic here
    //    //
    //}

    public static async Task<bool> SendNotification(string token, string message, int badge)
    {
        if (badge > 0)
        {
            //await SendGCMNotification(
            //       BrowserAPIKey
            //       , GetPostData(token, badge.ToString(), message)
            //       , CONTENT_TYPE_JSON
            //   );

            await SendFCMNotification(
                BrowserAPIKey
                   , GetPostData(token, badge.ToString(), message)
                );
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string GetPostData(string registrationID, string badge, string message)
    {
        return "{\"to\":\"" + registrationID + "\"," +
            "\"priority\": \"high\"," +
            "\"content_available\":true," +
            "\"notification\":{" +
            "\"sound\": \"default\"," +
            "\"badge\": \"" + badge + "\"," +
            "\"title\": \"Thông báo\"," +
            "\"body\":\"" + message + "\"}}"
            ;
    }

    public static async Task<string> SendFCMNotification(string apiKey, string postData)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://fcm.googleapis.com/");

        // Add an Accept header for JSON format.
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + apiKey);

        HttpContent content = new StringContent(
            postData
            , Encoding.UTF8
            , "application/json");

        HttpResponseMessage response = client.PostAsync("fcm/send", content).Result;

        if (response.IsSuccessStatusCode)
        {
            string sOutput = await response.Content.ReadAsStringAsync();
            //var objOutput = JsonConvert.DeserializeObject<UpdateLotteStatusResult>(sOutput);
            return sOutput;
        }
        else
        {
            string sOutput = await response.Content.ReadAsStringAsync();
            return sOutput;
        }
    }

    //public static async Task<string> SendGCMNotification(string apiKey, string postData, string postDataContentType)
    //{
    //    // from here:
    //    // http://stackoverflow.com/questions/11431261/unauthorized-when-calling-google-gcm
    //    //
    //    // original:
    //    // http://www.codeproject.com/Articles/339162/Android-push-notification-implementation-using-ASP
    //    // ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

    //    //
    //    //  MESSAGE CONTENT
    //    byte[] byteArray = Encoding.UTF8.GetBytes(postData);

    //    //
    //    //  CREATE REQUEST
    //    HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
    //    Request.Method = "POST";
    //    //Request.KeepAlive = false;
    //    Request.ContentType = postDataContentType;
    //    Request.Headers.AllKeys.SetValue(string.Format("Authorization: key={0}", apiKey));
    //    Request.Headers.AllKeys.SetValue(string.Format("Sender: id={0}", "824188599085"));
    //    //Request.ContentLength = byteArray.Length;

    //    Stream dataStream = await Request.GetRequestStreamAsync();
    //    dataStream.Write(byteArray, 0, byteArray.Length);

    //    //
    //    //  SEND MESSAGE
    //    try
    //    {
    //        WebResponse Response = await Request.GetResponseAsync();
    //        HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
    //        if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
    //        {
    //            var text = "Unauthorized - need new token";

    //        }
    //        else if (!ResponseCode.Equals(HttpStatusCode.OK))
    //        {
    //            var text = "Response from web service isn't OK";
    //        }

    //        StreamReader Reader = new StreamReader(Response.GetResponseStream());
    //        string responseLine = Reader.ReadToEnd();

    //        return responseLine;
    //    }
    //    catch (Exception e)
    //    {

    //    }
    //    return "error";
    //}

    public static bool ValidateServerCertificate(
                                              object sender,
                                              X509Certificate certificate,
                                              X509Chain chain,
                                              SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}
