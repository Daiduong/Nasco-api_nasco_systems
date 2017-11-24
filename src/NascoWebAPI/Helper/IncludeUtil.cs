using NascoWebAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper
{
    public static class IncludeUtil
    {
        public static Expression<Func<Lading, object>>[] GetLadingInclude(string cols)
        {
            List<Expression<Func<Lading, object>>> includeProperties = new List<Expression<Func<Lading, object>>>();
            if (!string.IsNullOrEmpty(cols))
            {
                foreach (var col in cols.Split(','))
                {
                    var colValue = col.Trim().ToLower();
                    if (colValue == "recipientid")
                    {
                        includeProperties.Add(inc => inc.Recipient);
                    }
                    else if (colValue == "senderid")
                    {
                        includeProperties.Add(inc => inc.Sender);
                    }
                    else if (colValue == "serviceid")
                    {
                        includeProperties.Add(inc => inc.Service);
                    }
                    else if (colValue == "citysendid")
                    {
                        includeProperties.Add(inc => inc.CitySend);
                    }
                    else if (colValue == "cityrecipientid")
                    {
                        includeProperties.Add(inc => inc.CityRecipient);
                    }
                    else if (colValue == "status")
                    {
                        includeProperties.Add(inc => inc.CurrenSttStatus);
                    }
                    else if (colValue == "districtfrom")
                    {
                        includeProperties.Add(inc => inc.DistrictFromObj);
                    }
                    else if (colValue == "districtto")
                    {
                        includeProperties.Add(inc => inc.DistrictToObj);
                    }
                    else if (colValue == "transport")
                    {
                        includeProperties.Add(inc => inc.Transport);
                    }
                }
            }
            return includeProperties.ToArray();
        }
    }
}
