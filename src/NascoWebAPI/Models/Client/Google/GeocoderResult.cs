using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class GeocoderResult
    {
        public string[] Types { get; set; }
        public string Formatted_address { get; set; }
        public AddressComponent[] Address_components { get; set; }
        public string Place_id { get; set; }
        public string[] Postcode_localities { get; set; }
        public Geometry Geometry { get; set; }
    }
    public class AddressComponent
    {
        public string Short_name { get; set; }
        public string Long_name { get; set; }
        public string Postcode_localities { get; set; }
        public string[] Types { get; set; }
    }
    public class Geometry
    {
        public LatLng Location { get; set; }
    }
    public class LatLng
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public LatLng() { }
        public LatLng(double lat, double lng) { this.Lat = lat; this.Lng = lng; }
    }
}
