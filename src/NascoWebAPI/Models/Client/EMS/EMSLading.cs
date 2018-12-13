using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class EMSLading
    {
        public string Code { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public DateTime ReleasedDate { get; set; }
        public TimeSpan ReleasedTime { get; set; }
        public string From_PO { get; set; }
        public string To_PO { get; set; }
        public string From_Name { get; set; }
        public string From_Phone { get; set; }
        public string From_Address { get; set; }
        public string To_Name { get; set; }
        public string To_Address { get; set; }
        public string To_Phone { get; set; }
        public string To_Province { get; set; }
        public string To_Province_Name { get; set; }
        public string From_Province { get; set; }
        public string From_Province_Name { get; set; }
        public double Weight { get; set; }
        public string Class { get; set; }
        public string Service { get; set; }
        public double Amount { get; set; }
        public double Transport_Fee { get; set; }
        public double Total_Fee { get; set; }
        public string Customer_Code { get; set; }
        public string Note { get; set; }
        public string Reference_Code { get; set; }
    }
    public class EMSLadingDeliveryRequest
    {
        public string E_CODE { get; set; }
        public string CUSTOMERCODE { get; set; }
        public string STATUS { get; set; }
        public string NOTE { get; set; }
        public string CITY { get; set; }
        public string WEIGHT { get; set; }
        public string COLLECT { get; set; }
        public DateTime DELIVERY_DATE { get; set; }
        public string POST_CODE { get; set; }
        public string REASON { get; set; }

        public EMSLadingDeliveryRequest() { }
        public EMSLadingDeliveryRequest(string code, string status, string city, string postCode, DateTime deliveryDate, string note, string reason = "")
        {
            E_CODE = code;
            STATUS = status;
            CITY = city;
            DELIVERY_DATE = deliveryDate;
            NOTE = note;
            REASON = reason;
            POST_CODE = postCode;
        }
    }
    public class EMSLadingDeliveryImageRequest
    {
        public string E_CODE { get; set; }
        public string IMAGE { get; set; }
        public EMSLadingDeliveryImageRequest() { }
        public EMSLadingDeliveryImageRequest(string code, string image)
        {
            E_CODE = code;
            IMAGE = image;
        }
    }
    public class EMSLadingResponse : EMSResponse<EMSLading>
    {

    }
    public class EMSLadingDeliveryResponse : EMSResponse
    {
        public string Value { get; set; }
        public string Id { get; set; }
    }
    public class EMSResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
    public class EMSResponse<T> : EMSResponse
    {
        public T Data { get; set; }
    }
}
